using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace LessonManager.Models
{
    public class Company : INotifyPropertyChanged
    {
        private static Company currentCompany_;
        public static event Action<Company> ChangeCurrentCompanyEvent;

        public static Company GetCurrentCompany()
        {
            return currentCompany_;
        }

        public static void SetCurrentCompany(Company company)
        {
            if (company != currentCompany_)
            {
                currentCompany_ = company;
                ChangeCurrentCompanyEvent?.Invoke(currentCompany_);
            }
        }
        public static bool IsSignedIn()
        {
            return currentCompany_ != null;
        }

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

        private string password_;
        public string Password
        {
            get { return password_; }
            set
            {
                if (value != password_)
                {
                    password_ = value;
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

        private string imageLink_;
        public string ImageLink
        {
            get { return imageLink_; }
            set
            {
                if (value != imageLink_)
                {
                    imageLink_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }
    }
}
