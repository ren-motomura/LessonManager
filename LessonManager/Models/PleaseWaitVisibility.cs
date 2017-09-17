using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace LessonManager.Models
{
    class PleaseWaitVisibility : INotifyPropertyChanged
    {
        private static PleaseWaitVisibility instance_;
        public static PleaseWaitVisibility Instance()
        {
            return instance_ == null ? instance_ = new PleaseWaitVisibility()
                                     : instance_;
        }

        private PleaseWaitVisibility()
        {
            IsVisible = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool isVisible_;
        public bool IsVisible {
            get { return isVisible_; } 
            set
            {
                if (value != isVisible_)
                {
                    isVisible_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }
        public string Visibility
        {
            get {
                return IsVisible ? "Visible" : "Hidden";
            }
        }
    }
}
