#define DEV_TESTING

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AngelLoader.Forms.WinFormsNative.Dialogs;
using Loader_Friendly_FM_Packager.WinFormsNative.Taskbar;

namespace Loader_Friendly_FM_Packager;

/*
TODO: Allow adding a context menu item, so the user can just right click and run it on a folder.
*/

public sealed partial class MainForm : Form, IEventDisabler
{
    private enum OperationType
    {
        None,
        CreateSingle,
        RepackBatch,
    }

    private OperationType _operationTypeInProgress = OperationType.None;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int EventsDisabled { get; set; }

    public MainForm()
    {
        InitializeComponent();

#if !DEV_TESTING
        Test1Button.Hide();
#endif

        CompressionMethodComboBox.Items.AddRange(CompressionMethodItems.ToFriendlyStrings());

        PopulateThreadsComboBox();

        MainProgressBar.CenterH(StatusGroupBox);
        Cancel_Button.CenterH(StatusGroupBox);
        ResetProgressMessage();
        ProgressMessageLabel.Location = ProgressMessageLabel.Location with { X = MainProgressBar.Left };
    }

    #region Getters and setters

    public bool DragDropEnabled => _operationTypeInProgress == OperationType.None;

    public void SetMode(Mode mode)
    {
        ModeTabControl.SelectedTab = mode switch
        {
            Mode.Repack => RepackTabPage,
            _ => CreateTabPage,
        };
    }

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

    public string[] Repack_Archives => ArchivesToRepackListBox.ItemsAsStrings();

    public string Repack_OutputDirectory => RepackOutputDirectoryTextBox.Text;

    public void SetCompressionLevel(int value)
    {
        CompressionLevelComboBox.SelectedIndex = CompressionLevelComboBox.IndexIsInRange(value)
            ? value
            : 0;
    }

    public void SetCompressionMethod(CompressionMethod value)
    {
        int index = (int)value;
        using (new DisableEvents(this))
        {
            CompressionMethodComboBox.SelectedIndex =
                CompressionMethodComboBox.IndexIsInRange(index)
                    ? index
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

    #endregion

    #region Progress reporting

    public void StartCreateSingleArchiveOperation() => Invoke(() =>
    {
        TaskBarProgress.SetState(Handle, TaskbarStates.Normal);
        _operationTypeInProgress = OperationType.CreateSingle;
        MainPanel.Enabled = false;
        ProgressMessageLabel.Text = "";
        MainProgressBar.Value = 0;
        MainProgressBar.Show();
        Cancel_Button.Show();
    });

    public void EndCreateSingleArchiveOperation(string? message = null) => Invoke(() =>
    {
        TaskBarProgress.SetState(Handle, TaskbarStates.NoProgress);
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
        _operationTypeInProgress = OperationType.None;
    });

    public void StartRepackBatchOperation() => Invoke(() =>
    {
        TaskBarProgress.SetState(Handle, TaskbarStates.Normal);
        _operationTypeInProgress = OperationType.RepackBatch;
        MainPanel.Enabled = false;
        ProgressMessageLabel.Text = "";
        ProgressSubMessageLabel.Text = "";
        ProgressSubMessageLabel.Show();
        MainProgressBar.Value = 0;
        SubProgressBar.Value = 0;
        MainProgressBar.Show();
        SubProgressBar.Show();
        Cancel_Button.Show();
    });

    public void EndRepackBatchOperation() => Invoke(() =>
    {
        TaskBarProgress.SetState(Handle, TaskbarStates.NoProgress);
        MainProgressBar.Hide();
        SubProgressBar.Hide();
        Cancel_Button.Hide();
        ResetProgressMessage();
        ProgressSubMessageLabel.Hide();
        MainProgressBar.Value = 0;
        SubProgressBar.Value = 0;
        MainPanel.Enabled = true;
        _operationTypeInProgress = OperationType.None;
    });

    public void SetProgressMessage(string message) => Invoke(() =>
    {
        if (_operationTypeInProgress == OperationType.CreateSingle)
        {
            ProgressMessageLabel.Text = message;
        }
        else
        {
            ProgressSubMessageLabel.Text = message;
        }
    });

    public void SetProgressBatchMainMessage(string message) => Invoke(() =>
    {
        ProgressMessageLabel.Text = message;
    });

    public void SetProgressPercent(int percent) => Invoke(() =>
    {
        if (_operationTypeInProgress == OperationType.CreateSingle)
        {
            TaskBarProgress.SetValue(Handle, percent.Clamp(0, 100), 100);
            MainProgressBar.Value = percent;
        }
        else
        {
            SubProgressBar.Value = percent;
        }
    });

    public void SetProgressBatchMainPercent(int percent) => Invoke(() =>
    {
        TaskBarProgress.SetValue(Handle, percent.Clamp(0, 100), 100);
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
        dialog.Title = "Choose source FM directory";

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        SourceFMDirectoryTextBox.Text = dialog.DirectoryName;
    }

    private void OutputArchiveBrowseButton_Click(object sender, EventArgs e)
    {
        using SaveFileDialog dialog = new();
        dialog.Title = "Create output archive";
        dialog.AddExtension = true;
        dialog.Filter = "7-Zip files (*.7z)|*.7z";
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

        Fen7z.Rename(
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

                const string outputDir = @"J:\__7z_scan_friendly_final_9";

                string outputArchive = Path.Combine(outputDir, extractedDirName + ".7z");
                Directory.CreateDirectory(outputDir);

                (Core.ListFileData listFileData, string listFile_Rest) = Core.GetListFile(ref tempExtractedDir, makeCopyOfFilesDir: true, CancellationToken.None);

                const string entriesListsDir = @"J:\_7z_al_scan_files_lists_9";
                Directory.CreateDirectory(entriesListsDir);

                //List<string> al_Scan_FileNames = new();
                //al_Scan_FileNames.AddRange(listFileData.Readmes);
                //al_Scan_FileNames.AddRange(listFileData.Thumbs);
                //al_Scan_FileNames.AddRange(listFileData.MainImages);

                //File.WriteAllLines(Path.Combine(testDir, Path.GetFileNameWithoutExtension(zip) + "__al_scan_entries.txt"), al_Scan_FileNames);
#if true
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
                        sw.WriteLine("-------------");
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
#endif

                _ = Core.Run7z_ALScanFiles(tempExtractedDir, outputArchive, listFileData, level, method, CancellationToken.None);
                _ = Core.Run7z_Rest(tempExtractedDir, outputArchive, listFile_Rest, listFileData, level, method, CancellationToken.None);

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
        if (_operationTypeInProgress != OperationType.None)
        {
            MessageBox.Show(
                text: "An operation is in progress. Please cancel it first.",
                caption: "Alert",
                buttons: MessageBoxButtons.OK,
                icon: MessageBoxIcon.Warning);
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
        CompressionMethodComboBox.Visible = CompressionLevelComboBox.SelectedIndex > 0;
    }

    private void CompressionMethodComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!CompressionMethodComboBox.SelectedIndexIsInRange()) return;
        Config.CompressionMethod = CompressionMethodItems[CompressionMethodComboBox.SelectedIndex].BackingValue;
        PopulateThreadsComboBox(switching: true);
    }

    private void PopulateThreadsComboBox(bool switching = false)
    {
        FriendlyStringAndBackingValue<int>[] items =
            Config.CompressionMethod == CompressionMethod.LZMA2
                ? Lzma2ThreadItems
                : LzmaThreadItems;

        NumberOfCPUThreadsComboBox.Items.Clear();
        NumberOfCPUThreadsComboBox.Items.AddRange(items.ToFriendlyStrings());

        if (switching && EventsDisabled <= 0)
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

    #endregion

    private async void CreateSingleArchiveButton_Click(object sender, EventArgs e)
    {
        await Core.CreateSingleArchive();
    }

    private void Cancel_Button_Click(object sender, EventArgs e)
    {
        Core.CancelToken();
    }

    private void AddSourceArchiveButton_Click(object sender, EventArgs e)
    {
        using var d = new OpenFileDialog();

        d.Title = "Add source FM archive(s)";
        d.Filter =
            "All archive files (*.7z, *.zip, *.rar)|*.7z;*.zip;*.rar|" +
            "Zip files (*.zip)|*.zip|" +
            "7-Zip files (*.7z)|*.7z|" +
            "Rar files (*.rar)|*.rar|" +
            "All files (*.*)|*.*";
        d.Multiselect = true;

        ListBox lb = ArchivesToRepackListBox;
        string selectedItem =
            lb.SelectedIndex > -1 ? lb.SelectedItem.ToStringOrEmpty() :
            lb.Items.Count > 0 ? lb.ItemsAsStrings()[lb.Items.Count - 1] :
            "";

        if (!selectedItem.IsWhiteSpace())
        {
            try
            {
                d.InitialDirectory = Path.GetDirectoryName(selectedItem) ?? "";
            }
            catch
            {
                // ignore
            }
        }

        if (d.ShowDialog(this) == DialogResult.OK)
        {
            lb.AddUniqueItems(d.FileNames);
        }
    }

    private void RemoveSourceArchiveButton_Click(object sender, EventArgs e)
    {
        ArchivesToRepackListBox.RemoveAndSelectNearest();
    }

    private void ClearSourceArchivesButton_Click(object sender, EventArgs e)
    {
        ArchivesToRepackListBox.Items.Clear();
    }

    private void RepackOutputDirectoryBrowseButton_Click(object sender, EventArgs e)
    {
        using VistaFolderBrowserDialog dialog = new();
        dialog.Title = "Choose output directory";

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        RepackOutputDirectoryTextBox.Text = dialog.DirectoryName;
    }

    private void ModeTabControl_Selected(object sender, TabControlEventArgs e)
    {
        Config.Mode = ModeTabControl.SelectedTab == RepackTabPage ? Mode.Repack : Mode.Create;
    }

    private async void RepackButton_Click(object sender, EventArgs e)
    {
        await Core.RepackBatch();
    }

    private void ArchivesToRepackListBox_DragEnter(object sender, DragEventArgs e)
    {
        object? data = e.Data?.GetData(DataFormats.FileDrop);
        if (data == null) return;

        if (Core.FilesDropped(data, out _))
        {
            e.Effect = DragDropEffects.Copy;
        }
    }

    private void ArchivesToRepackListBox_DragDrop(object sender, DragEventArgs e)
    {
        object? data = e.Data?.GetData(DataFormats.FileDrop);
        if (data == null) return;

        if (Core.FilesDropped(data, out string[]? droppedItems))
        {
            ArchivesToRepackListBox.AddUniqueItems(Core.GetStronglyCheckedFiles(droppedItems));
        }
    }

    /// <summary>
    /// This method is auto-invoked if <see cref="Core.View"/> is able to be invoked to.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    /// <param name="icon"></param>
    public void ShowError(string message, string? title = null, MessageBoxIcon icon = MessageBoxIcon.Error)
    {
        Invoke(() =>
        {
            ShowError_Internal(message, this, title, icon);
        });
    }

    // Private method, not invoked because all calls are
    private static void ShowError_Internal(string message, IWin32Window? owner, string? title, MessageBoxIcon icon)
    {
        using var d = new DarkErrorDialog(message, title, icon);
        if (owner != null)
        {
            d.ShowDialog(owner);
        }
        else
        {
            d.ShowDialog();
        }
    }

    /// <summary>
    /// This method is auto-invoked if <see cref="Core.View"/> is able to be invoked to.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="title"></param>
    /// <param name="icon"></param>
    /// <param name="yes"></param>
    /// <param name="no"></param>
    /// <param name="cancel"></param>
    /// <param name="yesIsDangerous"></param>
    /// <param name="noIsDangerous"></param>
    /// <param name="cancelIsDangerous"></param>
    /// <param name="checkBoxText"></param>
    /// <param name="defaultButton"></param>
    /// <param name="viewLogButtonVisible"></param>
    /// <returns></returns>
    public (MBoxButton ButtonPressed, bool CheckBoxChecked)
    ShowMultiChoiceDialog(string message,
        string title,
        MessageBoxIcon icon,
        string? yes,
        string? no,
        string? cancel = null,
        bool yesIsDangerous = false,
        bool noIsDangerous = false,
        bool cancelIsDangerous = false,
        string? checkBoxText = null,
        MBoxButton defaultButton = MBoxButton.Yes,
        bool viewLogButtonVisible = false) =>
        ((MBoxButton, bool))Invoke(() =>
        {
            using var d = new TaskDialogCustom(
                message: message,
                title: title,
                icon: icon,
                yesText: yes,
                noText: no,
                cancelText: cancel,
                yesIsDangerous: yesIsDangerous,
                noIsDangerous: noIsDangerous,
                cancelIsDangerous: cancelIsDangerous,
                checkBoxText: checkBoxText,
                defaultButton: defaultButton,
                viewLogButtonVisible: viewLogButtonVisible);

            DialogResult result = d.ShowDialog(this);

            return (DialogResultToMBoxButton(result), d.IsVerificationChecked);
        });

    internal static MBoxButton DialogResultToMBoxButton(DialogResult dialogResult) => dialogResult switch
    {
        DialogResult.Yes => MBoxButton.Yes,
        DialogResult.No => MBoxButton.No,
        _ => MBoxButton.Cancel,
    };
}
