using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonManager.Commands;

namespace LessonManager.ViewModels
{
    class SigninViewModel
    {
        public class Parameter
        {
            public string Name { get; set; }
            public string EmailAddress { get; set; }
            public string Password { get; set; }
            public string Password2 { get; set; }
        }

        public SignInCommand.Parameter SignInParameter { get; set; }
        public SignUpCommand.Parameter SignUpParameter { get; set; }
        public Parameter ResetPasswordParameter { get; set; }
        public SignInCommand SignInCommand { get; set; }
        public SignUpCommand SignUpCommand { get; set; }

        public SigninViewModel()
        {
            SignInParameter = new SignInCommand.Parameter();
            SignUpParameter = new SignUpCommand.Parameter();
            ResetPasswordParameter = new Parameter();
            SignInCommand = new SignInCommand();
            SignUpCommand = new SignUpCommand();

            /*
            // for debug
            SignInParameter.EmailAddress = "sample@example.com";
            SignInParameter.Password = "password";

            // for debug
            SignUpParameter.Name = "Sample";
            SignUpParameter.EmailAddress = "sample@example.com";
            SignUpParameter.Password = "password";
            SignUpParameter.Password2 = "password";
            */
        }
    }
}
