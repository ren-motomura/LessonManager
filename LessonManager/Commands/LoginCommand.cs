using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LessonManager.Models;
using LessonManager.WebAPIs;

namespace LessonManager.Commands
{
    class LoginCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var task = WebAPIs.User.Create("sample太郎", "sample@example.com", "password");
            task.Wait(); task.Result.Wait();
            var user = task.Result.Result;

            var message = "ID: " + user.Id.ToString() + ", Name: " + user.Name + ", EmailAddress: " + user.EmailAddress;

            MessageBox.Show(message);
        }
    }
}
