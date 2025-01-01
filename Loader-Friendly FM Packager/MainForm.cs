using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Loader_Friendly_FM_Packager;

/*
TODO: We'll be adding to the archive with every call, so make sure it doesn't exist beforehand and/or delete
 it first or something

TODO: When re-packing from a zip file, notify the user when an unknown-encoded filename comes up (where it would
 normally use "Default" encoding). Show the user the filename in various encodings and allow them to choose what
 it should be.
 We'll need to pull in the .NET zip code so as to customize it to expose the "used default encoding" situation.

TODO: Match 7z 2409's logic for calculating UI values

--

TODO: Mod.ini specifies a readme and icon file.
 There are no icon files in any SS2 FMs in the set that I have. But we could put it in the logic anyway.

TODO: GarrettLoader grabs the intrface/Main_*.pcx file(s). Main_1.pcx for static display at the top of the readme,
 and the rest to show the animation at the bottom of the window.
 It only supports pcx files. But NewDark can have .png and .tga and who knows what else.
 AFAIK only GarrettLoader uses these, but if we wanted to try to future-proof then we COULD add these to the logic.
 We could put them all in one block to avoid too much dilution of solid archives.

TODO: Re-run the 7z-scan-friendly convert with the final logic

--

TODO: Act on caught exceptions and add robust error handling everywhere

TODO: Add built-in progress reporting like AngelLoader, copy-paste the Fen7z code etc.
*/

public sealed partial class MainForm : Form
{
    public string SourceFMDir
    {
        get => SourceFMDirectoryTextBox.Text;
        set => SourceFMDirectoryTextBox.Text = value;
    }

    public string OutputArchive
    {
        get => OutputArchiveTextBox.Text;
        set => OutputArchiveTextBox.Text = value;
    }

    public int CompressionLevel
    {
        get => CompressionLevelComboBox.SelectedIndex;
        set => CompressionLevelComboBox.SelectedIndex = CompressionLevelComboBox.IndexIsInRange(value)
            ? value
            : 0;
    }

    public CompressionMethod CompressionMethod
    {
        get => CompressionMethodItems[CompressionMethodComboBox.SelectedIndex].BackingValue;
        set
        {
            int index = (int)value;
            CompressionMethodComboBox.SelectedIndex =
                CompressionMethodComboBox.IndexIsInRange(index)
                    ? index
                    : 0;
        }
    }

    // TODO: This goes to default value if it's the same as the selected one.
    //  We need to handle default values explicitly.
    public long DictionarySize
    {
        get =>
            DictionarySizeComboBox.SelectedIndex == 0
                ? -1
                : DictionarySizeItems[DictionarySizeComboBox.SelectedIndex].BackingValue;
        set
        {
            if (value == -1)
            {
                DictionarySizeComboBox.SelectedIndex = 0;
            }
            else
            {
                FriendlyStringAndBackingValue<long>? item = DictionarySizeItems.FirstOrDefault_PastFirstIndex(x => x.BackingValue == value);
                DictionarySizeComboBox.SelectedIndex = item != null
                    ? Array.IndexOf(DictionarySizeItems, item)
                    : 0;
            }
        }
    }

    public int Threads
    {
        get => NumberOfCPUThreadsComboBox.SelectedIndex == 0 ? -1 : NumberOfCPUThreadsComboBox.SelectedIndex;
        set =>
            NumberOfCPUThreadsComboBox.SelectedIndex =
                value == -1
                    ? 0
                    : NumberOfCPUThreadsComboBox.IndexIsInRange(value)
                        ? value
                        : 0;
    }

    public SevenZipApp SevenZipApp
    {
        get => SevenZipInternalRadioButton.Checked ? SevenZipApp.Internal : SevenZipApp.External;
        set
        {
            if (value == SevenZipApp.Internal)
            {
                SevenZipInternalRadioButton.Checked = true;
            }
            else
            {
                SevenZipExternalRadioButton.Checked = true;
            }
            UpdateExternalSevenZipLabel();
        }
    }

    public string SevenZipExternalAppPath
    {
        get => SevenZipExternalTextBox.Text;
        set
        {
            SevenZipExternalTextBox.Text = value;
            UpdateExternalSevenZipLabel();
        }
    }

    public MainForm()
    {
        InitializeComponent();
        CompressionLevelComboBox.SelectedIndex = 9;

        try
        {
            CompressionMethodComboBox.SuspendLayout();

        }
        finally
        {
            CompressionMethodComboBox.ResumeLayout();
        }

        CompressionMethodComboBox.Items.AddRange(CompressionMethodItems.ToFriendlyStrings());

        CompressionMethodComboBox.SelectedIndex = 0;

        PopulateDictionarySizeComboBox();

        PopulateThreadsComboBox();

        string internal7zVersion = Core.GetSevenZipVersion(Path.Combine(Application.StartupPath, "7z", "7z.exe"));
        if (internal7zVersion.IsEmpty())
        {
            internal7zVersion = "<Unable to determine version>";
        }
        SevenZipInternalRadioButton.Text = "Internal (" + internal7zVersion + ")";

        UpdateExternalSevenZipLabel();
        UpdateSevenZipExternalUISection();

        TimestampPrecisionComboBox.SelectedIndex = 0;
    }

    private void UpdateExternalSevenZipLabel()
    {
        string external7zVersion = Core.GetSevenZipVersion(SevenZipExternalTextBox.Text);
        if (external7zVersion.IsEmpty())
        {
            SevenZipExternalRadioButton.Text = "External:";
        }
        else
        {
            SevenZipExternalRadioButton.Text = "External: (" + external7zVersion + ")";
        }
    }

    private void GoButton_Click(object sender, EventArgs e)
    {
        Core.CreateSingleArchive();
    }

    private void FMDirectoryBrowseButton_Click(object sender, EventArgs e)
    {
        // TODO: Implement
    }

    private void OutputArchiveBrowseButton_Click(object sender, EventArgs e)
    {
        // TODO: Implement
    }

    private void SevenZipExternalTextBox_Leave(object sender, EventArgs e)
    {
        UpdateExternalSevenZipLabel();
    }

    private void SevenZipExternalBrowseButton_Click(object sender, EventArgs e)
    {
        // TODO: Implement
    }

    private void SevenZipExeRadioButtons_CheckedChanged(object sender, EventArgs e)
    {
        UpdateSevenZipExternalUISection();
        Config.SevenZipApp = SevenZipInternalRadioButton.Checked ? SevenZipApp.Internal : SevenZipApp.External;
    }

    private void UpdateSevenZipExternalUISection()
    {
        if (SevenZipInternalRadioButton.Checked)
        {
            SevenZipExternalTextBox.Enabled = false;
            SevenZipExternalBrowseButton.Enabled = false;
        }
        else
        {
            SevenZipExternalTextBox.Enabled = true;
            SevenZipExternalBrowseButton.Enabled = true;
        }
    }

    #region Testing

    private void Test1Button_Click(object sender, EventArgs e)
    {
        string[] zips = Directory.GetFiles(@"J:\__zip_Optimal_FMs", "*.zip", SearchOption.TopDirectoryOnly);

        int level = CompressionLevelComboBox.SelectedIndex;
        int methodIndex = CompressionMethodComboBox.SelectedIndex;

        for (int i = 0; i < zips.Length; i++)
        {
            string zip = zips[i];

            string extractedDirName = "";
            try
            {
                extractedDirName = Path.GetFileNameWithoutExtension(zip).Trim();

                //if (extractedDirName != "Religious_conflict_V_1_5")
                //{
                //    continue;
                //}

                string tempExtractedDir = Path.Combine(@"J:\_7z_temp", extractedDirName);
                using (var zipArchive = GetReadModeZipArchiveCharEnc(zip))
                {
                    zipArchive.ExtractToDirectory(tempExtractedDir);
                }

                string outputArchive = Path.Combine(@"J:\__7z_scan_friendly_hc_off", extractedDirName + ".7z");

                (List<string> al_Scan_FileNames, string listFile_Rest) = Core.GetListFile(tempExtractedDir);
                Core.Run7z_ALScanFiles(tempExtractedDir, outputArchive, al_Scan_FileNames, level, methodIndex);
                Core.Run7z_Rest(tempExtractedDir, outputArchive, listFile_Rest, level, methodIndex);

                try
                {
                    Utils.DirAndFileTree_UnSetReadOnly(tempExtractedDir);
                    Directory.Delete(tempExtractedDir, recursive: true);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                    // oh well, I'll delete any stragglers manually after
                }
            }
            catch (Exception exOuter)
            {
                Trace.WriteLine(extractedDirName + ":\r\n" + exOuter);
            }

            Trace.WriteLine((i + 1).ToStrInv() + " / " + zips.Length.ToStrInv() + ": " + extractedDirName);

            //break;

            //(List<string> al_Scan_FileNames, string listFile_Rest) = GetListFile(dir);
            //Run7z_ALScanFiles(dir, OutputArchiveTextBox.Text, al_Scan_FileNames, level, methodIndex);
            //Run7z_Rest(dir, OutputArchiveTextBox.Text, listFile_Rest, level, methodIndex);
        }
    }

    private static ZipArchive GetReadModeZipArchiveCharEnc(string fileName)
    {
        Encoding enc = GetOEMCodePageOrFallback(Encoding.UTF8);
        return new ZipArchive(File.OpenRead(fileName), ZipArchiveMode.Read, leaveOpen: false, enc);
    }

    private static Encoding GetOEMCodePageOrFallback(Encoding fallback)
    {
        Encoding enc;
        try
        {
            enc = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
        }
        catch
        {
            enc = fallback;
        }

        return enc;
    }

    #endregion

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        // TODO: Add check for operation in progress etc.
    }

    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        Core.Shutdown();
    }

    private void CompressionLevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!CompressionLevelComboBox.SelectedIndexIsInRange()) return;
        Config.CompressionLevel = CompressionLevelComboBox.SelectedIndex;
    }

    private void CompressionMethodComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!CompressionMethodComboBox.SelectedIndexIsInRange()) return;
        Config.CompressionMethod = CompressionMethodItems[CompressionMethodComboBox.SelectedIndex].BackingValue;
        PopulateThreadsComboBox(switching: true);
    }

    private void SevenZipExternalTextBox_TextChanged(object sender, EventArgs e)
    {
        Config.SevenZipExternalAppPath = SevenZipExternalTextBox.Text;
    }

    // TODO: Save/restore in config
    // TODO: Use in 7z call
    private void StoreCreationTimeEnableSettingCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (StoreCreationTimeEnableSettingCheckBox.Checked)
        {
            StoreCreationTimeCheckBox.Enabled = true;
        }
        else
        {
            StoreCreationTimeCheckBox.Checked = false;
            StoreCreationTimeCheckBox.Enabled = false;
        }
    }

    private void StoreLastAccessTimeEnableSettingCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (StoreLastAccessTimeEnableSettingCheckBox.Checked)
        {
            StoreLastAccessTimeCheckBox.Enabled = true;
        }
        else
        {
            StoreLastAccessTimeCheckBox.Checked = false;
            StoreLastAccessTimeCheckBox.Enabled = false;
        }
    }

    private void SetArchiveTimeToLatestFileTimeEnableSettingCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (SetArchiveTimeToLatestFileTimeEnableSettingCheckBox.Checked)
        {
            SetArchiveTimeToLatestFileTimeCheckBox.Enabled = true;
        }
        else
        {
            SetArchiveTimeToLatestFileTimeCheckBox.Checked = false;
            SetArchiveTimeToLatestFileTimeCheckBox.Enabled = false;
        }
    }

    private void PopulateDictionarySizeComboBox()
    {
        DictionarySizeComboBox.Items.AddRange(DictionarySizeItems.ToFriendlyStrings());
    }

    private void DictionarySizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DictionarySizeComboBox.SelectedIndexIsInRange())
        {
            Config.DictionarySize =
                DictionarySizeComboBox.SelectedIndex == 0
                    ? -1
                    : DictionarySizeItems[DictionarySizeComboBox.SelectedIndex].BackingValue;
        }
        else
        {
            Config.DictionarySize =
                ConfigData.DefaultDictionarySize;
        }
    }

    private void PopulateThreadsComboBox(bool switching = false)
    {
        FriendlyStringAndBackingValue<int>[] items =
            Config.CompressionMethod == CompressionMethod.LZMA2
                ? Lzma2ThreadItems
                : LzmaThreadItems;

        NumberOfCPUThreadsComboBox.Items.Clear();
        NumberOfCPUThreadsComboBox.Items.AddRange(items.ToFriendlyStrings());

        if (switching)
        {
            Threads = -1;
        }
    }

    private void NumberOfCPUThreadsComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!NumberOfCPUThreadsComboBox.SelectedIndexIsInRange()) return;
        Config.Threads =
            NumberOfCPUThreadsComboBox.SelectedIndex == 0
                ? -1
                : Config.CompressionMethod == CompressionMethod.LZMA2
                    ? Lzma2ThreadItems[NumberOfCPUThreadsComboBox.SelectedIndex].BackingValue
                    : LzmaThreadItems[NumberOfCPUThreadsComboBox.SelectedIndex].BackingValue;
    }
}
