using System.Collections.Generic;

namespace ScottLane.DataTidy.File
{
    public interface ISheetsProvider
    {
        List<string> GetSheets();
    }
}
