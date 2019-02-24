using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365StateConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!string.IsNullOrEmpty((string)value))
            {
                IDynamics365StatesProvider provider = (IDynamics365StatesProvider)context.Instance;
                List<Dynamics365State> states = provider.GetStates();
                int code = int.Parse(Regex.Match((string)value, CoreUtility.FieldMatchPattern).Groups[1].Value);
                return states.FirstOrDefault(x => x.Code == code);
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
                Dynamics365State state = (Dynamics365State)value;
                return string.Format("{0} ({1})", state.Name, state.Code);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDynamics365StatesProvider provider = (IDynamics365StatesProvider)context.Instance;
            return new StandardValuesCollection(provider.GetStates());
        }
    }
}
