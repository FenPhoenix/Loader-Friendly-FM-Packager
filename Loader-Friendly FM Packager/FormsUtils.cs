using System.Windows.Forms;

namespace Loader_Friendly_FM_Packager;

internal static class FormsUtils
{
    internal static bool SelectedIndexIsInRange(this ComboBox comboBox) =>
        comboBox.SelectedIndex > -1 && comboBox.SelectedIndex < comboBox.Items.Count;
}
