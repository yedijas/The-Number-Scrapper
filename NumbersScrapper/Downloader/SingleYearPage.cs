using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NumbersScrapper.DataModel;
using NumbersScrapper.Helper;

namespace NumbersScrapper.Downloader
{
    /// <summary>
    /// Get a Movie Yearly Page
    /// </summary>
    class SingleYearPage
    {
        private HtmlDocument page;
        public List<SingleMovieLink> Monthly
        {
            get { return monthly;  }
        }
        private List<SingleMovieLink> monthly;
        
        public SingleYearPage()
        {
            page = new HtmlDocument();
            monthly = new List<SingleMovieLink>();
        }

        /// <summary>
        /// Get list of movies in a single year
        /// </summary>
        /// <param name="year">year number</param>
        public void GetASingleYear(string year){
            
        }
    }
}
