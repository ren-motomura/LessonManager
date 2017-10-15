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

        private int id_;
        public int ID
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

        private string cardId_;
        public string CardId
        {
            get { return cardId_; }
            set
            {
                if (value != cardId_)
                {
                    cardId_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private int credit_;
        public int Credit
        {
            get { return credit_; }
            set
            {
                if (value != credit_)
                {
                    credit_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        public Customer()
        {
            ID = 0;
            Name = "";
            Description = "";
            CardId = "";
            Credit = 0;
        }
    }
}
