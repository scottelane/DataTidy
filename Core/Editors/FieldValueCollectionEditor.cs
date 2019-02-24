using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// A collection editor that allows configuration of collections of FieldValue derived types. This editor should be used as the basis for configuring collections of custom FieldValue types.
    /// </summary>
    public class FieldValueCollectionEditor : CollectionEditor
    {
        /// <summary>
        /// Initialises a new instance of the FieldValueCollectionEditor editor for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        public FieldValueCollectionEditor(Type type) : base(type)
        { }

        /// <summary>
        /// Creates the CollectionForm and overrides some standard control text and behaviours.
        /// </summary>
        /// <returns>The CollectionForm.</returns>
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm form = base.CreateCollectionForm();
            form.Text = "Value Mapping Editor";
            form.Width = 700;

            PropertyGrid propertyGrid = (PropertyGrid)form.Controls[0].Controls[5];
            propertyGrid.HelpVisible = true;

            Button addButton = (Button)form.Controls[0].Controls[1].Controls[0];

            if (addButton.ContextMenuStrip != default(ContextMenuStrip))
            {
                foreach (ToolStripItem item in form.Controls[0].Controls[1].Controls[0].ContextMenuStrip.Items)
                {
                    item.Text = GetAddButtonDisplayName(item.Text);
                }
            }

            return form;
        }

        /// <summary>
        /// Allows the creation of derived FieldValue types in the control.
        /// </summary>
        /// <returns>An array of FieldValue types.</returns>
        protected override Type[] CreateNewItemTypes()
        {
            return new Type[] { typeof(DataSourceValue), typeof(UserProvidedValue), typeof(LookupValue) };
        }

        /// <summary>
        /// Gets the display name for a derived FieldValue type.
        /// </summary>
        /// <param name="typeName">The FieldValue derived type name.</param>
        /// <returns>The display name.</returns>
        protected virtual string GetAddButtonDisplayName(string typeName)
        {
            string friendlyName = typeName;

            if (typeName == "DataSourceValue")
            {
                friendlyName = "Data Source Value";
            }
            else if (typeName == "UserProvidedValue")
            {
                friendlyName = "User-Provided Value";
            }
            else if (typeName == "LookupValue")
            {
                friendlyName = "Lookup Value";
            }

            return friendlyName;
        }

        /// <summary>
        /// Creates an instance of a FieldValue object of the specified type.
        /// </summary>
        /// <param name="type">The FieldValue derived type.</param>
        /// <returns>The derived FieldValue object.</returns>
        protected override object CreateInstance(Type type)
        {
            IFieldValueCreator creator = (IFieldValueCreator)Context.Instance;
            return creator.CreateFieldValue(type);
        }
    }
}
