namespace Loader_Friendly_FM_Packager;

sealed partial class MainForm
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
            this.SourceFMDirectoryTextBox = new System.Windows.Forms.TextBox();
            this.FMDirectoryBrowseButton = new System.Windows.Forms.Button();
            this.SourceFMDirectoryLabel = new System.Windows.Forms.Label();
            this.OutputArchiveTextBox = new System.Windows.Forms.TextBox();
            this.OutputArchiveLabel = new System.Windows.Forms.Label();
            this.CreateSingleArchiveButton = new System.Windows.Forms.Button();
            this.CompressionLevelComboBox = new System.Windows.Forms.ComboBox();
            this.CompressionLevelLabel = new System.Windows.Forms.Label();
            this.CompressionMethodComboBox = new System.Windows.Forms.ComboBox();
            this.CompressionMethodLabel = new System.Windows.Forms.Label();
            this.OutputArchiveBrowseButton = new System.Windows.Forms.Button();
            this.Test1Button = new System.Windows.Forms.Button();
            this.DictionarySizeLabel = new System.Windows.Forms.Label();
            this.DictionarySizeComboBox = new System.Windows.Forms.ComboBox();
            this.NumberOfCPUThreadsLabel = new System.Windows.Forms.Label();
            this.NumberOfCPUThreadsComboBox = new System.Windows.Forms.ComboBox();
            this.MemoryUsageForCompressingLabel = new System.Windows.Forms.Label();
            this.MemoryUsageForCompressingComboBox = new System.Windows.Forms.ComboBox();
            this.NumberOfCPUThreadsOutOfFLP = new System.Windows.Forms.FlowLayoutPanel();
            this.NumberOfCPUThreadsOutOfLabel = new System.Windows.Forms.Label();
            this.ProgressMessageLabel = new System.Windows.Forms.Label();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.StatusGroupBox = new System.Windows.Forms.GroupBox();
            this.MainProgressBar = new Loader_Friendly_FM_Packager.ProgressBarCustom();
            this.NumberOfCPUThreadsOutOfFLP.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.StatusGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // SourceFMDirectoryTextBox
            // 
            this.SourceFMDirectoryTextBox.Location = new System.Drawing.Point(16, 32);
            this.SourceFMDirectoryTextBox.Name = "SourceFMDirectoryTextBox";
            this.SourceFMDirectoryTextBox.Size = new System.Drawing.Size(640, 20);
            this.SourceFMDirectoryTextBox.TabIndex = 0;
            this.SourceFMDirectoryTextBox.Text = "J:\\_BlackParade_Blocks\\TheBlackParade_1.0";
            // 
            // FMDirectoryBrowseButton
            // 
            this.FMDirectoryBrowseButton.Location = new System.Drawing.Point(656, 31);
            this.FMDirectoryBrowseButton.Name = "FMDirectoryBrowseButton";
            this.FMDirectoryBrowseButton.Size = new System.Drawing.Size(81, 23);
            this.FMDirectoryBrowseButton.TabIndex = 1;
            this.FMDirectoryBrowseButton.Text = "Browse...";
            this.FMDirectoryBrowseButton.UseVisualStyleBackColor = true;
            this.FMDirectoryBrowseButton.Click += new System.EventHandler(this.FMDirectoryBrowseButton_Click);
            // 
            // SourceFMDirectoryLabel
            // 
            this.SourceFMDirectoryLabel.AutoSize = true;
            this.SourceFMDirectoryLabel.Location = new System.Drawing.Point(16, 16);
            this.SourceFMDirectoryLabel.Name = "SourceFMDirectoryLabel";
            this.SourceFMDirectoryLabel.Size = new System.Drawing.Size(105, 13);
            this.SourceFMDirectoryLabel.TabIndex = 2;
            this.SourceFMDirectoryLabel.Text = "Source FM directory:";
            // 
            // OutputArchiveTextBox
            // 
            this.OutputArchiveTextBox.Location = new System.Drawing.Point(16, 80);
            this.OutputArchiveTextBox.Name = "OutputArchiveTextBox";
            this.OutputArchiveTextBox.Size = new System.Drawing.Size(640, 20);
            this.OutputArchiveTextBox.TabIndex = 0;
            this.OutputArchiveTextBox.Text = "J:\\Local Storage NVME\\FMs\\__solid_blocks_test\\zz_test.7z";
            // 
            // OutputArchiveLabel
            // 
            this.OutputArchiveLabel.AutoSize = true;
            this.OutputArchiveLabel.Location = new System.Drawing.Point(16, 64);
            this.OutputArchiveLabel.Name = "OutputArchiveLabel";
            this.OutputArchiveLabel.Size = new System.Drawing.Size(80, 13);
            this.OutputArchiveLabel.TabIndex = 2;
            this.OutputArchiveLabel.Text = "Output archive:";
            // 
            // CreateSingleArchiveButton
            // 
            this.CreateSingleArchiveButton.Location = new System.Drawing.Point(637, 268);
            this.CreateSingleArchiveButton.Name = "CreateSingleArchiveButton";
            this.CreateSingleArchiveButton.Size = new System.Drawing.Size(100, 23);
            this.CreateSingleArchiveButton.TabIndex = 4;
            this.CreateSingleArchiveButton.Text = "Create 7-Zip file";
            this.CreateSingleArchiveButton.UseVisualStyleBackColor = true;
            this.CreateSingleArchiveButton.Click += new System.EventHandler(this.CreateSingleArchiveButton_Click);
            // 
            // CompressionLevelComboBox
            // 
            this.CompressionLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CompressionLevelComboBox.FormattingEnabled = true;
            this.CompressionLevelComboBox.Items.AddRange(new object[] {
            "0 - Store",
            "1 - Fastest",
            "2",
            "3 - Fast",
            "4",
            "5 - Normal",
            "6",
            "7 - Maximum",
            "8",
            "9 - Ultra"});
            this.CompressionLevelComboBox.Location = new System.Drawing.Point(175, 124);
            this.CompressionLevelComboBox.Name = "CompressionLevelComboBox";
            this.CompressionLevelComboBox.Size = new System.Drawing.Size(132, 21);
            this.CompressionLevelComboBox.TabIndex = 5;
            this.CompressionLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.CompressionLevelComboBox_SelectedIndexChanged);
            // 
            // CompressionLevelLabel
            // 
            this.CompressionLevelLabel.AutoSize = true;
            this.CompressionLevelLabel.Location = new System.Drawing.Point(16, 128);
            this.CompressionLevelLabel.Name = "CompressionLevelLabel";
            this.CompressionLevelLabel.Size = new System.Drawing.Size(95, 13);
            this.CompressionLevelLabel.TabIndex = 6;
            this.CompressionLevelLabel.Text = "Compression level:";
            // 
            // CompressionMethodComboBox
            // 
            this.CompressionMethodComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CompressionMethodComboBox.FormattingEnabled = true;
            this.CompressionMethodComboBox.Location = new System.Drawing.Point(175, 159);
            this.CompressionMethodComboBox.Name = "CompressionMethodComboBox";
            this.CompressionMethodComboBox.Size = new System.Drawing.Size(132, 21);
            this.CompressionMethodComboBox.TabIndex = 5;
            this.CompressionMethodComboBox.SelectedIndexChanged += new System.EventHandler(this.CompressionMethodComboBox_SelectedIndexChanged);
            // 
            // CompressionMethodLabel
            // 
            this.CompressionMethodLabel.AutoSize = true;
            this.CompressionMethodLabel.Location = new System.Drawing.Point(16, 163);
            this.CompressionMethodLabel.Name = "CompressionMethodLabel";
            this.CompressionMethodLabel.Size = new System.Drawing.Size(108, 13);
            this.CompressionMethodLabel.TabIndex = 6;
            this.CompressionMethodLabel.Text = "Compression method:";
            // 
            // OutputArchiveBrowseButton
            // 
            this.OutputArchiveBrowseButton.Location = new System.Drawing.Point(656, 79);
            this.OutputArchiveBrowseButton.Name = "OutputArchiveBrowseButton";
            this.OutputArchiveBrowseButton.Size = new System.Drawing.Size(81, 23);
            this.OutputArchiveBrowseButton.TabIndex = 1;
            this.OutputArchiveBrowseButton.Text = "Browse...";
            this.OutputArchiveBrowseButton.UseVisualStyleBackColor = true;
            this.OutputArchiveBrowseButton.Click += new System.EventHandler(this.OutputArchiveBrowseButton_Click);
            // 
            // Test1Button
            // 
            this.Test1Button.Location = new System.Drawing.Point(656, 120);
            this.Test1Button.Name = "Test1Button";
            this.Test1Button.Size = new System.Drawing.Size(81, 23);
            this.Test1Button.TabIndex = 8;
            this.Test1Button.Text = "Test1";
            this.Test1Button.UseVisualStyleBackColor = true;
            this.Test1Button.Click += new System.EventHandler(this.Test1Button_Click);
            // 
            // DictionarySizeLabel
            // 
            this.DictionarySizeLabel.AutoSize = true;
            this.DictionarySizeLabel.Location = new System.Drawing.Point(16, 198);
            this.DictionarySizeLabel.Name = "DictionarySizeLabel";
            this.DictionarySizeLabel.Size = new System.Drawing.Size(78, 13);
            this.DictionarySizeLabel.TabIndex = 10;
            this.DictionarySizeLabel.Text = "Dictionary size:";
            // 
            // DictionarySizeComboBox
            // 
            this.DictionarySizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DictionarySizeComboBox.FormattingEnabled = true;
            this.DictionarySizeComboBox.Location = new System.Drawing.Point(175, 194);
            this.DictionarySizeComboBox.Name = "DictionarySizeComboBox";
            this.DictionarySizeComboBox.Size = new System.Drawing.Size(132, 21);
            this.DictionarySizeComboBox.TabIndex = 9;
            this.DictionarySizeComboBox.SelectedIndexChanged += new System.EventHandler(this.DictionarySizeComboBox_SelectedIndexChanged);
            // 
            // NumberOfCPUThreadsLabel
            // 
            this.NumberOfCPUThreadsLabel.AutoSize = true;
            this.NumberOfCPUThreadsLabel.Location = new System.Drawing.Point(16, 233);
            this.NumberOfCPUThreadsLabel.Name = "NumberOfCPUThreadsLabel";
            this.NumberOfCPUThreadsLabel.Size = new System.Drawing.Size(122, 13);
            this.NumberOfCPUThreadsLabel.TabIndex = 16;
            this.NumberOfCPUThreadsLabel.Text = "Number of CPU threads:";
            // 
            // NumberOfCPUThreadsComboBox
            // 
            this.NumberOfCPUThreadsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NumberOfCPUThreadsComboBox.FormattingEnabled = true;
            this.NumberOfCPUThreadsComboBox.Location = new System.Drawing.Point(175, 229);
            this.NumberOfCPUThreadsComboBox.Name = "NumberOfCPUThreadsComboBox";
            this.NumberOfCPUThreadsComboBox.Size = new System.Drawing.Size(80, 21);
            this.NumberOfCPUThreadsComboBox.TabIndex = 15;
            this.NumberOfCPUThreadsComboBox.SelectedIndexChanged += new System.EventHandler(this.NumberOfCPUThreadsComboBox_SelectedIndexChanged);
            // 
            // MemoryUsageForCompressingLabel
            // 
            this.MemoryUsageForCompressingLabel.AutoSize = true;
            this.MemoryUsageForCompressingLabel.Location = new System.Drawing.Point(16, 268);
            this.MemoryUsageForCompressingLabel.Name = "MemoryUsageForCompressingLabel";
            this.MemoryUsageForCompressingLabel.Size = new System.Drawing.Size(157, 13);
            this.MemoryUsageForCompressingLabel.TabIndex = 18;
            this.MemoryUsageForCompressingLabel.Text = "Memory usage for Compressing:";
            // 
            // MemoryUsageForCompressingComboBox
            // 
            this.MemoryUsageForCompressingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MemoryUsageForCompressingComboBox.FormattingEnabled = true;
            this.MemoryUsageForCompressingComboBox.Location = new System.Drawing.Point(176, 264);
            this.MemoryUsageForCompressingComboBox.Name = "MemoryUsageForCompressingComboBox";
            this.MemoryUsageForCompressingComboBox.Size = new System.Drawing.Size(131, 21);
            this.MemoryUsageForCompressingComboBox.TabIndex = 17;
            this.MemoryUsageForCompressingComboBox.SelectedIndexChanged += new System.EventHandler(this.MemoryUsageForCompressingComboBox_SelectedIndexChanged);
            // 
            // NumberOfCPUThreadsOutOfFLP
            // 
            this.NumberOfCPUThreadsOutOfFLP.Controls.Add(this.NumberOfCPUThreadsOutOfLabel);
            this.NumberOfCPUThreadsOutOfFLP.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.NumberOfCPUThreadsOutOfFLP.Location = new System.Drawing.Point(256, 232);
            this.NumberOfCPUThreadsOutOfFLP.Name = "NumberOfCPUThreadsOutOfFLP";
            this.NumberOfCPUThreadsOutOfFLP.Size = new System.Drawing.Size(52, 16);
            this.NumberOfCPUThreadsOutOfFLP.TabIndex = 20;
            // 
            // NumberOfCPUThreadsOutOfLabel
            // 
            this.NumberOfCPUThreadsOutOfLabel.AutoSize = true;
            this.NumberOfCPUThreadsOutOfLabel.Location = new System.Drawing.Point(25, 0);
            this.NumberOfCPUThreadsOutOfLabel.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.NumberOfCPUThreadsOutOfLabel.Name = "NumberOfCPUThreadsOutOfLabel";
            this.NumberOfCPUThreadsOutOfLabel.Size = new System.Drawing.Size(27, 13);
            this.NumberOfCPUThreadsOutOfLabel.TabIndex = 18;
            this.NumberOfCPUThreadsOutOfLabel.Text = "/ 12";
            // 
            // ProgressMessageLabel
            // 
            this.ProgressMessageLabel.AutoSize = true;
            this.ProgressMessageLabel.Location = new System.Drawing.Point(16, 24);
            this.ProgressMessageLabel.Name = "ProgressMessageLabel";
            this.ProgressMessageLabel.Size = new System.Drawing.Size(97, 13);
            this.ProgressMessageLabel.TabIndex = 22;
            this.ProgressMessageLabel.Text = "[ProgressMessage]";
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Location = new System.Drawing.Point(328, 78);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_Button.TabIndex = 23;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Visible = false;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.SourceFMDirectoryLabel);
            this.MainPanel.Controls.Add(this.SourceFMDirectoryTextBox);
            this.MainPanel.Controls.Add(this.OutputArchiveTextBox);
            this.MainPanel.Controls.Add(this.FMDirectoryBrowseButton);
            this.MainPanel.Controls.Add(this.NumberOfCPUThreadsOutOfFLP);
            this.MainPanel.Controls.Add(this.OutputArchiveBrowseButton);
            this.MainPanel.Controls.Add(this.MemoryUsageForCompressingLabel);
            this.MainPanel.Controls.Add(this.OutputArchiveLabel);
            this.MainPanel.Controls.Add(this.MemoryUsageForCompressingComboBox);
            this.MainPanel.Controls.Add(this.CreateSingleArchiveButton);
            this.MainPanel.Controls.Add(this.NumberOfCPUThreadsLabel);
            this.MainPanel.Controls.Add(this.CompressionLevelComboBox);
            this.MainPanel.Controls.Add(this.NumberOfCPUThreadsComboBox);
            this.MainPanel.Controls.Add(this.CompressionLevelLabel);
            this.MainPanel.Controls.Add(this.DictionarySizeLabel);
            this.MainPanel.Controls.Add(this.CompressionMethodComboBox);
            this.MainPanel.Controls.Add(this.DictionarySizeComboBox);
            this.MainPanel.Controls.Add(this.CompressionMethodLabel);
            this.MainPanel.Controls.Add(this.Test1Button);
            this.MainPanel.Location = new System.Drawing.Point(0, 0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(752, 304);
            this.MainPanel.TabIndex = 24;
            // 
            // StatusGroupBox
            // 
            this.StatusGroupBox.Controls.Add(this.ProgressMessageLabel);
            this.StatusGroupBox.Controls.Add(this.MainProgressBar);
            this.StatusGroupBox.Controls.Add(this.Cancel_Button);
            this.StatusGroupBox.Location = new System.Drawing.Point(8, 307);
            this.StatusGroupBox.Name = "StatusGroupBox";
            this.StatusGroupBox.Size = new System.Drawing.Size(736, 112);
            this.StatusGroupBox.TabIndex = 25;
            this.StatusGroupBox.TabStop = false;
            this.StatusGroupBox.Text = "Status";
            // 
            // MainProgressBar
            // 
            this.MainProgressBar.Location = new System.Drawing.Point(16, 48);
            this.MainProgressBar.Name = "MainProgressBar";
            this.MainProgressBar.Size = new System.Drawing.Size(704, 23);
            this.MainProgressBar.TabIndex = 21;
            this.MainProgressBar.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 427);
            this.Controls.Add(this.StatusGroupBox);
            this.Controls.Add(this.MainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Loader-Friendly FM Packager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.NumberOfCPUThreadsOutOfFLP.ResumeLayout(false);
            this.NumberOfCPUThreadsOutOfFLP.PerformLayout();
            this.MainPanel.ResumeLayout(false);
            this.MainPanel.PerformLayout();
            this.StatusGroupBox.ResumeLayout(false);
            this.StatusGroupBox.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox SourceFMDirectoryTextBox;
    private System.Windows.Forms.Button FMDirectoryBrowseButton;
    private System.Windows.Forms.Label SourceFMDirectoryLabel;
    private System.Windows.Forms.TextBox OutputArchiveTextBox;
    private System.Windows.Forms.Label OutputArchiveLabel;
    private System.Windows.Forms.Button CreateSingleArchiveButton;
    private System.Windows.Forms.ComboBox CompressionLevelComboBox;
    private System.Windows.Forms.Label CompressionLevelLabel;
    private System.Windows.Forms.ComboBox CompressionMethodComboBox;
    private System.Windows.Forms.Label CompressionMethodLabel;
    private System.Windows.Forms.Button OutputArchiveBrowseButton;
    private System.Windows.Forms.Button Test1Button;
    private System.Windows.Forms.Label DictionarySizeLabel;
    private System.Windows.Forms.ComboBox DictionarySizeComboBox;
    private System.Windows.Forms.Label NumberOfCPUThreadsLabel;
    private System.Windows.Forms.ComboBox NumberOfCPUThreadsComboBox;
    private System.Windows.Forms.Label MemoryUsageForCompressingLabel;
    private System.Windows.Forms.ComboBox MemoryUsageForCompressingComboBox;
    private System.Windows.Forms.FlowLayoutPanel NumberOfCPUThreadsOutOfFLP;
    private System.Windows.Forms.Label NumberOfCPUThreadsOutOfLabel;
    private ProgressBarCustom MainProgressBar;
    private System.Windows.Forms.Label ProgressMessageLabel;
    private System.Windows.Forms.Button Cancel_Button;
    private System.Windows.Forms.Panel MainPanel;
    private System.Windows.Forms.GroupBox StatusGroupBox;
}
