namespace Myra
{
    partial class MyraGUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MyraGUI));
            Select_source_button = new Button();
            threads_label = new Label();
            input_panel = new Panel();
            move_mode_checkbox = new CheckBox();
            move_mode_label = new Label();
            force_overwrite_checkbox = new CheckBox();
            force_overwrite_label = new Label();
            source_dir = new Label();
            n_cpus_threads = new Label();
            n_thread_select_box = new ComboBox();
            add_destination_button = new Button();
            folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog2 = new FolderBrowserDialog();
            remove_destinations_selected = new Button();
            output_panel = new Panel();
            destination_clear = new Button();
            destination_display = new ListBox();
            go_button = new Button();
            stat_slash_bar = new Label();
            copy_progress_bar = new ProgressBar();
            panel1 = new Panel();
            copy_stats_panel = new Panel();
            time_elapsed_label = new Label();
            myraEngineBindingSource = new BindingSource(components);
            myraEngineOptionsBindingSource = new BindingSource(components);
            eta_label = new Label();
            input_panel.SuspendLayout();
            output_panel.SuspendLayout();
            panel1.SuspendLayout();
            copy_stats_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)myraEngineBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)myraEngineOptionsBindingSource).BeginInit();
            SuspendLayout();
            // 
            // Select_source_button
            // 
            Select_source_button.Location = new Point(16, 50);
            Select_source_button.Margin = new Padding(3, 2, 3, 2);
            Select_source_button.Name = "Select_source_button";
            Select_source_button.Size = new Size(683, 29);
            Select_source_button.TabIndex = 1;
            Select_source_button.Text = "Select Source Folder";
            Select_source_button.UseVisualStyleBackColor = true;
            Select_source_button.Click += Select_source_button_Click;
            // 
            // threads_label
            // 
            threads_label.AutoSize = true;
            threads_label.Location = new Point(20, 92);
            threads_label.Margin = new Padding(4, 0, 4, 0);
            threads_label.Name = "threads_label";
            threads_label.Size = new Size(168, 20);
            threads_label.TabIndex = 3;
            threads_label.Text = "Number of CPU threads:";
            // 
            // input_panel
            // 
            input_panel.Controls.Add(move_mode_checkbox);
            input_panel.Controls.Add(move_mode_label);
            input_panel.Controls.Add(force_overwrite_checkbox);
            input_panel.Controls.Add(force_overwrite_label);
            input_panel.Controls.Add(source_dir);
            input_panel.Controls.Add(n_cpus_threads);
            input_panel.Controls.Add(n_thread_select_box);
            input_panel.Controls.Add(threads_label);
            input_panel.Controls.Add(Select_source_button);
            input_panel.Location = new Point(4, 14);
            input_panel.Margin = new Padding(3, 2, 3, 2);
            input_panel.Name = "input_panel";
            input_panel.Size = new Size(703, 145);
            input_panel.TabIndex = 1;
            // 
            // move_mode_checkbox
            // 
            move_mode_checkbox.AutoSize = true;
            move_mode_checkbox.Location = new Point(290, 122);
            move_mode_checkbox.Name = "move_mode_checkbox";
            move_mode_checkbox.Size = new Size(18, 17);
            move_mode_checkbox.TabIndex = 10;
            move_mode_checkbox.UseVisualStyleBackColor = true;
            // 
            // move_mode_label
            // 
            move_mode_label.AutoSize = true;
            move_mode_label.Location = new Point(20, 120);
            move_mode_label.Name = "move_mode_label";
            move_mode_label.Size = new Size(92, 20);
            move_mode_label.TabIndex = 9;
            move_mode_label.Text = "Move mode:";
            // 
            // force_overwrite_checkbox
            // 
            force_overwrite_checkbox.AutoSize = true;
            force_overwrite_checkbox.Location = new Point(675, 98);
            force_overwrite_checkbox.Margin = new Padding(3, 4, 3, 4);
            force_overwrite_checkbox.Name = "force_overwrite_checkbox";
            force_overwrite_checkbox.Size = new Size(18, 17);
            force_overwrite_checkbox.TabIndex = 8;
            force_overwrite_checkbox.UseVisualStyleBackColor = true;
            // 
            // force_overwrite_label
            // 
            force_overwrite_label.AutoSize = true;
            force_overwrite_label.Location = new Point(380, 96);
            force_overwrite_label.Name = "force_overwrite_label";
            force_overwrite_label.Size = new Size(116, 20);
            force_overwrite_label.TabIndex = 7;
            force_overwrite_label.Text = "Force Overwrite:";
            // 
            // source_dir
            // 
            source_dir.AutoSize = true;
            source_dir.Location = new Point(20, 16);
            source_dir.Margin = new Padding(4, 0, 4, 0);
            source_dir.Name = "source_dir";
            source_dir.Size = new Size(140, 20);
            source_dir.TabIndex = 6;
            source_dir.Text = "Select source folder";
            // 
            // n_cpus_threads
            // 
            n_cpus_threads.AutoSize = true;
            n_cpus_threads.Location = new Point(278, 96);
            n_cpus_threads.Margin = new Padding(4, 0, 4, 0);
            n_cpus_threads.Name = "n_cpus_threads";
            n_cpus_threads.Size = new Size(36, 20);
            n_cpus_threads.TabIndex = 5;
            n_cpus_threads.Text = "_init";
            n_cpus_threads.TextAlign = ContentAlignment.TopRight;
            // 
            // n_thread_select_box
            // 
            n_thread_select_box.FormattingEnabled = true;
            n_thread_select_box.Location = new Point(186, 92);
            n_thread_select_box.Margin = new Padding(4, 5, 4, 5);
            n_thread_select_box.Name = "n_thread_select_box";
            n_thread_select_box.Size = new Size(87, 28);
            n_thread_select_box.TabIndex = 4;
            // 
            // add_destination_button
            // 
            add_destination_button.Location = new Point(16, 12);
            add_destination_button.Margin = new Padding(4, 5, 4, 5);
            add_destination_button.Name = "add_destination_button";
            add_destination_button.Size = new Size(683, 34);
            add_destination_button.TabIndex = 3;
            add_destination_button.Text = "Add Destination Folder";
            add_destination_button.UseVisualStyleBackColor = true;
            add_destination_button.Click += add_destination_button_Click;
            // 
            // remove_destinations_selected
            // 
            remove_destinations_selected.Location = new Point(16, 211);
            remove_destinations_selected.Margin = new Padding(4, 5, 4, 5);
            remove_destinations_selected.Name = "remove_destinations_selected";
            remove_destinations_selected.Size = new Size(340, 42);
            remove_destinations_selected.TabIndex = 5;
            remove_destinations_selected.Text = "Remove Selected Destination";
            remove_destinations_selected.UseVisualStyleBackColor = true;
            remove_destinations_selected.Click += remove_destinations_selected_Click;
            // 
            // output_panel
            // 
            output_panel.Controls.Add(destination_clear);
            output_panel.Controls.Add(destination_display);
            output_panel.Controls.Add(remove_destinations_selected);
            output_panel.Controls.Add(add_destination_button);
            output_panel.Location = new Point(4, 174);
            output_panel.Margin = new Padding(4, 5, 4, 5);
            output_panel.Name = "output_panel";
            output_panel.Size = new Size(703, 268);
            output_panel.TabIndex = 6;
            // 
            // destination_clear
            // 
            destination_clear.Location = new Point(364, 211);
            destination_clear.Margin = new Padding(4, 5, 4, 5);
            destination_clear.Name = "destination_clear";
            destination_clear.Size = new Size(332, 41);
            destination_clear.TabIndex = 10;
            destination_clear.Text = "Clear All Destinations";
            destination_clear.UseVisualStyleBackColor = true;
            destination_clear.Click += destination_clear_Click;
            // 
            // destination_display
            // 
            destination_display.FormattingEnabled = true;
            destination_display.Location = new Point(16, 55);
            destination_display.Margin = new Padding(4, 5, 4, 5);
            destination_display.Name = "destination_display";
            destination_display.Size = new Size(680, 144);
            destination_display.TabIndex = 9;
            // 
            // go_button
            // 
            go_button.Font = new Font("Microsoft Sans Serif", 16F);
            go_button.Location = new Point(15, 5);
            go_button.Margin = new Padding(4, 5, 4, 5);
            go_button.Name = "go_button";
            go_button.Size = new Size(681, 66);
            go_button.TabIndex = 7;
            go_button.Text = "Begin Copying";
            go_button.UseVisualStyleBackColor = true;
            go_button.Click += go_button_Click;
            // 
            // stat_slash_bar
            // 
            stat_slash_bar.AutoSize = true;
            stat_slash_bar.Location = new Point(10, 4);
            stat_slash_bar.Name = "stat_slash_bar";
            stat_slash_bar.Size = new Size(175, 20);
            stat_slash_bar.TabIndex = 8;
            stat_slash_bar.Text = "Start copying to see stats";
            // 
            // copy_progress_bar
            // 
            copy_progress_bar.Location = new Point(189, 33);
            copy_progress_bar.Margin = new Padding(3, 4, 3, 4);
            copy_progress_bar.Name = "copy_progress_bar";
            copy_progress_bar.Size = new Size(500, 20);
            copy_progress_bar.Step = 1;
            copy_progress_bar.TabIndex = 9;
            // 
            // panel1
            // 
            panel1.Controls.Add(copy_stats_panel);
            panel1.Controls.Add(go_button);
            panel1.Location = new Point(7, 466);
            panel1.Margin = new Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(703, 145);
            panel1.TabIndex = 10;
            // 
            // copy_stats_panel
            // 
            copy_stats_panel.Controls.Add(eta_label);
            copy_stats_panel.Controls.Add(time_elapsed_label);
            copy_stats_panel.Controls.Add(copy_progress_bar);
            copy_stats_panel.Controls.Add(stat_slash_bar);
            copy_stats_panel.Location = new Point(4, 76);
            copy_stats_panel.Name = "copy_stats_panel";
            copy_stats_panel.Size = new Size(693, 68);
            copy_stats_panel.TabIndex = 11;
            copy_stats_panel.Visible = false;
            // 
            // time_elapsed_label
            // 
            time_elapsed_label.AutoSize = true;
            time_elapsed_label.Location = new Point(10, 33);
            time_elapsed_label.Name = "time_elapsed_label";
            time_elapsed_label.Size = new Size(101, 20);
            time_elapsed_label.TabIndex = 10;
            time_elapsed_label.Text = "Time Elapsed:";
            // 
            // myraEngineBindingSource
            // 
            myraEngineBindingSource.DataSource = typeof(MyraEngine);
            // 
            // myraEngineOptionsBindingSource
            // 
            myraEngineOptionsBindingSource.DataSource = typeof(MyraEngineOptions);
            // 
            // eta_label
            // 
            eta_label.AutoSize = true;
            eta_label.Location = new Point(357, 4);
            eta_label.Name = "eta_label";
            eta_label.Size = new Size(36, 20);
            eta_label.TabIndex = 11;
            eta_label.Text = "_init";
            // 
            // MyraGUI
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(720, 549);
            Controls.Add(panel1);
            Controls.Add(output_panel);
            Controls.Add(input_panel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            Name = "MyraGUI";
            Text = "Myra";
            Load += MyraGUI_Load;
            input_panel.ResumeLayout(false);
            input_panel.PerformLayout();
            output_panel.ResumeLayout(false);
            panel1.ResumeLayout(false);
            copy_stats_panel.ResumeLayout(false);
            copy_stats_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)myraEngineBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)myraEngineOptionsBindingSource).EndInit();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button Select_source_button;
        private System.Windows.Forms.Label threads_label;
        private System.Windows.Forms.Panel input_panel;
        private System.Windows.Forms.Label n_cpus_threads;
        private System.Windows.Forms.ComboBox n_thread_select_box;
        private System.Windows.Forms.Label source_dir;
        private System.Windows.Forms.Button add_destination_button;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
        private System.Windows.Forms.Button remove_destinations_selected;
        private System.Windows.Forms.Panel output_panel;
        private System.Windows.Forms.Button go_button;
        private System.Windows.Forms.ListBox destination_display;
        private System.Windows.Forms.Button destination_clear;
        private System.Windows.Forms.Label stat_slash_bar;
        private System.Windows.Forms.ProgressBar copy_progress_bar;
        private System.Windows.Forms.CheckBox force_overwrite_checkbox;
        private System.Windows.Forms.Label force_overwrite_label;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.BindingSource myraEngineBindingSource;
        private System.Windows.Forms.BindingSource myraEngineOptionsBindingSource;
        private CheckBox move_mode_checkbox;
        private Label move_mode_label;
        private Panel copy_stats_panel;
        private Label time_elapsed_label;
        private Label eta_label;
    }
}

