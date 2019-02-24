using System.Collections.Generic;
using System.Windows.Forms;

namespace ScottLane.DataTidy.Core
{
    public interface ICustomMenuItemProvider
    {
        List<CustomMenuItem> GetCustomMenuItems();
    }
}
