using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LessonManager.Commands;
using LessonManager.Models;

namespace LessonManager.ViewModels
{
    class RegisterLessonViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public RegisterLessonViewModel()
        {
            SetTakenAtNowCommand = new DelegateCommand();
            SetTakenAtNowCommand.ExecuteHandler = SetTakenAtNowCommandExecute;

            SearchCustomerCommand = new DelegateCommand();
            SearchCustomerCommand.ExecuteHandler = SearchCustomerCommandExecute;

            SearchCustomerByCardCommand = new DelegateCommand();
            SearchCustomerByCardCommand.ExecuteHandler = SearchCustomerByCardCommandExecute;

            Studios = Storage.GetInstance().Studios;
            Staffs = Storage.GetInstance().Staffs;

            Storage.GetInstance().PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "Studios")
                {
                    Studios = Storage.GetInstance().Studios;
                }
                if (e.PropertyName == "Staffs")
                {
                    Staffs = Storage.GetInstance().Staffs;
                }
            };
        }

        private DateTime takenAt_;
        public DateTime TakenAt
        {
            get { return takenAt_; }
            set
            {
                takenAt_ = value;
                RaisePropertyChanged();
            }
        }

        private ImmutableList<Studio> studios_;
        public ImmutableList<Studio> Studios
        {
            get { return studios_; }
            set
            {
                studios_ = value;
                RaisePropertyChanged();
            }
        }

        private ImmutableList<Staff> staffs_;
        public ImmutableList<Staff> Staffs
        {
            get { return staffs_; }
            set
            {
                staffs_ = value;
                RaisePropertyChanged();
            }
        }

        private Customer selectedCustomer_;
        public Customer SelectedCustomer
        {
            get { return selectedCustomer_; }
            set
            {
                selectedCustomer_ = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand SetTakenAtNowCommand { get; set; }
        private void SetTakenAtNowCommandExecute(object parameter)
        {
            TakenAt = DateTime.Now;
        }

        public DelegateCommand SearchCustomerCommand { get; set; }
        private async void SearchCustomerCommandExecute(object paramter)
        {
            var modal = new Views.Domain.SearchCustomerModal();
            object result = await MaterialDesignThemes.Wpf.DialogHost.Show(modal);
            SelectedCustomer = result as Customer;
        }

        public DelegateCommand SearchCustomerByCardCommand { get; set; }
        private async void SearchCustomerByCardCommandExecute(object paramter)
        {
            var modal = new Views.Domain.WaitCardModal();
            object result = await MaterialDesignThemes.Wpf.DialogHost.Show(modal);
            var cardID = result as string;
            if (cardID == null)
            {
                SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                return;
            }

            SelectedCustomer = Storage.GetInstance().Customers.Find((c) =>
            {
                return c.CardId == cardID;
            });
            if (SelectedCustomer == null)
            {
                SnackbarMessageQueue.Instance().Enqueue("該当する顧客が見つかりませんでした");
                return;
            }
        }
    }
}
