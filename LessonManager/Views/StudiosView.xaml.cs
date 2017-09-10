using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            vm.Studios = vm.Studios.Remove(studio);
        }

        public void AddStudioButton_Click(object sender, RoutedEventArgs e)
        {
            StudiosViewModel vm = DataContext as StudiosViewModel;
            vm.Studios = vm.Studios.Add(new Studio());
        }
    }
}
