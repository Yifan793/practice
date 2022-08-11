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
using Newtonsoft.Json;

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
            this.DataContext = ViewModel;
            ViewModel.Model = Resource.LoadResource("./Resources/data.json");
        }

        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

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
    }

    public class GroupViewModel : INotifyPropertyChanged
    {
        public GroupViewModel()
        {
            Contacts = new ObservableCollection<PersonViewModel>();
        }
        private string _groupname;
        public string GroupName { get { return _groupname; } set { _groupname = value; OnPropertyChanged(); } }
        public ObservableCollection<PersonViewModel> Contacts { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Group Model
        {
            get
            {
                Group group = new Group();
                group.GroupName = this.GroupName;
                ObservableCollection<Person> people = new ObservableCollection<Person>();
                foreach (PersonViewModel personViewModel in Contacts)
                {
                    Person person = personViewModel.Model;
                    people.Add(person);
                }
                group.Contacts = people;
                return group;
            }
            set
            {
                GroupName = value.GroupName;
                ObservableCollection<PersonViewModel> peopleViewModel = new ObservableCollection<PersonViewModel>();
                foreach(Person person in value.Contacts)
                {
                    PersonViewModel personViewModel = new PersonViewModel();
                    personViewModel.Model = person;
                    peopleViewModel.Add(personViewModel);
                }
                Contacts = peopleViewModel;
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
        public MainWindowViewModel()
        {
            AddGroupCommand = new RelayCommand(AddGroup);
        }
        private void AddGroup()
        {
            Groups.Add(new GroupViewModel()
            {
                GroupName = "test add"
            });
        }

        public ObservableCollection<GroupViewModel> Groups { get; set; }
        public Book Model
        {
            get
            {
                Book book = new Book();
                ObservableCollection<Group> groups = new ObservableCollection<Group>();
                foreach(GroupViewModel groupViewModel in Groups )
                {
                    Group group = groupViewModel.Model;
                    groups.Add(group);
                }
                book.ContactBooks = groups;
                return book;
            }
            set
            {
                ObservableCollection<Group> groups = value.ContactBooks;
                ObservableCollection<GroupViewModel> groupsViewModel = new ObservableCollection<GroupViewModel>();
                foreach(Group group in groups)
                {
                    GroupViewModel groupViewModel = new GroupViewModel();
                    groupViewModel.Model = group;
                    groupsViewModel.Add(groupViewModel);
                }
                Groups = groupsViewModel;
            }
        }
    }
}
