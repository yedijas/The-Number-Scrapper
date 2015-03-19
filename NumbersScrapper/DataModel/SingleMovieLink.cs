using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NumbersScrapper.HelperClasses;
using System.Text.RegularExpressions;
using System.Globalization;
using NumbersScrapper.DataModel;

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
            url = @"http://www.the-numbers.com/" + _url.Split('#').First();
        }

        public void GetMovie()
        {
            string result = GetGeneralDesc();
            if (!result.Equals(""))
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

        private string GetMovieID(TheNumbersDataContext dc)
        {
            string latestID = (from n in dc.Movies
                               orderby n.ID descending
                               select n.ID).FirstOrDefault();
            latestID = (Int32.Parse(latestID) + 1).ToString();
            return latestID;
        }

        #region individual scrapper
        #region general decsription
        /// <summary>
        /// Get the #tab=summary page
        /// </summary>
        /// <returns>
        /// int the ID of movie in database;
        /// </returns>
        private string GetGeneralDesc()
        {
            string result = ""; // default ID
            bool downloaded = false;
            int count = 0;
            HtmlDocument doc = new HtmlDocument();
            TheNumbersDataContext dc = new TheNumbersDataContext();
            Movy movieDB = new Movy();
            List<ReleaseDate> reldateDB = new List<ReleaseDate>();

            while (!downloaded)
            {
                try
                {
                    doc.LoadHtml(Helper.GetHTML(url + @"#tab=summary"));
                    downloaded = true;
                }
                catch
                {
                    downloaded = false;
                    count++;
                    if (count >= 3)
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "Fail to download movie with URL - " + url + ". Aborting.");
                        return result;
                    }
                    else
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "Fail to download movie with URL - " + url + ". Aborting.");
                        Task.Delay(5000);
                    }
                }
            }
            if (downloaded)
            {
                movieDB.ID = GetMovieID(dc);
                // get summary div
                var summ = doc.DocumentNode.Descendants("div")
                    .Single(o => o.HasAttributes && o.Attributes.Any(p => p.Name.Equals("id"))
                    && o.Attributes["id"].Value.Equals("summary"));

                // rating
                var rating = summ.Descendants("table").FirstOrDefault();
                if (rating.Descendants("tr").Count() > 1)
                {
                    // rating exist
                    foreach (var a in rating.Descendants("tr").LastOrDefault().Descendants("a"))
                    {
                        if (a.InnerText.ToLowerInvariant().Contains("critics"))
                        { // critics
                            /// TODO : Add store to database object.
                            string temprating = Regex.Match(a.InnerText, @"\d+").Value;
                            //Console.WriteLine(temprating);
                            movieDB.RTCRating = Int32.Parse(temprating);
                        }
                        else
                        { // audience
                            /// TODO : Add store to database object.
                            string temprating = Regex.Match(a.InnerText, @"\d+").Value;
                            movieDB.RTARating = Int32.Parse(temprating);
                            //Console.WriteLine(temprating);
                        }
                    }
                }

                // general desc
                var gen = summ.Descendants("table").LastOrDefault();
                foreach (var tr in gen.Descendants("tr"))
                {
                    if (tr.InnerText.ToLowerInvariant().Contains("budget"))
                    {
                        var tempbudget = tr.InnerText.Split(':').LastOrDefault()
                            .Trim().Replace("$", string.Empty).Replace(",", string.Empty);
                        movieDB.Budget = Int32.Parse(tempbudget);
                        //Console.WriteLine(tempbudget);
                    }
                    else if (tr.InnerText.ToLowerInvariant().Contains("domestic"))
                    {
                        foreach (var rels in tr.Descendants("td").LastOrDefault()
                            .InnerHtml.Split(new string[] { "<br>" },
                            StringSplitOptions.RemoveEmptyEntries))
                        {
                            var rel = rels.Split(new string[] { "by" },
                                StringSplitOptions.RemoveEmptyEntries);
                            var kind = Regex.Match(rel[0], @"\((.*?)\)").Value.Equals("") ? "" : Regex.Match(rel[0], @"\((.*?)\)").Value;
                            string reldate = "";
                            DateTime realreldate = new DateTime();
                            if (kind.Length > 0)
                                reldate = rel[0].Remove(rel[0].IndexOf(',') - 2, 2).Replace(kind, String.Empty).Trim();
                            else
                                reldate = rel[0].Remove(rel[0].IndexOf(',') - 2, 2).Trim();
                            realreldate = DateTime.ParseExact(reldate, "MMMM d, yyyy", CultureInfo.InvariantCulture);
                            var publisher = HtmlNode.CreateNode(rel[1].Trim());
                            //Console.WriteLine(kind);
                            //Console.WriteLine(realreldate.ToString());
                            //Console.WriteLine(publisher.InnerText);
                            ReleaseDate reldateDBtemp = new ReleaseDate();
                            reldateDBtemp.ReleaseDate1 = realreldate;
                            reldateDBtemp.Remarks = kind;
                            reldateDBtemp.Company = publisher.InnerText;
                            reldateDBtemp.Desc = kind;
                            reldateDB.Add(reldateDBtemp);
                        }
                    }
                    else if (tr.InnerText.ToLowerInvariant().IndexOf("video") == 0)
                    {
                        foreach (var rels in tr.Descendants("td").LastOrDefault()
                            .InnerHtml.Split(new string[] { "<br>" },
                            StringSplitOptions.RemoveEmptyEntries))
                        {
                            var rel = rels.Split(new string[] { "by" },
                                StringSplitOptions.RemoveEmptyEntries);
                            var kind = Regex.Match(rel[0], @"\((.*?)\)").Value.Equals("") ? "" : Regex.Match(rel[0], @"\((.*?)\)").Value;
                            string reldate = "";
                            DateTime realreldate = new DateTime();
                            if (kind.Length > 0)
                                reldate = rel[0].Remove(rel[0].IndexOf(',') - 2, 2).Replace(kind, String.Empty).Trim();
                            else
                                reldate = rel[0].Remove(rel[0].IndexOf(',') - 2, 2).Trim();
                            realreldate = DateTime.ParseExact(reldate, "MMMM d, yyyy", CultureInfo.InvariantCulture);
                            var publisher = HtmlNode.CreateNode(rel[1].Trim());
                            //Console.WriteLine(kind);
                            //Console.WriteLine(realreldate.ToString());
                            //Console.WriteLine(publisher.InnerText);
                            ReleaseDate reldateDBtemp = new ReleaseDate();
                            reldateDBtemp.ReleaseDate1 = realreldate;
                            reldateDBtemp.Remarks = kind;
                            reldateDBtemp.Company = publisher.InnerText;
                            reldateDBtemp.Desc = kind;
                            reldateDB.Add(reldateDBtemp);
                        }
                    }
                    else if (tr.InnerText.ToLowerInvariant().IndexOf("mpaa") == 0)
                    {
                        var mpaarating = tr.Descendants("td").LastOrDefault().Descendants("a").Single()
                            .InnerText;
                        Console.WriteLine(mpaarating);
                        movieDB.MPAARating = mpaarating;
                    }
                    else if (tr.InnerText.ToLowerInvariant().IndexOf("running") == 0)
                    {
                        var runningtime = Int32.Parse(Regex.Match(tr.Descendants("td").LastOrDefault()
                            .InnerText, @"\d+").Value);
                        //Console.WriteLine(runningtime);
                        movieDB.RunningTime = runningtime;
                    }
                    else if (tr.InnerText.ToLowerInvariant().IndexOf("franchise") == 0)
                    {
                        var franchise = tr.Descendants("td").LastOrDefault()
                            .Descendants("a").Single().InnerText;
                        //Console.WriteLine(franchise);
                        movieDB.Franchise = franchise;
                    }
                    else if (tr.InnerText.ToLowerInvariant().IndexOf("genre") == 0)
                    {
                        var genre = tr.Descendants("td").LastOrDefault()
                            .Descendants("a").Single().InnerText;
                        Console.WriteLine(genre);
                        movieDB.Genre = genre;
                    }
                    else if (tr.InnerText.ToLowerInvariant().IndexOf("production") == 0)
                    {
                        var company = "";
                        foreach (var a in tr.Descendants("td").LastOrDefault()
                            .Descendants("a"))
                        {
                            company += a.InnerText.Trim();
                        }
                        movieDB.Company = company;
                    }
                }
                try
                {
                    dc.ReleaseDates.InsertAllOnSubmit(reldateDB.Select(rd => { rd.MovieID = movieDB.ID; return rd; }).ToList());
                    dc.Movies.InsertOnSubmit(movieDB);
                    dc.SubmitChanges();
                    result = movieDB.ID;
                }
                catch
                {
                    // save error to database.
                }
            }
            return result;
        }
        #endregion

        #region box office
        /// <summary>
        /// get the #tab=box-office page
        /// </summary>
        private void GetBO(string movieID)
        {
            GetDailyBO(movieID);
            //GetWeeklyBO(movieID);
        }

        private void GetDailyBO(string movieID)
        {
            bool downloaded = false;
            int count = 0;
            HtmlDocument doc = new HtmlDocument();
            TheNumbersDataContext dc = new TheNumbersDataContext();
            List<DailyBO> dboDB = new List<DailyBO>();

            while (!downloaded)
            {
                try
                {
                    doc.LoadHtml(Helper.GetHTML(url + @"#tab=box-office"));
                    downloaded = true;
                }
                catch
                {
                    downloaded = false;
                    count++;
                    if (count >= 3)
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "Fail to download box office of a movie with URL - " + url + ". Aborting.");
                    }
                    else
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "Fail to download box office of a movie with URL - " + url + ". Aborting.");
                        Task.Delay(5000);
                    }
                }
            }
            if (downloaded)
            {
                var table = doc.DocumentNode.Descendants("div")
                    .First(div => div.HasAttributes && div.Attributes.Any(attrib => attrib.Name.Equals("class")) && div.Attributes["class"].Value.Equals("content active"))
                        .Descendants("div").First(obj => obj.HasAttributes && obj.Attributes.Any(attr => attr.Name.Equals("id")) && obj.Attributes["id"].Value.Equals("box_office_chart"))
                        .Descendants("table").FirstOrDefault();
                List<HtmlNode> trs = table.Descendants("tr").ToList();
                trs.RemoveAt(0); // remove header
                
                foreach(HtmlNode tr in trs){
                    var tds = tr.Descendants("td").ToArray();
                    DailyBO tempDBO = new DailyBO { MovieID = movieID };
                    tempDBO.DateCounted = DateTime.ParseExact(tds[0].Descendants("a").Single().InnerText, "YYYY/MM/dd", CultureInfo.InvariantCulture);
                    tempDBO.Rank = Int32.Parse(tds[1].InnerText);
                    tempDBO.Gross = Double.Parse(tds[2].InnerText.Replace("$", String.Empty).Replace(",",String.Empty));
                    tempDBO.TheatersCount = Int32.Parse(tds[4].InnerText.Replace(",", string.Empty));
                    tempDBO.TotalGross = Double.Parse(tds[6].InnerText.Replace("$", string.Empty).Replace(",",string.Empty));
                    tempDBO.NumDays = Int32.Parse(tds[7].InnerText);
                    dboDB.Add(tempDBO);
                }

                try
                {
                    dc.DailyBOs.InsertAllOnSubmit(dboDB);
                    dc.SubmitChanges();
                }
                catch
                {
                    // save to DB
                }
            }
        }

        //private void GetWeeklyBO(long movieID)
        //{

        //}
        #endregion

        #region Video Sales
        /// <summary>
        /// get the #tab=video-sales page
        /// </summary>
        private void GetVideoSales(string movieID)
        {
            bool downloaded = false;
            int count = 0;
            HtmlDocument doc = new HtmlDocument();

            while (!downloaded)
            {
                try
                {
                    doc.LoadHtml(Helper.GetHTML(url + @"#tab=video-sales"));
                    downloaded = true;
                }
                catch
                {
                    downloaded = false;
                    count++;
                    if (count >= 3)
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "Fail to download box office of a movie with URL - " + url + ". Aborting.");
                    }
                    else
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "Fail to download box office of a movie with URL - " + url + ". Aborting.");
                        Task.Delay(5000);
                    }
                }
            }
            if (downloaded)
            {
                var table = doc.DocumentNode.Descendants("div")
                    .First(div => div.HasAttributes && div.Attributes.Any(attrib => attrib.Name.Equals("class")) && div.Attributes["class"].Value.Equals("content active"))
                        .Descendants("div").Where(obj => obj.HasAttributes && obj.Attributes.Any(attr => attr.Name.Equals("id")) && obj.Attributes["id"].Value.Equals("box_office_chart"));


                GetDVDSales(table.First(), movieID);
                GetBluRaySales(table.Last(), movieID);
            }
        }

        private void GetDVDSales(HtmlNode div, string movieID)
        {
            TheNumbersDataContext dc = new TheNumbersDataContext();
            List<DailyBO> dboDB = new List<DailyBO>();
        }

        private void GetBluRaySales(HtmlNode div, string movieID)
        {
            TheNumbersDataContext dc = new TheNumbersDataContext();
            
        }
        #endregion

        #region cast and crews
        /// <summary>
        /// get the #tab=cast-and-crew page
        /// </summary>
        private void GetRole(string movieID)
        {

            bool downloaded = false;
            int count = 0;
            HtmlDocument doc = new HtmlDocument();
            TheNumbersDataContext dc = new TheNumbersDataContext();
            List<Role> dboDB = new List<Role>();

            while (!downloaded)
            {
                try
                {
                    doc.LoadHtml(Helper.GetHTML(url + @"#tab=cast-and-crew"));
                    downloaded = true;
                }
                catch
                {
                    downloaded = false;
                    count++;
                    if (count >= 3)
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "Fail to download crews of a movie with URL - " + url + ". Aborting.");
                    }
                    else
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "Fail to download crews of a movie with URL - " + url + ". Aborting.");
                        Task.Delay(5000);
                    }
                }
            }
            if (downloaded)
            {
                var tables = doc.DocumentNode.Descendants("div")
                    .First(div => div.HasAttributes && div.Attributes.Any(attrib => attrib.Name.Equals("class")) && div.Attributes["class"].Value.Equals("content active"))
                        .Descendants("div");

                foreach (var table in tables)
                {
                    List<HtmlNode> trs = table.Descendants("tr").ToList();
                    trs.RemoveAt(0); // remove header

                    foreach (HtmlNode tr in trs)
                    {
                        var tds = tr.Descendants("td").ToList();
                        Role tempDBO = new Role { IDMovie = movieID };

                        foreach (var td in tds)
                        {
                            if (!String.IsNullOrEmpty(td.InnerText.Replace(@"&nbsp;", String.Empty)))
                            {
                                if (td.Descendants("b").Count() > 0)
                                {
                                    tempDBO.Name = td.Descendants("a").Single().InnerText.Replace("&nbsp;", String.Empty).Trim();
                                }
                                else
                                {
                                    tempDBO.Role1 = td.InnerText.Replace("&nbsp;", String.Empty).Trim();
                                }
                            }
                        }

                        dboDB.Add(tempDBO);
                    }

                    try
                    {
                        dc.Roles.InsertAllOnSubmit(dboDB);
                        dc.SubmitChanges();
                    }
                    catch
                    {
                        // save to DB
                    }
                }
            }
        }
        #endregion
        #endregion
    }
}
