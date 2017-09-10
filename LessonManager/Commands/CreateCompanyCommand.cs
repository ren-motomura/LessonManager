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
    class CreateCompanyCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Models.Company company = parameter as Models.Company;
            WebAPIs.Company.Create(company.Name, company.EmailAddress, company.Password).ContinueWith((t) =>
            {
                string message;
                Result<Models.Company> result = t.Result;
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
