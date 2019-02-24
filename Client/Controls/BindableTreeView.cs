using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace ScottLane.DataTidy.Client.Controls
{
    /// <summary>
    /// A TreeView control that supports data binding and preservation of node expansion state.
    /// </summary>
    /// <remarks>
    /// Based on https://www.codeproject.com/Articles/15396/Implementing-complex-data-binding-in-custom-contro
    /// </remarks>
    public class BindableTreeView : TreeView
    {
        private CurrencyManager currencyManager;

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

        private string dataMember;

        /// <summary>
        /// Gets or sets the data source property that contains a list of items to display.
        /// </summary>
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

        /// <summary>
        /// Event raised when a node is being bound.
        /// </summary>
        public event NodeEventHandler NodeDataBinding;

        public event NodeEventHandler NodeDataBound;

        /// <summary>
        /// Initialises a new instance of the BindableTreeView class.
        /// </summary>
        public BindableTreeView()
        { }

        public override void Refresh()
        {
            base.Refresh();

            if (SelectedNode is BindableTreeNode node)
            {
                node.DataBind();
            }
        }

        /// <summary>
        /// Rebinds the control if the underlying binding context changes.
        /// </summary>
        protected override void OnBindingContextChanged(EventArgs e)
        {
            DataBind();
            base.OnBindingContextChanged(e);
        }

        /// <summary>
        /// Binds the TreeView to the underlying data source.
        /// </summary>
        private void DataBind()
        {
            if (dataSource == default(object) && Nodes.Count > 0)
            {
                Nodes.Clear();
            }

            if (dataSource == default(object) || dataMember == default(string) || base.BindingContext == default(BindingContext)) return;

            CurrencyManager newCurrencyManager = (CurrencyManager)base.BindingContext[dataSource, dataMember];

            if (currencyManager != newCurrencyManager)
            {
                if (currencyManager != default(CurrencyManager))
                {
                    currencyManager.ListChanged -= CurrencyManager_ListChanged;
                    currencyManager.ItemChanged -= CurrencyManager_ItemChanged;
                }

                currencyManager = newCurrencyManager;

                if (currencyManager != default(CurrencyManager))
                {
                    currencyManager.ListChanged += CurrencyManager_ListChanged;
                    currencyManager.ItemChanged += CurrencyManager_ItemChanged;
                }
            }

            CreateNodes();
        }

        /// <summary>
        /// Redraws nodes if the underlying list changes.
        /// </summary>
        private void CurrencyManager_ListChanged(object sender, ListChangedEventArgs e)
        {
            CreateNodes();
        }

        private void CurrencyManager_ItemChanged(object sender, ItemChangedEventArgs e)
        {
            if (e.Index > -1)
            {
                ((BindableTreeNode)Nodes[e.Index]).DataBind();
            }
        }

        /// <summary>
        /// Creates nodes for each item in the DataMember list.
        /// </summary>
        private void CreateNodes()
        {
            Dictionary<Guid, bool> initialState = GetExpandedState();

            BeginUpdate();
            Nodes.Clear();

            foreach (object item in currencyManager.List)
            {
                BindableTreeNode node = new BindableTreeNode();
                node.DataBinding += Node_DataBinding;
                node.DataBound += Node_DataBound;
                node.DataSource = item;
                Nodes.Add(node);
            }

            SetExpandedState(initialState);
            EndUpdate();
        }

        /// <summary>
        /// Bubbles a DataBinding event from a child node.
        /// </summary>
        private void Node_DataBinding(object sender, NodeEventArgs e)
        {
            OnNodeDataBinding(e);
        }

        private void Node_DataBound(object sender, NodeEventArgs e)
        {
            OnNodeDataBound(e);
        }

        /// <summary>
        /// Gets a Dictionary containing the expanded state of each node in the TreeView.
        /// </summary>
        /// <returns>The Dictionary.</returns>
        public Dictionary<Guid, bool> GetExpandedState()
        {
            Dictionary<Guid, bool> expandedState = new Dictionary<Guid, bool>();

            foreach (BindableTreeNode node in Nodes)
            {
                expandedState.Add(node.ID, node.IsExpanded);
                GetExpandedState(node, expandedState);
            }

            return expandedState;
        }

        /// <summary>
        /// Gets the expande state of each child node in the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="expandedState">The expanded state of each node.</param>
        private void GetExpandedState(BindableTreeNode node, Dictionary<Guid, bool> expandedState)
        {
            foreach (BindableTreeNode childNode in node.Nodes)
            {
                if (!expandedState.ContainsKey(childNode.ID) && childNode.Nodes.Count > 0)
                {
                    expandedState.Add(childNode.ID, node.IsExpanded);
                }
            }
        }

        /// <summary>
        /// Sets the expanded state of each node in the tree to the specified state.
        /// </summary>
        /// <param name="expandedState">The expanded state Dictionary.</param>
        public void SetExpandedState(Dictionary<Guid, bool> expandedState)
        {
            foreach (BindableTreeNode node in Nodes)
            {
                SetExpandedState(node, expandedState);
            }
        }

        /// <summary>
        /// Sets the expanded state of each child node in the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="expandedState">The expanded state of each node.</param>
        private void SetExpandedState(BindableTreeNode node, Dictionary<Guid, bool> expandedState)
        {
            if (expandedState.ContainsKey(node.ID))
            {
                if (expandedState[node.ID] && !node.IsExpanded)
                {
                    node.Expand();
                }
                else if (expandedState[node.ID] && node.IsExpanded)
                {
                    node.Collapse();
                }
            }

            foreach (BindableTreeNode childNode in node.Nodes)
            {
                SetExpandedState(childNode, expandedState);
            }
        }

        /// <summary>
        /// Finds a node with the specified ID.
        /// </summary>
        /// <param name="id">The ID to find.</param>
        public BindableTreeNode Find(Guid id)
        {
            BindableTreeNode node = default(BindableTreeNode);

            for (int nodeIndex = 0; nodeIndex < Nodes.Count && node == default(BindableTreeNode); nodeIndex++)
            {
                node = Find((BindableTreeNode)Nodes[nodeIndex], id);
            }

            return node;
        }

        /// <summary>
        /// Finds a node with the specified ID within the specified node.
        /// </summary>
        /// <param name="node">The node to search.</param>
        /// <param name="id">The ID to find.</param>
        private BindableTreeNode Find(BindableTreeNode node, Guid id)
        {
            BindableTreeNode foundNode = node.ID == id ? node : default(BindableTreeNode);

            for (int nodeIndex = 0; nodeIndex < node?.Nodes.Count && foundNode == default(BindableTreeNode); nodeIndex++)
            {
                foundNode = Find((BindableTreeNode)node.Nodes[nodeIndex], id);
            }

            return foundNode;
        }

        /// <summary>
        /// Raises the NodeDataBinding event.
        /// </summary>
        /// <param name="e">The NodeEventArgs.</param>
        protected virtual void OnNodeDataBinding(NodeEventArgs e)
        {
            NodeDataBinding?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the NodeDataBound event.
        /// </summary>
        /// <param name="e">The NodeEventArgs.</param>
        protected virtual void OnNodeDataBound(NodeEventArgs e)
        {
            NodeDataBound?.Invoke(this, e);
        }
    }
}
