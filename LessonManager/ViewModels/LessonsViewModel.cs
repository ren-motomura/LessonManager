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
    class LessonsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public LessonsViewModel()
        {
            TakenAtFrom = DateTime.Today;
            TakenAtTo = DateTime.Today;

            SearchCommand = new DelegateCommand();
            SearchCommand.ExecuteHandler = SearchCommandExecute;

            SearchCustomerCommand = new DelegateCommand();
            SearchCustomerCommand.ExecuteHandler = SearchCustomerCommandExecute;

            SearchCustomerByCardCommand = new DelegateCommand();
            SearchCustomerByCardCommand.ExecuteHandler = SearchCustomerByCardCommandExecute;

            ResetCommand = new DelegateCommand();
            ResetCommand.ExecuteHandler = ResetCommandExecute;

            DeleteLessonCommand = new DelegateCommand();
            DeleteLessonCommand.ExecuteHandler = DeleteLessonCommandExecute;

            SendMailCommand = new DelegateCommand();
            SendMailCommand.ExecuteHandler = SendMailCommandExecute;

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

        private void ResetConditions()
        {
            TakenAtFrom = DateTime.Today;
            TakenAtTo = DateTime.Today;

            Studio = null;
            Staff = null;
            Customer = null;
        }

        private DateTime takenAtFrom_;
        public DateTime TakenAtFrom
        {
            get { return takenAtFrom_; }
            set
            {
                takenAtFrom_ = value;
                RaisePropertyChanged();
            }
        }

        private DateTime takenAtTo_;
        public DateTime TakenAtTo
        {
            get { return takenAtTo_; }
            set
            {
                takenAtTo_ = value;
                RaisePropertyChanged();
            }
        }

        private Studio studio_;
        public Studio Studio
        {
            get { return studio_; }
            set
            {
                studio_ = value;
                RaisePropertyChanged();
            }
        }

        private Staff staff_;
        public Staff Staff
        {
            get { return staff_; }
            set
            {
                staff_ = value;
                RaisePropertyChanged();
            }
        }

        private Customer customer_;
        public Customer Customer
        {
            get { return customer_; }
            set
            {
                customer_ = value;
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

        private ImmutableList<Customer> customers_;
        public ImmutableList<Customer> Customers
        {
            get { return customers_; }
            set
            {
                customers_ = value;
                RaisePropertyChanged();
            }
        }

        private ImmutableList<Lesson> lessons_;
        public ImmutableList<Lesson> Lessons
        {
            get { return lessons_; }
            set
            {
                lessons_ = value;
                RaisePropertyChanged();
            }
        }

        public DelegateCommand SearchCommand { get; set; }
        private async void SearchCommandExecute(object parameter)
        {
            PleaseWaitVisibility.Instance().IsVisible = true;
            var result = await WebAPIs.Lesson.Search(Studio, Staff, Customer, TakenAtFrom, TakenAtTo.AddDays(1));
            PleaseWaitVisibility.Instance().IsVisible = false;
            if (result.IsSuccess)
            {
                Lessons = result.SuccessData.ToImmutableList();
            }
            else
            {
                SnackbarMessageQueue.Instance().Enqueue("検索に失敗しました");
            }
        }

        public DelegateCommand SearchCustomerCommand { get; set; }
        private async void SearchCustomerCommandExecute(object paramter)
        {
            var modal = new Views.Domain.SearchCustomerModal();
            object result = await MaterialDesignThemes.Wpf.DialogHost.Show(modal);
            Customer = result as Customer;
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

            Customer = Storage.GetInstance().Customers.Find((c) =>
            {
                return c.CardId == cardID;
            });
            if (Customer == null)
            {
                SnackbarMessageQueue.Instance().Enqueue("該当する顧客が見つかりませんでした");
                return;
            }
        }

        public DelegateCommand ResetCommand { get; set; }
        private void ResetCommandExecute(object parameter)
        {
            ResetConditions();
        }

        public DelegateCommand DeleteLessonCommand { get; set; }
        private async void DeleteLessonCommandExecute(object parameter)
        {
            // confirmation
            {
                var view = new Views.Domain.ConfirmModal();
                view.DataContext = String.Format(@"レッスンを削除します。
この操作は取り消せません。
本当によろしいですか？");

                object confirmResult = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
                if (!(bool)confirmResult)
                {
                    SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                    return;
                }
            }

            var target = parameter as Lesson;
            PleaseWaitVisibility.Instance().IsVisible = true;
            var result = await WebAPIs.Lesson.Delete(target.ID);
            PleaseWaitVisibility.Instance().IsVisible = false;
            if (result.IsSuccess)
            {
                Lessons = Lessons.Remove(target);
                SnackbarMessageQueue.Instance().Enqueue("レッスンを削除しました");
            }
            else
            {
                SnackbarMessageQueue.Instance().Enqueue("検索に失敗しました");
            }
        }

        public DelegateCommand SendMailCommand { get; set; }
        private async void SendMailCommandExecute(object parameter)
        {
            Mail mail = new Mail();

            while (true)
            {
                { // modal
                    var view = new Views.Domain.SendMailModal();
                    (view.DataContext as ViewModels.Domain.SendMailModalViewModel).Mail = mail;
                    object result = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
                    mail = result as Mail;

                    if (mail == null)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                        return;
                    }
                }

                if (mail.ToAddresses.Count == 0)
                {
                    var view = new Views.Domain.ConfirmModal();
                    view.DataContext = String.Format("宛先を指定してください");
                    object result = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
                    if (!(bool)result)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                        return;
                    }
                    continue;
                }
                
                if (!mail.IsAllAddressValid())
                {
                    var view = new Views.Domain.ConfirmModal();
                    view.DataContext = String.Format("宛先にメールアドレスとして正しくないものが含まれています。\n宛先を修正してください");
                    object result = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
                    if (!(bool)result)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                        return;
                    }
                    continue;
                }

                if (mail.Subject == "" || mail.Body == "")
                {
                    var view = new Views.Domain.ConfirmModal();
                    var emptyThing =
                        mail.Subject == "" && mail.Body == "" ? "件名と本文" :
                        mail.Subject == "" ? "件名" : "本文";
                    view.DataContext = String.Format(@"{0:s}が空です。内容を追加してください", emptyThing);
                    object result = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
                    if (!(bool)result)
                    {
                        SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                        return;
                    }
                    continue;
                }

                break;
            }

            { // CSVを添付
                var sb = new StringBuilder();
                sb.AppendLine("ID,スタジオ,スタッフ,顧客,料金,支払いタイプ,実施日時");
                Lessons.ForEach((lesson) =>
                {
                    sb.AppendFormat(
                        "{0:d},{1},{2},{3},{4:d},{5},{6}\n",
                        lesson.ID, 
                        lesson.StudioName,
                        lesson.StaffName,
                        lesson.CustomerName, 
                        lesson.Fee,
                        lesson.PaymentType,
                        lesson.TakenAt.ToString("G")
                    );
                });
                mail.Attachments = mail.Attachments.Add(new Mail.Attachment() {
                    Name = String.Format("lessons-{0}.csv", DateTime.Now.ToString("yyyyMMdd")),
                    Data = Encoding.UTF8.GetBytes(sb.ToString())
                });
            }

            {
                PleaseWaitVisibility.Instance().IsVisible = true;
                var result = await WebAPIs.Mail.Send(mail);
                PleaseWaitVisibility.Instance().IsVisible = false;
                if (result.IsSuccess)
                {
                    SnackbarMessageQueue.Instance().Enqueue("メールを送信しました");
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("メールの送信に失敗しました");
                }
            }
        }
    }
}
