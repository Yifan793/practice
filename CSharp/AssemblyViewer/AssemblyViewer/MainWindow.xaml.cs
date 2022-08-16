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
            Assembly assembly = Assembly.LoadFile("C:\\Users\\ShaySong\\Downloads\\TestAssemblyForTraining.exe");
            ViewModel.DealAssembly(assembly);
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
            NameSpaecList = new List<ViewModelNameSpace>();
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
                Assembly assembly = Assembly.LoadFile(_filename);
                DealAssembly(assembly);
            }
        }
        public List<ViewModelNameSpace> NameSpaecList { get; set; }

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
                    string[] enums = type.GetEnumNames();
                    foreach (string enumName in enums)
                    {
                        ViewModelObject vmObject = new ViewModelObject();
                        vmObject.Name = enumName;
                        vmBase.ChildList.Add(vmObject);
                    }
                }
                else
                {
                    if (type.IsClass)
                    {
                        vmBase = new ViewModelClass();
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
                        vmBase.ChildList.AddRange(DealMethods(type));
                        vmBase.ChildList.AddRange(DealEvents(type));
                        vmBase.ChildList.AddRange(DealProperties(type));
                        vmBase.ChildList.AddRange(DealFields(type));
                    }
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
        }

        private List<ViewModelMethod> DealMethods(Type type)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            List<ViewModelMethod> methodList = new List<ViewModelMethod>();
            foreach(MethodInfo method in methods)
            {
                Type declaringType = method.DeclaringType;
                if (!declaringType.Name.EndsWith(type.Name) || method.IsSpecialName)
                {
                    continue;
                }
                ViewModelMethod vmMethod = new ViewModelMethod();
                vmMethod.Name = method.Name;
                methodList.Add(vmMethod);
            }
            return methodList;
        }
        private List<ViewModelEvent> DealEvents(Type type)
        {
            EventInfo[] eventInfos = type.GetEvents(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            List<ViewModelEvent> eventList = new List<ViewModelEvent>();
            foreach (EventInfo eventInfo in eventInfos)
            {
                Type declaringType = eventInfo.DeclaringType;
                if (!declaringType.Name.EndsWith(type.Name))
                {
                    continue;
                }
                ViewModelEvent vmEvent = new ViewModelEvent();
                vmEvent.Name = eventInfo.Name;  
                eventList.Add(vmEvent);
            }
            return eventList;
        }
        private List<ViewModelProperty> DealProperties(Type type)
        {
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            List<ViewModelProperty> propertyList = new List<ViewModelProperty>();
            foreach(PropertyInfo propertyInfo in propertyInfos)
            {
                Type declaringType = propertyInfo.DeclaringType;
                if (!declaringType.Name.EndsWith(type.Name))
                {
                    continue;
                }
                ViewModelProperty vmProperty = new ViewModelProperty();
                vmProperty.Name = propertyInfo.Name;
                propertyInfo.GetAccessors();
                propertyList.Add(vmProperty);
            }
            return propertyList;
        }
        private List<ViewModelField> DealFields(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            List<ViewModelField> fieldList = new List<ViewModelField>();
            foreach(FieldInfo fieldInfo in fieldInfos)
            {
                Type declaringType = fieldInfo.DeclaringType;
                if (!declaringType.Name.EndsWith(type.Name) || fieldInfo.IsPrivate)
                {
                    continue;
                }
                ViewModelField vmField = new ViewModelField();
                vmField.Name = fieldInfo.Name;
                fieldList.Add(vmField);
            }
            return fieldList;
        }
    }
}
