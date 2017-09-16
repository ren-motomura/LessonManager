using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonManager.Models
{
    public class Company
    {
        private static Company currentCompany_;
        public static event Action<Company> ChangeCurrentCompanyEvent;
        public static void SetCurrentCompany(Company company)
        {
            if (company != currentCompany_)
            {
                currentCompany_ = company;
                ChangeCurrentCompanyEvent?.Invoke(currentCompany_);
            }
        }
        public static bool IsSignedIn()
        {
            return currentCompany_ != null;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
