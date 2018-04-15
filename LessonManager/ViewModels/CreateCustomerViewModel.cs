using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LessonManager.Commands;
using LessonManager.Models;

namespace LessonManager.ViewModels
{
    class CreateCustomerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public CreateCustomerViewModel()
        {
            MovePageCommand = new MovePageCommand();

            GenderDefinitions = Customer.GenderDefinitions;

            CreateCustomerCommand = new DelegateCommand();
            CreateCustomerCommand.ExecuteHandler = CreateCustomerCommandExecute;

            PickCardCommand = new DelegateCommand();
            PickCardCommand.ExecuteHandler = PickCardCommandExecute;

            ResetAllProperties();
        }

        // <commands>

        public MovePageCommand MovePageCommand { get; private set; }

        public DelegateCommand CreateCustomerCommand { get; private set; }
        private async void CreateCustomerCommandExecute(object parameter)
        {
            // validation
            if (Name == "")
            {
                SnackbarMessageQueue.Instance().Enqueue("名前を入力してください");
                return;
            }
            if (Kana == "")
            {
                SnackbarMessageQueue.Instance().Enqueue("かなを入力してください");
                return;
            }
            if (CardID == "")
            {
                SnackbarMessageQueue.Instance().Enqueue("カードを設定してください");
                return;
            }

            // confirmation
            {
                var view = new Views.Domain.ConfirmModal();
                view.DataContext = "お客様情報を登録します";

                object result = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
                if (!(bool)result)
                {
                    SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                    return;
                }
            }

            // 通信
            {
                PleaseWaitVisibility.Instance().IsVisible = true;
                var result = await WebAPIs.Customer.Create(
                    Name, Kana, Birthday, Gender, PostalCode1, PostalCode2, Address, PhoneNumber, JoinDate, EmailAddress, CanMail, CanEmail, CanCall, Description, CardID
                );
                PleaseWaitVisibility.Instance().IsVisible = false;
                if (result.IsSuccess)
                {
                    SnackbarMessageQueue.Instance().Enqueue("お客様情報を新たに登録しました");
                    ResetAllProperties();
                    return;
                }
                else
                {
                    if (result.FailData.Body.ErrorType == Protobufs.ErrorType.AlreadyExist)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("既に使われているカードです");
                        return;
                    }
                    SnackbarMessageQueue.Instance().Enqueue("不明なエラー");
                    return;
                }
            }
        }

        public DelegateCommand PickCardCommand { get; private set; }
        private async void PickCardCommandExecute(object parameter)
        {
            var cardModal = new Views.Domain.WaitCardModal();
            object cardModalResult = await MaterialDesignThemes.Wpf.DialogHost.Show(cardModal);
            CardID = cardModalResult as string;
        }

        // </commands>

        // <プルダウン表示用>

        public List<Customer.GenderDefinition> GenderDefinitions { get; set; }

        // </プルダウン表示用>

        // <customer properties>

        private string name_;
        public string Name
        {
            get { return name_; }
            set
            {
                name_ = value;
                RaisePropertyChanged();
            }
        }

        private string kana_;
        public string Kana
        {
            get { return kana_; }
            set
            {
                kana_ = value;
                RaisePropertyChanged();
            }
        }

        private string cardID_;
        public string CardID
        {
            get { return cardID_; }
            set
            {
                cardID_ = value;
                RaisePropertyChanged();
            }
        }

        private DateTime birthday_;
        public DateTime Birthday
        {
            get { return birthday_; }
            set
            {
                birthday_ = value;
                RaisePropertyChanged();
            }
        }

        private int gender_;
        public int Gender
        {
            get { return gender_; }
            set
            {
                gender_ = value;
                RaisePropertyChanged();
            }
        }

        private string postalCode1_;
        public string PostalCode1
        {
            get { return postalCode1_; }
            set
            {
                postalCode1_ = value;
                RaisePropertyChanged();
            }
        }

        private string postalCode2_;
        public string PostalCode2
        {
            get { return postalCode2_; }
            set
            {
                postalCode2_ = value;
                RaisePropertyChanged();
            }
        }

        private string address_;
        public string Address
        {
            get { return address_; }
            set
            {
                address_ = value;
                RaisePropertyChanged();
            }
        }

        private string phoneNumber_;
        public string PhoneNumber
        {
            get { return phoneNumber_; }
            set
            {
                phoneNumber_ = value;
                RaisePropertyChanged();
            }
        }

        private DateTime joinDate_;
        public DateTime JoinDate
        {
            get { return joinDate_; }
            set
            {
                joinDate_ = value;
                RaisePropertyChanged();
            }
        }

        private string emailAddress_;
        public string EmailAddress
        {
            get { return emailAddress_; }
            set
            {
                emailAddress_ = value;
                RaisePropertyChanged();
            }
        }

        private bool canMail_;
        public bool CanMail
        {
            get { return canMail_; }
            set
            {
                canMail_ = value;
                RaisePropertyChanged();
            }
        }

        private bool canEmail_;
        public bool CanEmail
        {
            get { return canEmail_; }
            set
            {
                canEmail_ = value;
                RaisePropertyChanged();
            }
        }

        private bool canCall_;
        public bool CanCall
        {
            get { return canCall_; }
            set
            {
                canCall_ = value;
                RaisePropertyChanged();
            }
        }

        private string description_;
        public string Description
        {
            get { return description_; }
            set
            {
                description_ = value;
                RaisePropertyChanged();
            }
        }

        // </customer properties>

        // <other methods>

        private void ResetAllProperties()
        {
            Name = "";
            Kana = "";
            CardID = "";
            Birthday = new DateTime(1970, 1, 1); 
            Gender = 0;
            PostalCode1 = "";
            PostalCode2 = "";
            Address = "";
            PhoneNumber = "";
            JoinDate = DateTime.Today;
            EmailAddress = "";
            CanMail = false;
            CanEmail = false;
            CanCall = false;
            Description = "";
        }

        // </other methods>
    }
}
