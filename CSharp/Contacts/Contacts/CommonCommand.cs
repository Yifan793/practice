using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Contacts
{
    internal class ChangeTextCommand : ICommand
    {
        public MainWindowViewModel mwv;
        private int i = 0;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            i++;
            mwv.MyString = "clicked" + i.ToString();
        }
    }

}
