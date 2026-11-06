using System;
using System.Configuration;
using System.Web;

namespace GUI
{
    public static class WebHelper
    {
        public static string UrlAbsoluta(string relativa)
        {
            string baseConfig = ConfigurationManager.AppSettings["FALKE_BASE_URL"];

            Uri baseUri;
            if (string.IsNullOrWhiteSpace(baseConfig))
            {
                baseUri = HttpContext.Current.Request.Url;
            }
            else
            {
                baseUri = new Uri(baseConfig);
            }

            return new Uri(baseUri, relativa).AbsoluteUri;
        }
    }
}
