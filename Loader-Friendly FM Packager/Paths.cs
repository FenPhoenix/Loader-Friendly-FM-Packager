using System;
using System.IO;
using System.Windows.Forms;
using static Loader_Friendly_FM_Packager.Logger;

namespace Loader_Friendly_FM_Packager;

internal static class Paths
{
    internal sealed class TempPath(Func<string> pathGetter)
    {
        internal readonly Func<string> PathGetter = pathGetter;
    }

    internal static class TempPaths
    {
        internal static readonly TempPath Base = new(static () => Temp);
        internal static readonly TempPath SourceCopy = new(static () => Temp_SourceCopy);
    }

    internal static readonly string Startup = Application.StartupPath;
    internal static readonly string ConfigFile = Path.Combine(Startup, "Config.ini");
    // TODO: Make a better / more unique temp folder name
    internal static readonly string Temp = Path.Combine(Path.GetTempPath(), "LFFMP");
    internal static readonly string Temp_SourceCopy = Path.Combine(Temp, "SrcCopy");
    internal static readonly string SevenZipExe = Path.Combine(Startup, "7z", "7z.exe");
    internal static readonly string SevenZipDll = Path.Combine(Startup, "7z", "7z.dll");

    internal static readonly string LogFile = Path.Combine(Startup, "log.txt");

    internal const string MissFlagStr = "missflag.str";

    // TODO: Act on failure
    internal static bool CreateOrClearTempPath(TempPath pathType)
    {
        string path = pathType.PathGetter.Invoke();

        if (Directory.Exists(path))
        {
            try
            {
                Utils.DirAndFileTree_UnSetReadOnly(path, throwException: true);
            }
            catch (Exception ex)
            {
                Log("Exception setting temp path subtree to all non-readonly." + $"{NL}" +
                    "path was: " + path, ex);
            }

            try
            {
                foreach (string f in Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly))
                {
                    File.Delete(f);
                }
                foreach (string d in Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly))
                {
                    Directory.Delete(d, recursive: true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Log("Exception clearing temp path " + path, ex);
            }
        }
        else
        {
            try
            {
                Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception ex)
            {
                Log("Exception creating temp path " + path, ex);
            }
        }

        return false;
    }
}
