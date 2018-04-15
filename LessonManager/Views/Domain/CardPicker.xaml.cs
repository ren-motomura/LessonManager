using System;
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
    /// CardPicker.xaml の相互作用ロジック
    /// </summary>
    public partial class CardPicker : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        public CardPicker()
        {
            InitializeComponent();
            CardID = "";
        }

        public static readonly DependencyProperty CardIDProperty =
            DependencyProperty.Register("CardID",
                typeof(string),
                typeof(CardPicker),
                new FrameworkPropertyMetadata());
        public string CardID
        {
            get { return (string)GetValue(CardIDProperty); }
            set { SetValue(CardIDProperty, value); RaisePropertyChanged(); }
        }

        public static readonly DependencyProperty PickCardCommandProperty =
            DependencyProperty.Register("PickCardCommand",
                typeof(ICommand),
                typeof(CardPicker),
                new FrameworkPropertyMetadata());
        public ICommand PickCardCommand
        {
            get { return (ICommand)GetValue(PickCardCommandProperty); }
            set { SetValue(PickCardCommandProperty, value); RaisePropertyChanged(); }
        }
    }
}
