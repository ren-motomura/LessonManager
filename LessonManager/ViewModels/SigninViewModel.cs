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
        public Parameter SignUpParameter { get; set; }
        public Parameter ResetPasswordParameter { get; set; }
        public SignInCommand SignInCommand { get; set; }

        public SigninViewModel()
        {
            SignInParameter = new SignInCommand.Parameter();
            SignUpParameter = new Parameter();
            ResetPasswordParameter = new Parameter();
            SignInCommand = new SignInCommand();
        }
    }
}
