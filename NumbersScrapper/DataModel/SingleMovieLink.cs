using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NumbersScrapper.HelperClasses;

namespace NumbersScrapper.DataModel
{
    class SingleMovieLink
    {
        public string url { get; set; }
        //HtmlDocument maindoc = new HtmlDocument();
        //HtmlDocument bodoc = new HtmlDocument();
        //HtmlDocument vidsales = new HtmlDocument();
        //HtmlDocument roles = new HtmlDocument();

        public SingleMovieLink(string _url)
        {
            url = _url.Split('#').First();
        }

        public void GetMovie()
        {
            long result = GetGeneralDesc();
            if (result != 0)
            {
                GetBO(result);
                GetVideoSales(result);
                GetRole(result);
            }
            else
            {
                throw new Exception("Fail to get movie with link " + this.url);
            }
        }

        #region individual scrapper
        #region general decsription
        /// <summary>
        /// Get the #tab=summary page
        /// </summary>
        /// <returns>
        /// int the ID of movie in database;
        /// </returns>
        private long GetGeneralDesc()
        {
            long result = 0; // default ID
            bool downloaded = false;
            int count = 0;
            try
            {
                while (!downloaded)
                {

                }
            }
            catch
            {
                downloaded = false;
                Task.Delay(5000);
                count++;
                if (count >= 3)
                    Helper.WriteToLog(ProgramStatus.Error, "Fail to download movie with URL - " + url + ". Aborting.");
                else
                {
                    Helper.WriteToLog(ProgramStatus.Error, "Fail to download movie with URL - " + url + ". Aborting.");
                }
                return result;
            }
            return result;
        }

        private void GetReleaseDate(HtmlDocument generalPage)
        {

        }
        #endregion

        #region box office
        /// <summary>
        /// get the #tab=box-office page
        /// </summary>
        private void GetBO(long movieID)
        {
            GetDailyBO(movieID);
            GetWeeklyBO(movieID);
        }

        private void GetDailyBO(long movieID)
        {

        }

        private void GetWeeklyBO(long movieID)
        {

        }
        #endregion 

        #region Video Sales
        /// <summary>
        /// get the #tab=video-sales page
        /// </summary>
        private void GetVideoSales(long movieID)
        {
            GetDVDSales(movieID);
            GetBluRaySales(movieID);
        }

        private void GetDVDSales(long movieID)
        {
            
        }

        private void GetBluRaySales(long movieID)
        {

        }
        #endregion

        #region cast and crews
        /// <summary>
        /// get the #tab=cast-and-crew page
        /// </summary>
        private void GetRole(long movieID)
        {

        }
        #endregion
        #endregion
    }
}
