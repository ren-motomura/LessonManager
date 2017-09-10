using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LessonManager.WebAPIs;
using LessonManager.Models;

namespace LessonManager.Commands
{
    class SignOutCommand : ICommand
    {
        public class Parameter
        {
            public string EmailAddress { get; set; }
            public string Password { get; set; }
            public Parameter()
            {
                EmailAddress = "";
                Password = "";
            }
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Session.Remove();
            Models.Company.SetCurrentCompany(null);
        }
    }
}
