using System.Windows.Forms;

namespace ScottLane.DataTidy.Client.Model
{
    /// <summary>
    /// Sorts a tool strip item by text.
    /// </summary>
    public class ToolStripItemComparer : System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            ToolStripItem oItem1 = (ToolStripItem)x;
            ToolStripItem oItem2 = (ToolStripItem)y;
            return string.Compare(oItem1.Text, oItem2.Text, true);
        }
    }
}
