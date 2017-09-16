using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using LessonManager.ViewModels;
using LessonManager.Models;

namespace LessonManager.Views
{
    /// <summary>
    /// StudiosView.xaml の相互作用ロジック
    /// </summary>
    public partial class StudiosView : UserControl
    {
        public StudiosView()
        {
            InitializeComponent();
        }

        public void RemoveStudioButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var studio = button.Tag as Studio;
            StudiosViewModel vm = DataContext as StudiosViewModel;
            vm.StudioAndImages = vm.StudioAndImages.RemoveAll((s) =>
            {
                return s.Studio == studio;
            });
        }

        public void AddStudioButton_Click(object sender, RoutedEventArgs e)
        {
            StudiosViewModel vm = DataContext as StudiosViewModel;
            vm.StudioAndImages = vm.StudioAndImages.Add(new StudiosViewModel.StudioAndImage(new Studio()));
        }

        public void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "画像を選択する";
            dialog.Filter = "Image File (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (dialog.ShowDialog() == true)
            {
                var button = sender as System.Windows.Controls.Button;
                var studioAndImage = button.Tag as StudiosViewModel.StudioAndImage;
                var fileName = dialog.FileName;

                string contentType = Regex.IsMatch(fileName, "jpe?g$") ? "image/jpeg" : "image/png";

                var param = new StudiosViewModel.UploadImageParameter();
                param.StudioAndImage = studioAndImage;
                param.FileName = fileName;
                param.ContentType = contentType;

                StudiosViewModel vm = DataContext as StudiosViewModel;
                vm.UploadImageCommand.Execute(param);
            }
        }
    }
}
