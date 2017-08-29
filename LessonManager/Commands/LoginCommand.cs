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
            WebAPIs.Session.Create("sample@example.com", "password").ContinueWith((t) =>
            {
                string message;
                Result<Boolean> result = t.Result;
                if (result.IsSuccess)
                {
                    message = result.SuccessData.ToString();
                }
                else
                {
                    var failData = result.FailData;
                    message = "Fail! Status: " + failData.Status.ToString() + ", ErrorType: " + failData.Body.ErrorType.ToString();
                }
                MessageBox.Show(message);
            });
        }
    }
}
