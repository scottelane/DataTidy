using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace ScottLane.DataTidy.Core
{
    public class DataTableFieldCheckedListBoxEditor : UITypeEditor
    {
        private const string FIELD_DELIMITER = ", ";

        private CheckedListBox checkedListBox;

        /// <summary>
        /// Gets a value indicating whether the drop down list is resizable.
        /// </summary>
        public override bool IsDropDownResizable
        {
            get { return true; }
        }

        /// <summary>
        /// Initialises a new instance of the DataTableFieldCheckedListBoxEditor class.
        /// </summary>
        public DataTableFieldCheckedListBoxEditor()
        {
            checkedListBox = new CheckedListBox()
            {
                BorderStyle = BorderStyle.None,
                CheckOnClick = true
            };
        }

        /// <summary>
        /// Gets a list of selected Dynamics365Field objects.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="value">The value.</param>
        /// <returns>The selected objects.</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IDataSourceFieldsProvider fieldsProvider = (IDataSourceFieldsProvider)context.Instance;
            BindingList<DataTableField> currentValue = (BindingList<DataTableField>)value;
            checkedListBox.Items.Clear();
            checkedListBox.Items.Add("Select All", false);
            checkedListBox.ItemCheck += CheckedListBox_ItemCheck;

            List<DataTableField> fields = fieldsProvider.GetDataSourceFields();
            fields.ForEach(field => checkedListBox.Items.Add(field, currentValue.Contains((DataTableField)field)));

            IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            service.DropDownControl(checkedListBox);

            BindingList<DataTableField> checkedFields = new BindingList<DataTableField>();

            for (int itemIndex = 0; itemIndex < checkedListBox.CheckedIndices.Count; itemIndex++)
            {
                if (checkedListBox.CheckedIndices[itemIndex] != 0)
                {
                    DataTableField field = (DataTableField)checkedListBox.CheckedItems[itemIndex];
                    checkedFields.Add(field);
                }
            }

            return checkedFields;
        }

        /// <summary>
        /// Checks an item if it's selected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The ItemCheckEventArgs.</param>
        private void CheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index == 0)
            {
                for (int itemIndex = 1; itemIndex < checkedListBox.Items.Count; itemIndex++)
                {
                    checkedListBox.SetItemChecked(itemIndex, e.NewValue == CheckState.Checked);
                }
            }
        }

        /// <summary>
        /// Gets the edit style.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The edit style.</returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}
