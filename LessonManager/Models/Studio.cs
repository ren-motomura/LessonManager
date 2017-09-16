using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace LessonManager.Models
{
    class Studio : INotifyPropertyChanged
    {
        private int id_;
        public int ID
        {
            get { return id_;  }
            set
            {
                if (value != id_)
                {
                    id_ = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string name_;
        public string Name
        {
            get { return name_;  }
            set
            {
                if (value != name_)
                {
                    name_ = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string address_;
        public string Address
        {
            get { return address_;  }
            set
            {
                if (value != address_)
                {
                    address_ = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string phoneNumber_;
        public string PhoneNumber
        {
            get { return phoneNumber_;  }
            set
            {
                if (value != phoneNumber_)
                {
                    phoneNumber_ = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string imageLink_;
        public string ImageLink
        {
            get { return imageLink_;  }
            set
            {
                if (value != imageLink_)
                {
                    imageLink_ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public Studio()
        {
            id_ = 0;
            name_ = "";
            address_ = "";
            phoneNumber_ = "";
            imageLink_ = "";
        }
    }
}
