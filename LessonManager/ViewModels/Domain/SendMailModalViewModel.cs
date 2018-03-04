using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LessonManager.Models;

namespace LessonManager.ViewModels.Domain
{
    class SendMailModalViewModel : INotifyPropertyChanged
    {
        public SendMailModalViewModel()
        {
            Mail = new Mail();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        private Mail mail_;
        public Mail Mail
        {
            get { return mail_; }
            set
            {
                mail_ = value;
                RaisePropertyChanged();
            }
        }
    }
}
