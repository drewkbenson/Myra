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
            foreach (var dest in destination_display.Items) { destinations.Add(dest.ToString()); }

            Action<int, int, TimeSpan> updateProgress = (nFilesDone, nFiles, timeElapsed) =>
            {
                stat_slash_bar.Invoke((MethodInvoker)
                delegate {
                    stat_slash_bar.Text = $"{nFilesDone} / {nFiles}";
                });

                time_elapsed_label.Invoke((MethodInvoker)
                delegate {
                    time_elapsed_label.Text = $"Time Elapsed: {timeElapsed.ToString(@"hh\:mm\:ss")}";
                });

                copy_progress_bar.Invoke((MethodInvoker)
                delegate {
                    if (copy_progress_bar.Maximum != nFiles)
                    {
                        copy_progress_bar.Maximum = nFiles;
                    }
                    copy_progress_bar.Value = nFilesDone;
                });

                if (nFilesDone == nFiles)
                {
                    stat_slash_bar.Invoke((MethodInvoker)
                    delegate {
                        stat_slash_bar.Text = $"{nFiles} Copied!";
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

            MyraEngine.StartMyraEngine(
                options,
                updateProgress,
                threadFinish
            );
        }
    }
}
