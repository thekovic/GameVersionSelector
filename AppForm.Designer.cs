namespace SteamGameVersionSelector
{
    partial class SteamGameVersionSelectorForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBoxSteamPath = new TextBox();
            buttonSteamPath = new Button();
            labelSteamPath = new Label();
            comboBoxGameSelector = new ComboBox();
            labelPatchSelector = new Label();
            labelGameSelector = new Label();
            comboBoxPatchSelector = new ComboBox();
            splitContainer1 = new SplitContainer();
            labelSteamPassword = new Label();
            labelSteamUsername = new Label();
            textBoxSteamUsername = new TextBox();
            textBoxSteamPassword = new TextBox();
            richTextBoxInstallMessages = new RichTextBox();
            labelInstall = new Label();
            buttonInstall = new Button();
            progressBar1 = new ProgressBar();
            buttonCancel = new Button();
            panelContentWrapper = new Panel();
            folderBrowserDialogSteamPath = new FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize) splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panelContentWrapper.SuspendLayout();
            SuspendLayout();
            // 
            // textBoxSteamPath
            // 
            textBoxSteamPath.Location = new Point(12, 32);
            textBoxSteamPath.Name = "textBoxSteamPath";
            textBoxSteamPath.Size = new Size(658, 27);
            textBoxSteamPath.TabIndex = 0;
            textBoxSteamPath.TextChanged += Gui_textBoxSteamPath_TextChanged;
            // 
            // buttonSteamPath
            // 
            buttonSteamPath.Location = new Point(676, 31);
            buttonSteamPath.Name = "buttonSteamPath";
            buttonSteamPath.Size = new Size(94, 29);
            buttonSteamPath.TabIndex = 1;
            buttonSteamPath.Text = "Browse...";
            buttonSteamPath.UseVisualStyleBackColor = true;
            buttonSteamPath.Click += Gui_buttonSteamPath_Click;
            // 
            // labelSteamPath
            // 
            labelSteamPath.AutoSize = true;
            labelSteamPath.Location = new Point(12, 9);
            labelSteamPath.Name = "labelSteamPath";
            labelSteamPath.Size = new Size(328, 20);
            labelSteamPath.TabIndex = 2;
            labelSteamPath.Text = "Select path to your 'steamapps\\common' folder:";
            labelSteamPath.UseMnemonic = false;
            // 
            // comboBoxGameSelector
            // 
            comboBoxGameSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxGameSelector.FormattingEnabled = true;
            comboBoxGameSelector.Location = new Point(132, 4);
            comboBoxGameSelector.Margin = new Padding(3, 3, 0, 3);
            comboBoxGameSelector.Name = "comboBoxGameSelector";
            comboBoxGameSelector.Size = new Size(246, 28);
            comboBoxGameSelector.TabIndex = 3;
            comboBoxGameSelector.SelectedIndexChanged += Gui_comboBoxGameSelector_SelectedIndexChanged;
            // 
            // labelPatchSelector
            // 
            labelPatchSelector.AutoSize = true;
            labelPatchSelector.Location = new Point(6, 39);
            labelPatchSelector.Name = "labelPatchSelector";
            labelPatchSelector.Size = new Size(114, 20);
            labelPatchSelector.TabIndex = 4;
            labelPatchSelector.Text = "Choose a patch:";
            // 
            // labelGameSelector
            // 
            labelGameSelector.AutoSize = true;
            labelGameSelector.Location = new Point(6, 6);
            labelGameSelector.Name = "labelGameSelector";
            labelGameSelector.Size = new Size(115, 20);
            labelGameSelector.TabIndex = 5;
            labelGameSelector.Text = "Choose a game:";
            // 
            // comboBoxPatchSelector
            // 
            comboBoxPatchSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxPatchSelector.FormattingEnabled = true;
            comboBoxPatchSelector.Location = new Point(132, 37);
            comboBoxPatchSelector.Name = "comboBoxPatchSelector";
            comboBoxPatchSelector.Size = new Size(246, 28);
            comboBoxPatchSelector.TabIndex = 6;
            comboBoxPatchSelector.SelectedIndexChanged += Gui_comboBoxPatchSelector_SelectedIndexChanged;
            // 
            // splitContainer1
            // 
            splitContainer1.Location = new Point(12, 65);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(labelSteamPassword);
            splitContainer1.Panel1.Controls.Add(labelSteamUsername);
            splitContainer1.Panel1.Controls.Add(textBoxSteamUsername);
            splitContainer1.Panel1.Controls.Add(textBoxSteamPassword);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(labelPatchSelector);
            splitContainer1.Panel2.Controls.Add(comboBoxPatchSelector);
            splitContainer1.Panel2.Controls.Add(comboBoxGameSelector);
            splitContainer1.Panel2.Controls.Add(labelGameSelector);
            splitContainer1.Size = new Size(758, 74);
            splitContainer1.SplitterDistance = 378;
            splitContainer1.SplitterWidth = 2;
            splitContainer1.TabIndex = 7;
            // 
            // labelSteamPassword
            // 
            labelSteamPassword.AutoSize = true;
            labelSteamPassword.Location = new Point(0, 39);
            labelSteamPassword.Name = "labelSteamPassword";
            labelSteamPassword.Size = new Size(192, 20);
            labelSteamPassword.TabIndex = 3;
            labelSteamPassword.Text = "Enter your Steam password:";
            // 
            // labelSteamUsername
            // 
            labelSteamUsername.AutoSize = true;
            labelSteamUsername.Location = new Point(0, 6);
            labelSteamUsername.Name = "labelSteamUsername";
            labelSteamUsername.Size = new Size(193, 20);
            labelSteamUsername.TabIndex = 2;
            labelSteamUsername.Text = "Enter your Steam username:";
            // 
            // textBoxSteamUsername
            // 
            textBoxSteamUsername.Location = new Point(205, 4);
            textBoxSteamUsername.Name = "textBoxSteamUsername";
            textBoxSteamUsername.Size = new Size(167, 27);
            textBoxSteamUsername.TabIndex = 0;
            textBoxSteamUsername.TextChanged += Gui_textBoxSteamUsername_TextChanged;
            // 
            // textBoxSteamPassword
            // 
            textBoxSteamPassword.Location = new Point(205, 37);
            textBoxSteamPassword.Name = "textBoxSteamPassword";
            textBoxSteamPassword.PasswordChar = '*';
            textBoxSteamPassword.Size = new Size(167, 27);
            textBoxSteamPassword.TabIndex = 1;
            textBoxSteamPassword.TextChanged += Gui_textBoxSteamPassword_TextChanged;
            // 
            // richTextBoxInstallMessages
            // 
            richTextBoxInstallMessages.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxInstallMessages.Location = new Point(12, 165);
            richTextBoxInstallMessages.Name = "richTextBoxInstallMessages";
            richTextBoxInstallMessages.ReadOnly = true;
            richTextBoxInstallMessages.Size = new Size(758, 180);
            richTextBoxInstallMessages.TabIndex = 8;
            richTextBoxInstallMessages.Text = "";
            // 
            // labelInstall
            // 
            labelInstall.AutoSize = true;
            labelInstall.Location = new Point(12, 142);
            labelInstall.Name = "labelInstall";
            labelInstall.Size = new Size(145, 20);
            labelInstall.TabIndex = 9;
            labelInstall.Text = "Installation Progress:";
            // 
            // buttonInstall
            // 
            buttonInstall.Location = new Point(576, 392);
            buttonInstall.Name = "buttonInstall";
            buttonInstall.Size = new Size(94, 29);
            buttonInstall.TabIndex = 10;
            buttonInstall.Text = "Install";
            buttonInstall.UseVisualStyleBackColor = true;
            buttonInstall.Click += Gui_buttonInstall_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 351);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(758, 29);
            progressBar1.TabIndex = 11;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(676, 392);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(94, 29);
            buttonCancel.TabIndex = 12;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += Gui_buttonCancel_Click;
            // 
            // panelContentWrapper
            // 
            panelContentWrapper.Controls.Add(buttonCancel);
            panelContentWrapper.Controls.Add(progressBar1);
            panelContentWrapper.Controls.Add(buttonInstall);
            panelContentWrapper.Controls.Add(labelInstall);
            panelContentWrapper.Controls.Add(richTextBoxInstallMessages);
            panelContentWrapper.Controls.Add(splitContainer1);
            panelContentWrapper.Controls.Add(labelSteamPath);
            panelContentWrapper.Controls.Add(buttonSteamPath);
            panelContentWrapper.Controls.Add(textBoxSteamPath);
            panelContentWrapper.Dock = DockStyle.Fill;
            panelContentWrapper.Location = new Point(0, 0);
            panelContentWrapper.Name = "panelContentWrapper";
            panelContentWrapper.Size = new Size(782, 673);
            panelContentWrapper.TabIndex = 13;
            // 
            // SteamGameVersionSelectorForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(782, 673);
            Controls.Add(panelContentWrapper);
            MinimumSize = new Size(800, 480);
            Name = "SteamGameVersionSelectorForm";
            Text = "Steam Game Version Selector";
            Resize += Gui_window_Resize;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panelContentWrapper.ResumeLayout(false);
            panelContentWrapper.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox textBoxSteamPath;
        private Button buttonSteamPath;
        private Label labelSteamPath;
        private ComboBox comboBoxGameSelector;
        private Label labelPatchSelector;
        private Label labelGameSelector;
        private ComboBox comboBoxPatchSelector;
        private SplitContainer splitContainer1;
        private TextBox textBoxSteamPassword;
        private TextBox textBoxSteamUsername;
        private Label labelSteamPassword;
        private Label labelSteamUsername;
        private RichTextBox richTextBoxInstallMessages;
        private Label labelInstall;
        private Button buttonInstall;
        private ProgressBar progressBar1;
        private Button buttonCancel;
        private Panel panelContentWrapper;
        private FolderBrowserDialog folderBrowserDialogSteamPath;
    }
}
