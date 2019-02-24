using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Converts a Connection to and from a string to support configuring connections in a property grid.
    /// </summary>
    public class ConnectionConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether the converter can convert from the specified source type to a Connection.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sourceType">The source type.</param>
        /// <returns>True if the source type is a string, false otherwise.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Converts a value from a string to a Connection.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>A Connection.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!string.IsNullOrEmpty((string)value))
            {
                IConnectionsProvider provider = (IConnectionsProvider)context.Instance;
                List<IConnection> connections = provider.GetConnections();
                Guid id = Guid.Parse(Regex.Match((string)value, CoreUtility.FieldMatchPattern).Groups[1].Value);
                return connections.FirstOrDefault(connection => connection.ID == id);
            }

            return null;
        }

        /// <summary>
        /// Determines whether the converter can convert a Connection to a string.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>True if the destination type is a string, false otherwise.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// Converts a Connection to a string.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="value">The Connection to convert.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>A string representation of the Connection.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null)
            {
                IConnection connection = (IConnection)value;
                return string.Format("{0} ({1})", connection.Name, connection.ID);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Geta a value indicating whether a list of Connections can be provided.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>True.</returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets a list of Connection objects.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A list of Connection items.</returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IConnectionsProvider provider = (IConnectionsProvider)context.Instance;
            return new StandardValuesCollection(provider.GetConnections());
        }        
    }
}
