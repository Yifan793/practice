using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts
{
    public class Group
    {
        public string Name { get; set; }
        public ObservableCollection<Person> Members { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }

        public string Number { get; set; }
        public bool Gender { get; set; }
        public string Birthday { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
    }
}
