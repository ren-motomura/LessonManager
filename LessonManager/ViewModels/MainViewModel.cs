using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonManager.Commands;
using LessonManager.Models;

namespace LessonManager.ViewModels
{
    class MainViewModel
    {
        public MainViewModel()
        {
            this.LoginCommand = new LoginCommand();
            this.Company = new Company();
        }

        public LoginCommand LoginCommand { get; set; }
        public Company Company { get; set; }
    }
}
