using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LessonManager.Views;

namespace LessonManager.Models
{
    class PageManager
    {
        private static PageManager instance_;
        public static PageManager Instance()
        {
            return instance_ == null ?
                instance_ = new PageManager() :
                instance_;
        }

        private PageManager()
        {
            pageMap_ = new Dictionary<string, UserControl>();
            pageMap_.Add("Signin", new SigninView());
            pageMap_.Add("Main", new MainMenuView());
            pageMap_.Add("CreateCustomer", new CreateCustomerView());
            pageMap_.Add("Customers", new CustomersView());
            pageMap_.Add("Lessons", new LessonsView());

            CurrentPage = pageMap_["Signin"];
        }

        public UserControl CurrentPage { get; private set; }
        public void SetCurrentPageByKey(string key)
        {
            if (!pageMap_.ContainsKey(key)) return;
            CurrentPage = pageMap_[key];
            ChangeCurrentPageEvent?.Invoke(CurrentPage);
        }

        public event Action<UserControl> ChangeCurrentPageEvent;

        private Dictionary<string, UserControl> pageMap_;
    }
}
