using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Models
{
    public class MRedirectUrl
    {
        public class Form {
            public string RedirectUrl { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Tags { get; set; }
            public string DomainId { get; set; }
            public string Code { get; set; }
            public EnUrlAccessTypes UrlAccessType { get; set; } = EnUrlAccessTypes.OnlyThoseWhoHaveTheLinkCanAccess;
            public string SpecificMembersOnly { get; set; }
            public int UrlType { get; set; } = 1;



        }
        
        public class Response
        {
            public string title { get; set; }
            public string description { get; set; }
            public object content { get; set; }
            public string tags { get; set; }
            public string id { get; set; }
            public string userID { get; set; }
            public string domainID { get; set; }
            public string code { get; set; }
            public string redirectUrl { get; set; }
            public string scheme { get; set; }
            public string shortUrl { get; set; }
            public object contentType { get; set; }
            public object fileType { get; set; }
            public object fileSize { get; set; }
            public DateTime lastVisitDate { get; set; }
            public int visitCount { get; set; }
            public int uniqueVisitorCount { get; set; }
            public int messageCount { get; set; }
            public int status { get; set; }
            public object statusText { get; set; }
            public int urlType { get; set; }
            public int urlAccessType { get; set; }
            public string specificMembersOnly { get; set; }
            public int adViews { get; set; }
            public int adUniqueViews { get; set; }
            public double revenueAmount { get; set; }
            public bool showAd { get; set; }
            public object listImage { get; set; }
            public DateTime updatedOn { get; set; }
            public DateTime createdOn { get; set; }
            public string createdBy { get; set; }
            public object updatedBy { get; set; }
        }
    }
}
