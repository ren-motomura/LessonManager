using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LessonManager.Models;

namespace LessonManager.Commands
{
    class MovePageCommand : ICommand
    {
        public MovePageCommand()
        {
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            string key = parameter as string;
            PageManager.Instance().SetCurrentPageByKey(key);
        }
    }
}
