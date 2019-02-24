using System;
using System.ComponentModel;
using System.Resources;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// A globalised CategoryAttribute that retrieves the display name from a resource file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class GlobalisedCategoryAttribute : CategoryAttribute
    {
        public GlobalisedCategoryAttribute(string category) : base(category)
        { }

        public GlobalisedCategoryAttribute(Type type, string propertyName) : base(GetStringFromResource(type, propertyName))
        { }

        private static string GetStringFromResource(Type type, string propertyName)
        {
            string localisedString = "Category";
            ResourceManager manager = new ResourceManager(string.Concat(type.Namespace, ".Properties.Resources"), type.Assembly);

            if (manager != default(ResourceManager))
            {
                string resourceName = string.Concat(type.Name, propertyName, "Category");
                localisedString = manager.GetString(resourceName);
            }

            return localisedString;
        }

    }
}
