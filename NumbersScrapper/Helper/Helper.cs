using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NumbersScrapper.Helper
{
    class Helper
    {
        public static string ValidateDir(string dir)
        {
            if (!dir.Last().Equals('\\'))
                return dir + '\\';
            else
                return dir;
        }

        /// <summary>
        /// Get a HTML body of a URL
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>String containing the HTML</returns>
        public static string GetHTML(string url)
        {
            StringBuilder retval = new StringBuilder();
            WebClient wc = new WebClient();
            try
            {
                retval.Append(wc.DownloadString(url));
            }
            catch
            {
                throw;
            }
            return retval.ToString();
        }
    }
}
