using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365TargetEntityConverter : Dynamics365EntityConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!string.IsNullOrEmpty((string)value))
            {
                IDynamics365TargetEntitiesProvider provider = (IDynamics365TargetEntitiesProvider)context.Instance;
                List<Dynamics365Entity> entities = provider.GetTargetEntities();
                string logicalName = Regex.Match((string)value, CoreUtility.FieldMatchPattern).Groups[1].Value;
                return entities.FirstOrDefault(entity => entity.LogicalName == logicalName);
            }

            return null;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDynamics365TargetEntitiesProvider provider = (IDynamics365TargetEntitiesProvider)context.Instance;
            return new StandardValuesCollection(provider.GetTargetEntities());
        }
    }
}
