using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Converts a data source DataColumn to and from a string to support configuring data columns in a property grid.
    /// </summary>
    public class DataSourceFieldConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether the converter can convert from the specified source type to a DataColumn.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sourceType">The source type.</param>
        /// <returns>True if the source type is a string, false otherwise.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Converts a value from a string to a DataColumn.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>A DataColumn.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!string.IsNullOrEmpty((string)value))
            {
                IDataSourceFieldsProvider provider = (IDataSourceFieldsProvider)context.Instance;
                List<DataTableField> fields = provider.GetDataSourceFields();
                string columnName = Regex.Match((string)value, CoreUtility.FieldMatchPattern).Groups[1].Value;
                return fields.First(field => field.ColumnName == columnName);
            }

            return null;
        }

        /// <summary>
        /// Determines whether the converter can convert a DataColumn to a string.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>True if the destination type is a string, false otherwise.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }
        
        /// <summary>
        /// Converts a DataColumn to a string.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="value">The DataColumn to convert.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>A string representation of the DataColumn.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != default(object))
            {
                DataTableField field = (DataTableField)value;
                return string.Format("{0} ({1})", field.DisplayName, field.ColumnName);
            }

            return null;
        }

        /// <summary>
        /// Geta a value indicating whether a list of Operation DataColumns can be provided.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>True.</returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets a list of Operation DataColumn objects.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The list of Operation DataColumn objects.</returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDataSourceFieldsProvider provider = (IDataSourceFieldsProvider)context.Instance;
            return new StandardValuesCollection(provider.GetDataSourceFields());
        }
    }
}
