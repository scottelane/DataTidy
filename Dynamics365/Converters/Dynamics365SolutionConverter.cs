using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365SolutionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!string.IsNullOrEmpty((string)value))
            {
                IDynamics365SolutionsProvider provider = (IDynamics365SolutionsProvider)context.Instance;
                List<Dynamics365Solution> solutions = provider.GetSolutions();
                string schemaName = Regex.Match((string)value, CoreUtility.FieldMatchPattern).Groups[1].Value;
                return solutions.FirstOrDefault(solution => solution.UniqueName == schemaName);
            }

            return null;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != default(Dynamics365Solution))
            {
                Dynamics365Solution solution = (Dynamics365Solution)value;
                return string.Format("{0} ({1})", solution.FriendlyName, solution.UniqueName);
            }

            return null;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDynamics365SolutionsProvider provider = (IDynamics365SolutionsProvider)context.Instance;
            return new StandardValuesCollection(provider.GetSolutions());
        }
    }
}
