using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LessonManager.Models;

namespace LessonManager.ViewModels
{
    class StudiosViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ImmutableList<Studio> studios_;
        public ImmutableList<Studio> Studios {
            get { return studios_; }
            set
            {
                var orgCount = studios_.Count;
                studios_ = value;
                if (studios_.Count != orgCount) // 数が変わったときだけ発火する
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        public StudiosViewModel()
        {
            // TODO: サーバーから取得する
            var builder = ImmutableList.CreateBuilder<Studio>();
            builder.Add(
                new Studio
                {
                    Name = "サードダンススクール",
                    Address = "東京都杉並区西荻北３丁目２０−２ オタニビル 4F",
                    PhoneNumber = "03-3301-7071"
                }
            );
            builder.Add(
                new Studio
                {
                    Name = "サードダンススクール",
                    Address = "東京都杉並区西荻北３丁目２０−２ オタニビル 4F",
                    PhoneNumber = "03-3301-7071"
                }
            );
            studios_ = builder.ToImmutable();
        }
    }
}
