using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Contacts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = e.OriginalSource as DependencyObject;
            TreeViewItem item = GetDependencyObjectFromVisualTree(obj, typeof(TreeViewItem)) as TreeViewItem;
            if (item == null)
            {
                return;
            }
            Console.WriteLine(typeof(GroupNodeItem));
            Console.WriteLine(item.Header.GetType());
            return;
            GroupNodeItem groupItem = (GroupNodeItem)item.Header;
            if (groupItem != null)
            {
                return;
            }

            PersonNodeItem personItem = (PersonNodeItem)item.Header;
            if (personItem != null)
            {
                return;
            }

        }
        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = e.OriginalSource as DependencyObject;
            TreeViewItem item = GetDependencyObjectFromVisualTree(obj, typeof(TreeViewItem)) as TreeViewItem;
            if (item == null)
            {
                return;
            }
            GroupNodeItem selectedElement = (GroupNodeItem)item.Header;
            string header = selectedElement.Name;
            if (header.ToUpper() == "PACKAGES")
            {
                // Packages root node
                MenuItem mnuItem = new MenuItem();
                mnuItem.Header = "New Package";
                ContextMenu menu = new ContextMenu() { };
                menu.Items.Add(mnuItem);
                (sender as TreeViewItem).ContextMenu = menu;
            }
        }
        private static DependencyObject GetDependencyObjectFromVisualTree(
            DependencyObject startObject,
            Type type)
        {
            var parent = startObject;
            while (parent != null)
            {
                if (type.IsInstanceOfType(parent))
                    break;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }

        //public void AddGroup_Click(object sender, RoutedEventArgs e)
        //{
        //    var item = this.tree1.SelectedItem as NodeX;
        //    if (item != null)
        //    {
        //        MessageBox.Show(item.Name.ToString());
        //    }
        //}

        //public void DeleteGroup_Click(object sender, RoutedEventArgs e)
        //{
        //    var item = this.tree1.selecteditem as nodex;
        //    if (item != null)
        //    {
        //        MessageBox.show(item.name.tostring());
        //    }
        //}
    }
}
