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
using LitJson;
using System.IO;
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
            LoadResources("./Resources/data.json");
        }

        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        public void LoadResources(string path)
        {
            ObservableCollection<Group> groups = new ObservableCollection<Group>();
            JsonData data = JsonMapper.ToObject(File.ReadAllText(path));
            JsonData contactBooks  = data["ContactBooks"];
            for (int i = 0; i < contactBooks.Count; i++)
            {
                Group group = new Group();
                group.Name = contactBooks[i]["GroupName"].ToString();
                JsonData contacts = contactBooks[i]["Contacts"];
                for (int j = 0; j < contacts.Count; j++)
                {
                    Person person = new Person();
                    PersonViewModel personViewModel = new PersonViewModel();
                    person.Name = contacts[j]["Name"]?.ToString();
                    person.Number = contacts[j]["Number"]?.ToString();
                    person.Gender = (bool)contacts[j]["Gender"];
                    person.Birthday = contacts[j]["Birthday"]?.ToString();
                    person.Avatar = contacts[j]["Avatar"]?.ToString();
                    person.Email = contacts[j]["Email"]?.ToString();
                    person.Notes = contacts[j]["Notes"]?.ToString();

                    personViewModel.Model = person;
                    group.Members.Add(person);
                }
                groups.Add(group);
            }
            ViewModel.Model = groups;
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

        public Group Model
        {
            get
            {
                Group group = new Group();
                group.Name = this.Name;
                ObservableCollection<Person> people = new ObservableCollection<Person>();
                foreach (PersonViewModel personViewModel in Members)
                {
                    Person person = personViewModel.Model;
                    people.Add(person);
                }
                group.Members = people;
                return group;
            }
            set
            {
                Name = value.Name;
                ObservableCollection<PersonViewModel> peopleViewModel = new ObservableCollection<PersonViewModel>();
                foreach(Person person in value.Members)
                {
                    PersonViewModel personViewModel = new PersonViewModel();
                    personViewModel.Model = person;
                    peopleViewModel.Add(personViewModel);
                }
                Members = peopleViewModel;
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
                Name = "test add"
            });
        }

        public ObservableCollection<GroupViewModel> Groups { get; set; }
        public ObservableCollection<Group> Model
        {
            get
            {
                ObservableCollection<Group> groups = new ObservableCollection<Group>();
                foreach(GroupViewModel groupViewModel in Groups )
                {
                    Group group = groupViewModel.Model;
                    groups.Add(group);
                }
                return groups;
            }
            set
            {
                ObservableCollection<GroupViewModel> groupsViewModel = new ObservableCollection<GroupViewModel>();
                foreach(Group group in value)
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
