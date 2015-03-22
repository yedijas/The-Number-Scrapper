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
        public int year { get; set; }
        //HtmlDocument maindoc = new HtmlDocument();
        //HtmlDocument bodoc = new HtmlDocument();
        //HtmlDocument vidsales = new HtmlDocument();
        //HtmlDocument roles = new HtmlDocument();

        public SingleMovieLink(string _url, int _year)
        {
            url = @"http://www.the-numbers.com/" + _url.Split('#').First();
            year = _year;
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
                catch (Exception e)
                {
                    downloaded = false;
                    count++;
                    if (count >= 3)
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "error loading the movie");
                        Helper.WriteToError(url, e.StackTrace, year);
                    }
                    else
                    {
                        Task.Delay(5000);
                    }
                }
            }
            if (downloaded)
            {
                try
                {
                    movieDB.ID = dc.GetMovieID();
                    // get summary div
                    var summ = doc.DocumentNode.Descendants("div")
                        .Single(o => o.HasAttributes && o.Attributes.Any(p => p.Name.Equals("id"))
                        && o.Attributes["id"].Value.Equals("summary"));

                    // title
                    movieDB.Title = doc.DocumentNode.Descendants("h1").Single(tit => tit.HasAttributes && tit.Attributes.Any(attr => attr.Name.ToLowerInvariant().Equals("itemprop")) && tit.Attributes["itemprop"].Value.ToLowerInvariant().Equals("name"))
                        .InnerText.Trim();

                    // rating
                    var rating = summ.Descendants("table").FirstOrDefault();
                    if (rating.Descendants("tr").Count() > 1)
                    {
                        // rating exist
                        foreach (var a in rating.Descendants("tr").LastOrDefault().Descendants("a"))
                        {
                            if (a.InnerText.ToLowerInvariant().Contains("critics"))
                            { // critics
                                string temprating = Regex.Match(a.InnerText, @"\d+").Value;
                                movieDB.RTCRating = Int32.Parse(temprating);
                            }
                            else
                            { // audience
                                string temprating = Regex.Match(a.InnerText, @"\d+").Value;
                                movieDB.RTARating = Int32.Parse(temprating);
                            }
                        }
                    }

                    movieDB.year = this.year;

                    // general desc
                    var gen = summ.Descendants("table").LastOrDefault();
                    foreach (var tr in gen.Descendants("tr"))
                    {
                        if (tr.InnerText.ToLowerInvariant().Contains("budget"))
                        {
                            var tempbudget = tr.InnerText.Split(':').LastOrDefault()
                                .Trim().Replace("$", string.Empty).Replace(",", string.Empty).Replace("&nbsp;", string.Empty);
                            movieDB.Budget = Int32.Parse(tempbudget);
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
                                HtmlNode publisher = rel.Count() > 2 ? HtmlNode.CreateNode(rel[1].Trim()) : null;
                                ReleaseDate reldateDBtemp = new ReleaseDate();
                                reldateDBtemp.ReleaseDate1 = realreldate;
                                reldateDBtemp.Remarks = kind;
                                reldateDBtemp.Company = publisher == null ? "Unknown" : publisher.InnerText;
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
                            //Console.WriteLine(mpaarating);
                            movieDB.MPAARating = mpaarating;
                        }
                        else if (tr.InnerText.ToLowerInvariant().IndexOf("running") == 0)
                        {
                            var runningtime = Int32.Parse(Regex.Match(tr.Descendants("td").LastOrDefault()
                                .InnerText, @"\d+").Value);
                            movieDB.RunningTime = runningtime;
                        }
                        else if (tr.InnerText.ToLowerInvariant().IndexOf("franchise") == 0)
                        {
                            var franchise = tr.Descendants("td").LastOrDefault()
                                .Descendants("a").Single().InnerText;
                            movieDB.Franchise = franchise;
                        }
                        else if (tr.InnerText.ToLowerInvariant().IndexOf("genre") == 0)
                        {
                            var genre = tr.Descendants("td").LastOrDefault()
                                .Descendants("a").Single().InnerText;
                            //Console.WriteLine(genre);
                            movieDB.Genre = genre;
                        }
                        else if (tr.InnerText.ToLowerInvariant().IndexOf("compan") == 0)
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
                        var check = dc.CheckIfExistsTheSame(movieDB.Title, year);
                        if (check.Equals("false"))
                        {
                            dc.ReleaseDates.InsertAllOnSubmit(reldateDB.Select(rd => { rd.MovieID = movieDB.ID; return rd; }).ToList());
                            dc.Movies.InsertOnSubmit(movieDB);
                            dc.SubmitChanges();
                            result = movieDB.ID;
                        }
                        else
                        {
                            result = check;
                        }
                    }
                    catch (Exception e)
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "error loading the movie");
                        Helper.WriteToError(url, e.StackTrace, year);
                    }
                }
                catch
                {
                    Console.WriteLine("");
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
        }

        private void GetDailyBO(string movieID)
        {
            bool downloaded = false;
            int count = 0;
            HtmlDocument doc = new HtmlDocument();
            TheNumbersDataContext dc = new TheNumbersDataContext();
            List<DailyBO> dboDB = new List<DailyBO>();

            // clean everything first
            dc.DailyBOs.DeleteAllOnSubmit(dc.DailyBOs.Where(dc1 => dc1.MovieID.Equals(movieID)));
            dc.SubmitChanges();

            while (!downloaded)
            {
                try
                {
                    doc.LoadHtml(Helper.GetHTML(url + @"#tab=box-office"));
                    downloaded = true;
                }
                catch (Exception e)
                {
                    downloaded = false;
                    count++;
                    if (count >= 3)
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "error loading the movie");
                        Helper.WriteToError(url, e.StackTrace, year);
                    }
                    else
                    {
                        Task.Delay(5000);
                    }
                }
            }
            if (downloaded)
            {
                try
                {
                    var table1 = doc.DocumentNode.Descendants("div")
                        .First(div => div.HasAttributes && div.Attributes.Any(attrib => attrib.Name.Equals("id")) && div.Attributes["id"].Value.Equals("box-office"));
                    if (table1.Descendants("div").Count() > 1)
                    {
                        var table2 = table1
                                .Descendants("div").First(obj => obj.HasAttributes && obj.Attributes.Any(attr => attr.Name.Equals("id")) && obj.Attributes["id"].Value.Equals("box_office_chart"));
                        var table = table2
                                .Descendants("table").FirstOrDefault();
                        List<HtmlNode> trs = table.Descendants("tr").ToList();
                        trs.RemoveAt(0); // remove header

                        try
                        {
                            foreach (HtmlNode tr in trs)
                            {
                                var tds = tr.Descendants("td").ToArray();
                                DailyBO tempDBO = new DailyBO { MovieID = movieID };
                                tempDBO.DateCounted = DateTime.ParseExact(tds[0].Descendants("a").Single().InnerText, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                                tempDBO.Rank = tds[1].InnerText.Equals("-") ? 0 : Int32.Parse(tds[1].InnerText);
                                tempDBO.Gross = Double.Parse(tds[2].InnerText.Replace("$", String.Empty).Replace(",", String.Empty).Replace("&nbsp;", string.Empty));
                                tempDBO.TheatersCount = Int32.Parse(tds[4].InnerText.Replace(",", string.Empty));
                                tempDBO.TotalGross = Double.Parse(tds[6].InnerText.Replace("$", string.Empty).Replace(",", string.Empty).Replace("&nbsp;", string.Empty));
                                tempDBO.NumDays = Int32.Parse(tds[7].InnerText.Replace("$", string.Empty).Replace(",", string.Empty).Replace("&nbsp;", string.Empty));
                                dboDB.Add(tempDBO);
                            }
                        }
                        catch
                        {
                            throw;
                        }

                        dc.DailyBOs.InsertAllOnSubmit(dboDB);
                        dc.SubmitChanges();
                    }
                }
                catch (Exception e)
                {
                    Helper.WriteToLog(ProgramStatus.Error, "error loading the movie");
                    Helper.WriteToError(url, e.StackTrace, year);
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

            // clean everything first
            TheNumbersDataContext dc = new TheNumbersDataContext();
            dc.Videos.DeleteAllOnSubmit(dc.Videos.Where(dc1 => dc1.MovieID.Equals(movieID)));
            dc.SubmitChanges();

            while (!downloaded)
            {
                try
                {
                    doc.LoadHtml(Helper.GetHTML(url + @"#tab=video-sales"));
                    downloaded = true;
                }
                catch (Exception e)
                {
                    downloaded = false;
                    count++;
                    if (count >= 3)
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "error loading the movie");
                        Helper.WriteToError(url, e.StackTrace, year);
                    }
                    else
                    {
                        Task.Delay(5000);
                    }
                }
            }
            if (downloaded)
            {
                try
                {
                    var table1 = doc.DocumentNode.Descendants("div")
                        .First(div => div.HasAttributes && div.Attributes.Any(attrib => attrib.Name.Equals("id")) && div.Attributes["id"].Value.Equals("video-sales"));
                    if (table1.Descendants("div").Count() > 0)
                    {
                        var table = table1.Descendants("div")
                            .Where(obj => obj.HasAttributes && obj.Attributes.Any(attr => attr.Name.Equals("id"))
                                && obj.Attributes["id"].Value.Equals("box_office_chart"));
                        if (table.Count() > 0)
                        {
                            GetDVDSales(table.First(), movieID);
                            if (table.Count() == 2)
                                GetBluRaySales(table.Last(), movieID);
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        private void GetDVDSales(HtmlNode div, string movieID)
        {
            TheNumbersDataContext dc = new TheNumbersDataContext();
            List<Video> vidDB = new List<Video>();

            var trs = div.Descendants("table").FirstOrDefault().Descendants("tr").ToList();
            trs.Remove(trs.First());

            try
            {
                foreach (var tr in trs)
                {
                    var tds = tr.Descendants("td").ToList();

                    Video tempVid = new Video { MovieID = movieID, Type = "DVD" };
                    tempVid.DateCounted = DateTime.ParseExact(tds[0].Descendants("a").Single().InnerText, "M/d/yyyy", CultureInfo.InvariantCulture);
                    tempVid.Rank = tds[1].InnerText.Equals("-") ? 0 : Int32.Parse(tds[1].InnerText.Trim());
                    tempVid.Units = Int32.Parse(tds[2].InnerText.Replace(",", string.Empty).Trim());
                    tempVid.Spending = Int32.Parse(tds[5].InnerText.Replace("$", string.Empty).Replace(",", string.Empty).Trim().Replace("&nbsp;", string.Empty));
                    tempVid.TotalSpending = Int32.Parse(tds[6].InnerText.Replace("$", string.Empty).Replace(",", string.Empty).Trim().Replace("&nbsp;", string.Empty));
                    tempVid.Week = Int32.Parse(tds[7].InnerText.Trim().Replace("$", string.Empty).Replace(",", string.Empty).Replace("&nbsp;", string.Empty));

                    vidDB.Add(tempVid);
                }
                try
                {
                    dc.Videos.InsertAllOnSubmit(vidDB);
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                Helper.WriteToLog(ProgramStatus.Error, "error loading the movie");
                Helper.WriteToError(url, e.StackTrace, year);
            }
        }

        private void GetBluRaySales(HtmlNode div, string movieID)
        {

            TheNumbersDataContext dc = new TheNumbersDataContext();
            List<Video> vidDB = new List<Video>();

            var trs = div.Descendants("table").FirstOrDefault().Descendants("tr").ToList();
            trs.Remove(trs.First());

            try
            {
                foreach (var tr in trs)
                {
                    var tds = tr.Descendants("td").ToList();

                    Video tempVid = new Video { MovieID = movieID, Type = "BluRay" };
                    tempVid.DateCounted = DateTime.ParseExact(tds[0].Descendants("a").Single().InnerText, "M/d/yyyy", CultureInfo.InvariantCulture);
                    tempVid.Rank = tds[1].InnerText.Equals("-") ? 0 : Int32.Parse(tds[1].InnerText.Trim());
                    tempVid.Units = Int32.Parse(tds[2].InnerText.Replace(",", string.Empty).Trim());
                    tempVid.Spending = Int32.Parse(tds[5].InnerText.Replace("$", string.Empty).Replace(",", string.Empty).Trim().Replace("&nbsp;", string.Empty));
                    tempVid.TotalSpending = Int32.Parse(tds[6].InnerText.Replace("$", string.Empty).Replace(",", string.Empty).Trim().Replace("&nbsp;", string.Empty));
                    tempVid.Week = Int32.Parse(tds[7].InnerText.Trim().Replace("$", string.Empty).Replace(",", string.Empty).Replace("&nbsp;", string.Empty));

                    vidDB.Add(tempVid);
                }
                try
                {
                    dc.Videos.InsertAllOnSubmit(vidDB);
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                Helper.WriteToLog(ProgramStatus.Error, "error loading the movie");
                Helper.WriteToError(url, e.StackTrace, year);
            }


        }
        #endregion

        #region cast and crews
        /// <summary>
        /// get the #tab=cast-and-crew page
        /// </summary>
        private void GetRole(string movieID)
        {
            // clean everything first
            TheNumbersDataContext dc = new TheNumbersDataContext();
            dc.Roles.DeleteAllOnSubmit(dc.Roles.Where(dc1 => dc1.IDMovie.Equals(movieID)));
            dc.SubmitChanges();

            bool downloaded = false;
            int count = 0;
            HtmlDocument doc = new HtmlDocument();
            List<Role> dboDB = new List<Role>();

            while (!downloaded)
            {
                try
                {
                    doc.LoadHtml(Helper.GetHTML(url + @"#tab=cast-and-crew"));
                    downloaded = true;
                }
                catch (Exception e)
                {
                    downloaded = false;
                    count++;
                    if (count >= 3)
                    {
                        Helper.WriteToLog(ProgramStatus.Error, "error loading the movie");
                        Helper.WriteToError(url, e.StackTrace, year);
                    }
                    else
                    {
                        Task.Delay(5000);
                    }
                }
            }
            if (downloaded)
            {
                try
                {
                    var tables = doc.DocumentNode.Descendants("div")
                        .First(div => div.HasAttributes && div.Attributes.Any(attrib => attrib.Name.Equals("id")) && div.Attributes["id"].Value.Equals("cast-and-crew"))
                            .Descendants("div");

                    foreach (var table in tables)
                    {
                        dboDB = new List<Role>();
                        List<HtmlNode> trs = table.Descendants("tr").ToList();
                        //trs.RemoveAt(0); // remove header

                        foreach (HtmlNode tr in trs)
                        {
                            var tds = tr.ChildNodes.Where(o => o.Name.Equals("td"));
                            Role tempDBO = new Role { IDMovie = movieID, Name = "", Role1 = "" };

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
                            throw;
                        }
                    }
                }
                catch (Exception e)
                {
                    Helper.WriteToLog(ProgramStatus.Error, "error loading the movie");
                    Helper.WriteToError(url, e.StackTrace, year);
                }
            }
        }
        #endregion
        #endregion
    }
}
