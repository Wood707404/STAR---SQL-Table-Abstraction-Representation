using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using WPFSamples.Samples.HousePlan;

namespace WPFSamples.HousePlan
{
    public partial class ErDiagram 
    {
        public List<Node> Nodes { get; set; }
        public List<Connector> Connectors { get; set; }
        public ObservableCollection<string> MyLists { get; set; }
        public List<DataGrid> SelectedDatagridsList { get; set; }

        public ErDiagram()
        {
            InitializeComponent();
            this.Width = SystemParameters.WorkArea.Width;
            this.Height = SystemParameters.WorkArea.Height;
            DataContext = new MainViewModel();
            SelectedDatagridsList = new List<DataGrid>();
        }     

        private void Thumb_Drag(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb == null)
                return;

            var node = thumb.DataContext as Node;
            if (node == null)
                return;

            node.Location.Value = Point.Add(node.Location.Value, new Vector(e.HorizontalChange, e.VerticalChange));      
        }

        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var listbox = sender as ListBox;

            if (listbox == null)
                return;

            var vm = listbox.DataContext as MainViewModel;

            if (vm == null)
                return;

            if (vm.SelectedObject != null && vm.SelectedObject is Node && vm.SelectedObject.IsNew)
            {
                vm.SelectedObject.Location.Value = e.GetPosition(listbox);
            }
            else if (vm.SelectedObject != null && vm.SelectedObject is Connector && vm.SelectedObject.IsNew)
            {
                var node = GetNodeUnderMouse();
                if (node == null)
                    return;

                var connector = vm.SelectedObject as Connector;
            }
        }

        private void ListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (vm != null)
            {
                if (vm.CreatingNewConnector)
                {
                    var node = GetNodeUnderMouse();
                    var connector = vm.SelectedObject as Connector;
                    if (node != null && connector != null && connector.Start == null)
                    {
                        //connector.Start = node;
                        node.IsHighlighted = true;
                        e.Handled = true;
                        return;
                    }
                }
                if (vm.SelectedObject != null)
                    vm.SelectedObject.IsNew = false;
                    
                vm.CreatingNewNode = false;
                vm.CreatingNewConnector = false;
            }
        }

        private Node GetNodeUnderMouse()
        {
            var item = Mouse.DirectlyOver as Border;
            if (item == null)
                return null;

            return item.DataContext as Node;
        }

        private void MidPoint_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb == null)
                return;

            var connector = thumb.DataContext as Connector;
            if (connector == null)
                return;

            connector.MidPoint.Value = Point.Add(connector.MidPoint.Value, new Vector(e.HorizontalChange, e.VerticalChange));
        }

        private void Snap_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb == null)
                return;

            var snap = thumb.DataContext as SnapSpot;
            if (snap == null)
                return;

            snap.Offset.Value = Point.Add(snap.Offset.Value, new Vector(snap.LockX ? 0 : e.HorizontalChange / 1000, snap.LockY ? 0 : e.VerticalChange / 1000));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mainView = DataContext as MainViewModel;
            
            Tuple<string, string> selectedRow1 = (Tuple<string, string>) SelectedDatagridsList[0].SelectedValue;
            int selectedRowIndex1 = 0;
            Node node1 = new Node();

            foreach (Node node in mainView.Nodes)
            {
                int index = 0; 
                foreach(Tuple<string, string> row in node.Data)
                {
                    if (row == selectedRow1)
                    {
                        selectedRowIndex1 = index;
                        node1 = node;
                    }
                    index++;
                }
            }
            SnapSpot snapSpot1 = node1.Snaps[selectedRowIndex1*2+1];

            Tuple<string, string> selectedRow2 = (Tuple<string, string>)SelectedDatagridsList[1].SelectedValue;
            int selectedRowIndex2 = 0;
            Node node2 = new Node();
            foreach (Node node in mainView.Nodes)
            {
                int index = 0;
                foreach (Tuple<string, string> row in node.Data)
                {
                    if (row == selectedRow2)
                    {
                        selectedRowIndex2 = index;
                        node2 = node;
                    }
                    index++;
                }
            }
            SnapSpot snapSpot2 = node2.Snaps[selectedRowIndex2 * 2 ];

            mainView.ConnectSnaps(snapSpot1, snapSpot2);

            SelectedDatagridsList.Clear();


        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (!SelectedDatagridsList.Contains(dataGrid))
            {
                SelectedDatagridsList.Add(dataGrid);
            }
        }
    }
}
