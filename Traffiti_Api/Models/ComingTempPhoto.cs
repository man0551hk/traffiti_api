using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Traffiti_Api.Models
{
    public class ComingTempPhoto
    {
        public int id { set; get; }
        public int authorID { set; get; }
        public string photoPath { set; get; }
    }
}