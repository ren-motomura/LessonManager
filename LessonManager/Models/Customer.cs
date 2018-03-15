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

        public class GenderDefinition
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
        public static List<GenderDefinition> GenderDefinitions = new List<GenderDefinition>
        {
            new GenderDefinition { Name = "未指定", Value = 0 },
            new GenderDefinition { Name = "男性", Value = 1 },
            new GenderDefinition { Name = "女性", Value = 2 },
        };

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

        private string kana_;
        public string Kana
        {
            get { return kana_; }
            set
            {
                if (value != kana_)
                {
                    kana_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private DateTime birthday_;
        public DateTime Birthday
        {
            get { return birthday_; }
            set
            {
                if (value != birthday_)
                {
                    birthday_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private int gender_;
        public int Gender
        {
            get { return gender_; }
            set
            {
                if (value != gender_)
                {
                    gender_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }
        public string GenderName
        {
            get
            {
                GenderDefinition gd_ = GenderDefinitions.Find(gd => gd.Value == Gender);
                return gd_?.Name;
            }
        }

        private string postalCode1_;
        public string PostalCode1
        {
            get { return postalCode1_; }
            set
            {
                if (value != postalCode1_)
                {
                    postalCode1_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private string postalCode2_;
        public string PostalCode2
        {
            get { return postalCode2_; }
            set
            {
                if (value != postalCode2_)
                {
                    postalCode2_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        public string PostalCode
        {
            get { return PostalCode1 + "-" + PostalCode2; }
        }

        private string address_;
        public string Address
        {
            get { return address_; }
            set
            {
                if (value != address_)
                {
                    address_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private string phoneNumber_;
        public string PhoneNumber
        {
            get { return phoneNumber_; }
            set
            {
                if (value != phoneNumber_)
                {
                    phoneNumber_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private DateTime joinDate_;
        public DateTime JoinDate
        {
            get { return joinDate_; }
            set
            {
                if (value != joinDate_)
                {
                    joinDate_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private string emailAddress_;
        public string EmailAddress
        {
            get { return emailAddress_; }
            set
            {
                if (value != emailAddress_)
                {
                    emailAddress_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private bool canMail_;
        public bool CanMail
        {
            get { return canMail_; }
            set
            {
                if (value != canMail_)
                {
                    canMail_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private bool canEmail_;
        public bool CanEmail
        {
            get { return canEmail_; }
            set
            {
                if (value != canEmail_)
                {
                    canEmail_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private bool canCall_;
        public bool CanCall
        {
            get { return canCall_; }
            set
            {
                if (value != canCall_)
                {
                    canCall_ = value;
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
