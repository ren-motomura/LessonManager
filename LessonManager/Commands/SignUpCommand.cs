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
    class SignUpCommand : ICommand
    {
        public class Parameter
        {
            public string Name { get; set; }
            public string EmailAddress { get; set; }
            public string Password { get; set; }
            public string Password2 { get; set; }
            public Parameter()
            {
                Name = "";
                EmailAddress = "";
                Password = "";
                Password2 = "";
            }
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var p = parameter as Parameter;
            if (p.Password != p.Password2)
            {
                SnackbarMessageQueue.Instance().Enqueue("パスワードが一致していません");
                return;
            }

            WebAPIs.Company.Create(p.Name, p.EmailAddress, p.Password).ContinueWith((t) =>
            {
                Result<Models.Company> result = t.Result;
                if (result.IsSuccess)
                {
                    Models.Company.SetCurrentCompany(result.SuccessData);
                }
                else
                {
                    if (result.FailData.Body.ErrorType == Protobufs.ErrorType.AlreadyExist)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("そのメールアドレスは既に使わrています");
                    }
                    else
                    {
                        SnackbarMessageQueue.Instance().Enqueue("不明なエラー");
                    }
                }
            });
        }
    }
}
