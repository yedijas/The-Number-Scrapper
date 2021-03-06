﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NumbersScrapper.DataModel;

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

            try
            {
                TheNumbersDataContext dc = new TheNumbersDataContext();

                dc.Logs.InsertOnSubmit(new DataModel.Log { LogTime = DateTime.Now, Desc = log });
                dc.SubmitChanges();
            }
            catch
            {
                throw;
            }

            return;
        }

        public static void WriteToError(string _link, string _detail, int _year)
        {
            TheNumbersDataContext dc = new TheNumbersDataContext();
            dc.Errors.InsertOnSubmit(new Error { detail = _detail, link = _link.Replace(@"http://www.the-numbers.com/",String.Empty), year = _year });
            dc.SubmitChanges();
        }
    }
}
