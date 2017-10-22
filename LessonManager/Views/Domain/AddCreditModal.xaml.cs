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

namespace LessonManager.Views.Domain
{
    /// <summary>
    /// AddCreditModal.xaml の相互作用ロジック
    /// </summary>
    public partial class AddCreditModal : UserControl
    {
        public AddCreditModal()
        {
            InitializeComponent();
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var toggleButton = sender as System.Windows.Controls.Primitives.ToggleButton;
            if ((bool)toggleButton.IsChecked)
            {
                CreditAmount.Minimum = -100000;
            }
            else
            {
                CreditAmount.Minimum = 1000;
            }
        }
    }
}
