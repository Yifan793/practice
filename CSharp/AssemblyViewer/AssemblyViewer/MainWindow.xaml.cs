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

        public void DealAssembly(Assembly assembly)
        {
            int i = 0;
            foreach(Type type in assembly.GetTypes())
            {
                if (i != -1)
                {
                    Console.WriteLine("namespace: " + type.Namespace + " " + type.Name);
                    MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    EventInfo[] events = type.GetEvents(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (type.IsEnum)
                    {
                        string[] enums = type.GetEnumNames();
                        DealEnums(enums);
                    }
                    if (type.IsInterface)
                    {
                        Type[] interfaceTypes = type.GetInterfaces();
                        foreach(Type interfaceType in interfaceTypes)
                        {
                            Console.WriteLine("interface " + interfaceType.Name);
                        }
                    }
                    DealMethods(methods);
                    DealEvents(events);
                    DealProperties(properties);
                    DealFields(fields);
                }
                i++;
            }
        }

        private void DealMethods(MethodInfo[] methods)
        {
            foreach(MethodInfo method in methods)
            {
                Console.WriteLine("method " + method.ToString());
                object[] attributes = method.GetCustomAttributes(false);
                //foreach(object attr in attributes)
                //{
                //    Console.WriteLine("method attr " + attr);
                //}
            }
        }
        private void DealProperties(PropertyInfo[] propertyInfos)
        {
            foreach(PropertyInfo propertyInfo in propertyInfos)
            {
                Console.WriteLine("property " + propertyInfo.ToString());
                object[] attributes = propertyInfo.GetCustomAttributes(false);
                //foreach(object attr in attributes)
                //{
                //    Console.WriteLine("property attr " + attr);
                //}
            }
        }
        private void DealEvents(EventInfo[] eventInfos)
        {
            foreach(EventInfo eventInfo in eventInfos)
            {
                Console.WriteLine("eventInfo " + eventInfo.ToString());
                object[] attributes = eventInfo.GetCustomAttributes(false);
                //foreach(string attr in attributes)
                //{
                //    Console.WriteLine("eventInfo attr " + attr);
                //}
            }
        }
        private void DealFields(FieldInfo[] fieldInfos)
        {
            foreach(FieldInfo fieldInfo in fieldInfos)
            {
                Console.WriteLine("fieldInfo " + fieldInfo.Name);
            }
        }
        private void DealEnums(string[] enums)
        {
            foreach(string enumName in enums)
            {
                Console.WriteLine("enumName " + enumName);
            }
        }
    }
}
