using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScottLane.DataTidy.Core;
using ScottLane.DataTidy.Client.Model;

namespace ScottLane.DataTidy.Client.Forms
{
    public partial class PropertiesForm : ChildForm
    {
        public PropertiesForm()
        {
            InitializeComponent();

            ApplicationState.Default.ActiveProjectChanged += ApplicationState_ActiveProjectChanged;
            ApplicationState.Default.AsyncProcessStarted += ApplicationState_AsyncProcessStarted;
            ApplicationState.Default.AsyncProcessStopped += ApplicationState_AsyncProcessStopped;
            ApplicationState.Default.SelectedItemChanged += ApplicationState_SelectedItemChanged;
            ApplicationState.Default.SelectedErrorChanged += ApplicationState_SelectedErrorChanged;
        }

        private void ApplicationState_SelectedItemChanged(object sender, ObjectEventArgs e)
        {
            if (Visible)
            {
                selectedItemPropertyGrid.SelectedObject = e.Item;
                try
                {
                    selectedItemPropertyGrid.ExpandAllGridItems();
                }
                catch { }

                if (e.Item is INotifyPropertyChanged item)
                {
                    item.PropertyChanged += SelectedItem_PropertyChanged;
                }
            }
        }

        private void ApplicationState_SelectedErrorChanged(object sender, NotificationEventArgs e)
        {
            if (e.NotificationType == NotificationType.Error && e.PropertyName != default(string))
            {
                GridItem errorItem = GetGridItem(e.PropertyName);

                if (errorItem != default(GridItem))
                {
                    selectedItemPropertyGrid.Select();
                    selectedItemPropertyGrid.SelectedGridItem = errorItem;
                }
            }
        }

        private void ApplicationState_AsyncProcessStarted(object sender, AsyncEventArgs e)
        {
            selectedItemPropertyGrid.Enabled = false;
        }

        private void ApplicationState_AsyncProcessStopped(object sender, EventArgs e)
        {
            selectedItemPropertyGrid.Enabled = true;
        }

        private void ApplicationState_ActiveProjectChanged(object sender, ProjectEventArgs e)
        {
            selectedItemPropertyGrid.SelectedObject = default(object);
        }

        private void SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            selectedItemPropertyGrid.Refresh();
            ApplicationState.Default.OnSelectedItemPropertyChanged(new ObjectEventArgs(ApplicationState.Default.SelectedItem)); // todo - remove after treeview inotifypropertychanged event bug fixed
        }

        private GridItem GetGridItem(string propertyName)
        {
            GridItem gridItem = selectedItemPropertyGrid.SelectedGridItem;

            if (gridItem != default(GridItem))
            {
                while (gridItem.Parent != null)
                {
                    gridItem = gridItem.Parent;
                }

                gridItem = GetGridItem(gridItem, propertyName);
            }

            return gridItem;
        }

        private GridItem GetGridItem(GridItem parentGridItem, string propertyName)
        {
            GridItem gridItem = default(GridItem);

            if (parentGridItem?.PropertyDescriptor?.Name == propertyName)
            {
                gridItem = parentGridItem;
            }
            else
            {
                for (int itemIndex = 0; itemIndex < parentGridItem.GridItems.Count && gridItem == default(GridItem); itemIndex++)
                {
                    gridItem = GetGridItem(parentGridItem.GridItems[itemIndex], propertyName);
                }
            }

            return gridItem;
        }

        private void PropertiesForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ApplicationState.Default.ActiveProjectChanged -= ApplicationState_ActiveProjectChanged;
            ApplicationState.Default.AsyncProcessStarted -= ApplicationState_AsyncProcessStarted;
            ApplicationState.Default.AsyncProcessStopped -= ApplicationState_AsyncProcessStopped;
            ApplicationState.Default.SelectedItemChanged -= ApplicationState_SelectedItemChanged;
            ApplicationState.Default.SelectedErrorChanged -= ApplicationState_SelectedErrorChanged;
        }
    }
}
