#define DEV_TESTING

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
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

TODO: Default dictionary size changes with compression level in 7-Zip official, we need to match it

--

TODO: Re-run the 7z-scan-friendly convert with the final logic

--

TODO: Act on caught exceptions and add robust error handling everywhere
*/

public sealed partial class MainForm : Form
{
    private bool _operationInProgress;

    public MainForm()
    {
        InitializeComponent();

#if !DEV_TESTING
        Test1Button.Hide();
#endif

        CompressionMethodComboBox.Items.AddRange(CompressionMethodItems.ToFriendlyStrings());

        DictionarySizeComboBox.Items.AddRange(DictionarySizeItems.ToFriendlyStrings());

        PopulateThreadsComboBox();

        MemoryUsageForCompressingComboBox.Items.AddRange(MemoryUseItems.ToFriendlyStrings());

        MainProgressBar.CenterH(StatusGroupBox);
        Cancel_Button.CenterH(StatusGroupBox);
        ResetProgressMessage();
        ProgressMessageLabel.Location = ProgressMessageLabel.Location with { X = MainProgressBar.Left };
    }

    #region Getters and setters

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

    #endregion

    #region Progress reporting

    public void StartCreateSingleArchiveOperation() => Invoke(() =>
    {
        _operationInProgress = true;
        MainPanel.Enabled = false;
        ProgressMessageLabel.Text = "";
        MainProgressBar.Value = 0;
        MainProgressBar.Show();
        Cancel_Button.Show();
    });

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

    private void ResetProgressMessage()
    {
        ProgressMessageLabel.Text = "No operation in progress.";
    }

    #endregion

    #region Directories

    private void FMDirectoryBrowseButton_Click(object sender, EventArgs e)
    {
        using VistaFolderBrowserDialog dialog = new();
        dialog.Description = "Choose source FM directory";
        dialog.UseDescriptionForTitle = true;

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        SourceFMDirectoryTextBox.Text = dialog.SelectedPath;
    }

    private void OutputArchiveBrowseButton_Click(object sender, EventArgs e)
    {
        using VistaSaveFileDialog dialog = new();
        dialog.Title = "Choose output archive";
        dialog.AddExtension = true;
        dialog.Filter = "7-Zip file|*.7z";
        dialog.DefaultExt = "7z";
        dialog.ValidateNames = true;
        dialog.OverwritePrompt = true;

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        OutputArchiveTextBox.Text = dialog.FileName;
    }

    #endregion

    #region Testing

    private readonly string[] LargeSizeFiles =
    {
        @"Elemental Excursion 1.0a",
        @"2002-06-21_Awaken!",
        @"Awaken!",
        @"2008-09-09_The_Mask_part_I",
        @"2003-09-01_FriendBasso_v2",
        @"FallingInLove",
        @"2010-01-23_thespaceproject-1.0",
        @"2013-The Widow's Ire",
        @"2002-04-12_ThreeMagesTomb_demo",
        @"2002-03-23_OsamaBitestheDust",
        @"2005-08-31_NC_RosariesAreRed",
        @"2006-12-18_SomeShopping",
        @"2006-03-15_SSR_NightShift,The_v1.1",
        @"2015-Working Late 1.0b",
        @"2007-08-18_Ura",
        @"2006-12-07_Incubus",
        @"2006-02-04_SepulchreoftheSinistral",
        @"2008-09-16_CTE_PlotThickens,The_v1",
        @"2013-01-27_MSG_final",
        @"2005-11-07_Ganbatte,Thief-San!",
        @"2008-11-30_TAC_FallingDown",
        @"RttUNNv1.08_patch_rc11",
        @"2005-08-18_Haddur",
        @"2000-08-20_OrderoftheVine,The",
        @"2005-05-29_RightUpThereintheMountains_nolagv1.3",
        @"2013-10-31_IslandOfSorrow",
        @"Blind Disposition v1.3",
        @"2006-03-15_SSR_ChooseYourOwnAdventure",
        @"TowerofJorge",
        @"Dielya_v1.23",
        @"2006-05-30_KarrasApartments",
        @"2008-05-14_Hookshot_demo",
        @"2007-11-16_HammeriteDeathMatch_v10",
        @"2013-08-18_LOTP Winter",
        @"The Stupid Grimrock Quote",
        @"AscendTheDimValley-V1",
        @"Lord_Ashton_Rework",
        @"AscendTheDimValley-V1-1",
        @"2009-02-10_Niggsters_erste_FM",
        @"reminiscences",
        @"2005-11-27_15days 1_VolDeNuit",
        @"2004-03-15_Nightmares_Cauchemars",
        @"2006-12-07_Medalage",
        @"2008-07-03_Skylab-1.0",
        @"2006-03-17_SSR_Complications",
        @"2012-07-03_StrifeC_V1c",
        @"2008-05-16_BruderSnuck_demo_GER",
        @"2008-02-09_SlysUTArena2_v1.1_demo",
        @"2001-12-15_c2RedRidge",
        @"2008-01-01_Doschtles_Labyrinth",
        @"TDP20AC_An_Enigmatic_Treasure_With_A_Recondite_Discovery",
        @"2003-01-25_c4MissionWithNoName",
        @"2005-07-19_Ruined_1-2",
        @"2006-03-17_SSR_SlowButSteadyProgress_v12",
        @"2002-07-10_breakingandentering",
        @"2005-04-15_db1_CloisterofStLazarius,The",
        @"2006-12-07_OMGGYBB",
        @"TMA20AC_ShowOff",
        @"2001-11-23_TowerofKarras,The_demo",
        @"2002-02-17_ChasingSergeantChase",
        @"2010-10-06_StealingtheStolen",
        @"2006-06-09_OrthodoxWedding",
        @"2000-08-14_Journey-EscapefromGuilesatpeak,TuttocombsTomb",
        @"JustMyLuck_NNC",
        @"2003-03-28_AbominableDrDragon,The",
        @"2009-10-18_strangetemple-1.0",
        @"2009-02-12_ReturnofRamirez",
        @"2005-08-25_Gearheart_v3",
        @"2009-05-05_WalkinOnMars-1.0",
        @"2001-08-30_La_patisserie_Oswald",
        @"2004-03-23_schulmod",
        @"2000-10-02_Treachery",
        @"2003-01-25_c4Thievery",
        @"2004-02-05_EasyBank_V1.0",
        @"2001-08-30_c1OswaldsPatisserie",
        @"2006-03-17_SSR_AuldaleChessTournament,The",
        @"2012-12-05_Precious1.0d",
        @"2007-06-24_ObligatoryPrisonMission",
        @"2005-02-25_AllTorc",
        @"2006-03-17_SSR_February",
        @"2006-05-03_CurseoftheHammerites_eng_v1.3",
        @"2003-01-25_c4CarelessLittleFly",
        @"TDP20AC_TheScarletCascabel_v1.1",
        @"2003-01-25_c4NoMoreClientsforMonty",
        @"2005-08-31_NC_LastGleamingoftheRisingSun,The",
        @"2003-06-13_ElevatorMissionT2",
        @"SS2_Zombe_FMProov_1v2",
        @"2002-09-23_LessonLearned,A_v1.1",
        @"Ransom_v1",
        @"theunn",
        @"2008-06-17_GoodvsBadv2_5",
        @"2006-02-19_SSR_AdventureinEastport,An_1.1",
        @"2001-12-15_c2SirLectorComestoDine",
        @"2004-02-29_c5Plagiarism",
        @"2008-04-01_WemissyouBob",
        @"2002-08-20_GarretttotheRescue2",
        @"2006-03-17_SSR_CrypticCravings",
    };

    private void Test1Button_Click(object sender, EventArgs e)
    {
#if false
        const string testBaseDir = @"F:\Local Storage HDD\AngelLoader local\__7z_order_test";
        const string outputDir = @"F:\Local Storage HDD\AngelLoader local\__7z_order_test\output";
        const string misFilesDir = @"F:\Local Storage HDD\AngelLoader local\__7z_order_test\mis_files";

        //string[] misFiles = Directory.GetFiles(misFilesDir, "*.mis", SearchOption.TopDirectoryOnly);
        //for (int i = 0; i < misFiles.Length; i++)
        //{
        //    misFiles[i] = Path.GetFileName(misFiles[i]);
        //}
        string[] misFiles =
        {
            "miss20.mis",
            "miss21.mis",
            "aaaaaaa_miss22.mis",
        };

        string listFile = Path.Combine(testBaseDir, "list.txt");
        File.WriteAllLines(listFile, misFiles);

        string outputArchive = Path.Combine(outputDir, "archive.7z");

        Fen7z.Compress(
            Paths.SevenZipExe,
            misFilesDir,
            outputArchive,
            Core.GetArgs(Config.CompressionLevel, Config.CompressionMethod, friendly: false),
            CancellationToken.None);

        Fen7z.Update(
            sevenZipPathAndExe: Paths.SevenZipExe,
            sourcePath: misFilesDir,
            outputArchive: outputArchive,
            originalFileName: "aaaaaaa_miss22.mis",
            newFileName: "miss22.mis",
            cancellationToken: CancellationToken.None);
#endif

#if true
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

                //if (extractedDirName != "2013-04-16_NiceGameofChess")
                //{
                //    continue;
                //}

                //if (!LargeSizeFiles.Contains(extractedDirName, StringComparer.OrdinalIgnoreCase))
                //{
                //    continue;
                //}

                //if (extractedDirName != "2003-09-01_FriendBasso_v2")
                //{
                //    continue;
                //}

                string tempExtractedDir = Path.Combine(@"J:\_7z_temp", extractedDirName);
                using (var zipArchive = GetReadModeZipArchiveCharEnc(zip))
                {
                    //bool anyHtmlFiles = false;
                    //foreach (var entry in zipArchive.Entries)
                    //{
                    //    if (entry.FullName.ExtIsHtml())
                    //    {
                    //        anyHtmlFiles = true;
                    //    }
                    //}
                    //if (!anyHtmlFiles) continue;
                    zipArchive.ExtractToDirectory(tempExtractedDir);
                }

                const string outputDir = @"J:\__7z_scan_friendly_final_5";

                string outputArchive = Path.Combine(outputDir, extractedDirName + ".7z");
                Directory.CreateDirectory(outputDir);

                (Core.ListFileData listFileData, string listFile_Rest) = Core.GetListFile(tempExtractedDir);

                const string entriesListsDir = @"J:\_7z_al_scan_files_lists";
                Directory.CreateDirectory(entriesListsDir);

                //List<string> al_Scan_FileNames = new();
                //al_Scan_FileNames.AddRange(listFileData.Readmes);
                //al_Scan_FileNames.AddRange(listFileData.Thumbs);
                //al_Scan_FileNames.AddRange(listFileData.MainImages);

                //File.WriteAllLines(Path.Combine(testDir, Path.GetFileNameWithoutExtension(zip) + "__al_scan_entries.txt"), al_Scan_FileNames);
                using (var sw = new StreamWriter(Path.Combine(entriesListsDir, Path.GetFileNameWithoutExtension(zip) + "__al_scan_entries.txt")))
                {
                    if (listFileData.Readmes.Count > 0)
                    {
                        sw.WriteLine("Readme block:");
                        foreach (string readme in listFileData.Readmes)
                        {
                            sw.WriteLine(readme);
                        }
                        sw.WriteLine("-------------");
                    }

                    if (listFileData.Thumbs.Count > 0)
                    {
                        sw.WriteLine("Thumbs block:");
                        foreach (string thumb in listFileData.Thumbs)
                        {
                            sw.WriteLine(thumb);
                        }
                        sw.WriteLine("-------------");
                    }

                    if (listFileData.MainImages.Count > 0)
                    {
                        sw.WriteLine("MainImages block:");
                        foreach (string mainImage in listFileData.MainImages)
                        {
                            sw.WriteLine(mainImage);
                        }
                        sw.WriteLine("-------------");
                    }

                    if (listFileData.GamFiles.Count > 0)
                    {
                        sw.WriteLine(".gam files block:");
                        foreach (var gamFile in listFileData.GamFiles)
                        {
                            sw.WriteLine(gamFile);
                        }
                        sw.WriteLine("-------------");
                    }

                    if (listFileData.OnePerBlockItems.Count > 0)
                    {
                        sw.WriteLine("One-per-block items:");
                        foreach (string item in listFileData.OnePerBlockItems)
                        {
                            sw.WriteLine(item);
                        }
                    }

                    if (listFileData.FilesToRename.Count > 0)
                    {
                        sw.WriteLine("FilesToRename items:");
                        foreach (Core.FileToRename item in listFileData.FilesToRename)
                        {
                            sw.WriteLine(item.Name + " / " + item.TempSortedName);
                        }
                    }
                }

                Core.Run7z_ALScanFiles(tempExtractedDir, outputArchive, listFileData, level, method, CancellationToken.None);
                Core.Run7z_Rest(tempExtractedDir, outputArchive, listFile_Rest, listFileData, level, method, CancellationToken.None);

                //Fen7z.Compress(
                //    Paths.SevenZipExe,
                //    tempExtractedDir,
                //    outputArchive,
                //    Core.GetArgs(level, method, friendly: false),
                //    CancellationToken.None);

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
#endif
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

    #region Form events

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

    #endregion

    #region 7-Zip fields

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

    private void MemoryUsageForCompressingComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!MemoryUsageForCompressingComboBox.SelectedIndexIsInRange()) return;
        Config.MemoryUseForCompression =
            MemoryUsageForCompressingComboBox.SelectedIndex == 0
                ? MemoryUseItem.Default
                : MemoryUseItems[MemoryUsageForCompressingComboBox.SelectedIndex].BackingValue;
    }

    #endregion

    private async void CreateSingleArchiveButton_Click(object sender, EventArgs e)
    {
        await Core.CreateSingleArchive();
    }

    private void Cancel_Button_Click(object sender, EventArgs e)
    {
        Core.CancelToken();
    }
}
