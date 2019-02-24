using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365ViewConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!string.IsNullOrEmpty((string)value))
            {
                IDynamics365ViewsProvider provider = (IDynamics365ViewsProvider)context.Instance;
                List<Dynamics365View> views = provider.GetDynamics365Views();
                Guid id = Guid.Parse(Regex.Match((string)value, CoreUtility.FieldMatchPattern).Groups[1].Value);                
                return views.FirstOrDefault(view => view.ID == id);
            }

            return null;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != default(object))
            {
                Dynamics365View view = (Dynamics365View)value;
                return string.Format("{0} ({1})", view.DisplayName, view.ID);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDynamics365ViewsProvider provider = (IDynamics365ViewsProvider)context.Instance;
            return new StandardValuesCollection(provider.GetDynamics365Views());        
        }
    }
}
