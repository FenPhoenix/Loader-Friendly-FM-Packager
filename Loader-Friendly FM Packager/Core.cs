﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Loader_Friendly_FM_Packager;

internal static class Core
{
    private static MainForm View = null!;

    private const string AL_Scan_Block_ListFileName = "list_al_scan.lst";
    private const string Rest_Block_ListFileName = "list_rest.lst";

    internal static void Init()
    {
        View = new MainForm();

        ConfigIni.ReadConfigData();

        View.SetCompressionLevel(Config.CompressionLevel);
        View.SetCompressionMethod(Config.CompressionMethod);
        View.SetThreads(Config.Threads);
        View.SetDictionarySize(Config.DictionarySize);
        View.SetMemoryUseForCompression(Config.MemoryUseForCompression);

        View.Show();
    }

    internal static void Shutdown()
    {
        ConfigIni.WriteConfigData();
        Application.Exit();
    }

    private static void RunProcess(
    string sourcePath,
    string outputArchive,
    string listFile,
    int level,
    CompressionMethod method,
    CancellationToken cancellationToken)
    {
        Progress<Fen7z.ProgressReport> progress = new(ReportProgress);

        Fen7z.Result result = Fen7z.Compress(
            // TODO: Cache 7z.exe path
            sevenZipPathAndExe: Path.Combine(Application.StartupPath, "7z", "7z.exe"),
            sourcePath: sourcePath,
            outputArchive: outputArchive,
            args: GetArgs(level, method),
            // TODO: Add cancellation
            cancellationToken: cancellationToken,
            listFile: listFile,
            // TODO: Add progress reporting
            progress: progress
        );

        if (result.ErrorOccurred)
        {
            // TODO: Handle the error. Also add logging maybe?
        }
        else if (result.Canceled)
        {
            throw new OperationCanceledException(_cts.Token);
        }
        else
        {
            // TODO: Handle creation complete
        }

        return;

        static void ReportProgress(Fen7z.ProgressReport pr)
        {
            if (!pr.Canceling)
            {
                View.SetProgressPercent(pr.CompressPercent);
            }
        }
    }

    /*
    -Put every file in its own block - that way AL can do extract cost calculations and really just get the first
     used .mis file.
    -Use a list file even when there's only one entry in it, to prevent any filename encoding issues.
    */
    internal static void Run7z_ALScanFiles(
        string sourcePath,
        string outputArchive,
        List<string> fileNames,
        int level,
        CompressionMethod method,
        CancellationToken cancellationToken)
    {
        string listFile = Path.Combine(Paths.Temp, AL_Scan_Block_ListFileName);

        for (int i = 0; i < fileNames.Count; i++)
        {
            string fileName = fileNames[i];

            View.SetProgressPercent(0);
            View.SetProgressMessage("Adding " + fileName + " to its own block...");

            if (!File.Exists(Path.Combine(sourcePath, fileName)))
            {
                // TODO: Handle this better - placeholder for now
                string msg =
                    "----------\r\n" +
                    "Filename we're about to pass to 7z via the list file (" + nameof(Run7z_ALScanFiles) +
                    ") doesn't exist on disk! Character encoding difference?\r\n" +
                    "Filename: +" + fileName + "\r\n" +
                    "----------";
                Trace.WriteLine(msg);
                MessageBox.Show(
                    msg,
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            using (var sw = new StreamWriter(listFile, false, UTF8NoBOM))
            {
                sw.WriteLine(fileName);
            }

            /*
            TODO: .mis, .gam, missflag.str files all need separate one-file blocks each, but the rest of our
             files can share a block, as long as all of our files are collectively in their own block or blocks
             that don't have any non-scan-required files in them.

            *Except we're thinking of other loaders now, so we should just keep all files in their own blocks.

            So like:

            our stuff
            { 
            Block 1:
            miss20.mis

            Block 2:
            miss21.mis

            Block 3:
            miss20.gam

            Block 4:
            strings/missflag.str

            Block 5:
            readme.rtf
            readme2.txt
            copyright.txt
            readme.html
            intrface/titles.str
            fm.ini
            strings/newgame.str
            }

            Block 6:
            [other stuff]
            [other stuff]
            [other stuff]

            Block 7:
            [other stuff]

            Block 8:
            [other stuff]
            [other stuff]
            [other stuff]
            [other stuff]
            [other stuff]

            etc.
            */

            RunProcess(
                sourcePath: sourcePath,
                outputArchive: outputArchive,
                listFile: listFile,
                level: level,
                method: method,
                cancellationToken: cancellationToken);
        }
    }

    internal static void Run7z_Rest(
        string sourcePath,
        string outputArchive,
        string listFile,
        int level,
        CompressionMethod method,
        CancellationToken cancellationToken)
    {
        View.SetProgressMessage("Adding remaining files to archive...");
        View.SetProgressPercent(0);

        RunProcess(
            sourcePath: sourcePath,
            outputArchive: outputArchive,
            listFile: listFile,
            level: level,
            method: method,
            cancellationToken: cancellationToken);
    }

    private static string GetArgs(int level, CompressionMethod method)
    {
        if (level is < 0 or > 9)
        {
            level = 5;
        }

        /*
        -mx   = Compression level 0-9
        -m0   = You put a number for poorly explained reasons (0 here), then you put the actual compression method
                as a string
        -myv  = Don't use filters not supported by this version and below. 4 digits, ex. 1600 for 16.00
        -mhc  = Enable or disable header compression
        -y    = Say yes to all prompts automatically
        -r    = Recurse directories - basic thing required to put the files into the archive keeping the folder
                structure
        -bsp1 = Redirect progress information to stdout stream
        -bb1  = Show names of processed files in log (needed for smooth reporting of entries done count)
        -sas  = Add extension only if specified name has no extension. It's default option.
        -scs  = Set charset for list files (default is UTF-8, but let's be explicit)
        -t7z  = Explicitly set archive type to 7z
        */
        string args = "-mx=" + level;
        if (level == 0)
        {
            args += " -m0=Copy";
        }
        else
        {
            args += " -m0=" + GetCompressionMethodString(method);
            long dictSize = Config.DictionarySize;
            if (dictSize > -1)
            {
                args += ":d=" + dictSize.ToStrInv() + "b";
            }
        }

        /*
        Compatibility all the way back to FMSel with its v9.20. Of course it's highly unlikely any new filters
        will trigger as nobody will be putting ARM64 or RISC-V executables in their FMs, but hey.
        */
        args += " -myv=0920";

        int threads = Config.Threads;
        if (threads > -1)
        {
            args += " -mmt=" + threads.ToStrInv();
        }

        MemoryUseItem memUse = Config.MemoryUseForCompression;
        if (memUse.Value > -1)
        {
            args += " -mmemuse=";
            if (memUse.IsPercent)
            {
                args += "p" + memUse.Value.ToStrInv();
            }
            else
            {
                args += memUse.Value.ToStrInv() + "b";
            }
        }

        args += " -y -r -bsp1 -bb1 -sas -scsUTF-8 -t7z -mhc=off";

        // TODO: "Word size" for LZMA/LZMA2 is actually "fast bytes" (-m0=LZMA2:fb=N)

        //Trace.WriteLine("*** " + nameof(GetArgs) + ": " + args);
        return args;
    }

    private static CancellationTokenSource _cts = new();
    internal static void CancelToken()
    {
        View.SetProgressMessage("Canceling...");
        _cts.CancelIfNotDisposed();
    }

    internal static async Task CreateSingleArchive()
    {
        await Task.Run(static () =>
        {
            string sourcePath = View.SourceFMPath;
            string outputArchive = View.OutputArchive;

            try
            {
                View.StartCreateSingleArchiveOperation();

                _cts = _cts.Recreate();

                Directory.SetCurrentDirectory(sourcePath);

                try
                {
                    // TODO: Tell user if it exists and confirm delete!
                    File.Delete(outputArchive);
                }
                catch
                {

                }

                int level = Config.CompressionLevel;
                CompressionMethod method = Config.CompressionMethod;

                View.SetProgressMessage("Generating archive packaging logic...");

                (List<string> al_Scan_FileNames, string listFile_Rest) = GetListFile(sourcePath);

                Run7z_ALScanFiles(sourcePath, outputArchive, al_Scan_FileNames, level, method, _cts.Token);
                Run7z_Rest(sourcePath, outputArchive, listFile_Rest, level, method, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                try
                {
                    // TODO: Maybe we shouldn't delete the archive on cancel.
                    //  Instead, on create start, if the archive exists we'll tell the user and ask them to delete it.
                    //File.Delete(outputArchive);
                }
                catch
                {
                    // ignore
                }
            }
            finally
            {
                Utils.CreateOrClearTempPath(Paths.Temp);
                View.EndCreateSingleArchiveOperation("Successfully created '" + outputArchive + "'.");
                _cts.Dispose();
            }
        });
    }

    internal static (List<string> AL_FileNames, string RestListFile)
    GetListFile(string filesDir)
    {
        Utils.CreateOrClearTempPath(Paths.Temp);

        string[] files = Directory.GetFiles(filesDir, "*", SearchOption.AllDirectories);

        HashSet<string> dupePreventionHash = new(StringComparer.OrdinalIgnoreCase);

        List<string> alScanFileNames = new();
        List<string> restFileNames = new();
        for (int i = 0; i < files.Length; i++)
        {
            string fn = files[i].Substring(filesDir.Length).TrimStart('/', '\\');

            int dirSeps;
            long fileSize = new FileInfo(files[i]).Length;
            if ((fn.IsValidReadme() || fn.IsGlTitle()) &&
                // TODO: Should we be permissive and allow even 0-length readmes?
                fileSize > 0 &&
                (((dirSeps = fn.Rel_CountDirSepsUpToAmount(2)) == 1 &&
                  (fn.PathStartsWithI_AsciiSecond(FMDirs.T3FMExtras1S) ||
                   fn.PathStartsWithI_AsciiSecond(FMDirs.T3FMExtras2S))) ||
                 dirSeps == 0))
            {
                AddScanFile(fn);
            }
            // FMSel (both original and Sneaky Upgrade version) only look for this file in the root dir, not T3
            // readme dir(s), tested.
            else if (!fn.Rel_ContainsDirSep() &&
                     fn.EqualsI(FMFiles.FMThumbJpg))
            {
                AddScanFile(fn);
            }
            else if (!fn.Rel_ContainsDirSep() &&
                     (fn.ExtIsMis()
                     || fn.ExtIsGam()
                     ))
            {
                AddScanFile(fn);
            }
            else if (!fn.Rel_ContainsDirSep() &&
                     (fn.EqualsI(FMFiles.FMInfoXml) ||
                      fn.EqualsI(FMFiles.FMIni) ||
                      fn.EqualsI(FMFiles.ModIni)))
            {
                AddScanFile(fn);
            }
            else if (fn.PathStartsWithI_AsciiSecond(FMDirs.StringsS) &&
                     fn.PathEndsWithI(FMFiles.SMissFlag))
            {
                AddScanFile(fn);
            }
            else if (fn.PathEndsWithI(FMFiles.SNewGameStr))
            {
                AddScanFile(fn);
            }
            else if (EndsWithTitleFile(fn))
            {
                AddScanFile(fn);
            }
            else
            {
                restFileNames.Add(fn);
            }
        }

        if (Utils.TryGetInfoFileFromFmIni(filesDir, out string infoFile))
        {
            AddIfNotInList(infoFile);
        }

        if (Utils.TryGetReadmeFromModIni(filesDir, out string readme))
        {
            AddIfNotInList(readme);
        }

        //string listFile_ALScan = Path.Combine(TempPath, AL_Scan_Block_ListFileName);
        string listFile_Rest = Path.Combine(Paths.Temp, Rest_Block_ListFileName);

        foreach (string fileName in restFileNames)
        {
            if (!File.Exists(Path.Combine(filesDir, fileName)))
            {
                // TODO: Handle this better - placeholder for now
                string msg =
                    "----------\r\n" +
                    "Filename we're about to pass to 7z via the list file (" + nameof(GetListFile) +
                    ") doesn't exist on disk! Character encoding difference?\r\n" +
                    "Filename: +" + fileName + "\r\n" +
                    "----------";
                Trace.WriteLine(msg);
                MessageBox.Show(
                    msg,
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        //File.WriteAllLines(listFile_ALScan, alScanFileNames);
        File.WriteAllLines(listFile_Rest, restFileNames);

        return (alScanFileNames, listFile_Rest);

        void AddScanFile(string fn)
        {
            dupePreventionHash.Add(fn);
            alScanFileNames.Add(fn);
        }

        void AddIfNotInList(string value)
        {
            if (!value.IsEmpty() &&
                File.Exists(Path.Combine(filesDir, value)) &&
                dupePreventionHash.Add(value))
            {
                restFileNames.Remove(value);
                alScanFileNames.Add(value);
            }
        }

        static bool EndsWithTitleFile(string fileName)
        {
            return fileName.PathEndsWithI("/titles.str") ||
                   fileName.PathEndsWithI("/title.str");
        }
    }

    internal static string GetSevenZipVersion(string exe)
    {
        try
        {
            FileVersionInfo vi = FileVersionInfo.GetVersionInfo(exe);

            string versionString = vi.ProductMajorPart.ToStrInv() + "." +
                                   vi.ProductMinorPart.ToStrInv().PadLeft(2, '0');
            if (vi.ProductPrivatePart > 0)
            {
                versionString += "." + vi.ProductBuildPart.ToStrInv() + "." + vi.ProductPrivatePart.ToStrInv();
            }
            if (vi.ProductBuildPart > 0)
            {
                versionString += "." + vi.ProductBuildPart.ToStrInv();
            }

            return versionString;
        }
        catch
        {
            return "";
        }
    }
}
