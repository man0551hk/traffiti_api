using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Traffiti_Api.Models
{
    public class ComingAuthor
    {
        public string authorLogin { set; get; }
        public string authorPassword { set; get; }
        public int facebookID { set; get; }
        public int googleID { set; get; }
        public string accessKey { set; get; }
    }
}