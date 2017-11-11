using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using LessonManager.Models;
using LessonManager.Commands;

namespace LessonManager.ViewModels
{
    class StaffsViewModel : INotifyPropertyChanged
    {
        public class StaffAndImage : INotifyPropertyChanged
        {
            public StaffAndImage(Staff s)
            {
                Staff = s;
                s.PropertyChanged += (object sender, PropertyChangedEventArgs args) =>
                {
                    PropertyChanged(this, args);
                };
            }
            public Staff Staff { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            private Views.Domain.ImagePlaceHolder imagePlaceHolder_;
            private System.Windows.Controls.Image image_;
            public System.Windows.FrameworkElement ImageControl // UIスレッドからしか呼んではいけない
            {
                get
                {
                    if (Staff.ImageLink != null && Staff.ImageLink != "")
                    {
                        if (image_ != null && image_.Source != null && (image_.Source as System.Windows.Media.Imaging.BitmapImage).UriSource.ToString() == Staff.ImageLink)
                        {
                            return image_;
                        }
                        else
                        {
                            image_ = new System.Windows.Controls.Image();
                            var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.UriSource = new Uri(Staff.ImageLink);
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
                    return Staff.ID > 0;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        private IImmutableList<StaffAndImage> staffAndImages_;
        public IImmutableList<StaffAndImage> StaffAndImages
        {
            get { return staffAndImages_; }
            set
            {
                staffAndImages_ = value;
                RaisePropertyChanged();
            }
        }

        public StaffsViewModel()
        {
            AddStaffCommand = new DelegateCommand {
                ExecuteHandler = AddStaffCommandExecute
            };

            UploadImageCommand = new DelegateCommand
            {
                ExecuteHandler = UploadImageCommandExecute
            };

            CreateOrUpdateStaffCommand = new DelegateCommand
            {
                ExecuteHandler = CreateOrUpdateStaffCommandConfirm
            };

            DeleteStaffCommand = new DelegateCommand
            {
                ExecuteHandler = DeleteStaffCommandConfirm
            };

            StaffAndImages = new List<StaffAndImage>().ToImmutableList();
            Storage.GetInstance().PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == "Staffs")
                {
                    var builder = ImmutableList.CreateBuilder<StaffAndImage>();
                    Storage.GetInstance().Staffs.ForEach((s) =>
                    {
                        builder.Add(new StaffAndImage(s));
                    });
                    StaffAndImages = builder.ToImmutableList();
                }
            };
        }

        public DelegateCommand AddStaffCommand { get; set; } 
        private void AddStaffCommandExecute(object parameter)
        {
            StaffAndImages = StaffAndImages.Add(new StaffAndImage(new Staff()));
        }

        public DelegateCommand UploadImageCommand { get; set; }
        private void UploadImageCommandExecute(object parameter)
        {
            var staffAndImage = parameter as StaffAndImage;

            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "画像を選択する";
            dialog.Filter = "Image File (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;
                var fs = File.Open(dialog.FileName, FileMode.Open); // なかったらエラーになる

                string contentType = Regex.IsMatch(fileName, "jpe?g$") ? "image/jpeg" : "image/png";

                PleaseWaitVisibility.Instance().IsVisible = true;

                WebAPIs.Image.Upload(fs, contentType).ContinueWith((t) =>
                {
                    PleaseWaitVisibility.Instance().IsVisible = false;

                    string imageLink = t.Result;
                    staffAndImage.Staff.ImageLink = imageLink;
                });
            }
        }

        public DelegateCommand CreateOrUpdateStaffCommand { get; set; }
        private async void CreateOrUpdateStaffCommandConfirm(object parameter)
        {
            var staff = parameter as Staff;

            if (staff.ID == 0)
            {
                var view = new Views.Domain.ConfirmModal();
                view.DataContext = "スタッフを登録します。\nよろしいですか？";

                object result = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
                if ((bool)result)
                {
                    CreateStaff(staff);
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                }
            }
            else
            {
                var view = new Views.Domain.ConfirmModal();
                view.DataContext = "スタッフ情報を更新します。\nよろしいですか？";

                object result = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
                if ((bool)result)
                {
                    UpdateStaff(staff);
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
                }
            }
        }
        private void CreateStaff(Staff staff)
        {
            PleaseWaitVisibility.Instance().IsVisible = true;
            WebAPIs.Staff.Create(staff.Name, staff.ImageLink).ContinueWith(t =>
            {
                PleaseWaitVisibility.Instance().IsVisible = false;
                var result = t.Result;
                if (result.IsSuccess)
                {
                    staff.ID = result.SuccessData.ID;
                    Storage.GetInstance().LoadStaffs();
                    SnackbarMessageQueue.Instance().Enqueue("スタッフを新たに登録しました");
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
        private void UpdateStaff(Staff staff)
        {
            PleaseWaitVisibility.Instance().IsVisible = true;
            WebAPIs.Staff.Update(staff.ID, staff.ImageLink).ContinueWith(t =>
            {
                PleaseWaitVisibility.Instance().IsVisible = false;
                var result = t.Result;
                if (result.IsSuccess)
                {
                    SnackbarMessageQueue.Instance().Enqueue("スタッフの情報を更新しました");
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("不明なエラー");
                }
            });
        }

        public DelegateCommand DeleteStaffCommand { get; set; }
        private async void DeleteStaffCommandConfirm(object parameter)
        {
            var staff = parameter as Staff;

            if (staff.ID == 0) // まだ保存していないので、ローカルのコレクションから削除するだけ
            {
                StaffAndImages = StaffAndImages.RemoveAll((st) =>
                {
                    return st.Staff == staff;
                });
                return;
            }

            var view = new Views.Domain.ConfirmModal();
            view.DataContext = "スタッフを登録抹消します。\nこの操作は取り消せません\nよろしいですか？";

            object result = await MaterialDesignThemes.Wpf.DialogHost.Show(view);
            if ((bool)result)
            {
                DeleteStaff(staff);
            }
            else
            {
                SnackbarMessageQueue.Instance().Enqueue("キャンセルしました");
            }
        }

        private void DeleteStaff(Staff staff)
        {
            PleaseWaitVisibility.Instance().IsVisible = true;
            WebAPIs.Studio.Delete(staff.ID).ContinueWith(t =>
            {
                PleaseWaitVisibility.Instance().IsVisible = false;
                var result = t.Result;
                if (result.IsSuccess)
                {
                    StaffAndImages = StaffAndImages.RemoveAll((s) =>
                    {
                        return s.Staff == staff;
                    });
                    Storage.GetInstance().LoadStaffs();
                    SnackbarMessageQueue.Instance().Enqueue("スタッフを登録抹消しました");
                }
                else
                {
                    SnackbarMessageQueue.Instance().Enqueue("不明なエラー");
                }
            });
        }
    }
}
