using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Traffiti_Api.Models
{
    public class Wall
    {
        public int wall_id { set; get; }
        public string location { set; get; }
        public string content { set; get; }
        public string photo_path { set; get; }
        public int author_id { set; get; }
        public string author_name { set; get; }
        public string profile_pic { set; get; }
        public string date_text { set; get; }
    }
}