using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using LessonManager.Utils;

namespace LessonManager.ViewModels.Domain
{
    class WaitCardModalViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        private bool isLoading_;
        public bool IsLoading
        {
            get { return isLoading_; }
            set
            {
                if (value != isLoading_)
                {
                    isLoading_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }
        public string LoadingVisibility
        {
            get
            {
                return IsLoading ? "Visible" : "Hidden";
            }
        }

        private bool isSucceeded_;
        public bool IsSucceeded
        {
            get { return isSucceeded_; }
            set
            {
                if (value != isSucceeded_)
                {
                    isSucceeded_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private string cardID_;
        public string CardID
        {
            get { return cardID_; }
            set
            {
                if (value != cardID_)
                {
                    cardID_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private string message_;
        public string Message
        {
            get { return message_; }
            set
            {
                if (value != message_)
                {
                    message_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        public WaitCardModalViewModel()
        {
            IsLoading = true;
            IsSucceeded = false;
            CardID = "";
            Message = "";

            Task.Run(() =>
            {
                DateTime startedAt = DateTime.Now;
                string cardID;
                NFC.Result result;
                while(true)
                {
                    result = NFC.GetCardID(out cardID);

                    if (result == NFC.Result.GET_ID_SUCCESS || result == NFC.Result.GET_ID_REMOVE_TIMEOUT)
                    {
                        CardID = cardID;
                        IsSucceeded = true;
                        setMessage(result);
                        break;
                    }
                    setMessage(result);

                    if ((DateTime.Now - startedAt) > TimeSpan.FromSeconds(10))
                    {
                        Message = "タイムアウトしました";
                        break;
                    }

                    Thread.Sleep(500);
                }
                IsLoading = false;
            });
        }

        private void setMessage(NFC.Result result)
        {
            switch (result)
            {
                case NFC.Result.GET_ID_SUCCESS:
                case NFC.Result.GET_ID_REMOVE_TIMEOUT:
                    Message = "読み取りに成功しました\nカードID：" + CardID;
                    break;
                case NFC.Result.GET_ID_FAILURE:
                case NFC.Result.GET_ID_NO_SERVICE:
                case NFC.Result.GET_ID_CARD_TIMEOUT:
                case NFC.Result.GET_ID_RELEASE_ERROR:
                case NFC.Result.GET_ID_COMMAND_ERROR:
                    Message = "読み取りに失敗しました";
                    break;
                case NFC.Result.GET_ID_NO_READERS:
                    Message = "リーダーが接続されていません";
                    break;
                case NFC.Result.GET_ID_NO_CARD:
                    Message = "カードが見つかりません";
                    break;
                case NFC.Result.GET_ID_REMOVE_CARD:
                    Message = "カードがはずされました";
                    break;
                default:
                    break;
            }
        }
    }
}
