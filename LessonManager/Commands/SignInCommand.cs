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
    class SignInCommand : ICommand
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
            var p = parameter as Parameter;
            PleaseWaitVisibility.Instance().IsVisible = true;
            WebAPIs.Session.Create(p.EmailAddress, p.Password).ContinueWith((t) =>
            {
                PleaseWaitVisibility.Instance().IsVisible = false;
                Result<Models.Company> result = t.Result;
                if (result.IsSuccess)
                {
                    Models.Company.SetCurrentCompany(result.SuccessData);
                }
                else
                {
                    if (result.FailData.Status == System.Net.HttpStatusCode.NotFound)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("メールアドレスが見つかりませんでした");
                    }
                    else if (result.FailData.Body.ErrorType == Protobufs.ErrorType.InvalidPassword)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("パスワードが間違っています");
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
