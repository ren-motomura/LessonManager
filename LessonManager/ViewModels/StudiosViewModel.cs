using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using LessonManager.Models;
using LessonManager.Commands;

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

        public DelegateCommand CreateOrUpdateStudioCommand { get; set; }

        public StudiosViewModel()
        {
            studios_ = new List<Studio>().ToImmutableList();
            CreateOrUpdateStudioCommand = new DelegateCommand();
            CreateOrUpdateStudioCommand.ExecuteHandler = CreateOrUpdateStudioExecute;

            Models.Company.ChangeCurrentCompanyEvent += (c) =>
            {
                LoadStudios();
            };
        }

        public void LoadStudios()
        {
            if (!Models.Company.IsSignedIn())
            {
                Studios = new List<Studio>().ToImmutableList();
                return;
            }

            WebAPIs.Studio.GetAll().ContinueWith(t =>
            {
                var result = t.Result;
                if (result.IsSuccess)
                {
                    Studios = result.SuccessData.ToImmutableList();
                }
                else
                {
                    // TODO
                    SnackbarMessageQueue.Instance().Enqueue(String.Format("失敗したみたい {0:D}", result.FailData.Status));
                }
            });

        }

        private void CreateOrUpdateStudioExecute(object parameter)
        {
            var s = parameter as Models.Studio;
            var targetStudio = Studios.Find(st => st == s);
            if (targetStudio == null) return;

            if (targetStudio.ID == 0)
            {
                WebAPIs.Studio.Create(targetStudio.Name, targetStudio.Address, targetStudio.PhoneNumber).ContinueWith(t =>
                {
                    var result = t.Result;
                    if (result.IsSuccess)
                    {
                        targetStudio.ID = result.SuccessData.ID;
                        SnackbarMessageQueue.Instance().Enqueue("スタジオを新たに作成しました");
                    }
                    else
                    {
                        if (result.FailData.Body.ErrorType == Protobufs.ErrorType.AlreadyExist)
                        {
                            SnackbarMessageQueue.Instance().Enqueue("その名前は既に使われています");
                        }
                        else
                        {
                            SnackbarMessageQueue.Instance().Enqueue("不明なエラー");
                        }
                    }
                });
            }
            else
            {
            }
        }
    }
}
