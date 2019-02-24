using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365RelationshipConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!string.IsNullOrEmpty((string)value))
            {
                string relationshipSchemaName = (string)value;

                Dynamics365RelationshipOperation operation = (Dynamics365RelationshipOperation)context.Instance;
                List<Dynamics365Relationship> relationships = Dynamics365Relationship.GetRelationships(operation.Entity, operation.Connection);
                Dynamics365Relationship relationship = Dynamics365Relationship.GetRelationships(operation.Entity, operation.Connection).FirstOrDefault(r => r.SchemaName == relationshipSchemaName);
                return relationship;
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
                Dynamics365Relationship relationship = (Dynamics365Relationship)value;
                return relationship.SchemaName;
            }

            return null;
        }
        
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDynamics365RelationshipsProvider provider = (IDynamics365RelationshipsProvider)context.Instance;
            return new StandardValuesCollection(provider.GetRelationships());
        }
    }
}
