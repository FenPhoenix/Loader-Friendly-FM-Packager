using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Loader_Friendly_FM_Packager;

internal static class FormsUtils
{
    internal static void RemoveAndSelectNearest(this ListBox listBox)
    {
        if (listBox.SelectedIndex == -1) return;

        int oldSelectedIndex = listBox.SelectedIndex;

        listBox.Items.RemoveAt(listBox.SelectedIndex);

        if (oldSelectedIndex < listBox.Items.Count && listBox.Items.Count > 1)
        {
            listBox.SelectedIndex = oldSelectedIndex;
        }
        else if (listBox.Items.Count > 1)
        {
            listBox.SelectedIndex = oldSelectedIndex - 1;
        }
        else if (listBox.Items.Count == 1)
        {
            listBox.SelectedIndex = 0;
        }
    }

    public static string[] ItemsAsStrings(this ListBox listBox)
    {
        string[] ret = new string[listBox.Items.Count];
        for (int i = 0; i < listBox.Items.Count; i++)
        {
            ret[i] = listBox.Items[i].ToStringOrEmpty();
        }
        return ret;
    }

    internal static bool SelectedIndexIsInRange(this ComboBox comboBox) =>
        comboBox.SelectedIndex > -1 && comboBox.SelectedIndex < comboBox.Items.Count;

    internal static bool IndexIsInRange(this ComboBox comboBox, int index) =>
        index > -1 && index < comboBox.Items.Count;

    #region Centering

    internal static void CenterH(this Control control, Control parent, bool clientSize = false)
    {
        int pWidth = clientSize ? parent.ClientSize.Width : parent.Width;
        control.Location = control.Location with { X = (pWidth / 2) - (control.Width / 2) };
    }

#if false
    internal static void CenterV(this Control control, Control parent)
    {
        control.Location = control.Location with { Y = (parent.Height / 2) - (control.Height / 2) };
    }
#endif

    internal static void CenterHV(this Control control, Control parent, bool clientSize = false)
    {
        int pWidth = clientSize ? parent.ClientSize.Width : parent.Width;
        int pHeight = clientSize ? parent.ClientSize.Height : parent.Height;
        control.Location = new Point((pWidth / 2) - (control.Width / 2), (pHeight / 2) - (control.Height / 2));
    }

    #endregion

    #region DisableEvents

    public interface IEventDisabler
    {
        /// <summary>
        /// True if greater than 0.
        /// </summary>
        int EventsDisabled { get; set; }
    }

    [StructLayout(LayoutKind.Auto)]
    internal readonly ref struct DisableEvents
    {
        public static void Open(IEventDisabler obj, bool active = true)
        {
            if (active) obj.EventsDisabled++;
        }

        public static void Close(IEventDisabler obj, bool active = true)
        {
            if (active) obj.EventsDisabled = (obj.EventsDisabled - 1).ClampToZero();
        }

        private readonly bool _active;
        private readonly IEventDisabler _obj;
        public DisableEvents(IEventDisabler obj, bool active = true)
        {
            _active = active;
            _obj = obj;

            if (_active) _obj.EventsDisabled++;
        }

        public void Dispose()
        {
            if (_active) _obj.EventsDisabled = (_obj.EventsDisabled - 1).ClampToZero();
        }
    }

    #endregion
}
