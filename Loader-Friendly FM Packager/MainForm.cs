#define DEV_TESTING

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Ookii.Dialogs.WinForms;

namespace Loader_Friendly_FM_Packager;

/*
TODO: We'll be adding to the archive with every call, so make sure it doesn't exist beforehand and/or delete
 it first or something

TODO: When re-packing from a zip file, notify the user when an unknown-encoded filename comes up (where it would
 normally use "Default" encoding). Show the user the filename in various encodings and allow them to choose what
 it should be.
 We'll need to pull in the .NET zip code so as to customize it to expose the "used default encoding" situation.

TODO: Allow adding a context menu item, so the user can just right click and run it on a folder.

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
*/

public sealed partial class MainForm : Form
{
    private bool _operationInProgress;

    public string SourceFMPath
    {
        get => SourceFMDirectoryTextBox.Text;
        set => SourceFMDirectoryTextBox.Text = value;
    }

    public string OutputArchive
    {
        get => OutputArchiveTextBox.Text;
        set => OutputArchiveTextBox.Text = value;
    }

    public void SetCompressionLevel(int value)
    {
        CompressionLevelComboBox.SelectedIndex = CompressionLevelComboBox.IndexIsInRange(value)
            ? value
            : 0;
    }

    public void SetCompressionMethod(CompressionMethod value)
    {
        int index = (int)value;
        CompressionMethodComboBox.SelectedIndex =
            CompressionMethodComboBox.IndexIsInRange(index)
                ? index
                : 0;
    }

    public void SetDictionarySize(long value)
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

    public void SetThreads(int value)
    {
        NumberOfCPUThreadsComboBox.SelectedIndex =
            value == -1
                ? 0
                : NumberOfCPUThreadsComboBox.IndexIsInRange(value)
                    ? value
                    : 0;
    }

    public void SetMemoryUseForCompression(MemoryUseItem value)
    {
        if (value.Value == -1)
        {
            MemoryUsageForCompressingComboBox.SelectedIndex = 0;
        }
        else
        {
            FriendlyStringAndBackingValue<MemoryUseItem>? item =
                MemoryUseItems.FirstOrDefault_PastFirstIndex(x => x.BackingValue.Value == value.Value && x.BackingValue.IsPercent == value.IsPercent);
            MemoryUsageForCompressingComboBox.SelectedIndex = item != null
                ? Array.IndexOf(MemoryUseItems, item)
                : 0;
        }
    }

    public void StartCreateSingleArchiveOperation() => Invoke(() =>
    {
        _operationInProgress = true;
        MainPanel.Enabled = false;
        ProgressMessageLabel.Text = "";
        MainProgressBar.Value = 0;
        MainProgressBar.Show();
        Cancel_Button.Show();
    });

    private void ResetProgressMessage()
    {
        ProgressMessageLabel.Text = "No operation in progress.";
    }

    public void EndCreateSingleArchiveOperation(string? message = null) => Invoke(() =>
    {
        MainProgressBar.Hide();
        Cancel_Button.Hide();
        if (message != null)
        {
            ProgressMessageLabel.Text = message;
        }
        else
        {
            ResetProgressMessage();
        }
        MainProgressBar.Value = 0;
        MainPanel.Enabled = true;
        _operationInProgress = false;
    });

    public void SetProgressMessage(string message) => Invoke(() =>
    {
        ProgressMessageLabel.Text = message;
    });

    public void SetProgressPercent(int percent) => Invoke(() =>
    {
        MainProgressBar.Value = percent;
    });

    public MainForm()
    {
        InitializeComponent();

#if !DEV_TESTING
        Test1Button.Hide();
#endif

        CompressionMethodComboBox.Items.AddRange(CompressionMethodItems.ToFriendlyStrings());

        PopulateDictionarySizeComboBox();

        PopulateThreadsComboBox();

        PopulateMemoryUseComboBox();

        MainProgressBar.CenterH(StatusGroupBox);
        Cancel_Button.CenterH(StatusGroupBox);
        ResetProgressMessage();
        ProgressMessageLabel.Location = ProgressMessageLabel.Location with { X = MainProgressBar.Left };
    }

    private async void CreateSingleArchiveButton_Click(object sender, EventArgs e)
    {
        await Core.CreateSingleArchive();
    }

    private void FMDirectoryBrowseButton_Click(object sender, EventArgs e)
    {
        // TODO: Implement
    }

    private void OutputArchiveBrowseButton_Click(object sender, EventArgs e)
    {
        // TODO: Implement
    }

    #region Testing

    private void Test1Button_Click(object sender, EventArgs e)
    {
        string[] zips = Directory.GetFiles(@"J:\__zip_Optimal_FMs", "*.zip", SearchOption.TopDirectoryOnly);

        int level = Config.CompressionLevel;
        CompressionMethod method = Config.CompressionMethod;

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

                string outputArchive = Path.Combine(@"J:\__7z_scan_friendly_hc_off_5", extractedDirName + ".7z");

                (List<string> al_Scan_FileNames, string listFile_Rest) = Core.GetListFile(tempExtractedDir);
                Core.Run7z_ALScanFiles(tempExtractedDir, outputArchive, al_Scan_FileNames, level, method, CancellationToken.None);
                Core.Run7z_Rest(tempExtractedDir, outputArchive, listFile_Rest, level, method, CancellationToken.None);

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
        if (!_operationInProgress) return;

        using TaskDialog dialog = new();
        dialog.WindowTitle = "Alert";
        dialog.MainIcon = TaskDialogIcon.Warning;
        dialog.Content =
            "An operation is in progress. You should cancel it before exiting, or 7z.exe may be left running and archive files may be left in an incomplete state.";
        TaskDialogButton forceQuitButton = new("Exit anyway");
        TaskDialogButton cancelButton = new(ButtonType.Cancel) { Default = true };
        dialog.Buttons.Add(forceQuitButton);
        dialog.Buttons.Add(cancelButton);
        TaskDialogButton resultButton = dialog.ShowDialog(this);
        if (resultButton != forceQuitButton)
        {
            e.Cancel = true;
        }
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

    private void PopulateDictionarySizeComboBox()
    {
        DictionarySizeComboBox.Items.AddRange(DictionarySizeItems.ToFriendlyStrings());
    }

    private void DictionarySizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!DictionarySizeComboBox.SelectedIndexIsInRange()) return;
        Config.DictionarySize =
            DictionarySizeComboBox.SelectedIndex == 0
                ? -1
                : DictionarySizeItems[DictionarySizeComboBox.SelectedIndex].BackingValue;
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
            SetThreads(-1);
            Config.Threads = -1;
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

    private void PopulateMemoryUseComboBox()
    {
        MemoryUsageForCompressingComboBox.Items.Clear();
        MemoryUsageForCompressingComboBox.Items.AddRange(MemoryUseItems.ToFriendlyStrings());
    }

    private void MemoryUsageForCompressingComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!MemoryUsageForCompressingComboBox.SelectedIndexIsInRange()) return;
        Config.MemoryUseForCompression =
            MemoryUsageForCompressingComboBox.SelectedIndex == 0
                ? MemoryUseItem.Default
                : MemoryUseItems[MemoryUsageForCompressingComboBox.SelectedIndex].BackingValue;
    }

    private void Cancel_Button_Click(object sender, EventArgs e)
    {
        Core.CancelToken();
    }
}
