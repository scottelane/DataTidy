using System;
using System.ComponentModel;
using System.Resources;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// A globalised DescriptionAttribute that retrieves the description from a resource file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class GlobalisedDecriptionAttribute : DescriptionAttribute
    {
        public GlobalisedDecriptionAttribute(string description) : base(description)
        { }

        public GlobalisedDecriptionAttribute(Type type, string propertyName) : base(GetLocalizedString(type, propertyName))
        { }

        static string GetLocalizedString(Type type, string propertyName)
        {
            string localisedString = string.Empty;
            ResourceManager manager = new ResourceManager(string.Concat(type.Namespace, ".Properties.Resources"), type.Assembly);

            if (manager != default(ResourceManager))
            {
                string resourceName = string.Concat(type.Name, propertyName, "Description");
                localisedString = manager.GetString(resourceName);
            }

            return localisedString;
        }
    }
}
