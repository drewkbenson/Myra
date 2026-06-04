using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Myra
{

    public enum MyraProgress
    {
        NOT_STARTED = 0,
        ANALYZING_DIRECTORIES = 1,
        COPYING_FILES = 2,
        DONE = 3,
    };

    /*
     * Class for holding all the options for MyraEngine
     */
    public class MyraEngineOptions
    {
        public string source { get; }
        public List<string> destinations { get; }
        public int nThreads { get; }
        public bool forceOverride { get; }
        public bool showMessageBox { get; }
        public bool moveMode { get; }

        public int maxChunkSize { get; }
        public int maxBufferSize { get; }

        public MyraEngineOptions(
            string inSource,
            List<string> inDestinations,
            int inNThreads,
            bool inForceOverride,
            bool inShowMessageBox,
            bool inMoveMode
            )
        {
            // These are actually inputs
            source = inSource;
            destinations = inDestinations;
            nThreads = inNThreads;
            forceOverride = inForceOverride;
            showMessageBox = inShowMessageBox;
            moveMode = inMoveMode;

            // These ones are defined by me. Might include these as options in the future
            maxChunkSize = 1024 * 1024 * 64;
            maxBufferSize = 10000;
        }
    }

    public class MyraEngine
    {

        // EnumerationOptions are here for clearly defining the options that I should and shouldn't be using when iterating through directories/files
        private static EnumerationOptions enumerationRecurseOptions = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
        };

        private static EnumerationOptions enumerationNoRecurseOptions = new EnumerationOptions
        {
            RecurseSubdirectories = false,
            IgnoreInaccessible = true,
        };

        /*
         * Just a wrapper for another method that starts a thread
         */
        public static void StartMyraEngineThread(
            MyraEngineOptions options,
            Action<MyraProgress, int, int, TimeSpan> updateProgress,
            Action finished
            )
        {
            Thread th = new Thread(() =>
            StartMyraEngine(
                options,
                updateProgress,
                finished
            ));
            th.Start();
        }

        /*
         * Helper method - Do not call
         */
        private static void StartMyraEngine(
            MyraEngineOptions options,
            Action<MyraProgress, int, int, TimeSpan> updateProgress,
            Action finished
        )
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Checking to see if the call is gonna error out before we even begin to look at copying things
            var compliance = ensureCompliance(options);
            if (compliance > 0)
            {
                finished();
                return;
            }

            // Starts to build the directories of the destinations
            updateProgress(MyraProgress.ANALYZING_DIRECTORIES, 0, 0, stopWatch.Elapsed);
            var (nFiles, nFolders) = buildDestDirectory(options, updateProgress, stopWatch);

            if (nFiles < 0)
            {
                // Fail out somehow
                showMessage(options.showMessageBox, "No files were found.");
                finished();
                return;
            }

            // Folder structure built, start init'l threads
            Thread th = new Thread(() => wrapCopyFiles(options, nFiles, stopWatch, updateProgress, finished));
            th.Start();
        }

        /*
         * Just helps with alerting user to errors
         */
        private static void showMessage(bool showBox, string message)
        {
            if (showBox)
            {
                MessageBox.Show(message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        /*
         * Compliance rules:
         * 1. Options is null
         * 2. Source directory empty
         * 3. Any destination directory is invalid
         * 4. No destination directories
         * 5. Destination directory path is directly up stream from source directory path
         */
        private static int ensureCompliance(MyraEngineOptions options)
        {


            if (options is null)
            {
                showMessage(false, "Options cannot be null");
                return 1;
            }

            if (options.source is null || options.source == "" || options.source == "Select Source Folder" || !Directory.Exists(options.source))
            {
                showMessage(options.showMessageBox, "Source folder does not exist");
                return 2;
            }

            if (options.destinations is null || !options.destinations.Any() || options.destinations.Any(dest => !Directory.Exists(dest)))
            {
                showMessage(options.showMessageBox, "One or more destination folder(s) does not exist");
                return 3;
            }

            // We're going to remove destination directories if they're the source directory
            try { options.destinations.Remove(options.source); } catch { }

            if (options.destinations.Count == 0)
            {
                showMessage(options.showMessageBox, "No valid destination directories exist");
                return 4;
            }

            foreach (var destination in options.destinations)
            {
                if (options.source.ToString().StartsWith(destination.ToString()))
                {
                    showMessage(options.showMessageBox, "Source folder cannot be a sub folder of any destination folder");
                    return 5;
                }
            }

            return 0;
        }

        /*
         * Copies the file structure of the source to the destinations.
         * 
         * updateProgress and stopwatch are here to update the UI
         */
        private static (int, int) buildDestDirectory(MyraEngineOptions options, Action<MyraProgress, int, int, TimeSpan> updateProgress, Stopwatch stopWatch)
        {
            var nDirectories = 0;
            var nFiles = 0;
            var lastUpdate = DateTime.UtcNow;

            foreach (var entry in Directory.EnumerateDirectories(options.source, "*", enumerationRecurseOptions))
            {
                if (lastUpdate.AddMilliseconds(100) < DateTime.UtcNow)
                {
                    lastUpdate = DateTime.UtcNow;
                    updateProgress(MyraProgress.ANALYZING_DIRECTORIES, nDirectories, nFiles, stopWatch.Elapsed);
                }

                try
                {
                    var attrs = File.GetAttributes(entry);

                    if ((attrs & FileAttributes.Directory) != 0)
                    {
                        nDirectories++;

                        string relativePath = Path.GetRelativePath(options.source, entry);

                        nFiles += Directory.EnumerateFiles(entry, "*", enumerationNoRecurseOptions).Count();

                        foreach (var dest in options.destinations)
                        {
                            Directory.CreateDirectory(Path.Combine(dest, relativePath));
                        }
                    }
                    else
                    {
                        nFiles++;
                    }
                }
                catch
                {
                    // Skip bad entry
                }
            }

            updateProgress(MyraProgress.ANALYZING_DIRECTORIES, nDirectories, nFiles, stopWatch.Elapsed);
            return (nFiles, nDirectories);
        }

        /*
         * Returns a lazy way of enumerating through the file system.
         * 
         * We need the path relative to both the source and destination(s) so we're returning the slugs as well as the file names
         */
        private static IEnumerable<(string, string)> getFilesToProcess(MyraEngineOptions options, string slug)
        {
            var currentPath = Path.Combine(options.source, slug);
            var files = Directory.EnumerateFiles(currentPath, "*", enumerationNoRecurseOptions);
            foreach (var file in files)
            {
                yield return (slug, Path.GetFileName(file));
            }

            var dirs = Directory.EnumerateDirectories(currentPath, "*", enumerationNoRecurseOptions);
            foreach (var dir in dirs)
            {
                var newSlug = "";
                if (slug == "")
                {
                    newSlug = Path.GetFileName(dir);
                }
                else
                {
                    newSlug = System.IO.Path.Combine(slug, Path.GetFileName(dir));
                }

                // Iterating through each of the child directory returns
                foreach (var _ in getFilesToProcess(options, newSlug))
                {
                    yield return _;
                }
            }
        }

        /*
         * This function's job is to set up the file list buffer, start the worker threads, start filling the file list up, then cleanup when all's done
         */
        private static async void wrapCopyFiles(MyraEngineOptions options, int nFiles, Stopwatch stopWatch, Action<MyraProgress, int, int, TimeSpan> updateProgress, Action finished)
        {
            BlockingCollection<(string, string)> fileSource = new BlockingCollection<(string, string)>();

            var filesCompleted = 0;
            var cancelToken = new CancellationTokenSource();

            // Starting nThreads of worker threads. ToList is needed to actually create them as this is lazy
            var workers = Enumerable.Range(0, options.nThreads)
                .Select(_ => Task.Run(async () =>
                {
                    await startCopyThread(options, fileSource, () => Interlocked.Increment(ref filesCompleted), cancelToken.Token);
            })).ToList();

            // Start loading up the dispatch thread
            var lastUpdate = DateTime.UtcNow;
            foreach (var result in getFilesToProcess(options, ""))
            {
                while (fileSource.Count > options.maxBufferSize)
                {
                    Thread.Sleep(1);

                    if (lastUpdate.AddMilliseconds(100) < DateTime.UtcNow)
                    {
                        lastUpdate = DateTime.UtcNow;
                        updateProgress(MyraProgress.COPYING_FILES, Volatile.Read(ref filesCompleted), nFiles, stopWatch.Elapsed);
                    }
                }

                fileSource.Add(result);

                if (lastUpdate.AddMilliseconds(100) < DateTime.UtcNow)
                {
                    lastUpdate = DateTime.UtcNow;
                    updateProgress(MyraProgress.COPYING_FILES, Volatile.Read(ref filesCompleted), nFiles, stopWatch.Elapsed);
                }
            }

            // Checking to make sure that all the files are properly written before we finish this up
            while (Volatile.Read(ref filesCompleted) < nFiles)
            {
                if (lastUpdate.AddMilliseconds(100) < DateTime.UtcNow)
                {
                    lastUpdate = DateTime.UtcNow;
                    updateProgress(MyraProgress.COPYING_FILES, Volatile.Read(ref filesCompleted), nFiles, stopWatch.Elapsed);
                }
                Thread.Sleep(1);
            }

            // Cleanup
            // Stop stopwatch, call finished callback, tell progress bar we're done, cancel children threads
            cancelToken.Cancel();
            stopWatch.Stop();
            finished();
            updateProgress(MyraProgress.DONE, Volatile.Read(ref filesCompleted), nFiles, stopWatch.Elapsed);
            await Task.WhenAll(workers);
        }

        /*
         * Starts a copy thread that will continue to read and process (slugs, files) from fileSource until cancelToken is cancelled
         * 
         */
        private static async Task startCopyThread(MyraEngineOptions options, BlockingCollection<(string, string)> fileSource, Action fileCompleted, CancellationToken cancelToken)
        {
            var source = options.source;
            var destinations = options.destinations;
            while (!cancelToken.IsCancellationRequested)
            {
                try
                {
                    var (slug, file) = fileSource.Take();
                    Console.WriteLine($"{slug} / {file}");
                    var sourcePath = System.IO.Path.Combine(source, slug, file);

                    List<FileStream> fHandles = new List<FileStream>(destinations.Count);

                    foreach (var dest in destinations)
                    {
                        var destPath = System.IO.Path.Combine(dest, slug, file);
                        if (!options.forceOverride)
                        {
                            if (File.Exists(destPath) && File.GetLastWriteTimeUtc(destPath) > File.GetLastWriteTimeUtc(sourcePath))
                            {
                                continue;
                            }
                        }
                        fHandles.Add(System.IO.File.OpenWrite(destPath));
                    }

                    if (fHandles.Count > 0)
                    {
                        var fInfo = new FileInfo(sourcePath);
                        var chunkSize = options.maxChunkSize;
                        if (fInfo.Length < chunkSize)
                        {
                            chunkSize = (int) fInfo.Length;
                        }

                        int bytesRead;
                        byte[] buffer = ArrayPool<byte>.Shared.Rent(chunkSize);

                        using var readStream = System.IO.File.OpenRead(System.IO.Path.Combine(source, slug, file));

                        // Wrap this with a try, because we need to guarantee that the finally stuff happens
                        try
                        {
                            var writeTasks = new Task[fHandles.Count];

                            while ((bytesRead = await readStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                for (int i = 0; i < fHandles.Count; i++)
                                {
                                    writeTasks[i] = fHandles[i].WriteAsync(buffer, 0, bytesRead);
                                }

                                await Task.WhenAll(writeTasks);
                            }
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(buffer);
                            readStream.Close();
                        }
                    }

                    foreach (var fHandle in fHandles)
                    {
                        fHandle.Dispose();
                    }

                    if (options.moveMode)
                    {
                        File.Delete(sourcePath);
                    }

                    fileCompleted();
                }
                catch { }
            }
        }
    }
}
