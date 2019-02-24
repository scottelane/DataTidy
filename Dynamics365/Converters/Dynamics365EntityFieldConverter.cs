using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365EntityFieldConverter : TypeConverter
    {
        private const string FIELD_DELIMITER = ", ";

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // convert from delimited string to field list
            if (!string.IsNullOrEmpty((string)value))
            {
                string valueString = (string)value;
                string[] fieldStrings = valueString.Split(new string[] { FIELD_DELIMITER }, StringSplitOptions.None);

                IDynamics365EntityFieldsProvider provider = (IDynamics365EntityFieldsProvider)context.Instance;
                List<Dynamics365Field> entityFields = provider.GetDynamics365EntityFields();
                BindingList<Dynamics365Field> fields = new BindingList<Dynamics365Field>();

                foreach (string fieldString in fieldStrings)
                {
                    string id = Regex.Match(valueString, CoreUtility.FieldMatchPattern).Groups[1].Value;
                    Dynamics365Field field = entityFields.FirstOrDefault(f => f.LogicalName == id);
                    fields.Add(field);
                }

                return fields;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            // convert from field list to delimited string
            if (value != default(object))
            {
                BindingList<Dynamics365Field> fields = (BindingList<Dynamics365Field>)value;
                return string.Join(FIELD_DELIMITER, fields.Select(field => string.Format("{0} ({1})", field.DisplayName, field.LogicalName)));
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDynamics365EntityFieldsProvider provider = (IDynamics365EntityFieldsProvider)context.Instance;
            return new StandardValuesCollection(provider.GetDynamics365EntityFields());
        }
    }
}
