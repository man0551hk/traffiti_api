using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Traffiti_Api.Models
{
    public class ComingCreateWall
    {
        public int author_id { set; get; }
        public string location { set; get; }
        public string lat { set; get; }
        public string lon { set; get; }
        public string message { set; get; }
        public List<string> photoList { set; get; }
    }
}