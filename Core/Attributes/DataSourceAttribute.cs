using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Provide information that drive user interface elements for a DataSource. All plugins that subclass the DataSource class must include this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DataSourceAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name that is diplayed in the user interface for the data source.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description that is displayed in a user interface tooltip for the data source.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the location of an icon resource that is displayed in the user interface for the data source.
        /// </summary>
        public string IconResourceName { get; set; }

        /// <summary>
        /// Gets or sets the Connection class type that is supported by the data source.
        /// </summary>
        public Type ConnectionType { get; set; }

        /// <summary>
        /// Gets or sets the Connection class types that are supported by the data source.
        /// </summary>
        public Type[] ConnectionTypes { get; set; }

        /// <summary>
        /// Initialises a new instance of the DataSourceAttribute class with the specified name, description, icon resource type and connection type.
        /// </summary>
        /// <param name="displayName">The data source name.</param>
        /// <param name="description">The data source name.</param>
        /// <param name="iconResourceName">The location of the data source's icon resource.</param>
        /// <param name="connectionType">The supported Connection type.</param>
        public DataSourceAttribute(string displayName, string description, string iconResourceName, Type connectionType)
        {
            DisplayName = displayName;
            Description = description;
            IconResourceName = iconResourceName;
            ConnectionType = connectionType;
        }

        public DataSourceAttribute(Type type, string iconResourceName, Type connectionType) : this(GetResourceString(type, nameof(DisplayName)), GetResourceString(type, nameof(Description)), iconResourceName, connectionType)
        { }

        private static string GetResourceString(Type type, string suffix)
        {
            string localisedString = string.Empty;
            ResourceManager manager = new ResourceManager(string.Concat(type.Namespace, ".Properties.Resources"), type.Assembly);

            if (manager != default(ResourceManager))
            {
                string resourceName = string.Concat(type.Name, suffix);
                localisedString = manager.GetString(resourceName);
            }

            return localisedString;

        }

        /// <summary>
        /// Gets the data source icon from the specified plugin assembly.
        /// </summary>
        /// <param name="assembly">The plugin assembly.</param>
        /// <returns>The icon.</returns>
        public Bitmap GetIcon(Assembly assembly)
        {
            Bitmap icon = null;

            if (assembly != null && IconResourceName != null)
            {
                using (Stream stream = assembly.GetManifestResourceStream(IconResourceName))
                {
                    if (stream != null)
                    {
                        icon = new Bitmap(stream);
                    }
                    else
                    {
                        Debug.WriteLine(string.Format(Resources.DataSourceAttributeResourceNotFound, IconResourceName));
                    }
                }
            }

            return icon;
        }
    }
}
