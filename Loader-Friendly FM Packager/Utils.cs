using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
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

    internal static bool IsValidReadme(this string readme) =>
        readme.ExtIsTxt() ||
        readme.ExtIsRtf() ||
        readme.ExtIsWri() ||
        readme.ExtIsGlml() ||
        readme.ExtIsHtml() ||
        readme.ExtIsDoc() ||
        readme.ExtIsPdf();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsGlTitle(this string readme)
    {
        return Path.HasExtension(readme) && Path.GetFileNameWithoutExtension(readme).EqualsI("gltitle");
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

    internal static bool TryGetReadmeFromModIni(string filesDir, out string readme)
    {
        readme = "";

        string modIniFile = Path.Combine(filesDir, FMFiles.ModIni);

        if (!File.Exists(modIniFile))
        {
            return false;
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
        }

        return !readme.IsEmpty();
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

    internal const string FMThumbJpg = "fmthumb.jpg";

    // For Thief 3 missions, all of them have this file, and then any other .gmp files are the actual missions
    // TODO: Is there any possible thing a loader could do with the contents of T3 files that AL doesn't?
    //  Like language detection or some such? We should add all T3 files with potentially useful contents to the
    //  logic.
    internal const string EntryGmp = "Entry.gmp";

    internal const string TDM_DarkModTxt = "darkmod.txt";
    internal const string TDM_ReadmeTxt = "readme.txt";
    internal const string TDM_MapSequence = "tdm_mapsequence.txt";
}
