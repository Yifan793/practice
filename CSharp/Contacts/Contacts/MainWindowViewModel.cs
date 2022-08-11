using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.CommandWpf;

namespace Contacts
{
    //public class MainWindowViewModel : INotifyPropertyChanged
    //{
    //    private string _myString = "Some text from MainWindowViewModel";
    //    public string MyString
    //    {
    //        get { return _myString; }
    //        set
    //        {
    //            _myString = value;
    //            NotifyPropertyChanged();
    //        }
    //    }

    //    public ICommand ChangeText { get; set; }
    //    public MainWindowViewModel()
    //    {
    //        ChangeText = new ChangeTextCommand { mwv = this };
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;
    //    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    //    {
    //        if (PropertyChanged != null)
    //        {
    //            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    //        }
    //    }
    //}

    public class GroupNodeItem
    {
        public GroupNodeItem()
        {
            Members = new ObservableCollection<PersonNodeItem>();
        }
        public string Name { get; set; }
        public ObservableCollection<PersonNodeItem> Members { get; set; }
    }

    public class PersonNodeItem
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public bool Gender { get; set; }
        public string Birthday { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
    }

    public class MainWindowViewModel : ViewModelBase
    {

        public RelayCommand AddGroupCommand { get; set; }
        public RelayCommand DeleteGroupCommand { get; set; }
        public RelayCommand AddPersonComamnd { get; set; }
        public RelayCommand DeletePersonCommand { get; set; }

        public MainWindowViewModel ()
        {
            AddGroupCommand = new RelayCommand(AddGroup);
        }
        private void AddGroup()
        {
            Groups.Add(new GroupNodeItem()
            {
                Name = "test add"
            });
            //RaisePropertyChanged("Groups");
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

        public ObservableCollection<GroupNodeItem> Groups { get; set; } = new()
        {
            new GroupNodeItem() {
                Name = "test 1",
                Members = new ObservableCollection<PersonNodeItem>()
                {
                    new PersonNodeItem()
                    {
                        Name = "test child_1"
                    },
                    new PersonNodeItem() 
                    { 
                        Name = "test child_2"
                    }
                }
            }
        };
    }
}
