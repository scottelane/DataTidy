using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Converts a DataSource to and from a string to support configuring data sources in a property grid.
    /// </summary>
    public class DataSourceConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether the converter can convert from the specified source type to a DataSource.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sourceType">The source type.</param>
        /// <returns>True if the source type is a string, false otherwise.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Converts a value from a string to a DataSource.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="value">The value.</param>
        /// <returns>A DataSource.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!string.IsNullOrEmpty((string)value))
            {
                IDataSourcesProvider provider = (IDataSourcesProvider)context.Instance;
                BindingList<IDataSource> dataSources = provider.GetDataSources();
                Guid id = Guid.Parse(Regex.Match((string)value, CoreUtility.FieldMatchPattern).Groups[1].Value);
                return dataSources.FirstOrDefault(dataSource => dataSource.ID == id);
            }

            return null;
        }

        /// <summary>
        /// Determines whether the converter can convert a DataSource to a string.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>True if the destination type is a string, false otherwise.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// Converts a DataSource to a string.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="value">The DataSource to convert.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>A string representation of the DataSource.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null)
            {
                IDataSource dataSource = (IDataSource)value;
                return string.Format("{0} ({1})", dataSource.Name, dataSource.ID);
            }

            return base.ConvertTo(context, culture, value, destinationType);

        }

        /// <summary>
        /// Geta a value indicating whether a list of DataSources can be provided.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>True.</returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets a list of DataSource objects.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The list of DataDestination items.</returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDataSourcesProvider provider = (IDataSourcesProvider)context.Instance;
            return new StandardValuesCollection(provider.GetDataSources());
        }
    }
}
