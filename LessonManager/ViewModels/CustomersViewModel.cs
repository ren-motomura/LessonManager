using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LessonManager.Models;
using LessonManager.Commands;

namespace LessonManager.ViewModels
{
    class CustomersViewModel : INotifyPropertyChanged
    {
        public CustomersViewModel()
        {
            var builder = ImmutableList.CreateBuilder<Customer>();
            builder.Add(new Customer
            {
                Id = 1,
                Name = "Name",
            });
            Customers = builder.ToImmutable();

            AddCustomerCommand = new DelegateCommand();
            AddCustomerCommand.ExecuteHandler = AddCustomerCommandExecute;

            DeleteCustomerCommand = new DelegateCommand();
            DeleteCustomerCommand.ExecuteHandler = DeleteCustomerCommandExecute;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        private ImmutableList<Customer> customers_;
        public ImmutableList<Customer> Customers
        {
            get { return customers_; }
            set
            {
                var orgCount = customers_ == null ? 0 : customers_.Count;
                var newCount = value == null ? 0 : value.Count;
                customers_ = value;
                if (newCount != orgCount) // 数が変わったときだけ発火する
                {
                    RaisePropertyChanged();
                }
            }
        }

        public DelegateCommand AddCustomerCommand { get; set; }
        private void AddCustomerCommandExecute(object parameter)
        {
            Customers = Customers.Add(new Customer());
        }

        public DelegateCommand DeleteCustomerCommand { get; set; }
        private void DeleteCustomerCommandExecute(object parameter)
        {
            var target = parameter as Customer;
            Customers = Customers.Remove(target);
        }
    }
}
