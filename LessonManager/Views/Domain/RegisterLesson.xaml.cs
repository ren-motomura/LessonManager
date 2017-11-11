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
            int xx;
            e.Handled = !(Int32.TryParse(tmpStr, out xx));
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Up && e.Key != Key.Down)
                return;

            var textBox = sender as TextBox;
            var tmpStr = textBox.Text;
            int xx = 0;
            Int32.TryParse(tmpStr, out xx);

            if (e.Key == Key.Up)
                xx += 500;
            else
                xx = Math.Max(0, xx - 500);

            textBox.Text = xx.ToString();
        }
    }
}
