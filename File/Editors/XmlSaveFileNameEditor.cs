using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.File
{
    public class XmlSaveFileNameEditor : SaveFileNameEditor
    {
        public XmlSaveFileNameEditor()
        {
            filter = "XML files (*.xml)|*.xml";
        }
    }
}
