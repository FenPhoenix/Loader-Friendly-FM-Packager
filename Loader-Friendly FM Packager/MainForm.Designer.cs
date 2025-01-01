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
            this.GoButton = new System.Windows.Forms.Button();
            this.CompressionLevelComboBox = new System.Windows.Forms.ComboBox();
            this.CompressionLevelLabel = new System.Windows.Forms.Label();
            this.CompressionMethodComboBox = new System.Windows.Forms.ComboBox();
            this.CompressionMethodLabel = new System.Windows.Forms.Label();
            this.OutputArchiveBrowseButton = new System.Windows.Forms.Button();
            this.SevenZipExePanel = new System.Windows.Forms.Panel();
            this.SevenZipAppToUseLabel = new System.Windows.Forms.Label();
            this.SevenZipExternalBrowseButton = new System.Windows.Forms.Button();
            this.SevenZipExternalTextBox = new System.Windows.Forms.TextBox();
            this.SevenZipExternalRadioButton = new System.Windows.Forms.RadioButton();
            this.SevenZipInternalRadioButton = new System.Windows.Forms.RadioButton();
            this.Test1Button = new System.Windows.Forms.Button();
            this.DictionarySizeLabel = new System.Windows.Forms.Label();
            this.DictionarySizeComboBox = new System.Windows.Forms.ComboBox();
            this.NumberOfCPUThreadsLabel = new System.Windows.Forms.Label();
            this.NumberOfCPUThreadsComboBox = new System.Windows.Forms.ComboBox();
            this.MemoryUsageForCompressingLabel = new System.Windows.Forms.Label();
            this.MemoryUsageForCompressingComboBox = new System.Windows.Forms.ComboBox();
            this.NumberOfCPUThreadsOutOfFLP = new System.Windows.Forms.FlowLayoutPanel();
            this.NumberOfCPUThreadsOutOfLabel = new System.Windows.Forms.Label();
            this.TimeGroupBox = new System.Windows.Forms.GroupBox();
            this.SetArchiveTimeToLatestFileTimeCheckBox = new System.Windows.Forms.CheckBox();
            this.StoreLastAccessTimeCheckBox = new System.Windows.Forms.CheckBox();
            this.StoreCreationTimeCheckBox = new System.Windows.Forms.CheckBox();
            this.StoreModificationTimeCheckBox = new System.Windows.Forms.CheckBox();
            this.DoNotChangeSourceFilesLastAccessTimeCheckBox = new System.Windows.Forms.CheckBox();
            this.SetArchiveTimeToLatestFileTimeEnableSettingCheckBox = new System.Windows.Forms.CheckBox();
            this.StoreLastAccessTimeEnableSettingCheckBox = new System.Windows.Forms.CheckBox();
            this.StoreCreationTimeEnableSettingCheckBox = new System.Windows.Forms.CheckBox();
            this.SevenZipExePanel.SuspendLayout();
            this.NumberOfCPUThreadsOutOfFLP.SuspendLayout();
            this.TimeGroupBox.SuspendLayout();
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
            this.FMDirectoryBrowseButton.Size = new System.Drawing.Size(80, 23);
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
            // GoButton
            // 
            this.GoButton.Location = new System.Drawing.Point(640, 512);
            this.GoButton.Name = "GoButton";
            this.GoButton.Size = new System.Drawing.Size(99, 23);
            this.GoButton.TabIndex = 4;
            this.GoButton.Text = "Create 7-Zip file";
            this.GoButton.UseVisualStyleBackColor = true;
            this.GoButton.Click += new System.EventHandler(this.GoButton_Click);
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
            this.OutputArchiveBrowseButton.Size = new System.Drawing.Size(80, 23);
            this.OutputArchiveBrowseButton.TabIndex = 1;
            this.OutputArchiveBrowseButton.Text = "Browse...";
            this.OutputArchiveBrowseButton.UseVisualStyleBackColor = true;
            this.OutputArchiveBrowseButton.Click += new System.EventHandler(this.OutputArchiveBrowseButton_Click);
            // 
            // SevenZipExePanel
            // 
            this.SevenZipExePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SevenZipExePanel.Controls.Add(this.SevenZipAppToUseLabel);
            this.SevenZipExePanel.Controls.Add(this.SevenZipExternalBrowseButton);
            this.SevenZipExePanel.Controls.Add(this.SevenZipExternalTextBox);
            this.SevenZipExePanel.Controls.Add(this.SevenZipExternalRadioButton);
            this.SevenZipExePanel.Controls.Add(this.SevenZipInternalRadioButton);
            this.SevenZipExePanel.Location = new System.Drawing.Point(0, 553);
            this.SevenZipExePanel.Name = "SevenZipExePanel";
            this.SevenZipExePanel.Size = new System.Drawing.Size(752, 112);
            this.SevenZipExePanel.TabIndex = 7;
            // 
            // SevenZipAppToUseLabel
            // 
            this.SevenZipAppToUseLabel.AutoSize = true;
            this.SevenZipAppToUseLabel.Location = new System.Drawing.Point(16, 8);
            this.SevenZipAppToUseLabel.Name = "SevenZipAppToUseLabel";
            this.SevenZipAppToUseLabel.Size = new System.Drawing.Size(87, 13);
            this.SevenZipAppToUseLabel.TabIndex = 3;
            this.SevenZipAppToUseLabel.Text = "7-Zip app to use:";
            // 
            // SevenZipExternalBrowseButton
            // 
            this.SevenZipExternalBrowseButton.Location = new System.Drawing.Point(656, 79);
            this.SevenZipExternalBrowseButton.Name = "SevenZipExternalBrowseButton";
            this.SevenZipExternalBrowseButton.Size = new System.Drawing.Size(80, 23);
            this.SevenZipExternalBrowseButton.TabIndex = 2;
            this.SevenZipExternalBrowseButton.Text = "Browse...";
            this.SevenZipExternalBrowseButton.UseVisualStyleBackColor = true;
            this.SevenZipExternalBrowseButton.Click += new System.EventHandler(this.SevenZipExternalBrowseButton_Click);
            // 
            // SevenZipExternalTextBox
            // 
            this.SevenZipExternalTextBox.Location = new System.Drawing.Point(16, 80);
            this.SevenZipExternalTextBox.Name = "SevenZipExternalTextBox";
            this.SevenZipExternalTextBox.Size = new System.Drawing.Size(640, 20);
            this.SevenZipExternalTextBox.TabIndex = 1;
            this.SevenZipExternalTextBox.TextChanged += new System.EventHandler(this.SevenZipExternalTextBox_TextChanged);
            this.SevenZipExternalTextBox.Leave += new System.EventHandler(this.SevenZipExternalTextBox_Leave);
            // 
            // SevenZipExternalRadioButton
            // 
            this.SevenZipExternalRadioButton.AutoSize = true;
            this.SevenZipExternalRadioButton.Location = new System.Drawing.Point(16, 56);
            this.SevenZipExternalRadioButton.Name = "SevenZipExternalRadioButton";
            this.SevenZipExternalRadioButton.Size = new System.Drawing.Size(66, 17);
            this.SevenZipExternalRadioButton.TabIndex = 0;
            this.SevenZipExternalRadioButton.Text = "External:";
            this.SevenZipExternalRadioButton.UseVisualStyleBackColor = true;
            this.SevenZipExternalRadioButton.CheckedChanged += new System.EventHandler(this.SevenZipExeRadioButtons_CheckedChanged);
            // 
            // SevenZipInternalRadioButton
            // 
            this.SevenZipInternalRadioButton.AutoSize = true;
            this.SevenZipInternalRadioButton.Checked = true;
            this.SevenZipInternalRadioButton.Location = new System.Drawing.Point(16, 33);
            this.SevenZipInternalRadioButton.Name = "SevenZipInternalRadioButton";
            this.SevenZipInternalRadioButton.Size = new System.Drawing.Size(60, 17);
            this.SevenZipInternalRadioButton.TabIndex = 0;
            this.SevenZipInternalRadioButton.TabStop = true;
            this.SevenZipInternalRadioButton.Text = "Internal";
            this.SevenZipInternalRadioButton.UseVisualStyleBackColor = true;
            this.SevenZipInternalRadioButton.CheckedChanged += new System.EventHandler(this.SevenZipExeRadioButtons_CheckedChanged);
            // 
            // Test1Button
            // 
            this.Test1Button.Location = new System.Drawing.Point(664, 440);
            this.Test1Button.Name = "Test1Button";
            this.Test1Button.Size = new System.Drawing.Size(75, 23);
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
            // TimeGroupBox
            // 
            this.TimeGroupBox.Controls.Add(this.SetArchiveTimeToLatestFileTimeCheckBox);
            this.TimeGroupBox.Controls.Add(this.StoreLastAccessTimeCheckBox);
            this.TimeGroupBox.Controls.Add(this.StoreCreationTimeCheckBox);
            this.TimeGroupBox.Controls.Add(this.StoreModificationTimeCheckBox);
            this.TimeGroupBox.Controls.Add(this.DoNotChangeSourceFilesLastAccessTimeCheckBox);
            this.TimeGroupBox.Controls.Add(this.SetArchiveTimeToLatestFileTimeEnableSettingCheckBox);
            this.TimeGroupBox.Controls.Add(this.StoreLastAccessTimeEnableSettingCheckBox);
            this.TimeGroupBox.Controls.Add(this.StoreCreationTimeEnableSettingCheckBox);
            this.TimeGroupBox.Location = new System.Drawing.Point(320, 128);
            this.TimeGroupBox.Name = "TimeGroupBox";
            this.TimeGroupBox.Size = new System.Drawing.Size(360, 168);
            this.TimeGroupBox.TabIndex = 21;
            this.TimeGroupBox.TabStop = false;
            this.TimeGroupBox.Text = "Time";
            // 
            // SetArchiveTimeToLatestFileTimeCheckBox
            // 
            this.SetArchiveTimeToLatestFileTimeCheckBox.AutoSize = true;
            this.SetArchiveTimeToLatestFileTimeCheckBox.Enabled = false;
            this.SetArchiveTimeToLatestFileTimeCheckBox.Location = new System.Drawing.Point(39, 104);
            this.SetArchiveTimeToLatestFileTimeCheckBox.Name = "SetArchiveTimeToLatestFileTimeCheckBox";
            this.SetArchiveTimeToLatestFileTimeCheckBox.Size = new System.Drawing.Size(180, 17);
            this.SetArchiveTimeToLatestFileTimeCheckBox.TabIndex = 0;
            this.SetArchiveTimeToLatestFileTimeCheckBox.Text = "Set archive time to latest file time";
            this.SetArchiveTimeToLatestFileTimeCheckBox.UseVisualStyleBackColor = true;
            this.SetArchiveTimeToLatestFileTimeCheckBox.CheckedChanged += new System.EventHandler(this.SetArchiveTimeToLatestFileTimeCheckBox_CheckedChanged);
            // 
            // StoreLastAccessTimeCheckBox
            // 
            this.StoreLastAccessTimeCheckBox.AutoSize = true;
            this.StoreLastAccessTimeCheckBox.Enabled = false;
            this.StoreLastAccessTimeCheckBox.Location = new System.Drawing.Point(39, 73);
            this.StoreLastAccessTimeCheckBox.Name = "StoreLastAccessTimeCheckBox";
            this.StoreLastAccessTimeCheckBox.Size = new System.Drawing.Size(129, 17);
            this.StoreLastAccessTimeCheckBox.TabIndex = 0;
            this.StoreLastAccessTimeCheckBox.Text = "Store last access time";
            this.StoreLastAccessTimeCheckBox.UseVisualStyleBackColor = true;
            this.StoreLastAccessTimeCheckBox.CheckedChanged += new System.EventHandler(this.StoreLastAccessTimeCheckBox_CheckedChanged);
            // 
            // StoreCreationTimeCheckBox
            // 
            this.StoreCreationTimeCheckBox.AutoSize = true;
            this.StoreCreationTimeCheckBox.Enabled = false;
            this.StoreCreationTimeCheckBox.Location = new System.Drawing.Point(39, 50);
            this.StoreCreationTimeCheckBox.Name = "StoreCreationTimeCheckBox";
            this.StoreCreationTimeCheckBox.Size = new System.Drawing.Size(114, 17);
            this.StoreCreationTimeCheckBox.TabIndex = 0;
            this.StoreCreationTimeCheckBox.Text = "Store creation time";
            this.StoreCreationTimeCheckBox.UseVisualStyleBackColor = true;
            this.StoreCreationTimeCheckBox.CheckedChanged += new System.EventHandler(this.StoreCreationTimeCheckBox_CheckedChanged);
            // 
            // StoreModificationTimeCheckBox
            // 
            this.StoreModificationTimeCheckBox.AutoSize = true;
            this.StoreModificationTimeCheckBox.Checked = true;
            this.StoreModificationTimeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StoreModificationTimeCheckBox.Enabled = false;
            this.StoreModificationTimeCheckBox.Location = new System.Drawing.Point(39, 27);
            this.StoreModificationTimeCheckBox.Name = "StoreModificationTimeCheckBox";
            this.StoreModificationTimeCheckBox.Size = new System.Drawing.Size(132, 17);
            this.StoreModificationTimeCheckBox.TabIndex = 0;
            this.StoreModificationTimeCheckBox.Text = "Store modification time";
            this.StoreModificationTimeCheckBox.UseVisualStyleBackColor = true;
            // 
            // DoNotChangeSourceFilesLastAccessTimeCheckBox
            // 
            this.DoNotChangeSourceFilesLastAccessTimeCheckBox.AutoSize = true;
            this.DoNotChangeSourceFilesLastAccessTimeCheckBox.Location = new System.Drawing.Point(12, 136);
            this.DoNotChangeSourceFilesLastAccessTimeCheckBox.Name = "DoNotChangeSourceFilesLastAccessTimeCheckBox";
            this.DoNotChangeSourceFilesLastAccessTimeCheckBox.Size = new System.Drawing.Size(231, 17);
            this.DoNotChangeSourceFilesLastAccessTimeCheckBox.TabIndex = 0;
            this.DoNotChangeSourceFilesLastAccessTimeCheckBox.Text = "Do not change source files last access time";
            this.DoNotChangeSourceFilesLastAccessTimeCheckBox.UseVisualStyleBackColor = true;
            this.DoNotChangeSourceFilesLastAccessTimeCheckBox.CheckedChanged += new System.EventHandler(this.DoNotChangeSourceFilesLastAccessTimeCheckBox_CheckedChanged);
            // 
            // SetArchiveTimeToLatestFileTimeEnableSettingCheckBox
            // 
            this.SetArchiveTimeToLatestFileTimeEnableSettingCheckBox.AutoSize = true;
            this.SetArchiveTimeToLatestFileTimeEnableSettingCheckBox.Location = new System.Drawing.Point(12, 104);
            this.SetArchiveTimeToLatestFileTimeEnableSettingCheckBox.Name = "SetArchiveTimeToLatestFileTimeEnableSettingCheckBox";
            this.SetArchiveTimeToLatestFileTimeEnableSettingCheckBox.Size = new System.Drawing.Size(29, 17);
            this.SetArchiveTimeToLatestFileTimeEnableSettingCheckBox.TabIndex = 0;
            this.SetArchiveTimeToLatestFileTimeEnableSettingCheckBox.Text = ":";
            this.SetArchiveTimeToLatestFileTimeEnableSettingCheckBox.UseVisualStyleBackColor = true;
            this.SetArchiveTimeToLatestFileTimeEnableSettingCheckBox.CheckedChanged += new System.EventHandler(this.SetArchiveTimeToLatestFileTimeEnableSettingCheckBox_CheckedChanged);
            // 
            // StoreLastAccessTimeEnableSettingCheckBox
            // 
            this.StoreLastAccessTimeEnableSettingCheckBox.AutoSize = true;
            this.StoreLastAccessTimeEnableSettingCheckBox.Location = new System.Drawing.Point(12, 73);
            this.StoreLastAccessTimeEnableSettingCheckBox.Name = "StoreLastAccessTimeEnableSettingCheckBox";
            this.StoreLastAccessTimeEnableSettingCheckBox.Size = new System.Drawing.Size(29, 17);
            this.StoreLastAccessTimeEnableSettingCheckBox.TabIndex = 0;
            this.StoreLastAccessTimeEnableSettingCheckBox.Text = ":";
            this.StoreLastAccessTimeEnableSettingCheckBox.UseVisualStyleBackColor = true;
            this.StoreLastAccessTimeEnableSettingCheckBox.CheckedChanged += new System.EventHandler(this.StoreLastAccessTimeEnableSettingCheckBox_CheckedChanged);
            // 
            // StoreCreationTimeEnableSettingCheckBox
            // 
            this.StoreCreationTimeEnableSettingCheckBox.AutoSize = true;
            this.StoreCreationTimeEnableSettingCheckBox.Location = new System.Drawing.Point(12, 50);
            this.StoreCreationTimeEnableSettingCheckBox.Name = "StoreCreationTimeEnableSettingCheckBox";
            this.StoreCreationTimeEnableSettingCheckBox.Size = new System.Drawing.Size(29, 17);
            this.StoreCreationTimeEnableSettingCheckBox.TabIndex = 0;
            this.StoreCreationTimeEnableSettingCheckBox.Text = ":";
            this.StoreCreationTimeEnableSettingCheckBox.UseVisualStyleBackColor = true;
            this.StoreCreationTimeEnableSettingCheckBox.CheckedChanged += new System.EventHandler(this.StoreCreationTimeEnableSettingCheckBox_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 665);
            this.Controls.Add(this.TimeGroupBox);
            this.Controls.Add(this.NumberOfCPUThreadsOutOfFLP);
            this.Controls.Add(this.MemoryUsageForCompressingLabel);
            this.Controls.Add(this.MemoryUsageForCompressingComboBox);
            this.Controls.Add(this.NumberOfCPUThreadsLabel);
            this.Controls.Add(this.NumberOfCPUThreadsComboBox);
            this.Controls.Add(this.DictionarySizeLabel);
            this.Controls.Add(this.DictionarySizeComboBox);
            this.Controls.Add(this.Test1Button);
            this.Controls.Add(this.SevenZipExePanel);
            this.Controls.Add(this.CompressionMethodLabel);
            this.Controls.Add(this.CompressionMethodComboBox);
            this.Controls.Add(this.CompressionLevelLabel);
            this.Controls.Add(this.CompressionLevelComboBox);
            this.Controls.Add(this.GoButton);
            this.Controls.Add(this.OutputArchiveLabel);
            this.Controls.Add(this.SourceFMDirectoryLabel);
            this.Controls.Add(this.OutputArchiveBrowseButton);
            this.Controls.Add(this.FMDirectoryBrowseButton);
            this.Controls.Add(this.OutputArchiveTextBox);
            this.Controls.Add(this.SourceFMDirectoryTextBox);
            this.Name = "MainForm";
            this.Text = "Loader-Friendly FM Packager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.SevenZipExePanel.ResumeLayout(false);
            this.SevenZipExePanel.PerformLayout();
            this.NumberOfCPUThreadsOutOfFLP.ResumeLayout(false);
            this.NumberOfCPUThreadsOutOfFLP.PerformLayout();
            this.TimeGroupBox.ResumeLayout(false);
            this.TimeGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox SourceFMDirectoryTextBox;
    private System.Windows.Forms.Button FMDirectoryBrowseButton;
    private System.Windows.Forms.Label SourceFMDirectoryLabel;
    private System.Windows.Forms.TextBox OutputArchiveTextBox;
    private System.Windows.Forms.Label OutputArchiveLabel;
    private System.Windows.Forms.Button GoButton;
    private System.Windows.Forms.ComboBox CompressionLevelComboBox;
    private System.Windows.Forms.Label CompressionLevelLabel;
    private System.Windows.Forms.ComboBox CompressionMethodComboBox;
    private System.Windows.Forms.Label CompressionMethodLabel;
    private System.Windows.Forms.Button OutputArchiveBrowseButton;
    private System.Windows.Forms.Panel SevenZipExePanel;
    private System.Windows.Forms.RadioButton SevenZipExternalRadioButton;
    private System.Windows.Forms.RadioButton SevenZipInternalRadioButton;
    private System.Windows.Forms.Button SevenZipExternalBrowseButton;
    private System.Windows.Forms.TextBox SevenZipExternalTextBox;
    private System.Windows.Forms.Label SevenZipAppToUseLabel;
    private System.Windows.Forms.Button Test1Button;
    private System.Windows.Forms.Label DictionarySizeLabel;
    private System.Windows.Forms.ComboBox DictionarySizeComboBox;
    private System.Windows.Forms.Label NumberOfCPUThreadsLabel;
    private System.Windows.Forms.ComboBox NumberOfCPUThreadsComboBox;
    private System.Windows.Forms.Label MemoryUsageForCompressingLabel;
    private System.Windows.Forms.ComboBox MemoryUsageForCompressingComboBox;
    private System.Windows.Forms.FlowLayoutPanel NumberOfCPUThreadsOutOfFLP;
    private System.Windows.Forms.Label NumberOfCPUThreadsOutOfLabel;
    private System.Windows.Forms.GroupBox TimeGroupBox;
    private System.Windows.Forms.CheckBox DoNotChangeSourceFilesLastAccessTimeCheckBox;
    private System.Windows.Forms.CheckBox SetArchiveTimeToLatestFileTimeEnableSettingCheckBox;
    private System.Windows.Forms.CheckBox StoreLastAccessTimeEnableSettingCheckBox;
    private System.Windows.Forms.CheckBox StoreCreationTimeEnableSettingCheckBox;
    private System.Windows.Forms.CheckBox StoreModificationTimeCheckBox;
    private System.Windows.Forms.CheckBox SetArchiveTimeToLatestFileTimeCheckBox;
    private System.Windows.Forms.CheckBox StoreLastAccessTimeCheckBox;
    private System.Windows.Forms.CheckBox StoreCreationTimeCheckBox;
}
