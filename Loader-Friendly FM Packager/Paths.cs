using System.IO;
using System.Windows.Forms;

namespace Loader_Friendly_FM_Packager;

internal static class Paths
{
    internal static readonly string Startup = Application.StartupPath;
    internal static readonly string ConfigFile = Path.Combine(Startup, "Config.ini");
    internal static readonly string Temp = Path.Combine(Path.GetTempPath(), "LFFMP");
}
