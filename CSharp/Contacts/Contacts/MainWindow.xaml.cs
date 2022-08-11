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
using Caliburn.Micro;

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

        /**
         * @brief 左键点击树
         */
        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = e.OriginalSource as DependencyObject;
            TreeViewItem item = GetDependencyObjectFromVisualTree(obj, typeof(TreeViewItem)) as TreeViewItem;
            if (item == null)
            {
                return;
            }
            ViewModel.CurViewModel = (ViewModelBase)item.DataContext;
            if (item.Header is GroupViewModel)
            {
                ViewModel.ClickPerson = false;
            }
            else if (item.Header is PersonViewModel)
            {
                ViewModel.ClickPerson = true;
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

        /**
         * @brief 右键添加按钮，添加分组
         */
        private void OnAddGroupButtonClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Groups.Add(new GroupViewModel()
            {
                GroupName = "新分组"
            });
        }

        /**
         * @brief 右键删除按钮，删除分组
         */
        private void OnDeleteGroupButtonClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurViewModel == null)
            {
                MessageBox.Show("请选中一个分组", "", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.Yes);
                return;
            }
            if (ViewModel.CurViewModel is GroupViewModel group)
            {
                ViewModel.Groups.Remove(group);
            }
        }

        /**
         * @brief 点击Add按钮，添加用户
         */
        private void OnAddPersonButtonClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurViewModel == null)
            {
                MessageBox.Show("请选中一个分组", "", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.Yes);
                return;
            }
            if (ViewModel.CurViewModel is GroupViewModel group)
            {
                group.Contacts.Add(new PersonViewModel()
                {
                    Name = "未命名",
                    ParentGroup = group
                });
            }
            else if (ViewModel.CurViewModel is PersonViewModel person)
            {
                GroupViewModel parentGroup = person.ParentGroup;
                parentGroup.Contacts.Add(new PersonViewModel()
                {
                    Name = "未命名",
                    ParentGroup = parentGroup
                });
            }
        }

        /**
         * @brief 点击Delete按钮，删除用户
         */
        private void OnDeletePersonButtonClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.CurViewModel == null)
            {
                return;
            }
            if (ViewModel.CurViewModel is PersonViewModel person)
            {
                GroupViewModel parentGroup = person.ParentGroup;
                parentGroup.Contacts.Remove(person);
            }
        }
    }

    public class GroupViewModel : ViewModelBase
    {
        public GroupViewModel()
        {
            Contacts = new ObservableCollection<PersonViewModel>();
        }
        private string _groupname;
        public string GroupName { 
            get 
            { 
                return _groupname; 
            } 
            set 
            {
                if (value == _groupname)
                {
                    return;
                }
                _groupname = value; 
                RaisePropertyChanged(); 
            } 
        }
        public ObservableCollection<PersonViewModel> Contacts { get; set; }

        public Group Model
        {
            get
            {
                Group group = new Group();
                group.GroupName = this.GroupName;
                List<Person> people = new List<Person>();
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
                foreach (Person person in value.Contacts)
                {
                    PersonViewModel personViewModel = new PersonViewModel();
                    personViewModel.Model = person;
                    personViewModel.ParentGroup = this;
                    peopleViewModel.Add(personViewModel);
                }
                Contacts = peopleViewModel;
            }
        }
    }

    public class PersonViewModel : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value == _name)
                {
                    return;
                }
                _name = value;
                RaisePropertyChanged();
            }
        }
        private string _number;
        public string Number 
        { 
            get 
            { 
                return _number; 
            } 
            set 
            {
                if (value == _number)
                {
                    return;
                }
                _number = value; 
                RaisePropertyChanged(); 
            } 
        }
        private bool _gender;
        public bool Gender { 
            get 
            { 
                return _gender; 
            } 
            set 
            {
                if (value == _gender)
                {
                    return;
                }
                _gender = value; 
                RaisePropertyChanged(); 
            } 
        }
        private string _birthday;
        public string Birthday { 
            get 
            { 
                return _birthday; 
            } 
            set 
            {
                if (value == _birthday)
                {
                    return;
                }
                _birthday = value; 
                RaisePropertyChanged(); 
            } 
        }
        private string _avatar;
        public string Avatar 
        { 
            get 
            {
                return _avatar; 
            } 
            set 
            {
                if (value == _avatar)
                {
                    return;
                }
                _avatar = value; 
                RaisePropertyChanged(); 
            } 
        }
        private string _email;
        public string Email 
        { 
            get 
            { 
                return _email; 
            } 
            set 
            {
                if (value == _email)
                {
                    return;
                }
                _email = value; 
                RaisePropertyChanged(); 
            } 
        }
        private string _notes;
        public string Notes 
        { 
            get 
            { 
                return _notes; 
            } 
            set 
            {
                if (value == _notes)
                {
                    return;
                }
                _notes = value; 
                RaisePropertyChanged(); 
            } 
        }
        public GroupViewModel ParentGroup { get; set; }

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
    }

    public class MainWindowViewModel : ViewModelBase
    {
        private bool _clickperson = false;
        public bool ClickPerson
        { 
            get
            {
                return _clickperson;
            }
            set
            {
                if (value == _clickperson)
                {
                    return;
                }
                _clickperson = value;
                RaisePropertyChanged();
            }
        }
        private ViewModelBase _curViewModel;
        public ViewModelBase CurViewModel 
        { 
            get
            {
                return _curViewModel;
            }
            set
            {
                if (_curViewModel == value)
                {
                    return;
                }
                _curViewModel = value;
                RaisePropertyChanged();
            } 
        }
        public ObservableCollection<GroupViewModel> Groups { get; set; }
        public Book Model
        {
            get
            {
                Book book = new Book();
                List<Group> groups = new List<Group>();
                foreach (GroupViewModel groupViewModel in Groups)
                {
                    Group group = groupViewModel.Model;
                    groups.Add(group);
                }
                book.ContactBooks = groups;
                return book;
            }
            set
            {
                List<Group> groups = value.ContactBooks;
                ObservableCollection<GroupViewModel> groupsViewModel = new ObservableCollection<GroupViewModel>();
                foreach (Group group in groups)
                {
                    GroupViewModel groupViewModel = new GroupViewModel();
                    groupViewModel.Model = group;
                    groupsViewModel.Add(groupViewModel);
                }
                Groups = groupsViewModel;
            }
        }
    }

    public class NegateBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    public class UrlToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new BitmapImage(new Uri(value.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
