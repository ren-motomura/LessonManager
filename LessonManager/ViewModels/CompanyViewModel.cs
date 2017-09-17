using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.IO;
using LessonManager.Models;
using LessonManager.Commands;
using LessonManager.Views.Domain;

namespace LessonManager.ViewModels
{
    class CompanyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private PropertyChangedEventHandler companyPropertyChangedEventHander;

        private Company company_;
        public Company Company {
            get { return company_; }
            set
            {
                if (value != company_)
                {
                    if (company_ != null) company_.PropertyChanged -= companyPropertyChangedEventHander;

                    company_ = value;

                    if (company_ != null) company_.PropertyChanged += companyPropertyChangedEventHander;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        private CompanyImagePlaceHolder imagePlaceHolder_;
        private CompanyImage image_;
        public FrameworkElement CompanyImage
        {
            get
            {
                if (Company == null || Company.ImageLink == null || Company.ImageLink == "")
                {
                    return imagePlaceHolder_ == null ? imagePlaceHolder_ = new CompanyImagePlaceHolder() : imagePlaceHolder_;
                }
                else
                {
                    if (image_ == null)
                    {
                        image_ = new CompanyImage();
                        image_.DataContext = Company;
                    }
                    else if (image_.DataContext != Company)
                    {
                        image_.DataContext = Company;
                    }
                    return image_;
                }
            }
        }

        public CompanyViewModel()
        {
            Company = Company.GetCurrentCompany();
            Company.ChangeCurrentCompanyEvent += (c) =>
            {
                Company = c;
            };

            UploadImageCommand = new DelegateCommand { ExecuteHandler = UploadImageExecute };

            companyPropertyChangedEventHander = (object c, PropertyChangedEventArgs args) =>
            {
                PropertyChanged?.Invoke(this, args);
            };
        }

        public DelegateCommand UploadImageCommand { get; set; }
        private void UploadImageExecute(object parameter)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = "画像を選択する";
            dialog.Filter = "Image File (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;
                string contentType = Regex.IsMatch(fileName, "jpe?g$") ? "image/jpeg" : "image/png";

                PleaseWaitVisibility.Instance().IsVisible = true;

                var fs = File.Open(fileName, FileMode.Open); // なかったらエラーになる
                WebAPIs.Image.Upload(fs, contentType).ContinueWith((t) =>
                {
                    string imageLink = t.Result;

                    WebAPIs.Company.SetImageLink(imageLink).ContinueWith((t2) =>
                    {
                        var res = t2.Result;

                        PleaseWaitVisibility.Instance().IsVisible = false;

                        if (res.IsSuccess)
                        {
                            Company.ImageLink = imageLink;
                            SnackbarMessageQueue.Instance().Enqueue("画像を設定しました");
                        }
                        else
                        {
                            SnackbarMessageQueue.Instance().Enqueue("画像の設定に失敗しました");
                        }
                    });
                });
            }
        }
    }
}
