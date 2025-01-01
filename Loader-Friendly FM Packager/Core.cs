using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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

        ReadConfigData();

        View.CompressionLevel = Config.CompressionLevel;
        View.CompressionMethod = Config.CompressionMethod;
        View.Threads = Config.Threads;
        View.DictionarySize = Config.DictionarySize;
        View.MemoryUseForCompression = Config.MemoryUseForCompression;
        View.StoreCreationTime = Config.StoreCreationTime;
        View.StoreLastAccessTime = Config.StoreLastAccessTime;
        View.SetArchiveTimeToLatestFileTime = Config.SetArchiveTimeToLatestTileTime;
        View.DoNotChangeSourceFilesLastAccessTime = Config.DoNotChangeSourceFilesLastAccessTime;
        View.SevenZipApp = Config.SevenZipApp;
        View.SevenZipExternalAppPath = Config.SevenZipExternalAppPath;

        View.Show();
    }

    internal static void Shutdown()
    {
        WriteConfigData();
        Application.Exit();
    }

    private static void ReadConfigData()
    {
        if (!File.Exists(Paths.ConfigFile))
        {
            return;
        }

        const BindingFlags _bFlagsEnum =
        BindingFlags.Instance |
        BindingFlags.Static |
        BindingFlags.Public |
        BindingFlags.NonPublic;

        string[] lines = File.ReadAllLines(Paths.ConfigFile);
        for (int i = 0; i < lines.Length; i++)
        {
            string lineT = lines[i].Trim();
            if (lineT.TryGetValueO("CompressionLevel=", out string value))
            {
                if (int.TryParse(value, out int result))
                {
                    Config.CompressionLevel = result;
                }
            }
            else if (lineT.TryGetValueO("CompressionMethod=", out value))
            {
                FieldInfo? field = typeof(CompressionMethod).GetField(value, _bFlagsEnum);
                if (field != null)
                {
                    Config.CompressionMethod = (CompressionMethod)field.GetValue(null);
                }
            }
            else if (lineT.TryGetValueO("Threads=", out value))
            {
                if (int.TryParse(value, out int result))
                {
                    Config.Threads = result;
                }
            }
            else if (lineT.TryGetValueO("DictionarySize=", out value))
            {
                if (long.TryParse(value, out long result))
                {
                    Config.DictionarySize = result;
                }
            }
            else if (lineT.TryGetValueO("SevenZipApp=", out value))
            {
                FieldInfo? field = typeof(SevenZipApp).GetField(value, _bFlagsEnum);
                if (field != null)
                {
                    Config.SevenZipApp = (SevenZipApp)field.GetValue(null);
                }
            }
            else if (lineT.TryGetValueO("SevenZipExternalAppPath=", out value))
            {
                Config.SevenZipExternalAppPath = value.Trim();
            }
            else if (lineT.TryGetValueO("MemoryUsageForCompressing=", out value))
            {
                bool isPercent = false;
                if (value.Length > 0 && value[^1] == '%')
                {
                    isPercent = true;
                    value = value.TrimEnd('%');
                }
                if (long.TryParse(value, out long result))
                {
                    Config.MemoryUseForCompression = new MemoryUseItem(result, isPercent);
                }
            }
            else if (lineT.TryGetValueO("StoreCreationTime=", out value))
            {
                Config.StoreCreationTime = GetNullableBoolValue(value);
            }
            else if (lineT.TryGetValueO("StoreLastAccessTime=", out value))
            {
                Config.StoreLastAccessTime = GetNullableBoolValue(value);
            }
            else if (lineT.TryGetValueO("SetArchiveTimeToLatestTileTime=", out value))
            {
                Config.SetArchiveTimeToLatestTileTime = GetNullableBoolValue(value);
            }
            else if (lineT.TryGetValueO("DoNotChangeSourceFilesLastAccessTime=", out value))
            {
                Config.DoNotChangeSourceFilesLastAccessTime = value.EqualsI(bool.TrueString);
            }
        }

        return;

        static bool? GetNullableBoolValue(string value)
        {
            if (value == bool.TrueString)
            {
                return true;
            }
            else if (value == bool.FalseString)
            {
                return false;
            }
            else
            {
                return null;
            }
        }
    }

    private static void WriteConfigData()
    {
        using var sw = new StreamWriter(Paths.ConfigFile);
        sw.WriteLine("CompressionLevel=" + Config.CompressionLevel.ToStrInv());
        sw.WriteLine("CompressionMethod=" + Config.CompressionMethod);
        sw.WriteLine("Threads=" + Config.Threads.ToStrInv());
        sw.WriteLine("DictionarySize=" + Config.DictionarySize.ToStrInv());
        sw.WriteLine("SevenZipApp=" + Config.SevenZipApp);
        sw.WriteLine("MemoryUsageForCompressing=" +
                     (Config.MemoryUseForCompression.IsPercent && Config.MemoryUseForCompression.Value != -1
                         ? Config.MemoryUseForCompression.Value + "%"
                         : Config.MemoryUseForCompression.Value));
        sw.WriteLine("StoreCreationTime=" + GetNullableBoolValue(Config.StoreCreationTime));
        sw.WriteLine("StoreLastAccessTime=" + GetNullableBoolValue(Config.StoreLastAccessTime));
        sw.WriteLine("SetArchiveTimeToLatestTileTime=" + GetNullableBoolValue(Config.SetArchiveTimeToLatestTileTime));
        sw.WriteLine("DoNotChangeSourceFilesLastAccessTime=" + Config.DoNotChangeSourceFilesLastAccessTime);
        sw.WriteLine("SevenZipExternalAppPath=" + Config.SevenZipExternalAppPath);

        return;

        static string GetNullableBoolValue(bool? value) => value switch
        {
            true => bool.TrueString,
            false => bool.FalseString,
            _ => "",
        };
    }

    private static void RunProcess(
    string sourcePath,
    string outputArchive,
    string listFile,
    int level,
    int methodIndex)
    {
        Process p = new();
        p.StartInfo.FileName = Path.Combine(Application.StartupPath, "7z", "7z.exe");
        p.StartInfo.WorkingDirectory = sourcePath;
        p.StartInfo.Arguments =
            "a " + GetArgs(level, methodIndex) + " \"" +
            outputArchive + "\" " +
            "@\"" + listFile + "\"";

        //Trace.WriteLine(p.StartInfo.Arguments);

        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.UseShellExecute = false;

        p.OutputDataReceived += (sender, e) =>
        {
            if (e.Data.IsEmpty()) return;
            try
            {
                Trace.WriteLine(e.Data);
            }
            catch
            {
                // ignore, it just means we won't report progress... meh
            }
        };

        p.ErrorDataReceived += (_, e) =>
        {
            if (!e.Data.IsWhiteSpace())
            {
                Trace.WriteLine("**** ERROR:\r\n" + e.Data);
            }
        };

        try
        {
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();
        }
        catch (Exception ex)
        {

        }
        finally
        {
            try
            {
                if (!p.HasExited)
                {
                    p.Kill();
                }
            }
            catch
            {
                // ignore
            }
            finally
            {
                try
                {
                    if (!p.HasExited)
                    {
                        p.WaitForExit();
                    }
                }
                catch
                {
                    // ignore...
                }

                p.Dispose();
            }
            try
            {
                if (!listFile.IsEmpty())
                {
                    File.Delete(listFile);
                }
            }
            catch
            {
                // ignore
            }
        }
    }

    /*
    -Put every file in its own block - that way AL can do extract cost calculations and really just get the first
     used .mis file.
    -Use a list file even when there's only one entry in it, to prevent any filename encoding issues.
    */
    internal static void Run7z_ALScanFiles(string sourcePath, string outputArchive, List<string> fileNames, int level, int methodIndex)
    {
        string listFile = Path.Combine(Paths.Temp, AL_Scan_Block_ListFileName);

        for (int i = 0; i < fileNames.Count; i++)
        {
            string fileName = fileNames[i];

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
                methodIndex: methodIndex);
        }
    }

    internal static void Run7z_Rest(string sourcePath, string outputArchive, string listFile, int level, int methodIndex)
    {
        RunProcess(
            sourcePath: sourcePath,
            outputArchive: outputArchive,
            listFile: listFile,
            level: level,
            methodIndex: methodIndex);
    }

    private static string GetArgs(int level, int methodIndex)
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
            args += " -m0=" + CompressionMethodArgStrings[methodIndex];
        }

        /*
        Compatibility all the way back to FMSel with its v9.20. Of course it's highly unlikely any new filters
        will trigger as nobody will be putting ARM64 or RISC-V executables in their FMs, but hey.
        */
        args += " -myv=0920";

        args += " -y -r -bsp1 -bb1 -sas -scsUTF-8 -t7z -mhc=off";

        // TODO: "Word size" for LZMA/LZMA2 is actually "fast bytes" (-m0=LZMA2:fb=N)

        //Trace.WriteLine("*** " + nameof(GetArgs) + ": " + args);
        return args;
    }

    internal static void CreateSingleArchive()
    {
        try
        {
            string dir = View.SourceFMDir;
            Directory.SetCurrentDirectory(dir);

            try
            {
                File.Delete(View.OutputArchive);
            }
            catch
            {

            }

            int level = View.CompressionLevel;
            int methodIndex = (int)View.CompressionMethod;

            (List<string> al_Scan_FileNames, string listFile_Rest) = GetListFile(dir);
            Run7z_ALScanFiles(dir, View.OutputArchive, al_Scan_FileNames, level, methodIndex);
            Run7z_Rest(dir, View.OutputArchive, listFile_Rest, level, methodIndex);
        }
        finally
        {
            Utils.CreateOrClearTempPath(Paths.Temp);
        }
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
