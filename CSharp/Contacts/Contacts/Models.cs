using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contacts
{
    public class Book
    {
        public Book()
        {
            ContactBooks = new List<Group>();
        }
        public List<Group> ContactBooks { get; set; }
    }

    public class Group
    {
        public Group()
        {
            Contacts = new List<Person>();
        }
        public string GroupName { get; set; }
        public List<Person> Contacts { get; set; }
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
