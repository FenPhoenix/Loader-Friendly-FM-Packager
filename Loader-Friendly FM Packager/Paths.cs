using System.IO;
using System.Windows.Forms;

namespace Loader_Friendly_FM_Packager;

internal static class Paths
{
    internal static readonly string Startup = Application.StartupPath;
    internal static readonly string ConfigFile = Path.Combine(Startup, "Config.ini");
    // TODO: Make a better / more unique temp folder name
    internal static readonly string Temp = Path.Combine(Path.GetTempPath(), "LFFMP");
    internal static readonly string SevenZipExe = Path.Combine(Startup, "7z", "7z.exe");
}
