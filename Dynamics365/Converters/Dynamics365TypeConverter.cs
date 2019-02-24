using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Converts to and from Dynamics 365 data types.
    /// </summary>
    public class Dynamics365TypeConverter : TypeConverter
    {
        public const string MULTI_OPTION_SET_DELIMITER = ";";

        /// <summary>
        /// Gets AttributeMetadata for the field to be converted.
        /// </summary>
        public AttributeMetadata AttributeMetadata { get; }

        /// <summary>
        /// Gets the target entity logical name for entity references.
        /// </summary>
        public string TargetEntityLogicalName { get; }

        /// <summary>
        /// Gets the neutral type that attribute values are converted to.
        /// </summary>
        public Type NeutralType
        {
            get
            {
                Type type = default(Type);

                if (AttributeMetadata.AttributeType == AttributeTypeCode.BigInt)
                {
                    type = typeof(Int64);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Boolean)
                {
                    type = typeof(bool);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Customer || AttributeMetadata.AttributeType == AttributeTypeCode.Lookup || AttributeMetadata.AttributeType == AttributeTypeCode.Owner)
                {
                    type = typeof(Guid);    // this should never be called - remove?
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.DateTime)
                {
                    type = typeof(DateTime);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Decimal)
                {
                    type = typeof(decimal);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Double)
                {
                    type = typeof(double);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Integer)
                {
                    type = typeof(int);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Money)
                {
                    type = typeof(decimal);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Picklist || AttributeMetadata.AttributeType == AttributeTypeCode.State || AttributeMetadata.AttributeType == AttributeTypeCode.Status)
                {
                    type = typeof(int);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Memo || AttributeMetadata.AttributeType == AttributeTypeCode.String || AttributeMetadata.AttributeType == AttributeTypeCode.EntityName || (AttributeMetadata.AttributeType == AttributeTypeCode.Virtual && AttributeMetadata is MultiSelectPicklistAttributeMetadata))
                {
                    type = typeof(string);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Uniqueidentifier)
                {
                    type = typeof(Guid);
                }

                return type;
            }
        }

        /// <summary>
        /// Gets the Dynamics 365 data type for the attribute.
        /// </summary>
        public Type Dynamics365Type
        {
            get
            {
                Type type = default(Type);

                if (AttributeMetadata.AttributeType == AttributeTypeCode.BigInt)
                {
                    type = typeof(Int64);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Boolean)
                {
                    type = typeof(bool);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Customer || AttributeMetadata.AttributeType == AttributeTypeCode.Lookup || AttributeMetadata.AttributeType == AttributeTypeCode.Owner)
                {
                    type = typeof(EntityReference);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.DateTime)
                {
                    type = typeof(DateTime);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Decimal)
                {
                    type = typeof(decimal);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Double)
                {
                    type = typeof(double);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Integer)
                {
                    type = typeof(int);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Money)
                {
                    type = typeof(Money);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Picklist || AttributeMetadata.AttributeType == AttributeTypeCode.State || AttributeMetadata.AttributeType == AttributeTypeCode.Status)
                {
                    type = typeof(OptionSetValue);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Virtual && AttributeMetadata is MultiSelectPicklistAttributeMetadata)
                {
                    type = typeof(OptionSetValueCollection);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Memo || AttributeMetadata.AttributeType == AttributeTypeCode.String || AttributeMetadata.AttributeType == AttributeTypeCode.EntityName)
                {
                    type = typeof(string);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.Uniqueidentifier)
                {
                    type = typeof(Guid);
                }
                else if (AttributeMetadata.AttributeType == AttributeTypeCode.PartyList)
                {
                    type = typeof(EntityCollection);
                }

                return type;
            }
        }

        /// <summary>
        /// Initialises a new instance of the Dynamics365TypeConverter class with the specified field AttributeMetadata.
        /// </summary>
        /// <param name="attributeMetadata">The field AttributeMetadata.</param>
        public Dynamics365TypeConverter(AttributeMetadata attributeMetadata)
        {
            AttributeMetadata = attributeMetadata;
        }

        /// <summary>
        /// Initialises a new instance of the Dynamics365TypeConverter class with the specified field AttributeMetadata.
        /// </summary>
        /// <param name="attributeMetadata">The field AttributeMetadata.</param>
        /// <param name="targetEntityLogicalName">The target entity logical name for entity references.</param>
        public Dynamics365TypeConverter(AttributeMetadata attributeMetadata, string targetEntityLogicalName)
        {
            AttributeMetadata = attributeMetadata;
            TargetEntityLogicalName = targetEntityLogicalName;
        }

        /// <summary>
        /// Determines whether the converter can convert from the specified type.
        /// </summary>
        /// <param name="context">The context (not used).</param>
        /// <param name="sourceType">The source type.</param>
        /// <returns>True if the converter can convert from the specified type, otherwise false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(Int64)
                || sourceType == typeof(bool)
                || sourceType == typeof(EntityReference)
                || sourceType == typeof(DateTime)
                || sourceType == typeof(decimal)
                || sourceType == typeof(double)
                || sourceType == typeof(int)
                || sourceType == typeof(Money)
                || sourceType == typeof(OptionSetValue)
                || sourceType == typeof(OptionSetValueCollection)
                || sourceType == typeof(string)
                || sourceType == typeof(Guid)
                || sourceType == typeof(EntityCollection));
        }

        /// <summary>
        /// Convert from a CRM data type to a neutral data type.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)   // todo - eliminate this method? money needs to be in here though
        {
            object result = value;

            if (result != null)
            {
                if (value is EntityReference entityReferenceValue)
                {
                    result = entityReferenceValue.Id;
                }
                else if (value is OptionSetValue optionSetValueValue)
                {
                    result = optionSetValueValue.Value;
                }
                else if (value is OptionSetValueCollection optionSetValueCollectionValue)
                {
                    string.Join(MULTI_OPTION_SET_DELIMITER, optionSetValueCollectionValue.Select(optionSet => optionSet.Value.ToString()));
                }
                else if (value is Money moneyValue)
                {
                    result = moneyValue.Value;
                }
                else if (value is EntityCollection entityCollectionValue)
                {
                    result = entityCollectionValue[0].Id;
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether the converter can convert to the specified type.
        /// </summary>
        /// <param name="context">The context (not used).</param>
        /// <param name="destinationType">The destination type.</param>
        /// <returns>True if the converter can convert to the specified type, otherwise false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(Int64)
                || destinationType == typeof(bool)
                || destinationType == typeof(EntityReference)
                || destinationType == typeof(DateTime)
                || destinationType == typeof(decimal)
                || destinationType == typeof(double)
                || destinationType == typeof(int)
                || destinationType == typeof(Money)
                || destinationType == typeof(OptionSetValue)
                || destinationType == typeof(string)
                || destinationType == typeof(Guid)
                || destinationType == typeof(OptionSetValueCollection)
                || destinationType == typeof(EntityCollection));
        }

        /// <summary>
        /// Convert from a neutral data type to a CRM data type.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            // todo - switch items below to use 'value' then set result
            object result = value;

            if (result != null && value.GetType() != destinationType)
            {
                if ((bool)AttributeMetadata.IsPrimaryId)
                {
                    result = new Guid(result.ToString());
                }
                else if (destinationType == typeof(OptionSetValue))
                {
                    result = GetOptionSetValue(value);
                }
                else if (destinationType == typeof(OptionSetValueCollection))
                {
                    OptionSetValueCollection resultValues = new OptionSetValueCollection();

                    if (result is string)
                    {
                        string[] optionSetValues = result.ToString().Split(new string[] { MULTI_OPTION_SET_DELIMITER }, StringSplitOptions.None);

                        foreach (string optionSetValue in optionSetValues)
                        {
                            resultValues.Add(GetOptionSetValue(optionSetValue));
                        }
                    }
                    else
                    {
                        resultValues.Add(GetOptionSetValue(value));
                    }

                    return resultValues;
                }
                else if (destinationType == typeof(EntityReference))
                {
                    if (AttributeMetadata is LookupAttributeMetadata)
                    {
                        if (TargetEntityLogicalName != default(string))
                        {
                            result = new EntityReference(TargetEntityLogicalName, Guid.Parse(value.ToString()));
                        }
                        else
                        {
                            result = new EntityReference(((LookupAttributeMetadata)AttributeMetadata).Targets[0], Guid.Parse(value.ToString()));
                        }
                    }
                }
                else if (destinationType == typeof(EntityCollection))
                {
                    string[] targetEntities = TargetEntityLogicalName.Split(new string[] { Dynamics365DataSource.PARTYLIST_DELIMITER }, StringSplitOptions.RemoveEmptyEntries);
                    string[] values = value.ToString().Split(new string[] { Dynamics365DataSource.PARTYLIST_DELIMITER }, StringSplitOptions.RemoveEmptyEntries);    // todo - where should this delimiter live?

                    // if only one target entity is supplied, use that for each value
                    if (values.Length != targetEntities.Length && targetEntities.Length > 1)
                    {
                        throw new ApplicationException(string.Format("The number of Party List values ({0}) for {1} does not match the number of target entities ({2})", value, AttributeMetadata.DisplayName, TargetEntityLogicalName));
                    }

                    EntityCollection entities = new EntityCollection();

                    for (int valueIndex = 0; valueIndex < values.Length; valueIndex++)
                    {
                        Entity activityParty = new Entity("activityparty");
                        activityParty["partyid"] = new EntityReference(targetEntities.Length == 1 ? targetEntities[0] : targetEntities[valueIndex], Guid.Parse(values[valueIndex]));
                        entities.Entities.Add(activityParty);
                    }

                    result = entities;
                }
                else if (destinationType == typeof(bool))
                {
                    bool booleanValue;

                    if (bool.TryParse(result.ToString(), out booleanValue))
                    {
                        result = booleanValue;
                    }
                    else
                    {
                        BooleanAttributeMetadata booleanField = (BooleanAttributeMetadata)AttributeMetadata;
                        int integerValue;

                        if (int.TryParse(result.ToString(), out integerValue))
                        {
                            if (integerValue == booleanField.OptionSet.TrueOption.Value)
                            {
                                result = true;
                            }
                            else if (integerValue == booleanField.OptionSet.FalseOption.Value)
                            {
                                result = false;
                            }
                        }
                        else if (result.ToString().ToLower() == booleanField.OptionSet.TrueOption.Label.UserLocalizedLabel.Label.ToLower())
                        {
                            result = true;
                        }
                        else if (result.ToString().ToLower() == booleanField.OptionSet.FalseOption.Label.UserLocalizedLabel.Label.ToLower())
                        {
                            result = false;
                        }
                    }
                }
                else if (destinationType == typeof(DateTime))
                {
                    result = System.Convert.ToDateTime(result);    // todo - date only, etc
                }
                else if (destinationType == typeof(Money))
                {
                    result = new Money(System.Convert.ToDecimal(result));
                }
                else if (destinationType == typeof(decimal))
                {
                    result = System.Convert.ToDecimal(result);
                }
                else if (destinationType == typeof(int))
                {
                    result = System.Convert.ToInt32(result);
                }
                else if (destinationType == typeof(Int64))
                {
                    result = System.Convert.ToInt64(result);
                }
                else if (destinationType == typeof(string))
                {
                    result = System.Convert.ToString(result);
                }
                else if (destinationType == typeof(double))
                {
                    result = System.Convert.ToDouble(result);
                }
                else if (destinationType == typeof(Guid))
                {
                    result = Guid.Parse((string)value);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets an option set value from a single or multi-selection option set.
        /// </summary>
        /// <param name="value">The value to convert to an option set value.</param>
        private OptionSetValue GetOptionSetValue(object value)
        {
            int optionSetValue = 0;

            if (value is string)
            {
                bool success = int.TryParse(value.ToString(), out optionSetValue);

                if (!success)
                {
                    OptionMetadata optionMetadata = ((EnumAttributeMetadata)AttributeMetadata).OptionSet.Options.FirstOrDefault(option => option.Label.UserLocalizedLabel.Label.ToLower() == value.ToString().ToLower());

                    if (optionMetadata != default(OptionMetadata))
                    {
                        optionSetValue = (int)optionMetadata.Value;
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("'{1}' is not a valid option set value", AttributeMetadata.DisplayName.UserLocalizedLabel.Label, value.ToString()));
                    }
                }
            }
            else
            {
                optionSetValue = System.Convert.ToInt32(value);
            }

            return new OptionSetValue(optionSetValue);
        }

        public static object Convert(object value, Dynamics365Entity entity, string attributeLogicalName, Dynamics365Connection connection)
        {
            return Convert(value, entity, attributeLogicalName, connection, default(string));
        }

        public static object Convert(object value, Dynamics365Entity entity, string attributeLogicalName, Dynamics365Connection connection, string targetEntityLogicalName)
        {
            object convertedValue = default(object);
            EntityMetadata entityMetadata = entity.GetEntityMetadata(connection);
            AttributeMetadata attributeMetadata = entityMetadata.Attributes.FirstOrDefault(findField => findField.LogicalName == attributeLogicalName);
            Dynamics365TypeConverter converter = new Dynamics365TypeConverter(attributeMetadata, targetEntityLogicalName);
            Type destinationType = converter.Dynamics365Type;

            if (converter.CanConvertTo(destinationType))
            {
                try
                {
                    convertedValue = converter.ConvertTo(value, destinationType);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(string.Format("Cannot convert {0} '{1}' to {2}: {3}", attributeMetadata.DisplayName.UserLocalizedLabel.Label, value, destinationType.Name, ex.Message), ex);
                }
            }
            else
            {
                throw new ArgumentException(string.Format("Cannot convert {0} '{1}' to {2}.", attributeMetadata.DisplayName.UserLocalizedLabel.Label, value, destinationType.Name));
            }

            return convertedValue;
        }

    }
}
