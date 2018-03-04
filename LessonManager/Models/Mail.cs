using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace LessonManager.Models
{
    class Mail : INotifyPropertyChanged
    {

        public const string addressRegex = @"^(?:(?:(?:(?:[a-zA-Z0-9_!#\$\%&'*+/=?\^`{}~|\-]+)(?:\.(?:[a-zA-Z0-9_!#\$\%&'*+/=?\^`{}~|\-]+))*)|(?:""(?:\\[^\r\n] |[^\\""])*"")))\@(?:(?:(?:(?:[a-zA-Z0-9_!#\$\%&'*+/=?\^`{}~|\-]+)(?:\.(?:[a-zA-Z0-9_!#\$\%&'*+/=?\^`{}~|\-]+))*)|(?:\[(?:\\\S|[\x21-\x5a\x5e-\x7e])*\])))$";

        public class Attachment
        {
            public string Name { get; set; }
            public byte[] Data { get; set; }
        }

        public Mail()
        {
            toAddressesStr_ = "";
            subject_ = "";
            body_ = "";
            attachments_ = new List<Attachment>().ToImmutableList();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }

        private string toAddressesStr_;
        public string ToAddressesStr
        {
            get { return toAddressesStr_; }
            set
            {
                toAddressesStr_ = value;
                RaisePropertyChanged();
            }
        }

        public List<string> ToAddresses
        {
            get
            {
                if (Regex.IsMatch(ToAddressesStr, "^ *$"))
                    return new List<string>();
                return new List<string>(Regex.Split(ToAddressesStr, ", *"));
            }
        }

        private string subject_;
        public string Subject
        {
            get { return subject_; }
            set
            {
                subject_ = value;
                RaisePropertyChanged();
            }
        }

        private string body_;
        public string Body
        {
            get { return body_; }
            set
            {
                body_ = value;
                RaisePropertyChanged();
            }
        }

        private ImmutableList<Attachment> attachments_;
        public ImmutableList<Attachment> Attachments
        {
            get { return attachments_; }
            set
            {
                attachments_ = value;
                RaisePropertyChanged();
            }
        }

        public bool IsAllAddressValid()
        {
            return ToAddresses.All((address) =>
            {
                return Regex.IsMatch(address, addressRegex);
            });
        }
    }
}
