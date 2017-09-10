using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;


namespace LessonManager.Views.Domain
{
    class MenuItem : INotifyPropertyChanged
    {
        #region implement interface
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetProperty<TField>(ref TField field, TField newValue)
        {
            if (EqualityComparer<TField>.Default.Equals(field, newValue)) return;
            field = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
        #endregion

        #region properties
        private string _name;
        private object _content;
        private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement;
        private ScrollBarVisibility _verticalScrollBarVisibilityRequirement;
        private Thickness _marginRequirement = new Thickness(16);

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public object Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement
        {
            get { return _horizontalScrollBarVisibilityRequirement; }
            set { SetProperty(ref _horizontalScrollBarVisibilityRequirement, value); }
        }

        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement
        {
            get { return _verticalScrollBarVisibilityRequirement; }
            set { SetProperty(ref _verticalScrollBarVisibilityRequirement, value); }
        }

        public Thickness MarginRequirement
        {
            get { return _marginRequirement; }
            set { SetProperty(ref _marginRequirement, value); }
        }
        #endregion

        public MenuItem(string name, object content)
        {
            Name = name;
            Content = content;
        }

    }
}
