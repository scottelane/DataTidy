using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365FieldConverter : DataDestinationFieldConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!string.IsNullOrEmpty((string)value))
            {
                IDataDestinationFieldsProvider provider = (IDataDestinationFieldsProvider)context.Instance;
                List<Field> fields = provider.GetDataDestinationFields();
                string logicalName = Regex.Match((string)value, CoreUtility.FieldMatchPattern).Groups[1].Value;
                return fields.FirstOrDefault(field => ((Dynamics365Field)field).LogicalName == logicalName);
            }

            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != default(object))
            {
                Dynamics365Field field = (Dynamics365Field)value;

                if (context?.Instance != default(object))
                {
                    IDataDestinationFieldsProvider provider = (IDataDestinationFieldsProvider)context.Instance;
                    List<Field> fields = provider.GetDataDestinationFields();
                    Dynamics365Field renamedField = (Dynamics365Field)fields.FirstOrDefault(f => ((Dynamics365Field)f).LogicalName == field.LogicalName);

                    if (renamedField.DisplayName != field.DisplayName)
                    {
                        field.DisplayName = renamedField.DisplayName;
                    }
                }

                return string.Format("{0} ({1})", field.DisplayName, field.LogicalName);
            }

            return null;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
