using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            this.menuItemsWhenAuthorized_ = new List<MenuItem>
            {
                new MenuItem("スタジオ管理", new StudiosView())
                {
                    VerticalScrollBarVisibilityRequirement = System.Windows.Controls.ScrollBarVisibility.Auto
                }
            };
            this.menuItemsWhenUnauthorized_ = new List<MenuItem>
            {
                new MenuItem("ログイン", new SigninView())
            };

            // TODO: ログイン状態を見て切り替える
            this.MenuItems = this.menuItemsWhenUnauthorized_;

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

            SignOutCommand = new SignOutCommand();
        }

        private List<MenuItem> menuItems_;
        public List<MenuItem> MenuItems
        {
            get { return menuItems_; }
            set
            {
                if (value != menuItems_)
                {
                    menuItems_ = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private List<MenuItem> menuItemsWhenUnauthorized_ { get; set; }
        private List<MenuItem> menuItemsWhenAuthorized_ { get; set; }

        public SignOutCommand SignOutCommand { get; set; }
    }
}
