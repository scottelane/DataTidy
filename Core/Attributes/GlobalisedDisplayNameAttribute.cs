using System;
using System.ComponentModel;
using System.Resources;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// A globalised DisplayNameAttribute that retrieves the display name from a resource file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class GlobalisedDisplayNameAttribute : DisplayNameAttribute
    {
        public GlobalisedDisplayNameAttribute(string displayName) : base(displayName)
        { }

        public GlobalisedDisplayNameAttribute(Type type, string propertyName) : base(GetStringFromResource(type, propertyName))
        { }

        static string GetStringFromResource(Type type, string propertyName)
        {
            string localisedString = propertyName;
            ResourceManager manager = new ResourceManager(string.Concat(type.Namespace, ".Properties.Resources"), type.Assembly);

            if (manager != default(ResourceManager))
            {
                string resourceName = string.Concat(type.Name, propertyName, "DisplayName");
                localisedString = manager.GetString(resourceName);
            }

            return localisedString;
        }
    }
}
