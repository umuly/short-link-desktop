using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
   public class MDomain
    {
        public class Form
        {
            
        }
        public class Response
        {
            public string id { get; set; }
            public string userID { get; set; }
            public string name { get; set; }
            public string domainUrl { get; set; }
            public string scheme { get; set; }
            public string cfDomainId { get; set; }
            public string cfStatusMessage { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public DateTime updatedOn { get; set; }
            public DateTime createdOn { get; set; }
            public string createdBy { get; set; }
            public string updatedBy { get; set; }
        }
    }
}
