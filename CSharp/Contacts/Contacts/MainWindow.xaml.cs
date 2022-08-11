using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using GalaSoft.MvvmLight.CommandWpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

            this.DataContext = new MainWindowViewModel();
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = e.OriginalSource as DependencyObject;
            TreeViewItem item = GetDependencyObjectFromVisualTree(obj, typeof(TreeViewItem)) as TreeViewItem;
            if (item == null)
            {
                return;
            }
            if(item.Header is GroupViewModel)
            {
                Console.WriteLine("group");
            }
            else if (item.Header is PersonViewModel)
            {
                Console.WriteLine("Person");
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
            if (item.Header is GroupViewModel)
            {
                Console.WriteLine("group");
            }
            else if (item.Header is PersonViewModel)
            {
                Console.WriteLine("Person");
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

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

    public class GroupViewModel : INotifyPropertyChanged
    {
        public GroupViewModel()
        {
            Members = new ObservableCollection<PersonViewModel>();
        }
        private string _name;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }
        public ObservableCollection<PersonViewModel> Members { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class PersonViewModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }
        private string _number;
        public string Number { get { return _number; } set { _number = value; OnPropertyChanged(); } }
        private bool _gender;
        public bool Gender { get { return _gender; } set { _gender = value; OnPropertyChanged(); } }
        private string _birthday;
        public string Birthday { get { return _birthday; } set { _birthday = value; OnPropertyChanged(); } }
        private string _avatar;
        public string Avatar { get { return _avatar; } set { _avatar = value; OnPropertyChanged(); } }
        private string _email;
        public string Email { get { return _email; } set { _email = value; OnPropertyChanged(); } }
        private string _notes;
        public string Notes { get { return _notes; } set { _notes = value; OnPropertyChanged(); } }
        public bool Visible { get; set; }

        public Person Model
        {
            get
            {
                return new Person()
                {
                    Name = this.Name,
                    Number = this.Number,
                    Gender = this.Gender,
                    Birthday = this.Birthday,
                    Avatar = this.Avatar,
                    Email = this.Email,
                    Notes = this.Notes,
                };
            }
            set
            {
                Name = value.Name;
                Number = value.Number;
                Gender = value.Gender;
                Birthday = value.Birthday;
                Avatar = value.Avatar;
                Email = value.Email;
                Notes = value.Notes;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {

        public RelayCommand AddGroupCommand { get; set; }
        public RelayCommand DeleteGroupCommand { get; set; }
        public RelayCommand AddPersonCommand { get; set; }
        public RelayCommand DeletePersonCommand { get; set; }

        public MainWindowViewModel()
        {
            AddGroupCommand = new RelayCommand(AddGroup);
        }
        private void AddGroup()
        {
            Groups.Add(new GroupViewModel()
            {
                Name = "test add"
            });
        }

        private string myString = "Overtype this text";
        public string MyString
        {
            get { return myString; }
            set
            {
                myString = value;
                RaisePropertyChanged();  // implementation of INotifyPropertyChanged is from ViewModelBase
            }
        }

        public ObservableCollection<GroupViewModel> Groups { get; set; } = new()
        {
            new GroupViewModel() {
                Name = "test 1",
                Members = new ObservableCollection<PersonViewModel>()
                {
                    new PersonViewModel()
                    {
                        Name = "test child_1"
                    },
                    new PersonViewModel()
                    {
                        Name = "test child_2"
                    }
                }
            }
        };
    }
}
