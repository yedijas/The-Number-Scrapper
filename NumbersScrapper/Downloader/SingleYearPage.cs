using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NumbersScrapper.DataModel;
using NumbersScrapper.HelperClasses;

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
        public void GetASingleYear(string year)
        {
            bool downloaded = false; // flag that whole page is downloaded
            var yearPage = new HtmlDocument();
            int count = 0;
            while (!downloaded)
            {
                try
                {
                    yearPage.LoadHtml(Helper.GetHTML(@"http://www.the-numbers.com/movies/year/" + year));
                    downloaded = true;
                }
                catch
                {
                    downloaded = false;
                    Task.Delay(5000);
                    count++;
                    if (count >= 3)
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "Fail download page for year " + year  + ". Aborting....");
                        throw new Exception("Fail download page for year " + year + ". Aborting....");
                    }
                    else
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "Fail download page for year " + year + ". Retry after 5 second...");
                    }
                }
            }
            try
            {
                var fillingChart = yearPage.DocumentNode.Descendants("div")
                    .Single(o => o.HasAttributes &&
                        o.Attributes.Any(p => p.Name.Equals("id")) &&
                        o.Attributes["id"].Value.Equals("page_filling_chart"));
                var linkContainer = fillingChart.Descendants("table")
                    .Single(o => o.Name.Equals("table"))
                    .ChildNodes.Where(o => o.Name.Equals("tr") &&
                        o.ChildNodes.Any(p => p.HasAttributes &&
                            p.Attributes.Any(q => q.Name.Equals("class")) &&
                            p.Attributes["class"].Value.Equals("data")));
                foreach (HtmlNode tempLinkContainer in linkContainer)
                {
                    movies.Add(new SingleMovieLink(tempLinkContainer.Descendants("a")
                        .FirstOrDefault().Attributes["href"].Value));
                }
            }
            catch 
            {
                throw;
            }
        }
    }
}
