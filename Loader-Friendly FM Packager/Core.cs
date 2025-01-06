using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        View.SetMode(Config.Mode);
        View.SetCompressionLevel(Config.CompressionLevel);
        View.SetCompressionMethod(Config.CompressionMethod);
        View.SetThreads(Config.Threads);

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
            sevenZipPathAndExe: Paths.SevenZipExe,
            sourcePath: sourcePath,
            outputArchive: outputArchive,
            args: GetArgs(level, method),
            cancellationToken: cancellationToken,
            listFile: listFile,
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

    private static void RunProcess_Update(
        string sourcePath,
        string outputArchive,
        List<FileToRename> itemsToRename,
        CancellationToken cancellationToken)
    {
        foreach (var item in itemsToRename)
        {
            Fen7z.Result updateResult = Fen7z.Update(
                sevenZipPathAndExe: Paths.SevenZipExe,
                sourcePath: sourcePath,
                outputArchive: outputArchive,
                originalFileName: item.TempSortedName,
                newFileName: item.Name,
                cancellationToken: cancellationToken
            );

            if (updateResult.ErrorOccurred)
            {
                // TODO: Handle the error. Also add logging maybe?
            }
            else if (updateResult.Canceled)
            {
                throw new OperationCanceledException(_cts.Token);
            }
            else
            {
                // TODO: Handle update complete
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
        ListFileData listFileData,
        int level,
        CompressionMethod method,
        CancellationToken cancellationToken)
    {
        string listFile = Path.Combine(Paths.Temp, AL_Scan_Block_ListFileName);

        if (listFileData.Readmes.Count > 0)
        {
            View.SetProgressPercent(0);
            View.SetProgressMessage("Adding readmes to their own block...");

            File.WriteAllLines(listFile, listFileData.Readmes);

            RunProcess(
                sourcePath: sourcePath,
                outputArchive: outputArchive,
                listFile: listFile,
                level: level,
                method: method,
                cancellationToken: cancellationToken);
        }

        if (listFileData.Thumbs.Count > 0)
        {
            View.SetProgressPercent(0);
            View.SetProgressMessage("Adding thumbs to their own block...");

            File.WriteAllLines(listFile, listFileData.Thumbs);

            RunProcess(
                sourcePath: sourcePath,
                outputArchive: outputArchive,
                listFile: listFile,
                level: level,
                method: method,
                cancellationToken: cancellationToken);
        }

        if (listFileData.MainImages.Count > 0)
        {
            View.SetProgressPercent(0);
            View.SetProgressMessage("Adding \"main_\" images to their own block...");

            File.WriteAllLines(listFile, listFileData.MainImages);

            RunProcess(
                sourcePath: sourcePath,
                outputArchive: outputArchive,
                listFile: listFile,
                level: level,
                method: method,
                cancellationToken: cancellationToken);
        }

        if (listFileData.GamFiles.Count > 0)
        {
            View.SetProgressPercent(0);
            View.SetProgressMessage("Adding .gam files to their own block...");

            File.WriteAllLines(listFile, listFileData.GamFiles);

            RunProcess(
                sourcePath: sourcePath,
                outputArchive: outputArchive,
                listFile: listFile,
                level: level,
                method: method,
                cancellationToken: cancellationToken);
        }

        for (int i = 0; i < listFileData.OnePerBlockItems.Count; i++)
        {
            string fileName = listFileData.OnePerBlockItems[i];

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
        ListFileData listFileData,
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

        // TODO: Maybe run a compare on the archive filenames to the disk ones, to detect any failed renames
        if (listFileData.FilesToRename.Count > 0)
        {
            View.SetProgressPercent(0);
            View.SetProgressMessage("Finishing...");

            RunProcess_Update(
                sourcePath: sourcePath,
                outputArchive: outputArchive,
                listFileData.FilesToRename,
                cancellationToken: cancellationToken);
        }
    }

    internal static string GetArgs(int level, CompressionMethod method, bool friendly = true)
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
        }

        /*
        Compatibility all the way back to FMSel with its v9.20. Of course it's highly unlikely any new filters
        will trigger as nobody will be putting ARM64 or RISC-V executables in their FMs, but hey.
        */
        args += " -myv=0920";

        int threads = Config.Threads;
        if (threads > -1)
        {
            Trace.WriteLine("********** threads non-default: " + threads);

            args += " -mmt=" + threads.ToStrInv();
        }

        args += " -y -r -bsp1 -bb1 -sas -t7z";

        if (friendly)
        {
            args += " -scsUTF-8 -mhc=off";
        }

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

                (ListFileData listFileData, string listFile_Rest) = GetListFile(ref sourcePath, makeCopyOfFilesDir: true, _cts.Token);

                Run7z_ALScanFiles(sourcePath, outputArchive, listFileData, level, method, _cts.Token);
                Run7z_Rest(sourcePath, outputArchive, listFile_Rest, listFileData, level, method, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                View.SetProgressMessage("Canceled.");
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

    internal static async Task RepackBatch()
    {
        // TODO: Implement
    }

    /*
    TODO(Logic): We need to have fewer things in blocks by themselves; it's causing large file size increases in
     a bunch of files. We need to see if 7z will pack the files into blocks in the order they're specified in
     the list file. We need to know if we can just put the smallest used mis file as the first line in the list
     file for example, and have 7z be guaranteed to put it at the start of the block.

    IMPORTANT(Update on order): 7z puts the files in name order (or extension-then-name order) no matter what
     order you put them in the list file. So we can't do the start-of-block thing.

    But we could still put just the file we need in its own block, and the rest in another block together, that
    should at least help.

    TODO(Update on size testing):
    Of the 97 files that were >=2% larger, now only 13 are >=5%, and the largest is 67% rather than 94%.
    To get better, we'd have to make a custom version of 7-zip that allows determining block position by more
    than just name or extension, neither of which work for our particular case.

    ---

    TODO(Update on size testing): We can actually just do like this:

    Say we had these files:

    miss20.mis
    miss21.mis
    miss22.mis

    And miss22.mis was our smallest used .mis file. We then just rename it so it sorts at the top:

    miss22.mis -> aaaaaaa_miss22.mis

    (but then we're renaming a user's file, so we should I guess just copy the entire FM source dir to a temp dir
    first?)

    Then we add all 3 files to the archive, then we just rename the file inside the archive ("7z.exe rn")
    back to miss22.mis. Its name will now be correct but it will also still be at the start of the block.
    Hacky but whatever, it prevents having to have a whole custom version of 7-zip and all.

    Make sure we pass -qs=off as an arg (even though it's the default) just to be explicit.

    I guess also pass -mhc=off again in case it wants to compress the header when it changes the file name in it?
    Presumably?

    Also if any relevant file names are non-ascii then I guess maybe just skip this step and fall back? Cause you
    can only pass renames on the command line, not in a list file or anything, and the command line has never even
    heard of Unicode...

    ---

    That way, we could lay out the blocks like this:

    {
    miss24.mis <- smallest
    miss20.mis
    miss21.mis
    miss22.mis
    miss23.mis
    }

    {
    bar.gam <- smallest
    foo.gam
    }

    {
    [all readme files]
    }

    // These are in their own block and not with the readmes, because we don't want to have them in the way during
    // the scan
    {
    [all html referenced files]
    }

    {
    [all main_* image files]
    }

    {
    gltitle.*
    fmthumb.*
    }

    {
    fm.ini
    mod.ini
    fminfo.xml
    }

    {
    all title(s).str (in order scanner wants them)
    all newgame.str  (in order scanner wants them)
    all missflag.str (order doesn't matter - they're language agnostic)
    }

    // It's unlikely anything extra will end up here that isn't already elsewhere, so we don't have to care too
    // much about the size effect of this block
    {
    [all files referenced in fm.ini and mod.ini]
    }
    */

    /*
    For certain types of files, we want to keep them in a multi-file block, but put them at the start of said
    block. The only way to force 7-Zip to do this is by making sure the file you want at the start of the block
    has a name that sorts to the top. So we temporarily rename the appropriate files to ensure this, then rename
    them back to their correct names once they're in the archive.
    */
    internal sealed class NameAndTempName
    {
        internal readonly string Name;
        internal string TempSortedName;
        internal readonly bool IsInBaseDir;
        internal string PhysicalName => !TempSortedName.IsEmpty() ? TempSortedName : Name;

        public NameAndTempName(string name, string tempSortedName, bool isInBaseDir)
        {
            Name = name;
            TempSortedName = tempSortedName;
            IsInBaseDir = isInBaseDir;
        }
    }

    internal sealed class FileToRename
    {
        internal readonly string Name;
        internal readonly string TempSortedName;

        internal static bool TryCreate(NameAndTempName item, [NotNullWhen(true)] out FileToRename? result)
        {
            if (!item.Name.IsEmpty() && !item.TempSortedName.IsEmpty())
            {
                result = new FileToRename(item.Name, item.TempSortedName);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        private FileToRename(string name, string tempSortedName)
        {
            Name = name;
            TempSortedName = tempSortedName;
        }
    }

    internal sealed class ListFileData
    {
        /*
        We get much better compression by putting as many binary files into the same block as possible. This
        includes both .mis and .gam files, and many other types of files in an FM. However, we need both smallest
        used .mis and smallest .gam files to be at the start of their respective blocks, so we can't put them
        both in with the big remaining-files block. We choose to put .mis files in with the remaining-files block
        because they're almost always larger (and usually MUCH larger) than .gam files, and there are more likely
        to be more of them. Thus, a larger amount of data gets packed into the block, resulting in much better
        compression ratios, and greatly lowering the outlier size increases.
        */

        internal readonly List<string> GamFiles = new();
        internal readonly List<string> Readmes = new();
        internal readonly List<string> MainImages = new();
        internal readonly List<string> Thumbs = new();

        internal readonly List<string> OnePerBlockItems = new();

        internal readonly List<FileToRename> FilesToRename = new();
    }

    internal static (ListFileData ListFileData, string RestListFile)
    GetListFile(ref string filesDir, bool makeCopyOfFilesDir, CancellationToken cancellationToken)
    {
        View.SetProgressMessage("Preparing...");

        ListFileData ret = new();

        List<NameAndTempName> misFiles = new();
        List<NameAndTempName> gamFiles = new();

        Utils.CreateOrClearTempPath(Paths.Temp);
        Utils.CreateOrClearTempPath(Paths.Temp_SourceCopy);

        string[] files = Directory.GetFiles(filesDir, "*", SearchOption.AllDirectories);

        /*
        TODO: This is slow. We can't just copy our to-be-renamed files either because they need to be in the
         stupid list file with paths relative to the main folder if they want to be in the same block as other
         stuff, which they do.
         We could:
         -Keep this slow copy.
         -Do some crazy garbage with symbolic links (assuming that works).
         -Compile our own version of 7-zip that can take a list of files to sort to the top of their block.
         -Renaming the files in-place is too dangerous no matter how carefully we backup and restore the originals,
          as we'd be messing with a user's actual working FM files, which to me is a hard no.
        */
        if (makeCopyOfFilesDir)
        {
            View.SetProgressMessage("Making a copy of source FM directory...");
            View.SetProgressPercent(0);

            // TODO: Error handling needed
            string[] sourceFiles = Directory.GetFiles(filesDir, "*", SearchOption.AllDirectories);
            for (int i = 0; i < sourceFiles.Length; i++)
            {
                string file = sourceFiles[i];
                string destFile = Path.Combine(Paths.Temp_SourceCopy, file.RemoveLeadingPath(filesDir));
                string? dirName = Path.GetDirectoryName(destFile);
                if (!dirName.IsEmpty())
                {
                    Directory.CreateDirectory(dirName);

                    cancellationToken.ThrowIfCancellationRequested();
                }
                File.Copy(file, destFile);

                cancellationToken.ThrowIfCancellationRequested();

                View.SetProgressPercent(Utils.GetPercentFromValue_Int(i + 1, sourceFiles.Length));
            }

            filesDir = Paths.Temp_SourceCopy;
            files = Directory.GetFiles(filesDir, "*", SearchOption.AllDirectories);
        }

        View.SetProgressMessage("Generating archive packaging logic...");
        View.SetProgressPercent(0);

        HashSet<string> dupePreventionHash = new(StringComparer.OrdinalIgnoreCase);

        List<string> readmeFullFilePaths = new();

        //List<string> alScanFileNames = new();
        List<string> restFileNames = new();
        for (int i = 0; i < files.Length; i++)
        {
            string fn = files[i].RemoveLeadingPath(filesDir);

            int dirSeps;
            long fileSize = new FileInfo(files[i]).Length;
            if ((fn.IsValidReadme() || fn.IsGlTitle()) &&
                fileSize > 0 &&
                (((dirSeps = fn.Rel_CountDirSepsUpToAmount(2)) == 1 &&
                  (fn.PathStartsWithI(FMDirs.T3FMExtras1S) ||
                   fn.PathStartsWithI(FMDirs.T3FMExtras2S))) ||
                 dirSeps == 0))
            {
                AddScanFile(fn.IsGlTitle() ? ret.Thumbs : ret.Readmes, fn);
                readmeFullFilePaths.Add(files[i]);
            }
            /*
            FMSel (both original and Sneaky Upgrade version) only look for this file in the root dir, not T3
            readme dir(s), tested.
            Also, FMSel only reads fmthumb.jpg, but there's at least one FM with an fmthumb.png, so why not just
            allow any image type.
            */
            else if (IsInBaseDir(fn) &&
                     Path.GetFileNameWithoutExtension(fn).EqualsI(FMFiles.FMThumb) &&
                     Utils.FileExtensionFound(fn, FMFileExtensions.ImageFileExtensions))
            {
                AddScanFile(ret.Thumbs, fn);
            }
            else if (fn.ExtIsMis())
            {
                AddScanFile_Renameable(misFiles, fn);
            }
            else if (fn.ExtIsGam())
            {
                AddScanFile_Renameable(gamFiles, fn);
            }
            else if (IsInBaseDir(fn) &&
                     (fn.EqualsI(FMFiles.FMInfoXml) ||
                      fn.EqualsI(FMFiles.FMIni) ||
                      fn.EqualsI(FMFiles.ModIni)))
            {
                AddScanFile(ret.OnePerBlockItems, fn);
            }
            else if (fn.PathStartsWithI(FMDirs.StringsS) &&
                     fn.PathEndsWithI(FMFiles.SMissFlag))
            {
                AddScanFile(ret.OnePerBlockItems, fn);
            }
            else if (fn.PathStartsWithI(FMDirs.IntrfaceS) &&
                     Regex.Match(Path.GetFileNameWithoutExtension(fn), "^main_[0-9]+$",
                         RegexOptions.IgnoreCase | RegexOptions.CultureInvariant).Success &&
                     Utils.FileExtensionFound(fn, FMFileExtensions.ImageFileExtensions))
            {
                AddScanFile(ret.MainImages, fn);
            }
            else if (fn.PathStartsWithI(FMDirs.IntrfaceS) &&
                     fn.PathEndsWithI(FMFiles.SNewGameStr))
            {
                AddScanFile(ret.OnePerBlockItems, fn);
            }
            else if (fn.PathStartsWithI(FMDirs.StringsS) &&
                     EndsWithTitleFile(fn))
            {
                AddScanFile(ret.OnePerBlockItems, fn);
            }
            else
            {
                restFileNames.Add(fn);
            }
        }

        /*
        Limit the values to the formats they're supposed to be, to keep their hands off any of our temp-renamed
        files
        */
        if (Utils.TryGetInfoFileFromFmIni(filesDir, out string infoFile) && infoFile.IsValidReadme())
        {
            AddIfNotInList(ret.Readmes, infoFile, ref filesDir);
        }

        var modIniValues = Utils.GetValuesFromModIni(filesDir);
        if (!modIniValues.Readme.IsEmpty() && modIniValues.Readme.IsValidReadme())
        {
            AddIfNotInList(ret.Readmes, modIniValues.Readme, ref filesDir);
        }
        if (!modIniValues.Icon.IsEmpty() && modIniValues.Icon.ExtIsIco())
        {
            AddIfNotInList(ret.Thumbs, modIniValues.Icon, ref filesDir);
        }

        List<string> htmlRefFiles = HtmlReference.GetHtmlReferenceFiles(filesDir, readmeFullFilePaths, files);
        foreach (string htmlRefFile in htmlRefFiles)
        {
            // If it's referencing a .mis or .gam file then it will probably be broken by our renaming, so just
            // leave those alone and take whatever hit we take
            if (!htmlRefFile.ExtIsMis() && !htmlRefFile.ExtIsGam())
            {
                AddIfNotInList(ret.OnePerBlockItems, htmlRefFile, ref filesDir);
            }
        }

        #region Temp rename files for sorting

        // If this name doesn't end up sorting at the top then someone's being a troll, so oh well.
        const string sortAtTopPrefix = "!!!!!!!!_";

        if (Utils.TryGetSmallestUsedMisFile(filesDir, out string smallestUsedMisFile))
        {
            AddRenameable(misFiles, smallestUsedMisFile, ref filesDir);
        }

        if (Utils.TryGetSmallestGamFile(filesDir, out string smallestGamFile))
        {
            AddRenameable(gamFiles, smallestGamFile, ref filesDir);
        }

        static void AddRenameable(List<NameAndTempName> potentialRenameables, string targetFileName, ref string filesDir)
        {
            int index = potentialRenameables.FindIndex(x => x.Name.EqualsI(targetFileName));
            Trace.Assert(index > -1);

            NameAndTempName item = potentialRenameables[index];
            Trace.Assert(IsInBaseDir(item.Name));

            string newName = sortAtTopPrefix + item.Name;

            // If there actually is already a file named like "!!!!!!!!_miss21.mis" then someone's being a troll
            // so just give up.
            // TODO: But we should log it... it could be we couldn't or didn't clear our source folder and we
            //  have leftover renamed files?
            if (!potentialRenameables.Any(x => x.Name.EqualsI(newName)))
            {
                item.TempSortedName = newName;

                // TODO: Error handling needed
                File.Move(
                    Path.Combine(filesDir, item.Name),
                    Path.Combine(filesDir, item.TempSortedName));
            }
        }

        #endregion

        List<string> finalRestFileNames = new();

        foreach (NameAndTempName item in gamFiles)
        {
            ret.GamFiles.Add(item.PhysicalName);
            if (FileToRename.TryCreate(item, out FileToRename? fileToRename))
            {
                ret.FilesToRename.Add(fileToRename);
            }
        }

        foreach (NameAndTempName item in misFiles)
        {
            finalRestFileNames.Add(item.PhysicalName);
            if (FileToRename.TryCreate(item, out FileToRename? fileToRename))
            {
                ret.FilesToRename.Add(fileToRename);
            }
        }

        finalRestFileNames.AddRange(restFileNames);

        //string listFile_ALScan = Path.Combine(TempPath, AL_Scan_Block_ListFileName);
        string listFile_Rest = Path.Combine(Paths.Temp, Rest_Block_ListFileName);

        foreach (string fileName in finalRestFileNames)
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
        File.WriteAllLines(listFile_Rest, finalRestFileNames);

        return (ret, listFile_Rest);

        void AddScanFile(List<string> list, string fn)
        {
            string valNormalized = fn.ToBackSlashes();
            dupePreventionHash.Add(valNormalized);
            list.Add(valNormalized);
        }

        void AddScanFile_Renameable(List<NameAndTempName> list, string fn)
        {
            dupePreventionHash.Add(fn);
            list.Add(new NameAndTempName(fn, "", IsInBaseDir(fn)));
        }

        void AddIfNotInList(List<string> list, string value, ref string filesDir_)
        {
            string valNormalized = value.ToBackSlashes();
            if (!valNormalized.IsEmpty() &&
                File.Exists(Path.Combine(filesDir_, valNormalized)) &&
                dupePreventionHash.Add(valNormalized))
            {
                restFileNames.Remove(valNormalized);
                list.Add(valNormalized);
            }
        }

        static bool EndsWithTitleFile(string fileName)
        {
            return fileName.PathEndsWithI("/titles.str") ||
                   fileName.PathEndsWithI("/title.str");
        }

        static bool IsInBaseDir(string fn) => !fn.Rel_ContainsDirSep();
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
