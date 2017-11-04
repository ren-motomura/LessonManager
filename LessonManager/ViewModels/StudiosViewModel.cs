using System;
using System.IO;
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
        public class StudioAndImage : INotifyPropertyChanged
        {
            public StudioAndImage(Studio s)
            {
                Studio = s;
                s.PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
                {
                    PropertyChanged(this, args);
                };
            }
            public Studio Studio { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            private Views.Domain.ImagePlaceHolder imagePlaceHolder_;
            private System.Windows.Controls.Image image_;
            public System.Windows.FrameworkElement ImageControl // UIスレッドからしか呼んではいけない
            {
                get
                {
                    if (Studio.ImageLink != null && Studio.ImageLink != "")
                    {
                        if (image_ != null && image_.Source != null && (image_.Source as System.Windows.Media.Imaging.BitmapImage).UriSource.ToString() == Studio.ImageLink)
                        {
                            return image_;
                        }
                        else
                        {
                            image_ = new System.Windows.Controls.Image();
                            var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.UriSource = new Uri(Studio.ImageLink);
                            bitmapImage.EndInit();
                            image_.Source = bitmapImage;
                            image_.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            image_.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                            image_.Stretch = System.Windows.Media.Stretch.UniformToFill;
                            return image_;
                        }
                        
                    }
                    else
                    {
                        return imagePlaceHolder_ == null ? imagePlaceHolder_ = new Views.Domain.ImagePlaceHolder()
                                                         : imagePlaceHolder_;
                    }
                }
            }

            public bool IsNameReadOnly
            {
                get
                {
                    return Studio.ID > 0;
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        private ImmutableList<StudioAndImage> studioAndImages_;
        public ImmutableList<StudioAndImage> StudioAndImages {
            get { return studioAndImages_; }
            set
            {
                var orgCount = studioAndImages_ == null ? 0 : studioAndImages_.Count;
                var newCount = value == null ? 0 : value.Count;
                studioAndImages_ = value;
                if (newCount != orgCount) // 数が変わったときだけ発火する
                {
                    RaisePropertyChanged();
                }
            }
        }

        public DelegateCommand CreateOrUpdateStudioCommand { get; set; }
        public DelegateCommand DeleteStudioCommand { get; set; }
        public DelegateCommand UploadImageCommand { get; set; }

        public StudiosViewModel()
        {
            studioAndImages_ = new List<StudioAndImage>().ToImmutableList();
            CreateOrUpdateStudioCommand = new DelegateCommand();
            CreateOrUpdateStudioCommand.ExecuteHandler = CreateOrUpdateStudioExecuteConfirm;

            DeleteStudioCommand = new DelegateCommand();
            DeleteStudioCommand.ExecuteHandler = DeleteStudioExecuteConfirm;

            UploadImageCommand = new DelegateCommand();
            UploadImageCommand.ExecuteHandler = UploadImageExecute;

            StudioAndImages = new List<StudioAndImage>().ToImmutableList();
            Storage.GetInstance().PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "Studios")
                {
                    var builder = ImmutableList.CreateBuilder<StudioAndImage>();
                    Storage.GetInstance().Studios.ForEach((s) =>
                    {
                        builder.Add(new StudioAndImage(s));
                    });
                    StudioAndImages = builder.ToImmutableList();
                }
            };
        }

        private async void CreateOrUpdateStudioExecuteConfirm(object parameter)
        {
            var s = parameter as Models.Studio;
            var targetStudio = StudioAndImages.Find(st => st.Studio.ID == s.ID)?.Studio;
            if (targetStudio == null) return;

            if (targetStudio.ID == 0)
            {
                var view = new Views.Domain.ConfirmModal();
                view.DataContext = "スタジオを登録します。\nよろしいですか？";

                object result = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
                if ((bool)result)
                {
                    CreateStudio(targetStudio);
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                }
            }
            else
            {
                var view = new Views.Domain.ConfirmModal();
                view.DataContext = "スタジオ情報を更新します。\nよろしいですか？";

                object result = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
                if ((bool)result)
                {
                    UpdateStudio(targetStudio);
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                }
            }
        }

        private void CreateStudio(Studio targetStudio)
        {
            PleaseWaitVisibility.Instance().IsVisible = true;
            WebAPIs.Studio.Create(targetStudio.Name, targetStudio.Address, targetStudio.PhoneNumber, targetStudio.ImageLink).ContinueWith(t =>
            {
                PleaseWaitVisibility.Instance().IsVisible = false;
                var result = t.Result;
                if (result.IsSuccess)
                {
                    targetStudio.ID = result.SuccessData.ID;
                    Storage.GetInstance().LoadStudios();
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

        private void UpdateStudio(Studio targetStudio)
        {
            PleaseWaitVisibility.Instance().IsVisible = true;
            WebAPIs.Studio.Update(targetStudio.ID, targetStudio.Address, targetStudio.PhoneNumber, targetStudio.ImageLink).ContinueWith(t =>
            {
                PleaseWaitVisibility.Instance().IsVisible = false;
                var result = t.Result;
                if (result.IsSuccess)
                {
                    SnackbarMessageQueue.Instance().Enqueue("スタジオ情報を更新しました");
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("不明なエラー");
                }
            });
        }

        private async void DeleteStudioExecuteConfirm(object parameter)
        {
            var s = parameter as Models.Studio;
            var targetStudio = StudioAndImages.Find(st => st.Studio.ID == s.ID)?.Studio;
            if (targetStudio == null) return;

            if (targetStudio.ID == 0) // まだ保存していないので、ローカルのコレクションから削除するだけ
            {
                StudioAndImages = StudioAndImages.RemoveAll((st) =>
                {
                    return st.Studio == targetStudio;
                });
                return;
            }

            var view = new Views.Domain.ConfirmModal();
            view.DataContext = "スタジオを削除します。\nこの操作は取り消せません\nよろしいですか？";

            object result = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
            if ((bool)result)
            {
                DeleteStudio(targetStudio);
            }
            else
            {
                SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
            }
        }

        private void DeleteStudio(Studio targetStudio)
        {
            PleaseWaitVisibility.Instance().IsVisible = true;
            WebAPIs.Studio.Delete(targetStudio.ID).ContinueWith(t =>
            {
                PleaseWaitVisibility.Instance().IsVisible = false;
                var result = t.Result;
                if (result.IsSuccess)
                {
                    StudioAndImages = StudioAndImages.RemoveAll((s) =>
                    {
                        return s.Studio == targetStudio;
                    });
                    Storage.GetInstance().LoadStudios();
                    SnackbarMessageQueue.Instance().Enqueue("スタジオを削除しました");
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("不明なエラー");
                }
            });
        }

        public class UploadImageParameter
        {
            public StudioAndImage StudioAndImage { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
        }

        private void UploadImageExecute(object parameter)
        {
            PleaseWaitVisibility.Instance().IsVisible = true;

            var param = parameter as UploadImageParameter;

            var fs = File.Open(param.FileName, FileMode.Open); // なかったらエラーになる
            WebAPIs.Image.Upload(fs, param.ContentType).ContinueWith((t) =>
            {
                PleaseWaitVisibility.Instance().IsVisible = false;

                string imageLink = t.Result;
                param.StudioAndImage.Studio.ImageLink = imageLink;
            });
        }
    }
}
