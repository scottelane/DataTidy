using System;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ColumnMappingCollectionEditor : CollectionEditor
    {
        /// <summary>
        /// Initialises a new instance of the ColumnMappingCollectionEditor editor for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        public ColumnMappingCollectionEditor(Type type) : base(type)
        { }

        /// <summary>
        /// Creates the CollectionForm and overrides some standard control text and behaviours.
        /// </summary>
        /// <returns>The CollectionForm.</returns>
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm form = base.CreateCollectionForm();
            form.Text = "Column Mapping Editor";
            form.Width = 700;

            PropertyGrid propertyGrid = (PropertyGrid)form.Controls[0].Controls[5];
            propertyGrid.HelpVisible = true;

            return form;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected override object CreateInstance(Type type)
        {
            IFieldMappingCreator creator = (IFieldMappingCreator)Context.Instance;
            return creator.CreateFieldMapping(type);
        }
    }
}
