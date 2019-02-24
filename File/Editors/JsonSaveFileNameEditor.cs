using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.File
{
    public class JsonSaveFileNameEditor : SaveFileNameEditor
    {
        public JsonSaveFileNameEditor()
        {
            filter = "JSON files (*.json)|*.json";
        }
    }
}
