﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace LessonManager.Views.Domain
{
    /// <summary>
    /// UserControl1.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchBox : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public SearchBox()
        {
            InitializeComponent();
            SearchText = "";
        }

        private string searchText_;
        public string SearchText
        {
            get { return searchText_; }
            set
            {
                if (value != searchText_)
                {
                    searchText_ = value;
                    RaisePropertyChanged();
                }
            }
        }

        public static readonly DependencyProperty MagnifyButtonCommandProperty =
            DependencyProperty.Register("MagnifyButtonCommand",
                typeof(ICommand),
                typeof(SearchBox),
                new FrameworkPropertyMetadata());
        public ICommand MagnifyButtonCommand
        {
            get { return (ICommand)GetValue(MagnifyButtonCommandProperty); }
            set { SetValue(MagnifyButtonCommandProperty, value); RaisePropertyChanged(); }
        }

        public static readonly DependencyProperty MagnifyButtonCommandParameterProperty =
            DependencyProperty.Register("MagnifyButtonCommandParameter",
                typeof(object),
                typeof(SearchBox),
                new FrameworkPropertyMetadata());
        public object MagnifyButtonCommandParameter
        {
            get { return (object)GetValue(MagnifyButtonCommandParameterProperty); }
            set { SetValue(MagnifyButtonCommandParameterProperty, value); RaisePropertyChanged(); }
        }

        public static readonly DependencyProperty MagnifyButtonToolTipProperty =
            DependencyProperty.Register("MagnifyButtonToolTip",
                typeof(object),
                typeof(SearchBox),
                new FrameworkPropertyMetadata());
        public object MagnifyButtonToolTip
        {
            get { return (object)GetValue(MagnifyButtonToolTipProperty); }
            set { SetValue(MagnifyButtonToolTipProperty, value); RaisePropertyChanged(); }
        }

        public static readonly DependencyProperty CloseButtonCommandProperty =
            DependencyProperty.Register("CloseButtonCommand",
                typeof(ICommand),
                typeof(SearchBox),
                new FrameworkPropertyMetadata());
        public ICommand CloseButtonCommand
        {
            get { return (ICommand)GetValue(CloseButtonCommandProperty); }
            set { SetValue(CloseButtonCommandProperty, value); RaisePropertyChanged(); }
        }

        public static readonly DependencyProperty CloseButtonCommandParameterProperty =
            DependencyProperty.Register("CloseButtonCommandParameter",
                typeof(object),
                typeof(SearchBox),
                new FrameworkPropertyMetadata());
        public object CloseButtonCommandParameter
        {
            get { return (object)GetValue(CloseButtonCommandParameterProperty); }
            set { SetValue(CloseButtonCommandParameterProperty, value); RaisePropertyChanged(); }
        }

        public static readonly DependencyProperty CloseButtonToolTipProperty =
            DependencyProperty.Register("CloseButtonToolTip",
                typeof(object),
                typeof(SearchBox),
                new FrameworkPropertyMetadata());
        public object CloseButtonToolTip
        {
            get { return (object)GetValue(CloseButtonToolTipProperty); }
            set { SetValue(CloseButtonToolTipProperty, value); RaisePropertyChanged(); }
        }
    }
}
