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

            SearchFromNameOrDescriptionCommand = new DelegateCommand();
            SearchFromNameOrDescriptionCommand.ExecuteHandler = SearchFromNameOrDescriptionCommandExecute;

            SearchFromCardCommand = new DelegateCommand();
            SearchFromCardCommand.ExecuteHandler = SearchFromCardCommandExecute;

            SearchConditionRemoveCommand = new DelegateCommand();
            SearchConditionRemoveCommand.ExecuteHandler = SearchConditionRemoveCommandExecute;

            Customers = new List<Customer>().ToImmutableList();
            Storage.GetInstance().PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "Customers")
                {
                    Customers = Storage.GetInstance().Customers;
                }
            };
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
        private async void DeleteCustomerCommandExecute(object parameter)
        {
            var target = parameter as Customer;
            if (Customers.IndexOf(target) == -1)
                return;

            var view = new Views.Domain.ConfirmModal();
            view.DataContext = "顧客情報を削除します。この操作は取り消せません。\nよろしいですか？";

            object confirmResult = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
            if ((bool)confirmResult)
            {
                PleaseWaitVisibility.Instance().IsVisible = true;
                var result = await WebAPIs.Customer.Delete(target.ID);
                PleaseWaitVisibility.Instance().IsVisible = false;
                if (result.IsSuccess)
                {
                    Customers = Customers.Remove(target);
                    Storage.GetInstance().LoadCustomers();
                    SnackbarMessageQueue.Instance().Enqueue("顧客情報を削除しました");
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("不明なエラー");
                }
            }
            else
            {
                SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
            }
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
                    Storage.GetInstance().LoadCustomers();
                    SnackbarMessageQueue.Instance().Enqueue("顧客情報を新たに登録しました");
                }
                else
                {
                    if (result.FailData.Body.ErrorType == Protobufs.ErrorType.DuplicateNameExist)
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
                    if (result.FailData.Body.ErrorType == Protobufs.ErrorType.DuplicateNameExist)
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

        public DelegateCommand SearchFromNameOrDescriptionCommand { get; set; }
        private void SearchFromNameOrDescriptionCommandExecute(object parameter)
        {
            // TODO: だいぶ雑な作り
            var searchBox = parameter as Views.Domain.SearchBoxWithCard;
            string searchText = searchBox.SearchText;
            if (searchText == null || searchText == "")
            {
                return;
            }

            Customers = Storage.GetInstance().Customers;
            Customers = Customers.FindAll((Customer c) =>
            {
                return c.Name.Contains(searchText) || c.Description.Contains(searchText);
            });
        }

        public DelegateCommand SearchFromCardCommand { get; set; }
        private async void SearchFromCardCommandExecute(object parameter)
        {
            // TODO: だいぶ雑な作り
            var cardModal = new Views.Domain.WaitCardModal();
            object cardModalResult = await MaterialDesignThemes.Wpf.DialogHost.Show(cardModal);
            string cardID = cardModalResult as string;
            if (cardID == "")
            {
                SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                return;
            }

            var searchBox = parameter as Views.Domain.SearchBoxWithCard;
            searchBox.SearchText = cardID;

            Customers = Storage.GetInstance().Customers;
            Customers = Customers.FindAll((Customer c) =>
            {
                return c.CardId == cardID;
            });
        }

        public DelegateCommand SearchConditionRemoveCommand { get; set; }
        private void SearchConditionRemoveCommandExecute(object parameter)
        {
            // TODO: だいぶ雑な作り
            var searchBox = parameter as Views.Domain.SearchBoxWithCard;
            searchBox.SearchText = "";

            Customers = Storage.GetInstance().Customers;
        }
    }
}
