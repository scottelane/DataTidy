using System;
using System.ComponentModel;
using System.Globalization;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Converts an encrypted string to and from an unencrypted string to support modifying encrypted strings in a property grid.
    /// </summary>
    public class EncryptedStringConverter : TypeConverter
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
        /// Converts a value from a string to an encrypted string.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="value">The value to convert.</param>
        /// <returns>An encrypted string.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!string.IsNullOrEmpty((string)value))
            {
                return new AESEncrypter().Encrypt((string)value);
            }

            return null;
        }

        /// <summary>
        /// Determines whether the converter can decrypt and encrypted string.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>True if the destination type is a string, false otherwise.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        /// Converts an encrypted string to an unencrypted string.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="value">The DataSource to convert.</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>A string representation of the DataSource.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != default(object))
            {
                return new AESEncrypter().Decrypt((string)value);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
