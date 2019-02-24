using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    public class Dynamics365ProcessConverter : TypeConverter
    {
        private const string PROCESS_DELIMITER = ", ";

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // convert from delimited string to field list
            if (!string.IsNullOrEmpty((string)value))
            {
                string valueString = (string)value;
                string[] processStrings = valueString.Split(new string[] { PROCESS_DELIMITER }, StringSplitOptions.None);

                IDynamics365ProcessesProvider provider = (IDynamics365ProcessesProvider)context.Instance;
                List<Dynamics365Process> entityProcesses = provider.GetProcesses();
                BindingList<Dynamics365Process> processes = new BindingList<Dynamics365Process>();

                foreach (string processString in processStrings)
                {
                    Guid id = new Guid(Regex.Match(valueString, CoreUtility.FieldMatchPattern).Groups[1].Value);
                    Dynamics365Process process = entityProcesses.FirstOrDefault(p => p.ID == id);
                    processes.Add(process);
                }

                return processes;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != default(List<Dynamics365Process>))
            {
                BindingList<Dynamics365Process> processes = (BindingList<Dynamics365Process>)value;
                return string.Join(PROCESS_DELIMITER, processes.Select(process => string.Format("{0} ({1})", process.Name, process.ID)));
            }

            return null;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IDynamics365ProcessesProvider provider = (IDynamics365ProcessesProvider)context.Instance;
            return new StandardValuesCollection(provider.GetProcesses());
        }
    }
}
