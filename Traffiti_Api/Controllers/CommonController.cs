using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Reflection;
using System.Resources;
using System.Globalization;
using System.Web;

namespace Traffiti_Api.Controllers
{
    public class CommonController : ApiController
    {
        //ResourceManager resmgr = new ResourceManager("Common.Resource", Assembly.GetExecutingAssembly());

        public string CalculateDateTime(DateTime dt, int lang_id)
        {
            string language = "en-us";
            switch (lang_id)
            {
                case 1:
                    language = "en-us";
                    break;
                case 2:
                    language = "zh-hk";
                    break;
                case 3:
                    language = "zh-cn";
                    break;
                default: language = "zh-hk";
                    break;
            }
            CultureInfo ci = new CultureInfo(language);

            string text = string.Empty;
            TimeSpan ts = new TimeSpan();
            ts = DateTime.Now - dt;
            if (ts.TotalMinutes < 10)
            {
                text = (string)HttpContext.GetGlobalResourceObject("Common", "recently", ci);
            }
            else if (ts.TotalHours < 2)
            {
                text = (string)HttpContext.GetGlobalResourceObject("Common", "hour", ci);
            }
            else
            {
                text = dt.Year + "-" + dt.Month.ToString("00") + "-" + dt.Day.ToString("00") + " " + dt.Hour.ToString("00") + ":" + dt.Minute.ToString("00");
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
