using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace ScottLane.DataTidy.Core
{
    /// <summary>
    /// Provides the ability to specify a file name using a SaveFileDialog.
    /// </summary>
    public class SaveFileNameEditor : UITypeEditor
    {
        protected string filter;

        public string Filter { get; set; }

        public SaveFileNameEditor()
        {
            filter = "All files (*.*)|*.*";
        }

        /// <summary>
        /// Gets the edit style.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The edit style.</returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Edits a file name using a SaveFileDialog;
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="value">The value.</param>
        /// <returns>The file name.</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (!(context == null || context.Instance == null || provider == null))
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    if (value != null)
                    {
                        saveFileDialog.FileName = value.ToString();
                    }

                    saveFileDialog.Title = context.PropertyDescriptor.DisplayName;
                    saveFileDialog.Filter = filter;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        return saveFileDialog.FileName;
                    }
                }
            }

            return base.EditValue(context, provider, value);
        }
    }
}
