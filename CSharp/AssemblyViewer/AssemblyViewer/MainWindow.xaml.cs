using System;
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
            //Assembly assembly = Assembly.LoadFile("C:\\Users\\ShaySong\\Downloads\\TestAssemblyForTraining.exe");
            //ViewModel.DealAssembly(assembly);
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

    public class ViewModelBase : INotifyPropertyChanged, INotifyCollectionChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void onPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected void OnCollectionChanged([CallerMemberName] String propertyName = "")
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(propertyName));
            }
            NotifyCollectionChangedEventHandler h = CollectionChanged;
            if (h != null)
                h(this, e);
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            NameSpaecList = new List<ViewModelObject>();
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
                NameSpaecList.Clear();
                Assembly assembly = Assembly.LoadFile(_filename);
                DealAssembly(assembly);
            }
        }
        private List<ViewModelObject> _nameSpaceList;
        public List<ViewModelObject> NameSpaecList 
        { 
            get
            {
                return _nameSpaceList;
            }
            set
            {
                _nameSpaceList = value;
                onPropertyChanged();
            }
        }

        public void DealAssembly(Assembly assembly)
        {
            Dictionary<string, ViewModelNameSpace> ns = new Dictionary<string, ViewModelNameSpace>();
            Dictionary<string, ViewModelBaseClass> bs = new Dictionary<string, ViewModelBaseClass>();
            foreach (Type type in assembly.GetTypes())
            {
                if (!ns.ContainsKey(type.Namespace))
                {
                    ViewModelNameSpace viewModleNs = new ViewModelNameSpace();
                    viewModleNs.Name = type.Namespace;
                    ns[type.Namespace] = viewModleNs;
                    NameSpaecList.Add(ns[type.Namespace]);
                }
                Console.WriteLine("namespace: " + type.Namespace + " " + type.Name);
                ViewModelBaseClass vmBase = null;
                if (type.IsEnum)
                {
                    vmBase = new ViewModelEnum();
                    vmBase.Name = type.Name;
                    vmBase.Type = ViewerType.Enumeration;
                    string[] enums = type.GetEnumNames();
                    vmBase.FieldList.AddRange(Util.DealFields(type));
                    foreach (string enumName in enums)
                    {
                        ViewModelEnumItem vmObject = new ViewModelEnumItem();
                        vmObject.Name = enumName;
                        vmObject.ReturnValue = type.Name;
                        vmBase.ChildList.Add(vmObject);
                    }
                }
                else
                {
                    if (type.IsClass)
                    {
                        vmBase = new ViewModelClass();
                        ConstructorInfo[] infoList = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach(ConstructorInfo info in infoList)
                        {
                            ViewModelConstructor method = new ViewModelConstructor(info);
                            ((ViewModelClass)vmBase).ConstructorList.Add(method);
                        }
                    }
                    else if (type.IsInterface)
                    {
                        vmBase = new ViewModelInterface();
                    }
                    else if (type.IsValueType)
                    {
                        vmBase = new ViewModelStruct();
                    }
                    if (vmBase != null)
                    {
                        vmBase.Name = type.Name;
                        vmBase.MethodList.AddRange(Util.DealMethods(type));
                        vmBase.EventList.AddRange(Util.DealEvents(type));
                        vmBase.PropertyList.AddRange(Util.DealProperties(type));
                        vmBase.FieldList.AddRange(Util.DealFields(type));
                        vmBase.BaseList.Add(Util.DealBaseType(type));
                    }
                }
                if (vmBase != null)
                {
                    vmBase.AccessRights = Util.getAccessibility(type);
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
            Util.SortAll(NameSpaecList);
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
