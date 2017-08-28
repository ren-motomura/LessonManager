using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonManager.Commands;

namespace LessonManager.ViewModels
{
    class MainViewModel
    {
        public MainViewModel()
        {
            this.LoginCommand = new LoginCommand();
        }

        public LoginCommand LoginCommand { get; set; }
    }
}
