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
            Customers = builder.ToImmutable();

            AddCustomerCommand = new DelegateCommand();
            AddCustomerCommand.ExecuteHandler = AddCustomerCommandExecute;

            DeleteCustomerCommand = new DelegateCommand();
            DeleteCustomerCommand.ExecuteHandler = DeleteCustomerCommandExecute;

            CreateOrUpdateCustomerCommand = new DelegateCommand();
            CreateOrUpdateCustomerCommand.ExecuteHandler = CreateOrUpdateCustomerCommandExecute;

            LoadCustomers();
            Models.Company.ChangeCurrentCompanyEvent += (c) =>
            {
                LoadCustomers();
            };
        }

        public void LoadCustomers()
        {
            if (!Models.Company.IsSignedIn())
            {
                Customers = new List<Customer>().ToImmutableList();
                return;
            }

            WebAPIs.Customer.GetAll().ContinueWith(t =>
            {
                var result = t.Result;
                if (result.IsSuccess)
                {
                    Customers = result.SuccessData.ToImmutableList();
                }
                else
                {
                    // TODO
                    SnackbarMessageQueue.Instance().Enqueue(String.Format("失敗したみたい {0:D}", result.FailData.Status));
                }
            });
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

        public DelegateCommand CreateOrUpdateCustomerCommand { get; set; }
        private async void CreateOrUpdateCustomerCommandExecute(object parameter)
        {
            var target = parameter as Customer;
            if (target.ID == 0)
            {
                // Create
                var view = new Views.Domain.ConfirmModal();
                view.DataContext = "顧客情報を登録します。\nよろしいですか？";

                object result = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
                if ((bool)result)
                {
                    CreateCustomer(target);
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                }
            }
            else
            {
                // Update
            }
        }
        private void CreateCustomer(Customer customer)
        {
            PleaseWaitVisibility.Instance().IsVisible = true;
            WebAPIs.Customer.Create(customer.Name, customer.Description).ContinueWith(t =>
            {
                PleaseWaitVisibility.Instance().IsVisible = false;
                var result = t.Result;
                if (result.IsSuccess)
                {
                    customer.ID = result.SuccessData.ID;
                    SnackbarMessageQueue.Instance().Enqueue("顧客情報を新たに登録しました");
                }
                else
                {
                    if (result.FailData.Body.ErrorType == Protobufs.ErrorType.AlreadyExist)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("その名前は既に使われています");
                    }
                    else
                    {
                        SnackbarMessageQueue.Instance().Enqueue("不明なエラー");
                    }
                }
            });
        }
    }
}
