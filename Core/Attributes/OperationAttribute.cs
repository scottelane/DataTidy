using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Provide information that drive user interface elements for an Operation. All plugins that subclass the Operation class must include this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class OperationAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name that is diplayed in the user interface for the operation.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description that is displayed in a user interface tooltip for the operation.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the location of an icon resource that is displayed in the user interface for the operation.
        /// </summary>
        public string IconResourceName { get; set; }

        /// <summary>
        /// Initialises a new instance of the OperationAttribute class with the specified name, description and icon resource type.
        /// </summary>
        /// <param name="name">The operation name.</param>
        /// <param name="description">The operation name.</param>
        /// <param name="iconResourceName">The location of the operation's icon resource.</param>
        public OperationAttribute(string name, string description, string iconResourceName)
        {
            DisplayName = name;
            Description = description;
            IconResourceName = iconResourceName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="iconResourceName"></param>
        public OperationAttribute(Type type, string iconResourceName) : this(GetResourceString(type, nameof(DisplayName)), GetResourceString(type, nameof(Description)), iconResourceName)
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
        /// Gets the connection icon from the specified plugin assembly.
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
                        Debug.WriteLine(string.Format(Resources.OperationAttributeResourceNotFound, IconResourceName));
                    }
                }
            }

            return icon;
        }
    }
}
