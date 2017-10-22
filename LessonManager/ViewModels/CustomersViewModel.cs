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

            AddCardCommand = new DelegateCommand();
            AddCardCommand.ExecuteHandler = AddCardCommandExecute;

            AddCreditCommand = new DelegateCommand();
            AddCreditCommand.ExecuteHandler = AddCreditCommandExecute;

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
                var view = new Views.Domain.ConfirmModal();
                view.DataContext = "顧客情報を更新します。\nよろしいですか？";

                object result = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
                if ((bool)result)
                {
                    UpdateCustomer(target);
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                }
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
        private void UpdateCustomer(Customer customer)
        {
            PleaseWaitVisibility.Instance().IsVisible = true;
            WebAPIs.Customer.Update(customer.ID, customer.Name, customer.Description).ContinueWith(t =>
            {
                PleaseWaitVisibility.Instance().IsVisible = false;
                var result = t.Result;
                if (result.IsSuccess)
                {
                    customer.ID = result.SuccessData.ID;
                    SnackbarMessageQueue.Instance().Enqueue("顧客情報を更新しました");
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("不明なエラー");
                }
            });
        }

        public DelegateCommand AddCardCommand { get; set; }
        private async void AddCardCommandExecute(object parameter)
        {
            var target = parameter as Customer;
            if (target.ID == 0)
            {
                SnackbarMessageQueue.Instance().Enqueue("先に顧客情報を保存してから実行してください");
                return;
            }

            if (target.CardId != "")
            {
                var confirm = new Views.Domain.ConfirmModal();
                confirm.DataContext = "既にカードが登録されています\n古いカードを登録解除して、新しいカードを登録しなおしますか？";

                object confirmResult = await MaterialDesignThemes.Wpf.DialogHost.Show(confirm);
                if (!(bool)confirmResult)
                {
                    SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                    return;
                }
            }

            var cardModal = new Views.Domain.WaitCardModal();
            object cardModalResult = await MaterialDesignThemes.Wpf.DialogHost.Show(cardModal);
            string cardID = cardModalResult as string;
            if (cardID == "")
            {
                SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
            }
            else
            {
                PleaseWaitVisibility.Instance().IsVisible = true;
                var result = await WebAPIs.Customer.SetCard(target.ID, cardID, target.Credit);
                PleaseWaitVisibility.Instance().IsVisible = false;
                if (result.IsSuccess)
                {
                    target.CardId = result.SuccessData.CardId;
                    target.Credit = result.SuccessData.Credit;
                    SnackbarMessageQueue.Instance().Enqueue("カードを登録しました");
                }
                else
                {
                    if (result.FailData.Body.ErrorType == Protobufs.ErrorType.AlreadyExist)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("そのカードは既に使われています");
                    }
                    else
                    {
                        SnackbarMessageQueue.Instance().Enqueue("不明なエラー");
                    }
                }
            }
        }

        public DelegateCommand AddCreditCommand { get; set; }
        private async void AddCreditCommandExecute(object parameter)
        {
            var target = parameter as Customer;
            if (target.ID == 0)
            {
                SnackbarMessageQueue.Instance().Enqueue("先に顧客情報を保存してから実行してください");
                return;
            }

            if (target.CardId == "")
            {
                SnackbarMessageQueue.Instance().Enqueue("先にカードを登録してから実行してください");
                return;
            }

            var addCreditModal = new Views.Domain.AddCreditModal();
            object addCreditModalResult = await MaterialDesignThemes.Wpf.DialogHost.Show(addCreditModal);
            int amount = System.Convert.ToInt32(addCreditModalResult);

            if (amount == 0)
            {
                SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                return;
            }

            { // 確認
                var confirm = new Views.Domain.ConfirmModal();
                confirm.DataContext = amount > 0
                    ? String.Format("クレジットを {0} 追加します。本当によろしいですか？", amount)
                    : String.Format("!!! クレジットを {0} 【減らします】。本当によろしいですか？ !!!", -amount);

                object confirmResult = await MaterialDesignThemes.Wpf.DialogHost.Show(confirm);
                if (!(bool)confirmResult)
                {
                    SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                    return;
                }
            }


            PleaseWaitVisibility.Instance().IsVisible = true;
            var result = await WebAPIs.Customer.AddCredit(target.ID, amount);
            PleaseWaitVisibility.Instance().IsVisible = false;
            if (result.IsSuccess)
            {
                target.Credit = result.SuccessData.Credit;
                if (amount > 0)
                {
                    SnackbarMessageQueue.Instance().Enqueue("クレジットを追加しました");
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("クレジットを減らしました");
                }
            }
            else
            {
                if (result.FailData.Body.ErrorType == Protobufs.ErrorType.CardNotRegistered) 
                {
                    SnackbarMessageQueue.Instance().Enqueue("先にカードを登録してから実行してください");
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("不明なエラー");
                }
            }
        }
    }
}
