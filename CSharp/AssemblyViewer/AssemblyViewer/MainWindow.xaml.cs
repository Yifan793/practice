﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Intrinsics.Arm;

namespace AssemblyViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
        }
        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.Multiselect = true;
            openFileDlg.Filter = "exe files(.exe)|*.exe|dll files(.dll)|*.dll";
            if (openFileDlg.ShowDialog() == true)
            {
                ViewModel.FileName = openFileDlg.FileName;
            }
        }
    }

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void onPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            AssemblyList = new ObservableCollection<ViewModelObject>();
        }
        private string _filename;
        public string FileName
        {
            get
            {
                return _filename;
            }
            set
            {
                if (value == _filename)
                {
                    return;
                }
                _filename = value;
                onPropertyChanged();
                DealAssembly(_filename);
            }
        }
        public ObservableCollection<ViewModelObject> AssemblyList { get; set; }

        public void DealAssembly(string filename)
        {
            Assembly assembly = Assembly.LoadFile(filename);
            MethodInfo entry = assembly.EntryPoint;
            
            ViewModelObject vmAssembly = new ViewModelObject();
            vmAssembly.Name = assembly.ManifestModule.Name;
            vmAssembly.Type = ViewerType.Assembly;
            vmAssembly.AccessRights = AccessRights.InValid;
            AssemblyList.Add(vmAssembly);

            Dictionary<string, ViewModelNameSpace> ns = new Dictionary<string, ViewModelNameSpace>();
            Dictionary<string, ViewModelBaseClass> bs = new Dictionary<string, ViewModelBaseClass>();
            foreach (Type type in assembly.GetTypes())
            {
                if (!ns.ContainsKey(type.Namespace))
                {
                    ViewModelNameSpace viewModleNs = new ViewModelNameSpace();
                    viewModleNs.Name = type.Namespace;
                    ns[type.Namespace] = viewModleNs;
                    vmAssembly.ChildList.Add(ns[type.Namespace]);
                }
                ViewModelBaseClass vmBase = Util.DealClass(type);
                if (entry.DeclaringType == type)
                {
                    vmBase.ChildList.Add(new ViewModelMethod(entry));
                }
                if (vmBase != null)
                {
                    bs[type.Name] = vmBase;
                    if (type.DeclaringType != null && bs[type.DeclaringType.Name] != null)
                    {
                        bs[type.DeclaringType.Name].ChildList.Add(vmBase);
                    }
                    else
                    {
                        ns[type.Namespace].ChildList.Add(vmBase);
                    }
                }
            }
            Util.SortAll(vmAssembly.ChildList);
        }
    }

    public class CompositeCollectionConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var res = new CompositeCollection();
            foreach (var item in values)
            {
                if (item is IEnumerable && item != null)
                {
                    res.Add(new CollectionContainer()
                    {
                        Collection = item as IEnumerable
                    });
                }
            }
            return res;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class UrlToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            return new BitmapImage(new Uri(value.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
