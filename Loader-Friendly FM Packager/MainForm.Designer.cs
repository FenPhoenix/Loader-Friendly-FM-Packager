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
            this.SevenZipExePanel.SuspendLayout();
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
            this.GoButton.Location = new System.Drawing.Point(640, 192);
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
            this.CompressionLevelComboBox.Size = new System.Drawing.Size(144, 21);
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
            this.CompressionMethodComboBox.Location = new System.Drawing.Point(175, 164);
            this.CompressionMethodComboBox.Name = "CompressionMethodComboBox";
            this.CompressionMethodComboBox.Size = new System.Drawing.Size(144, 21);
            this.CompressionMethodComboBox.TabIndex = 5;
            this.CompressionMethodComboBox.SelectedIndexChanged += new System.EventHandler(this.CompressionMethodComboBox_SelectedIndexChanged);
            // 
            // CompressionMethodLabel
            // 
            this.CompressionMethodLabel.AutoSize = true;
            this.CompressionMethodLabel.Location = new System.Drawing.Point(16, 168);
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
            this.SevenZipExePanel.Controls.Add(this.SevenZipAppToUseLabel);
            this.SevenZipExePanel.Controls.Add(this.SevenZipExternalBrowseButton);
            this.SevenZipExePanel.Controls.Add(this.SevenZipExternalTextBox);
            this.SevenZipExePanel.Controls.Add(this.SevenZipExternalRadioButton);
            this.SevenZipExePanel.Controls.Add(this.SevenZipInternalRadioButton);
            this.SevenZipExePanel.Location = new System.Drawing.Point(0, 248);
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
            this.Test1Button.Location = new System.Drawing.Point(664, 120);
            this.Test1Button.Name = "Test1Button";
            this.Test1Button.Size = new System.Drawing.Size(75, 23);
            this.Test1Button.TabIndex = 8;
            this.Test1Button.Text = "Test1";
            this.Test1Button.UseVisualStyleBackColor = true;
            this.Test1Button.Click += new System.EventHandler(this.Test1Button_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 360);
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
}
