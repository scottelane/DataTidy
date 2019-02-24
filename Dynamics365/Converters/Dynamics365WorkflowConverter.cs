using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365WorkflowConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (!string.IsNullOrEmpty((string)value))
            {
                IDynamics365WorkflowProvider provider = (IDynamics365WorkflowProvider)context.Instance;
                List<Dynamics365Workflow> workflows = provider.GetWorkflows();
                string id = Regex.Match((string)value, CoreUtility.FieldMatchPattern).Groups[1].Value;
                return workflows.FirstOrDefault(workflow => workflow.ID.ToString() == id);
            }

            return null;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != default(Dynamics365Workflow))
            {
                Dynamics365Workflow workflow = (Dynamics365Workflow)value;
                return string.Format("{0} ({1})", workflow.Name, workflow.ID);
            }

            return null;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDynamics365WorkflowProvider provider = (IDynamics365WorkflowProvider)context.Instance;
            return new StandardValuesCollection(provider.GetWorkflows());
        }
    }
}
