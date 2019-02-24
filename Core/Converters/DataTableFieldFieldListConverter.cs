using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScottLane.DataTidy.Core
{
    public class DataTableFieldFieldListConverter : TypeConverter
    {
        private const string FIELD_DELIMITER = ", ";

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            BindingList<DataTableField> fields = new BindingList<DataTableField>();

            if (!string.IsNullOrEmpty((string)value))
            {
                IDataSourceFieldsProvider fieldsProvider = (IDataSourceFieldsProvider)context.Instance;
                List<DataTableField> sourceFields = fieldsProvider.GetDataSourceFields();
                string[] fieldStrings = ((string)value).Split(new string[] { FIELD_DELIMITER }, StringSplitOptions.None);

                foreach (string fieldString in fieldStrings)
                {
                    string columnName = Regex.Match(fieldString, CoreUtility.FieldMatchPattern).Groups[1].Value;
                    DataTableField field = sourceFields.FirstOrDefault(f => f.ColumnName == columnName);    // todo - comparer
                    fields.Add(field);
                }
            }

            return fields;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != default(object))
            {
                BindingList<DataTableField> fields = (BindingList<DataTableField>)value;
                return string.Join(FIELD_DELIMITER, fields.Select(field => string.Format("{0} ({1})", field.DisplayName, field.ColumnName)));
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDataSourceFieldsProvider fieldsProvider = (IDataSourceFieldsProvider)context.Instance;
            return new StandardValuesCollection(fieldsProvider.GetDataSourceFields());
        }
    }
}