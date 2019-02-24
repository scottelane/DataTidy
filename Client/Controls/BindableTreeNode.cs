using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ScottLane.DataTidy.Client.Controls
{
    /// <summary>
    /// A TreeNode control that supports data binding and preservation of node expansion state.
    /// </summary>
    [Serializable]
    public class BindableTreeNode : TreeNode
    {
        BindingContext bindingContext;
        CurrencyManager currencyManager;
        PropertyManager propertyManager;

        #region Properties

        private object dataSource;

        /// <summary>
        /// Gets or sets the data source to display.
        /// </summary>
        public object DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;
                    DataBind();
                }
            }
        }

        /// <summary>
        /// Gets or sets the data source property that contains a list of items to display.
        /// </summary>
        private string dataMember;

        public string DataMember
        {
            get { return dataMember; }
            set
            {
                if (dataMember != value)
                {
                    dataMember = value;
                    DataBind();
                }
            }
        }

        private string displayMember;

        /// <summary>
        /// Gets or sets the property to display as the node label.
        /// </summary>
        public string DisplayMember
        {
            get { return displayMember; }
            set
            {
                if (displayMember != value)
                {
                    displayMember = value;
                    DataBind();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the node is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a unique node identifier.
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Event raised when a node is being bound.
        /// </summary>
        public event NodeEventHandler DataBinding;

        /// <summary>
        /// Event raised after a node has been bound.
        /// </summary>
        public event NodeEventHandler DataBound;

        #endregion

        /// <summary>
        /// Initialises a new instance of the BindableTreeNode class.
        /// </summary>
        public BindableTreeNode()
        { }

        /// <summary>
        /// Binds the Node to the underlying data source.
        /// </summary>
        internal void DataBind()
        {
            OnDataBinding(new NodeEventArgs(this));

            if (dataSource == default(object)) return;

            if (displayMember != default(string))
            {
                Text = DataSource.GetType().GetProperty(displayMember).GetValue(DataSource)?.ToString();
            }
            else
            {
                Text = DataSource.ToString();
            }

            if (Enabled)
            {
                ForeColor = System.Drawing.Color.Black;
                
            }
            else
            {
                ForeColor = System.Drawing.Color.Gray;
            }

            if (bindingContext == default(BindingContext))
            {
                bindingContext = new BindingContext();

                if (dataMember == default(string))
                {
                    propertyManager = (PropertyManager)bindingContext[dataSource];
                }
                else
                {
                    currencyManager = (CurrencyManager)bindingContext[dataSource, dataMember];
                    currencyManager.ListChanged += CurrencyManager_ListChanged;
                    currencyManager.ItemChanged += CurrencyManager_ItemChanged;
                    CreateNodes(false);
                }
            }

            OnDataBound(new NodeEventArgs(this));
        }

        private void CurrencyManager_ItemChanged(object sender, ItemChangedEventArgs e)
        {
            if (e.Index > -1)
            {
                DataBind();
            }
        }

        /// <summary>
        /// Creates nodes for each item in the DataMember list.
        /// </summary>
        /// <param name="useUpdates">If true, indicates that the TreeView BeginUpdate and EndUpdate statements should be used to prevent flashing.</param>
        private void CreateNodes(bool useUpdates)
        {
            if (useUpdates)
            {
                TreeView?.BeginUpdate();
            }

            Nodes.Clear();

            foreach (Object item in currencyManager.List)
            {
                Nodes.Add(CreateNodeFromItem(item));
            }

            if (useUpdates)
            {
                TreeView?.EndUpdate();
            }
        }

        /// <summary>
        /// Creates a node from the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The node.</returns>
        private BindableTreeNode CreateNodeFromItem(object item)
        {
            BindableTreeNode node = new BindableTreeNode();
            node.DataBinding += Node_DataBinding;
            node.DataBound += Node_DataBound;
            node.DataSource = item;
            return node;
        }

        /// <summary>
        /// Redraws nodes if the underlying list changes.
        /// </summary>
        private void CurrencyManager_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemAdded)
            {
                Nodes.Insert(e.NewIndex, CreateNodeFromItem(currencyManager.List[e.NewIndex]));
            }
            else if (e.ListChangedType == ListChangedType.ItemDeleted)
            {
                Nodes.RemoveAt(e.NewIndex);
            }
            else if (e.ListChangedType != ListChangedType.Reset)
            {
                CreateNodes(true);
            }
        }

        /// <summary>
        /// Bubbles the node DataBinding event up to the parent BindableTreeView.
        /// </summary>
        private void Node_DataBinding(object sender, NodeEventArgs e)
        {
            OnDataBinding(e);
        }

        /// <summary>
        /// Raises the DataBinding event.
        /// </summary>
        protected virtual void OnDataBinding(NodeEventArgs e)
        {
            DataBinding?.Invoke(this, e);
        }

        private void Node_DataBound(object sender, NodeEventArgs e)
        {
            OnDataBound(e);
        }

        /// <summary>
        /// Raises the DataBinding event.
        /// </summary>
        protected virtual void OnDataBound(NodeEventArgs e)
        {
            DataBound?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Delegate for node-related events.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The NodeEventArgs.</param>
    public delegate void NodeEventHandler(object sender, NodeEventArgs e);

    /// <summary>
    /// Contains data for node-related events.
    /// </summary>
    public class NodeEventArgs : EventArgs
    {
        public BindableTreeNode Node { get; set; }

        public NodeEventArgs(BindableTreeNode node)
        {
            Node = node;
        }
    }
}
