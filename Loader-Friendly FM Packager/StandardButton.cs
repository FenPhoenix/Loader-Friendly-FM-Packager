using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Loader_Friendly_FM_Packager;

public sealed class StandardButton : Button
{
    public StandardButton()
    {
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        MinimumSize = new Size(75, 23);
        Padding = new Padding(6, 0, 6, 0);
    }
}
