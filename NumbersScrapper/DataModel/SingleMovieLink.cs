using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace NumbersScrapper.DataModel
{
    class SingleMovieLink
    {
        public string url { get; set; }
        HtmlDocument maindoc = new HtmlDocument();
        HtmlDocument bodoc = new HtmlDocument();
        HtmlDocument vidsales = new HtmlDocument();
        HtmlDocument roles = new HtmlDocument();

        public SingleMovieLink(string _url)
        {
            url = _url;
        }

        public void GetMovie(){

        }

        private void GetReleaseDate()
        {

        }

        private void GetDailyBO()
        {

        }

        private void GetWeeklyBO()
        {

        }

        private void GetRole()
        {

        }
    }
}
