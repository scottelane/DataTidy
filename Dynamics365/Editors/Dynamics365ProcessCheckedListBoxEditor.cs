using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace ScottLane.DataTidy.Dynamics365
{
    /// <summary>
    /// Custom editor for collections of Dynamics365Process objects that allows one, more than one or all Dynamics365Processes to be selected.
    /// </summary>
    public class Dynamics365ProcessCheckedListBoxEditor : UITypeEditor
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
        /// Initialises a new instance of the Dynamics365FieldCheckedListBoxEditor class.
        /// </summary>
        public Dynamics365ProcessCheckedListBoxEditor()
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
            IDynamics365ProcessesProvider processesProvider = (IDynamics365ProcessesProvider)context.Instance;
            List<Dynamics365Process> fields = processesProvider.GetProcesses();

            BindingList<Dynamics365Process> currentValue = (BindingList<Dynamics365Process>)value;
            checkedListBox.Items.Clear();
            checkedListBox.Items.Add("Select All", false);
            checkedListBox.ItemCheck += CheckedListBox_ItemCheck;

            fields.ForEach(process => checkedListBox.Items.Add(process, currentValue.Contains(process)));

            IWindowsFormsEditorService service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            service.DropDownControl(checkedListBox);

            BindingList<Dynamics365Process> checkedProcesses = new BindingList<Dynamics365Process>();

            for (int itemIndex = 0; itemIndex < checkedListBox.CheckedIndices.Count; itemIndex++)
            {
                if (checkedListBox.CheckedIndices[itemIndex] != 0)
                {
                    Dynamics365Process process = (Dynamics365Process)checkedListBox.CheckedItems[itemIndex];
                    checkedProcesses.Add(process);
                }
            }

            return checkedProcesses;
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
