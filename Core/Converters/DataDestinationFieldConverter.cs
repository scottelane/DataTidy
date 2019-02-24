using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Converts a Field to and from a string to support configuring data destination fields in a property grid.
    /// </summary>
    public class DataDestinationFieldConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether the converter can convert from the specified source type to a Field.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sourceType">The source type.</param>
        /// <returns>True if the source type is a string, false otherwise.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Converts a value from a string to a Field.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>A Field.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value != default(object))
            {
                IDataDestinationFieldsProvider provider = (IDataDestinationFieldsProvider)context.Instance;
                List<Field> fields = provider.GetDataDestinationFields();
                return fields.FirstOrDefault(field => field.DisplayName == (string)value);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Determines whether the converter can convert a Field to a string.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>True if the destination type is a string, false otherwise.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// Converts a Field to a string.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="value">The Field to convert.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>A string representation of the Field.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != default(object))
            {
                return ((Field)value).DisplayName;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Geta a value indicating whether a list of DataDestination Fields can be provided.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>True.</returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets a list of DataDestination Field objects.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The list of DataDestination Field items.</returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDataDestinationFieldsProvider provider = (IDataDestinationFieldsProvider)context.Instance;
            return new StandardValuesCollection(provider.GetDataDestinationFields());
        }
    }
}
