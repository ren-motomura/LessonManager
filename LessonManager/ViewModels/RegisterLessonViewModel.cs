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

            RegisterLessonCommand = new DelegateCommand();
            RegisterLessonCommand.ExecuteHandler = RegisterLessonCommandExecute;

            Studios = Storage.GetInstance().Studios;
            Staffs = Storage.GetInstance().Staffs;
            PaymentTypes = new List<PaymentTypeForView>() {
                new PaymentTypeForView() {
                    Name = "現金",
                    Value = Lesson.PType.Cash,
                },
                new PaymentTypeForView() {
                    Name = "カード",
                    Value = Lesson.PType.Card,
                },
            }.ToImmutableList();

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

            ResetAll();
        }

        private void ResetAll()
        {
            TakenAt = DateTime.Now;
            SelectedStudio = null;
            SelectedStaff = null;
            SelectedCustomer = null;
            Fee = 0;
            SelectedPaymentType = Lesson.PType.Cash;
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

        public class PaymentTypeForView
        {
            public string Name { get; set; }
            public Lesson.PType Value { get; set; }
        }

        private ImmutableList<PaymentTypeForView> paymentTypes_;
        public ImmutableList<PaymentTypeForView> PaymentTypes
        {
            get { return paymentTypes_; }
            set
            {
                paymentTypes_ = value;
                RaisePropertyChanged();
            }
        }

        private Studio selectedStudio_;
        public Studio SelectedStudio
        {
            get { return selectedStudio_; }
            set
            {
                selectedStudio_ = value;
                RaisePropertyChanged();
            }
        }

        private Staff selectedStaff_;
        public Staff SelectedStaff
        {
            get { return selectedStaff_; }
            set
            {
                selectedStaff_ = value;
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

        private int fee_;
        public int Fee
        {
            get { return fee_; }
            set
            {
                fee_ = value;
                RaisePropertyChanged();
            }
        }

        private Lesson.PType selectedPaymentType_;
        public Lesson.PType SelectedPaymentType
        {
            get { return selectedPaymentType_; }
            set
            {
                selectedPaymentType_ = value;
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

        public DelegateCommand RegisterLessonCommand { get; set; }
        private async void RegisterLessonCommandExecute(object paramter)
        {
            // validation
            if (SelectedStudio == null)
            {
                SnackbarMessageQueue.Instance().Enqueue("スタジオを選択してください");
                return;
            }
            if (SelectedStaff == null)
            {
                SnackbarMessageQueue.Instance().Enqueue("スタッフを選択してください");
                return;
            }
            if (SelectedCustomer == null)
            {
                SnackbarMessageQueue.Instance().Enqueue("顧客を選択してください");
                return;
            }
            if (Fee <= 0)
            {
                SnackbarMessageQueue.Instance().Enqueue("料金には0より大きい値を設定してください");
                return;
            }
            if (SelectedPaymentType == Lesson.PType.Card && SelectedCustomer.CardId == "")
            {
                SnackbarMessageQueue.Instance().Enqueue("カードが登録されていません。先にカードを登録するか、現金支払を選択してください");
                return;
            }
            if (SelectedPaymentType == Lesson.PType.Card && SelectedCustomer.Credit < Fee)
            {
                SnackbarMessageQueue.Instance().Enqueue("カード残高が不足しています。先に残高を追加してください");
                return;
            }

            // confirmation
            {
                string paymentTypeString = PaymentTypes.Find((pt) =>
                {
                    return pt.Value == SelectedPaymentType;
                }).Name;
                var view = new Views.Domain.ConfirmModal();
                view.DataContext = String.Format(@"以下の内容でレッスンを登録します。

スタジオ：{0}
スタッフ：{1}
顧客：{2}
料金：{3:C}
支払い方法：{4}

支払い方法がカードの場合は自動的に残高が引き落とされますが、現金の場合は先に料金を受け取ってください。
内容に問題が無ければOKを、修正が必要な場合はCANCELを押してください。", SelectedStudio.Name, SelectedStaff.Name, SelectedCustomer.Name, Fee, paymentTypeString);

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
                var result = await WebAPIs.Lesson.Register(SelectedStudio.ID, SelectedStaff.ID, SelectedCustomer.ID, Fee, SelectedPaymentType, TakenAt);
                PleaseWaitVisibility.Instance().IsVisible = false;
                if (result.IsSuccess)
                {
                    if (SelectedPaymentType == Lesson.PType.Card)
                    {
                        // カード残高が変わっているので
                        Storage.GetInstance().LoadCustomers();
                    }
                    ResetAll();
                    SnackbarMessageQueue.Instance().Enqueue("レッスンを新たに登録しました");
                }
                else
                {
                    if (result.FailData.Body.ErrorType == Protobufs.ErrorType.StudioNotFound)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("スタジオが存在しません");
                    }
                    else if (result.FailData.Body.ErrorType == Protobufs.ErrorType.StaffNotFound)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("スタッフが存在しません");
                    }
                    else if (result.FailData.Body.ErrorType == Protobufs.ErrorType.CustomerNotFound)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("顧客が存在しません");
                    }
                    else if (result.FailData.Body.ErrorType == Protobufs.ErrorType.CardNotRegistered)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("カードが登録されていません。先にカードを登録するか、現金支払を選択してください");
                    }
                    else if (result.FailData.Body.ErrorType == Protobufs.ErrorType.CreditShortage)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("カード残高が不足しています。先に残高を追加してください");
                    }
                    else
                    {
                        SnackbarMessageQueue.Instance().Enqueue("不明なエラー");
                    }
                }
            }
        }
    }
}
