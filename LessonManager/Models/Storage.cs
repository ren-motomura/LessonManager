using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace LessonManager.Models
{
    class Storage : INotifyPropertyChanged
    {
        private Storage()
        {
            LoadAll();
            Models.Company.ChangeCurrentCompanyEvent += (c) =>
            {
                LoadAll();
            };
        }

        private static Storage instance_;
        public static Storage GetInstance()
        {
            if (instance_ == null)
            {
                instance_ = new Storage();
            }
            return instance_;
        }

        public async Task LoadAll()
        {
            await Task.Run(() =>
            {
                Task t1 = LoadCustomers();
                Task t2 = LoadStaffs();
                Task t3 = LoadStudios();
                Task.WaitAll(t1, t2, t3);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string entityName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(entityName));
        }

        private ImmutableList<Customer> customers_;
        public ImmutableList<Customer> Customers {
            get { return customers_; }
            set
            {
                customers_ = value;
                RaisePropertyChanged("Customers");
            }
        }

        private ImmutableList<Staff> staffs_;
        public ImmutableList<Staff> Staffs {
            get { return staffs_; }
            set
            {
                staffs_ = value;
                RaisePropertyChanged("Staffs");
            }
        }

        private ImmutableList<Studio> studios_;
        public ImmutableList<Studio> Studios {
            get { return studios_; }
            set
            {
                studios_ = value;
                RaisePropertyChanged("Studios");
            }
        }

        public async Task LoadCustomers()
        {
            if (!Models.Company.IsSignedIn())
            {
                Customers = new List<Customer>().ToImmutableList();
                return;
            }

            var result = await WebAPIs.Customer.GetAll();
            if (result.IsSuccess)
            {
                Customers = result.SuccessData.ToImmutableList();
            }
            else
            {
                // TODO
            }
        }

        public async Task LoadStaffs()
        {
            if (!Models.Company.IsSignedIn())
            {
                Staffs = new List<Staff>().ToImmutableList();
                return;
            }

            var result = await WebAPIs.Staff.GetAll();
            if (result.IsSuccess)
            {
                Staffs = result.SuccessData.ToImmutableList();
            }
            else
            {
                // TODO
            }
        }

        public async Task LoadStudios()
        {
            if (!Models.Company.IsSignedIn())
            {
                Studios = new List<Studio>().ToImmutableList();
                return;
            }

            var result = await WebAPIs.Studio.GetAll();
            if (result.IsSuccess)
            {
                Studios = result.SuccessData.ToImmutableList();
            }
            else
            {
                // TODO
            }
        }
    }
}
