using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NumbersScrapper.HelperClasses
{
    public enum ProgramStatus
    {
        Error, Success, Start, End
    };

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

        /// <summary>
        /// Write into log.
        /// </summary>
        /// <param name="status">Status of the program</param>
        /// <param name="desc">Default is ""</param>
        public static void WriteToLog(ProgramStatus status, string desc = "")
        {
            string log = "";
            switch (status)
            {
                case ProgramStatus.Error:
                    log = "ERROR - " + DateTime.Now;
                    break;
                case ProgramStatus.End:
                    log = "END - " + DateTime.Now;
                    break;
                case ProgramStatus.Start:
                    log = "START - " + DateTime.Now;
                    break;
                case ProgramStatus.Success:
                    log = "SUCCESS - " + DateTime.Now;
                    break;
                default:
                    break;
            }
            Console.WriteLine(log + desc);
            return;
        }
    }
}
