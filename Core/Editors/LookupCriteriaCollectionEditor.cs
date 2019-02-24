using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// A collection editor that allows configuration of collections of LookupCriteria derived types for a Lookup. This editor should be used as the basis for configuring collections of custom LookupCriteria types.
    /// </summary>
    public class LookupCriteriaCollectionEditor : CollectionEditor
    {
        /// <summary>
        /// Initialises a new instance of the LookupCriteriaCollectionEditor editor for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        public LookupCriteriaCollectionEditor(Type type) : base(type)
        { }

        /// <summary>
        /// Creates the CollectionForm and overrides some standard control text and behaviours.
        /// </summary>
        /// <returns>The CollectionForm.</returns>
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm form = base.CreateCollectionForm();
            form.Text = "Lookup Criteria Editor";
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
        /// Allows the creation of derived LookupCriteria types in the control.
        /// </summary>
        /// <returns>An array of LookupCriteria types.</returns>
        protected override Type[] CreateNewItemTypes()
        {
            return new Type[] { typeof(DataSourceValueCriteria), typeof(UserProvidedValueCriteria)/*, typeof(LookupValueCriteria)*/ };
        }

        /// <summary>
        /// Gets the display name for a derived LookupCriteria type.
        /// </summary>
        /// <param name="typeName">The LookupCriteria derived type name.</param>
        /// <returns>The display name.</returns>
        protected virtual string GetAddButtonDisplayName(string typeName)
        {
            string friendlyName = typeName;

            if (typeName ==  nameof(DataSourceValueCriteria))
            {
                friendlyName = "Data Source Value";
            }
            else if (typeName == nameof(UserProvidedValueCriteria))
            {
                friendlyName = "User-Provided Value";
            }
            else if (typeName == nameof(LookupValueCriteria))
            {
                friendlyName = "Lookup Value";
            }

            return friendlyName;
        }

        /// <summary>
        /// Creates an instance of a LookupCriteria object of the specified type.
        /// </summary>
        /// <param name="type">The LookupCriteria derived type.</param>
        /// <returns>The derived LookupCriteria object.</returns>
        protected override object CreateInstance(Type itemType)
        {
            ILookupCriteriaCreator creator = (ILookupCriteriaCreator)Context.Instance;
            return creator.CreateLookupCriteria(itemType);
        }
    }
}
