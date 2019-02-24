using System.ComponentModel;

namespace ScottLane.DataTidy.File
{
    public class SheetTypeConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            ISheetsProvider provider = (ISheetsProvider)context.Instance;
            return new StandardValuesCollection(provider.GetSheets());
        }
    }
}
