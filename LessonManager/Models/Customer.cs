using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace LessonManager.Models
{
    class Customer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private long id_;
        public long Id
        {
            get { return id_; }
            set
            {
                if (value != id_)
                {
                    id_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private string name_;
        public string Name
        {
            get { return name_; }
            set
            {
                if (value != name_)
                {
                    name_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private string description_;
        public string Description
        {
            get { return description_; }
            set
            {
                if (value != description_)
                {
                    description_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private DateTime createdAt_;
        public DateTime CreatedAt
        {
            get { return createdAt_; }
            set
            {
                if (value != createdAt_)
                {
                    createdAt_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }
    }
}
