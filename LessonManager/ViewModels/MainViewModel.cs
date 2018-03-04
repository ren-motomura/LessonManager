using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.ComponentModel;
using LessonManager.Commands;
using LessonManager.Models;
using LessonManager.Views;
using LessonManager.Views.Domain;

namespace LessonManager.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            //this.Content = new SigninView();
            this.Content = new MainMenu();

            /*
            // current company の変更を watch する
            Models.Company.ChangeCurrentCompanyEvent += (company) =>
            {
                if (company != null)
                {
                    this.MenuItems = this.menuItemsWhenAuthorized_;
                }
                else
                {
                    this.MenuItems = this.menuItemsWhenUnauthorized_;
                }
            };
            */

            SignOutCommand = new SignOutCommand();
            SnackbarMessageQueue = SnackbarMessageQueue.Instance();
            ReloadCommand = new DelegateCommand();
            ReloadCommand.ExecuteHandler = ReloadCommandExecute;

            PleaseWaitVisibility = PleaseWaitVisibility.Instance();
        }

        private UserControl content_;
        public UserControl Content
        {
            get { return content_; }
            set
            {
                if (value != content_)
                {
                    content_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        public SignOutCommand SignOutCommand { get; set; }
        public SnackbarMessageQueue SnackbarMessageQueue { get; set; }
        public DelegateCommand ReloadCommand { get; set; }

        private void ReloadCommandExecute(object paramter)
        {
            Storage.GetInstance().LoadAll();
        }

        public PleaseWaitVisibility PleaseWaitVisibility { get; set; }
    }
}
