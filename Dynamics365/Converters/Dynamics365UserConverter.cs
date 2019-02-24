using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365UserConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!string.IsNullOrEmpty((string)value))
            {
                IDynamics365UsersProvider provider = (IDynamics365UsersProvider)context.Instance;
                List<Dynamics365User> users = provider.GetUsers();
                string id = Regex.Match((string)value, CoreUtility.FieldMatchPattern).Groups[1].Value;
                return users.FirstOrDefault(x => x.ID.ToString() == id);
            }

            return null;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != default(Dynamics365User))
            {
                Dynamics365User user = (Dynamics365User)value;
                return string.Format("{0} ({1})", user.Name, user.ID.ToString());
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDynamics365UsersProvider provider = (IDynamics365UsersProvider)context.Instance;
            return new StandardValuesCollection(provider.GetUsers());
        }
    }
}
