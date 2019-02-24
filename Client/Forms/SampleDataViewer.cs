using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ScottLane.DataTidy.Client.Forms
{
    /// <summary>
    /// Displays sample data from a DataSource.
    /// </summary>
    public partial class SampleDataViewer : Form
    {
        /// <summary>
        /// Gets or sets the DataTable to display.
        /// </summary>
        public DataTable DataTable { get; set; }

        /// <summary>
        /// Initialises a new instance of the SampleDataViewer class.
        /// </summary>
        public SampleDataViewer()
        {
            InitializeComponent();
            dataTableGridView.AutoGenerateColumns = true;
        }

        /// <summary>
        /// Loads the form and binds the DataTable to the grid.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The EventArgs.</param>
        private void DataTableViewer_Load(object sender, System.EventArgs e)
        {
            dataTableGridView.DataSource = DataTable;
            Text = string.Format("Sample Data Viewer - {0}", DataTable.TableName);
        }

        /// <summary>
        /// Sets the correct display and tooltip text for a column header.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The DataGridViewColumnEventArgs.</param>
        private void DataTableGridView_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            e.Column.HeaderText = DataTable.Columns[e.Column.Index].Caption;
            e.Column.ToolTipText = DataTable.Columns[e.Column.Index].ColumnName;
        }

        private void DataTableGridView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetDataObject(dataTableGridView.GetClipboardContent());
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Clipboard.SetDataObject(dataTableGridView.GetClipboardContent());
        }

        private void CopyWithHeadingsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            DataGridViewClipboardCopyMode previousMode = dataTableGridView.ClipboardCopyMode;
            dataTableGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            Clipboard.SetDataObject(dataTableGridView.GetClipboardContent());
            dataTableGridView.ClipboardCopyMode = previousMode;
        }
    }
}
