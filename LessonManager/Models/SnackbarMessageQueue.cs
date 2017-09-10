using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonManager.Models
{
    class SnackbarMessageQueue : MaterialDesignThemes.Wpf.SnackbarMessageQueue
    {
        private static SnackbarMessageQueue instance_;
        public static SnackbarMessageQueue Instance()
        {
            return instance_ == null ?
                instance_ = new SnackbarMessageQueue() :
                instance_;
        }

        private SnackbarMessageQueue()
        {
        }

    }
}
