global using static Loader_Friendly_FM_Packager.Global;
using System;
using System.Windows.Forms;

namespace Loader_Friendly_FM_Packager;

file static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        Logger.SetLogFile(Paths.LogFile);
        Logger.Log(Application.ProductVersion + " Started session" + $"{NL}");

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new AppContext());
    }

    private sealed class AppContext : ApplicationContext
    {
        internal AppContext() => Core.Init();
    }
}
