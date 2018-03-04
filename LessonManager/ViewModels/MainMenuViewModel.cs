using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonManager.Commands;

namespace LessonManager.ViewModels
{
    class MainMenuViewModel
    {
        public MovePageCommand MovePageCommand { get; set; }

        public MainMenuViewModel()
        {
            MovePageCommand = new MovePageCommand();
        }
    }
}
