using System;
using ScottLane.DataTidy.Core;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Custom editor for collections of Dynamics 365 FieldValue objects.
    /// </summary>
    /// <remarks>
    /// http://www.dotnetframework.org/default.aspx/DotNET/DotNET/8@0/untmp/whidbey/REDBITS/ndp/fx/src/Designer/CompMod/System/ComponentModel/Design/CollectionEditor@cs/1/CollectionEditor@cs
    /// </remarks>
    public class Dynamics365FieldValueCollectionEditor : FieldValueCollectionEditor
    {
        /// <summary>
        /// Initialises a new instance of the Dynamics365FieldValueCollectionEditor class that creates objects with the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        public Dynamics365FieldValueCollectionEditor(Type type) : base(type)
        { }

        /// <summary>
        /// Creates an array of object types that can be created.
        /// </summary>
        /// <returns>The Type array.</returns>
        protected override Type[] CreateNewItemTypes()
        {
            return new Type[] { typeof(Dynamics365DataSourceValue), typeof(Dynamics365UserProvidedValue), typeof(Dynamics365LookupValue) };
        }

        /// <summary>
        /// Gets the display name for each type.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <returns>The display name.</returns>
        protected override string GetAddButtonDisplayName(string typeName)
        {
            string friendlyName = typeName;

            if (typeName == nameof(Dynamics365DataSourceValue))
            {
                friendlyName = Properties.Resources.DataSourceValueName;
            }
            else if (typeName == nameof(Dynamics365UserProvidedValue))
            {
                friendlyName = Properties.Resources.UserProvidedValueName;
            }
            else if (typeName == nameof(Dynamics365LookupValue))
            {
                friendlyName = Properties.Resources.LookupValueName;
            }

            return friendlyName;
        }
    }
}
