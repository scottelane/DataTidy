using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365EntityConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!string.IsNullOrEmpty((string)value))
            {
                IDynamics365EntitiesProvider provider = (IDynamics365EntitiesProvider)context.Instance;
                List<Dynamics365Entity> entities = provider.GetEntities();
                string logicalName = Regex.Match((string)value, CoreUtility.FieldMatchPattern).Groups[1].Value;
                return entities.FirstOrDefault(entity => entity.LogicalName == logicalName);
            }

            return null;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != default(Dynamics365Entity))
            {
                Dynamics365Entity entity = (Dynamics365Entity)value;
                return string.Format("{0} ({1})", entity.DisplayName, entity.LogicalName);
            }

            return null;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDynamics365EntitiesProvider provider = (IDynamics365EntitiesProvider)context.Instance;
            return new StandardValuesCollection(provider.GetEntities());
        }
    }
}
