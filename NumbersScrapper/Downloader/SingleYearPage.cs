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
        public List<SingleMovieLink> Movies
        {
            get { return movies; }
        }
        private List<SingleMovieLink> movies;

        public SingleYearPage()
        {
            page = new HtmlDocument();
            movies = new List<SingleMovieLink>();
        }

        /// <summary>
        /// Get list of movies in a single year
        /// </summary>
        /// <param name="year">year number</param>
        public void GetASingleYear(string year){
            bool downloaded = false; // flag that whole page is downloaded
            var yearPage = new HtmlDocument();
            int count = 0;
            while (!downloaded)
            {
                try
                {
                    yearPage.LoadHtml(HelperClass.GetHTML(@"http://www.the-numbers.com/movies/year/" + year));
                    downloaded = true;
                }
                catch
                {
                    downloaded = false;
                    Task.Delay(5000);
                    count++;
                    if (count >= 3)
                        HelperClass.WriteToLog(ScrapperStatus.Error, "Fail download page for year " + year);
                    else
                    {
                        HelperClass.WriteToLog(ScrapperStatus.Error, "Fail download page for year " + year + ". Retry after 5 second...");
                        return;
                    }
                }
            }
            var fillingChart = yearPage.DocumentNode.ChildNodes
                .Single(o => o.HasAttributes && 
                    o.Attributes["id"].ToString().Equals("page_filling_chart"));
            var linkContainer = fillingChart.ChildNodes.Single(o => o.Name.Equals("table"))
                .ChildNodes.Where(o => o.Name.Equals("tr") &&
                    o.ChildNodes.Any(p => p.HasAttributes &&
                        p.Attributes["class"].Value.Equals("data")));
            foreach (HtmlNode tempLinkContainer in linkContainer)
            {
                movies.Add(new SingleMovieLink(tempLinkContainer.ChildNodes.ElementAt(1)
                    .ChildNodes.Single(o => o.Name.Equals("a") && o.HasAttributes)
                    .Attributes["href"].Value));
            }
        }
    }
}
