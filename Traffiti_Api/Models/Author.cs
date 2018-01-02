using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Traffiti_Api.Models
{
    public class Author
    {
        public int author_id { set; get; }
        public string author_name { set; get; }
        public string email { set; get; }
        public DateTime created_date { set; get; }
        public int facebook_id { set; get; }
        public int google_id { set; get; }
        public string profile_pic { set; get; }
        public string cover_pic { set; get; }
        public string access_key { set; get; }
        public int fans { set; get; }
        public int follows { set; get; }
    }
}