using MyraGUIUtil;

namespace Myra
{
    public partial class MyraGUI : Form
    {
        private bool progressShown = false;

        public MyraGUI()
        {
            InitializeComponent();
        }

        private void MyraGUI_Load(object sender, EventArgs e)
        {
            var n_max_cpu = Environment.ProcessorCount * 2;
            n_cpus_threads.Text = $"/ {n_max_cpu}";
            for (int i = 1; i < n_max_cpu + 1; i++)
            {
                n_thread_select_box.Items.Add(i);
            }
            n_thread_select_box.SelectedIndex = n_max_cpu - 2;

            destination_display.Items.Clear();
            //this.AutoScroll = true;
        }

        private void Select_source_button_Click(object sender, EventArgs e)
        {
            var selected = GUIUtil.SelectFolder();
            if (selected != null)
            {
                source_dir.Text = selected.ToString();
            }
        }

        private void add_destination_button_Click(object sender, EventArgs e)
        {
            // Adding potentially multiple selected folders to the selected folders list
            var selectedFolders = GUIUtil.SelectFolderMultiple();
            var alreadyInList = false;

            if (selectedFolders != null)
            {
                foreach (var selectedFolder in selectedFolders)
                {
                    foreach (var dest in destination_display.Items)
                    {
                        if (dest.ToString() == selectedFolder.ToString())
                        {
                            alreadyInList = true;
                        }
                    }

                    if (!alreadyInList)
                    {
                        destination_display.Items.Add(selectedFolder.ToString());
                    }
                }
            }
        }

        private void remove_destinations_selected_Click(object sender, EventArgs e)
        {
            var selected = destination_display.SelectedIndex;
            if (selected >= 0)
            {
                destination_display.Items.Remove(destination_display.Items[selected]);
            }
        }

        private void destination_clear_Click(object sender, EventArgs e)
        {
            destination_display.Items.Clear();
        }

        private void go_button_Click(object sender, EventArgs e)
        {
            go_button.Enabled = false;
            int nThreads = 0;
            var selected = n_thread_select_box?.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selected))
            {
                int.TryParse(selected, out nThreads);
            }

            // Expand and set
            if (!progressShown)
            {
                progressShown = true;
                this.Size = new Size(this.Size.Width, this.Size.Height + 64);
                copy_stats_panel.Visible = true;
            }

            List<string> destinations = new List<string>();
            foreach (var dest in destination_display.Items.OfType<string>())
            {
                destinations.Add(dest);
            }

            /*
             * This Action is what the engine will call when it wants the UI to update.
             * 
             * MyraProgress - progressState:
             *      This defines what state the program is in. See MayaProgress in MayaEngine.cs for more information on this Enum
             * nFilesDone - int:
             *      This represents the number of files that have been COMPLETED
             *      This becomes nDirectories when in the ANALYZING_DIRECTORIES progress state
             * nFiles - int:
             *      This represents the number of files that need to be completed
             * timeElapsed - timeSpan
             *      Represents the amount of time that has elapsed since pressing the "Copy Files" button
             */
            Action<MyraProgress, int, int, TimeSpan> updateProgress = (progressState, nFilesDone, nFiles, timeElapsed) =>
            {
                if (progressState == MyraProgress.NOT_STARTED)
                {

                } else if (progressState == MyraProgress.ANALYZING_DIRECTORIES)
                {
                    stat_slash_bar.Invoke((MethodInvoker)
                    delegate
                    {
                        stat_slash_bar.Text = $"Analyzing: {nFilesDone} Directories / {nFiles} Files";
                    });

                    time_elapsed_label.Invoke((MethodInvoker)
                    delegate {
                        time_elapsed_label.Text = $"Time Elapsed: {timeElapsed.ToString(@"hh\:mm\:ss")}";
                    });

                    copy_progress_bar.Invoke((MethodInvoker)
                    delegate {
                        if (copy_progress_bar.Style == ProgressBarStyle.Blocks)
                        {
                            copy_progress_bar.Style = ProgressBarStyle.Marquee;
                        }
                    });
                } else if (progressState == MyraProgress.COPYING_FILES)
                {
                    stat_slash_bar.Invoke((MethodInvoker)
                    delegate
                    {
                        stat_slash_bar.Text = $"{nFilesDone} / {nFiles}";
                    });

                    eta_label.Invoke((MethodInvoker)
                    delegate {
                        if (nFilesDone > 0)
                        {
                            var scaleFactor = ((float) nFiles / (float) nFilesDone) - 1;
                            eta_label.Text = $"Remaining Time: {TimeSpan.FromTicks((long) (timeElapsed.Ticks * scaleFactor)).ToString(@"hh\:mm\:ss")}";
                        } else
                        {
                            eta_label.Text = $"Calculating time remaining...";
                        }
                    });

                    time_elapsed_label.Invoke((MethodInvoker)
                    delegate {
                        time_elapsed_label.Text = $"Time Elapsed: {timeElapsed.ToString(@"hh\:mm\:ss")}";
                    });

                    copy_progress_bar.Invoke((MethodInvoker)
                    delegate {
                        if (copy_progress_bar.Style == ProgressBarStyle.Marquee)
                        {
                            copy_progress_bar.Style = ProgressBarStyle.Blocks;
                        }
                    });

                    copy_progress_bar.Invoke((MethodInvoker)
                    delegate {
                        if (copy_progress_bar.Maximum < nFilesDone)
                        {

                        }
                        if (copy_progress_bar.Maximum != nFiles)
                        {
                            copy_progress_bar.Maximum = nFiles;
                        }
                        copy_progress_bar.Value = nFilesDone;
                    });
                } else if (progressState == MyraProgress.DONE)
                {
                    stat_slash_bar.Invoke((MethodInvoker)
                    delegate
                    {
                        stat_slash_bar.Text = $"{nFiles} files copied!";
                    });

                    copy_progress_bar.Invoke((MethodInvoker)
                    delegate {
                        if (copy_progress_bar.Style == ProgressBarStyle.Marquee)
                        {
                            copy_progress_bar.Style = ProgressBarStyle.Blocks;
                            copy_progress_bar.Maximum = 1;
                            copy_progress_bar.Value = 1;
                        }
                    });
                }
            };

            Action threadFinish = () =>
            {
                go_button.Invoke((MethodInvoker)
                delegate
                {
                    go_button.Enabled = true;
                });
            };

            var options = new MyraEngineOptions(
                source_dir.Text,
                destinations,
                nThreads,
                force_overwrite_checkbox.Checked,
                true, // This is for showing the message as a pop up rather than as a console write
                move_mode_checkbox.Checked
            );

            MyraEngine.StartMyraEngineThread(
                options,
                updateProgress,
                threadFinish
            );
        }
    }
}
