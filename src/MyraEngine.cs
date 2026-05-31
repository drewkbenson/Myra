using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Myra
{

    /*
     * Class for holding all the options for MyraEngine
     */
    public class MyraEngineOptions
    {
        public string source { get; }
        public List<string> destinations { get; }
        public int nThreads { get; }
        public bool forceOverride { get; }
        public int maxChunkSize { get; }
        public bool showMessageBox { get; }
        public bool moveMode { get; }

        public MyraEngineOptions(
            string inSource,
            List<string> inDestinations,
            int inNThreads,
            bool inForceOverride,
            bool inShowMessageBox,
            bool inMoveMode
            )
        {
            source = inSource;
            destinations = inDestinations;
            nThreads = inNThreads;
            forceOverride = inForceOverride;
            maxChunkSize = 1024 * 1024 * 64;
            showMessageBox = inShowMessageBox;
            moveMode = inMoveMode;
        }
    }

    public class MyraEngine
    {
        public static void StartMyraEngine(
            MyraEngineOptions options,
            Action<int, int, TimeSpan> updateProgress,
            Action finished
            )
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var compliance = ensureCompliance(options);
            if (compliance > 0)
            {
                finished();
                return;
            }

            var (nFiles, nFolders) = buildDestDirectory(options, "");

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

        private static int ensureCompliance(MyraEngineOptions options)
        {
            // Compliance rules:
            // 1. Options is null
            // 2. Source directory empty
            // 3. Any destination directory is invalid
            // 4. No destination directories
            // 5. Destination directory path is directly up stream from source directory path

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

        private static (int, int) buildDestDirectory(MyraEngineOptions options, string slug)
        {
            foreach (var dest in options.destinations)
            {
                if (!Directory.Exists(System.IO.Path.Combine(dest, slug)))
                {
                    try
                    {
                        Directory.CreateDirectory(System.IO.Path.Combine(dest, slug));
                    }
                    catch
                    {
                        return (-1, -1);
                    }
                }
            }

            var (nFiles, nDirs) = (0, 0);

            nFiles += Directory.GetFiles(System.IO.Path.Combine(options.source, slug)).Length;
            var dirs = Directory.GetDirectories(System.IO.Path.Combine(options.source, slug)).Select(file => System.IO.Path.GetFileName(file)).ToArray(); ;

            foreach (var dir in dirs)
            {
                var (childFiles, childDirs) = (-1, -1);
                if (slug == "")
                {
                    (childFiles, childDirs) = buildDestDirectory(options, dir);
                }
                else
                {
                    (childFiles, childDirs) = buildDestDirectory(options, System.IO.Path.Combine(slug, dir));
                }

                if (childFiles < 0)
                {
                    return (-1, -1);
                }
                else
                {
                    nFiles += childFiles;
                    nDirs += childDirs;
                }
            }

            return (nFiles, nDirs);
        }

        private static IEnumerable<(string, string)> getFilesToProcess(MyraEngineOptions options, string slug)
        {
            var files = Directory.GetFiles(System.IO.Path.Combine(options.source, slug)).Select(file => System.IO.Path.GetFileName(file)).ToArray();
            foreach (var file in files)
            {
                yield return (slug, file);
            }

            var dirs = Directory.GetDirectories(System.IO.Path.Combine(options.source, slug)).Select(file => System.IO.Path.GetFileName(file)).ToArray();
            foreach (var dir in dirs)
            {
                var newSlug = "";
                if (slug == "")
                {
                    newSlug = dir;
                }
                else
                {
                    newSlug = System.IO.Path.Combine(slug, dir);
                }
                foreach (var _ in getFilesToProcess(options, newSlug))
                {
                    yield return _;
                }
            }
        }

        private static async void wrapCopyFiles(MyraEngineOptions options, int nFiles, Stopwatch stopWatch, Action<int, int, TimeSpan> updateProgress, Action finished)
        {
            BlockingCollection<(string, string)> fileSource = new BlockingCollection<(string, string)>();

            var filesCompleted = 0;
            var cancelToken = new CancellationTokenSource();

            // Starting n worker threads. ToList is needed to actually create them as this is lazy
            var workers = Enumerable.Range(0, options.nThreads)
                .Select(_ => Task.Run(async () =>
                {
                    await startCopyThread(options, fileSource, () => Interlocked.Increment(ref filesCompleted), cancelToken.Token);
            })).ToList();

            var lastUpdate = DateTime.UtcNow;
            foreach (var result in getFilesToProcess(options, ""))
            {
                while (fileSource.Count > 100)
                {
                    Thread.Sleep(1);
                }

                fileSource.Add(result);

                if (lastUpdate.AddMilliseconds(100) < DateTime.UtcNow)
                {
                    lastUpdate = DateTime.UtcNow;
                    updateProgress(Volatile.Read(ref filesCompleted), nFiles, stopWatch.Elapsed);
                }
            }

            while (Volatile.Read(ref filesCompleted) < nFiles)
            {
                if (lastUpdate.AddMilliseconds(100) < DateTime.UtcNow)
                {
                    lastUpdate = DateTime.UtcNow;
                    updateProgress(Volatile.Read(ref filesCompleted), nFiles, stopWatch.Elapsed);
                }
                Thread.Sleep(1);
            }

            stopWatch.Stop();
            finished();
            updateProgress(Volatile.Read(ref filesCompleted), nFiles, stopWatch.Elapsed);
            cancelToken.Cancel();

            await Task.WhenAll(workers);

            Console.WriteLine("Done");
        }

        private static async Task startCopyThread(MyraEngineOptions options, BlockingCollection<(string, string)> fileSource, Action fileCompleted, CancellationToken cancelToken)
        {
            var source = options.source;
            var destinations = options.destinations;
            while (!cancelToken.IsCancellationRequested)
            {
                try
                {
                    var (slug, file) = fileSource.Take();
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
