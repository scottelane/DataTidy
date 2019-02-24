using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.File
{
    public class CsvSaveFileNameEditor : SaveFileNameEditor
    {
        public CsvSaveFileNameEditor()
        {
            filter = "CSV files (*.csv)|*.csv";
        }
    }
}
