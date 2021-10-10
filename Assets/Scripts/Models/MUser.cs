using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Models
{
    public class MUser
    {
        public class Form
        {
            public string name;
            public string email;
            public string password;
            public string code;
        }
        public class Response
        {
            public string Id;
            public string Email;
            public string Name;
            public string ProfilePhoto;
            public string UpdatedOn;
            public string Status;
            public string AnalyticsPermission;
            public string BlogPermission;
            public string AppsPermission;
            public string EducationAndResourcesPermission;
            public string EventsPermission;
            public string ProductNewsAndPermission;
            public string Nickname;
            public string BankAccountName;
            public string BankAccountIban;
        }
    }
}
