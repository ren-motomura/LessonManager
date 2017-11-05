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
    /// RegisterLesson.xaml の相互作用ロジック
    /// </summary>
    public partial class RegisterLesson : UserControl
    {
        public RegisterLesson()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            var tmpStr = textBox.Text + e.Text;
            float xx;
            e.Handled = !(Single.TryParse(tmpStr, out xx));
        }
    }
}
