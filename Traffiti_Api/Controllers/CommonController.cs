﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Reflection;
using System.Resources;
using System.Globalization;

namespace Traffiti_Api.Controllers
{
    public class CommonController : ApiController
    {
        ResourceManager resmgr = new ResourceManager("Traffiti.Resource", Assembly.GetExecutingAssembly());

        public string CalculateDateTime(DateTime dt, int lang_id)
        {
            
            string text = string.Empty;
            TimeSpan ts = new TimeSpan();
            ts = DateTime.Now - dt;
            if (ts.TotalMinutes < 10)
            {
                text = resmgr.GetString("recently_" + ConvertLangID(lang_id));
            }
            else if (ts.TotalHours < 2)
            {
                text = resmgr.GetString("hour_" + ConvertLangID(lang_id));
            }
            return text;
        }

        public string ConvertLangID(int lang_id)
        {
            string lang = "tc";
            switch (lang_id)
            {
                case 0: lang = "en"; break;
                case 1: lang = "tc"; break;
                case 2: lang = "sc"; break;
            }
            return lang;
        }
         
    }
}
