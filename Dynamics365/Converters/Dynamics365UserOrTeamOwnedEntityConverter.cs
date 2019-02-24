using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365UserOrTeamOwnedEntityConverter : Dynamics365EntityConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value != default(object))
            {
                IDynamics365UserOrTeamOwnedEntitiesProvider provider = (IDynamics365UserOrTeamOwnedEntitiesProvider)context.Instance;
                List<Dynamics365Entity> entities = provider.GetUserOrTeamOwnedEntities();
                string logicalName = Regex.Match((string)value, CoreUtility.FieldMatchPattern).Groups[1].Value;
                return entities.FirstOrDefault(entity => entity.LogicalName == logicalName);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDynamics365UserOrTeamOwnedEntitiesProvider provider = (IDynamics365UserOrTeamOwnedEntitiesProvider)context.Instance;
            return new StandardValuesCollection(provider.GetUserOrTeamOwnedEntities());
        }
    }
}
