﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;

namespace Loader_Friendly_FM_Packager;

internal static class Utils
{
    public static TSource? FirstOrDefault_PastFirstIndex<TSource>(this TSource[] source, Func<TSource, bool> predicate)
    {
        for (int i = 0; i < source.Length; i++)
        {
            TSource item = source[i];
            if (i > 0 && predicate(item))
            {
                return item;
            }
        }

        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ToStrInv(this int value) => value.ToString(NumberFormatInfo.InvariantInfo);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ToStrInv(this ulong value) => value.ToString(NumberFormatInfo.InvariantInfo);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ToStrInv(this long value) => value.ToString(NumberFormatInfo.InvariantInfo);

    /// <summary>
    /// Returns true if <paramref name="value"/> is null or empty.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [ContractAnnotation("null => true")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsEmpty([NotNullWhen(false)] this string? value) => string.IsNullOrEmpty(value);

    /// <summary>
    /// Returns true if <paramref name="value"/> is null, empty, or whitespace.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [ContractAnnotation("null => true")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsWhiteSpace([NotNullWhen(false)] this string? value) => string.IsNullOrWhiteSpace(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool EqualsI(this string str, string value) => str.Equals(value, StringComparison.OrdinalIgnoreCase);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool StartsWithO(this string str, string value) => str.StartsWith(value, StringComparison.Ordinal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool StartsWithI(this string str, string value) => str.StartsWith(value, StringComparison.OrdinalIgnoreCase);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool EndsWithO(this string str, string value) => str.EndsWith(value, StringComparison.Ordinal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool EndsWithI(this string str, string value) => str.EndsWith(value, StringComparison.OrdinalIgnoreCase);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ExtIsTxt(this string value) => value.EndsWithI(".txt");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ExtIsRtf(this string value) => value.EndsWithI(".rtf");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ExtIsWri(this string value) => value.EndsWithI(".wri");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ExtIsHtml(this string value) => value.EndsWithI(".html") || value.EndsWithI(".htm");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ExtIsGlml(this string value) => value.EndsWithI(".glml");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ExtIsMis(this string value) => value.EndsWithI(".mis");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ExtIsGam(this string value) => value.EndsWithI(".gam");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ExtIsDoc(this string value) => value.EndsWithI(".doc");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ExtIsPdf(this string value) => value.EndsWithI(".pdf");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool ExtIsIco(this string value) => value.EndsWithI(".ico");

    internal static bool IsValidReadme(this string readme) =>
        readme.ExtIsTxt() ||
        readme.ExtIsRtf() ||
        readme.ExtIsWri() ||
        readme.ExtIsGlml() ||
        readme.ExtIsHtml() ||
        readme.ExtIsDoc() ||
        readme.ExtIsPdf();

    internal static bool FileExtensionFound(string fn, string[] extensions)
    {
        foreach (string extension in extensions)
        {
            if (fn.EndsWithI(extension))
            {
                return true;
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsGlTitle(this string readme)
    {
        return Path.GetFileNameWithoutExtension(readme).EqualsI("gltitle") && FileExtensionFound(readme, FMFileExtensions.ImageFileExtensions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsDirSep(this char character) => character is '/' or '\\';

    /// <summary>
    /// Returns the number of directory separators in a string, earlying-out once it's counted <paramref name="maxToCount"/>
    /// occurrences.
    /// <para>Do NOT use for full (non-relative) paths as it will count the "\\" at the start of UNC paths! </para>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="maxToCount">The maximum number of occurrences to count before earlying-out.</param>
    /// <returns></returns>
    internal static int Rel_CountDirSepsUpToAmount(this string value, int maxToCount)
    {
        int foundCount = 0;
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i].IsDirSep())
            {
                foundCount++;
                if (foundCount == maxToCount) break;
            }
        }

        return foundCount;
    }

    internal static void File_UnSetReadOnly(string fileOnDiskFullPath, bool throwException = false)
    {
        try
        {
            new FileInfo(fileOnDiskFullPath).IsReadOnly = false;
        }
        catch (Exception ex)
        {
            if (throwException) throw;
        }
    }

    internal static void DirAndFileTree_UnSetReadOnly(string path, bool throwException = false)
    {
        foreach (string f in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
        {
            File_UnSetReadOnly(f, throwException);
        }
    }

    internal static void CreateOrClearTempPath(string tempPath)
    {
        if (Directory.Exists(tempPath))
        {
            try
            {
                DirAndFileTree_UnSetReadOnly(tempPath, throwException: true);
            }
            catch (Exception ex)
            {
            }

            try
            {
                foreach (string f in Directory.GetFiles(tempPath, "*"))
                {
                    File.Delete(f);
                }
                foreach (string d in Directory.GetFiles(tempPath, "*"))
                {
                    Directory.Delete(d, recursive: true);
                }
            }
            catch (Exception ex)
            {
            }
        }
        else
        {
            try
            {
                Directory.CreateDirectory(tempPath);
            }
            catch (Exception ex)
            {
            }
        }
    }

    public static bool Contains(this string value, string substring, StringComparison comparison) =>
        // If substring is empty, IndexOf(string) returns 0, which would be a false "success" return
        !value.IsEmpty() && !substring.IsEmpty() && value.IndexOf(substring, comparison) >= 0;

    /// <summary>
    /// Determines whether a string contains a specified substring. Uses <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="substring"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsI(this string value, string substring) => value.Contains(substring, StringComparison.OrdinalIgnoreCase);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool StartsWithDirSep(this string value) => value.Length > 0 && value[0].IsDirSep();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EndsWithDirSep(this string value) => value.Length > 0 && value[^1].IsDirSep();

    internal static string ToBackSlashes(this string value) => value.Replace('/', '\\');

    /// <summary>
    /// Returns true if <paramref name="value"/> contains either directory separator character.
    /// <para>Do NOT use for full (non-relative) paths as it will count the "\\" at the start of UNC paths! </para>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static bool Rel_ContainsDirSep(this string value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i].IsDirSep()) return true;
        }
        return false;
    }

    /// <summary>
    /// Path starts-with check ignoring case and directory separator differences.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    internal static bool PathStartsWithI(this string first, string second)
    {
        return first.Length >= second.Length &&
               first.ToBackSlashes().StartsWithI(second.ToBackSlashes());
    }

    /// <summary>
    /// Path ends-with check ignoring case and directory separator differences.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    internal static bool PathEndsWithI(this string first, string second)
    {
        return first.Length >= second.Length &&
               first.ToBackSlashes().EndsWithI(second.ToBackSlashes());
    }

    internal static string RemoveLeadingPath(this string fullPath, string leadingPath)
    {
        return fullPath.Substring(leadingPath.Length).TrimStart('/', '\\');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsIniHeader(this string line) => !line.IsEmpty() && line[0] == '[' && line[^1] == ']';

    // TODO: We need to run encoding detection on this too...
    // Or we could just check if the InfoFile= value is ascii, and if not then oh well - fm.ini may be
    // too small to accurately detect encoding anyway?
    // TODO: Error handling needed here
    // TODO: Test this thoroughly (create FM archives that cover all expected cases)
    internal static bool TryGetInfoFileFromFmIni(string filesDir, out string infoFile)
    {
        infoFile = "";

        string fmIniFile = Path.Combine(filesDir, FMFiles.FMIni);

        if (!File.Exists(fmIniFile))
        {
            return false;
        }

        string[] lines = File.ReadAllLines(fmIniFile);

        for (int i = 0; i < lines.Length; i++)
        {
            string lineT = lines[i].Trim();
            if (lineT.StartsWithI("InfoFile="))
            {
                infoFile = lineT.Substring("InfoFile=".Length);
            }
        }

        return !infoFile.IsEmpty();
    }

    internal static (string Readme, string Icon)
    GetValuesFromModIni(string filesDir)
    {
        string readme = "";
        string icon = "";

        string modIniFile = Path.Combine(filesDir, FMFiles.ModIni);

        if (!File.Exists(modIniFile))
        {
            return ("", "");
        }

        string[] lines = File.ReadAllLines(modIniFile);

        for (int i = 0; i < lines.Length; i++)
        {
            string lineT = lines[i].Trim();
            if (lineT.EqualsI("[readMe]"))
            {
                while (i < lines.Length - 1)
                {
                    string lt = lines[i + 1].Trim();
                    if (lt.IsIniHeader())
                    {
                        break;
                    }
                    else if (!lt.IsEmpty() && lt[0] != ';')
                    {
                        readme = lt;
                        break;
                    }
                    i++;
                }
            }
            else if (lineT.EqualsI("[iconFile]"))
            {
                while (i < lines.Length - 1)
                {
                    string lt = lines[i + 1].Trim();
                    if (lt.IsIniHeader())
                    {
                        break;
                    }
                    else if (!lt.IsEmpty() && lt[0] != ';')
                    {
                        icon = lt;
                        break;
                    }
                    i++;
                }
            }
        }

        return (readme, icon);
    }

    #region Clamping

    /// <summary>
    /// Clamps a number to between <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T> =>
        value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;

    /// <summary>
    /// If <paramref name="value"/> is less than zero, returns zero. Otherwise, returns <paramref name="value"/>
    /// unchanged.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ClampToZero(this int value) => Math.Max(value, 0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ClampZeroToOne(this float value) => value.Clamp(0, 1.0f);

    /// <summary>
    /// Clamps a number to <paramref name="min"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <returns></returns>
    public static T ClampToMin<T>(this T value, T min) where T : IComparable<T> => value.CompareTo(min) < 0 ? min : value;

    #endregion

    internal static bool TryGetValueO(this string line, string key, out string value)
    {
        if (line.StartsWithO(key))
        {
            value = line.Substring(key.Length);
            return true;
        }
        else
        {
            value = "";
            return false;
        }
    }

    /// <summary>
    /// Calls <see langword="int"/>.TryParse(<paramref name="s"/>, <see cref="NumberStyles.Integer"/>, <see cref="NumberFormatInfo.InvariantInfo"/>, out <see langword="int"/> <paramref name="result"/>);
    /// </summary>
    /// <param name="s">A string representing a number to convert.</param>
    /// <param name="result"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <returns><see langword="true"/> if <paramref name="s"/> was converted successfully; otherwise, <see langword="false"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Int_TryParseInv(string s, out int result)
    {
        return int.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out result);
    }

    public static int GetPercentFromValue_Int(int current, int total) => total == 0 ? 0 : (100 * current) / total;

    internal static void CancelIfNotDisposed(this CancellationTokenSource value)
    {
        try { value.Cancel(); } catch (ObjectDisposedException) { }
    }

    /// <summary>
    /// Disposes and assigns a new one.
    /// </summary>
    /// <param name="cts"></param>
    /// <returns></returns>
    [MustUseReturnValue]
    internal static CancellationTokenSource Recreate(this CancellationTokenSource cts)
    {
        cts.Dispose();
        return new CancellationTokenSource();
    }

    internal static bool TryGetSmallestGamFile(string fmDir, out string smallestGamFile)
    {
        smallestGamFile = "";

        List<FileInfo> gamFileInfos;
        try
        {
            gamFileInfos = new DirectoryInfo(fmDir).GetFiles("*.gam", SearchOption.TopDirectoryOnly).ToList();
        }
        catch (Exception ex)
        {
            // TODO: Error handling needed
            //string msg = "Error trying to get .gam files in FM installed directory.";
            //fm.LogInfo(msg + " " + ErrorText.RetF, ex);
            //Core.Dialogs.ShowError(msg + $"{NL}{NL}" + ErrorText.OldDarkDependentFeaturesWillFail);
            return false;
        }

        // Workaround https://fenphoenix.github.io/AngelLoader/file_ext_note.html
        for (int i = 0; i < gamFileInfos.Count; i++)
        {
            if (!gamFileInfos[i].Name.EndsWithI(".gam"))
            {
                gamFileInfos.RemoveAt(i);
                i--;
            }
        }

        if (gamFileInfos.Count == 0)
        {
            return false;
        }

        gamFileInfos = gamFileInfos.OrderBy(static x => x.Length).ToList();

        // We can use bare name because .gam files will only be in the base directory
        smallestGamFile = gamFileInfos[0].Name;

        return true;
    }

    internal static bool TryGetSmallestUsedMisFile(string fmDir, out string smallestUsedMisFile)
    {
        smallestUsedMisFile = "";

        List<FileInfo> misFileInfos;
        try
        {
            misFileInfos = new DirectoryInfo(fmDir).GetFiles("*.mis", SearchOption.TopDirectoryOnly).ToList();
        }
        catch (Exception ex)
        {
            // TODO: Error handling needed
            //string msg = "Error trying to get .mis files in FM installed directory.";
            //fm.LogInfo(msg + " " + ErrorText.RetF, ex);
            //Core.Dialogs.ShowError(msg + $"{NL}{NL}" + ErrorText.OldDarkDependentFeaturesWillFail);
            return false;
        }

        // Workaround https://fenphoenix.github.io/AngelLoader/file_ext_note.html
        for (int i = 0; i < misFileInfos.Count; i++)
        {
            if (!misFileInfos[i].Name.EndsWithI(".mis"))
            {
                misFileInfos.RemoveAt(i);
                i--;
            }
        }

        if (misFileInfos.Count == 0)
        {
            return false;
        }

        var usedMisFileInfos = new List<FileInfo>(misFileInfos.Count);

        string? missFlag = null;

        string stringsPath = Path.Combine(fmDir, "strings");

        if (!Directory.Exists(stringsPath)) return false;

        string loc1 = Path.Combine(stringsPath, Paths.MissFlagStr);
        string loc2 = Path.Combine(stringsPath, "english", Paths.MissFlagStr);

        if (File.Exists(loc1))
        {
            missFlag = loc1;
        }
        else if (File.Exists(loc2))
        {
            missFlag = loc2;
        }
        else
        {
            try
            {
                string[] files = Directory.GetFiles(stringsPath, Paths.MissFlagStr, SearchOption.AllDirectories);
                if (files.Length > 0) missFlag = files[0];
            }
            catch (Exception ex)
            {
                // TODO: Error handling needed
                //string msg = "Error trying to get " + Paths.MissFlagStr + " files in " + stringsPath + " or any subdirectory.";
                //fm.LogInfo(msg + " " + ErrorText.RetF, ex);
                //Core.Dialogs.ShowError(msg + $"{NL}{NL}" + ErrorText.OldDarkDependentFeaturesWillFail);
                return false;
            }
        }

        if (missFlag != null)
        {
            // TODO: Error handling needed
            List<string> mfLines = File.ReadLines(missFlag).ToList();
            //if (!TryReadAllLines(missFlag, out List<string>? mfLines))
            //{
            //    Core.Dialogs.ShowError("Error trying to read '" + missFlag + $"'.{NL}{NL}" + ErrorText.OldDarkDependentFeaturesWillFail);
            //    return false;
            //}

            // Stupid alloc-allergic logic copied from AngelLoader; we don't need to be so optimized here but whatever
            for (int mfI = 0; mfI < misFileInfos.Count; mfI++)
            {
                FileInfo mf = misFileInfos[mfI];

                // Obtuse nonsense to avoid string allocations (perf)
                if (mf.Name.StartsWithI("miss") && mf.Name[4] != '.')
                {
                    // Since only files ending in .mis are in the misFiles list, we're guaranteed to find a .
                    // character and not get a -1 index. And since we know our file starts with "miss", the
                    // -4 is guaranteed not to take us negative either.
                    int count = mf.Name.IndexOf('.') - 4;
                    for (int mflI = 0; mflI < mfLines.Count; mflI++)
                    {
                        string line = mfLines[mflI];
                        if (line.StartsWithI("miss_") && line.Length > 5 + count && line[5 + count] == ':')
                        {
                            bool numsMatch = true;
                            for (int li = 4; li < 4 + count; li++)
                            {
                                if (line[li + 1] != mf.Name[li])
                                {
                                    numsMatch = false;
                                    break;
                                }
                            }
                            int qIndex;
                            if (numsMatch && (qIndex = line.IndexOf('\"')) > -1)
                            {
                                if (!(line.Length > qIndex + 5 &&
                                      // I don't think any files actually have "skip" in anything other than
                                      // lowercase, but I'm supporting any case anyway. You never know.
                                      (line[qIndex + 1] == 's' || line[qIndex + 1] == 'S') &&
                                      (line[qIndex + 2] == 'k' || line[qIndex + 2] == 'K') &&
                                      (line[qIndex + 3] == 'i' || line[qIndex + 3] == 'I') &&
                                      (line[qIndex + 4] == 'p' || line[qIndex + 4] == 'P') &&
                                      line[qIndex + 5] == '\"'))
                                {
                                    usedMisFileInfos.Add(mf);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (usedMisFileInfos.Count == 0) usedMisFileInfos.AddRange(misFileInfos);

        usedMisFileInfos = usedMisFileInfos.OrderBy(static x => x.Length).ToList();

        // We can use bare name because .gam files will only be in the base directory
        smallestUsedMisFile = usedMisFileInfos[0].Name;

        return true;
    }
}

internal static class FMFileExtensions
{
    internal static readonly string[] ImageFileExtensions =
    {
        ".bmp",
        ".dds",
        ".gif",
        ".jpg",
        ".jpeg",
        ".pcx",
        ".png",
        ".tga",
        ".tif",
        ".tiff",
    };
}

[SuppressMessage("ReSharper", "IdentifierTypo")]
internal static class FMDirs
{
    // PERF: const string concatenation is free (const concats are done at compile time), so do it to lessen
    // the chance of error.

    // We only need BooksS
    internal const string Fam = "fam";
    // We only need IntrfaceS
    internal const string Mesh = "mesh";
    internal const string Motions = "motions";
    internal const string Movies = "movies";
    internal const string Cutscenes = "cutscenes"; // SS2 only
    internal const string Obj = "obj";
    internal const string Scripts = "scripts";
    internal const string Snd = "snd";
    internal const string Snd2 = "snd2"; // SS2 only
                                         // We only need StringsS
    internal const string Subtitles = "subtitles";

    internal const string BooksS = "books/";
    internal const string FamS = Fam + "/";
    internal const string IntrfaceS = "intrface/";
    internal const int IntrfaceSLen = 9; // workaround for .NET 4.7.2 not inlining const string lengths
    internal const string MeshS = Mesh + "/";
    internal const string MotionsS = Motions + "/";
    internal const string MoviesS = Movies + "/";
    internal const string CutscenesS = Cutscenes + "/"; // SS2 only
    internal const string ObjS = Obj + "/";
    internal const string ScriptsS = Scripts + "/";
    internal const string SndS = Snd + "/";
    internal const string Snd2S = Snd2 + "/"; // SS2 only
    internal const string StringsS = "strings/";
    internal const string SubtitlesS = Subtitles + "/";

    internal const string T3FMExtras1S = "Fan Mission Extras/";
    internal const string T3FMExtras2S = "FanMissionExtras/";

    internal const string T3DetectS = "Content/T3/Maps/";
    internal const int T3DetectSLen = 16; // workaround for .NET 4.7.2 not inlining const string lengths
}

[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "CommentTypo")]
internal static class FMFiles
{
    internal const string SS2Fingerprint1 = "/usemsg.str";
    internal const string SS2Fingerprint2 = "/savename.str";
    internal const string SS2Fingerprint3 = "/objlooks.str";
    internal const string SS2Fingerprint4 = "/OBJSHORT.str";

    internal const string IntrfaceEnglishNewGameStr = "intrface/english/newgame.str";
    internal const string IntrfaceNewGameStr = "intrface/newgame.str";
    internal const string SNewGameStr = "/newgame.str";

    internal const string StringsMissFlag = "strings/missflag.str";
    internal const string StringsEnglishMissFlag = "strings/english/missflag.str";
    internal const string SMissFlag = "/missflag.str";

    // Telliamed's fminfo.xml file, used in a grand total of three missions
    internal const string FMInfoXml = "fminfo.xml";

    // fm.ini, a NewDark (or just FMSel?) file
    internal const string FMIni = "fm.ini";

    // System Shock 2 file
    internal const string ModIni = "mod.ini";

    internal const string FMThumb = "fmthumb";

    // For Thief 3 missions, all of them have this file, and then any other .gmp files are the actual missions
    // TODO: Is there any possible thing a loader could do with the contents of T3 files that AL doesn't?
    //  Like language detection or some such? We should add all T3 files with potentially useful contents to the
    //  logic.
    internal const string EntryGmp = "Entry.gmp";

    internal const string TDM_DarkModTxt = "darkmod.txt";
    internal const string TDM_ReadmeTxt = "readme.txt";
    internal const string TDM_MapSequence = "tdm_mapsequence.txt";
}
