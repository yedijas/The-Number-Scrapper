using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumbersScrapper.DataModel
{
    partial class TheNumbersDataContext
    {

        public string GetMovieID()
        {
            string latestID = (from n in this.Movies
                               orderby n.ID descending
                               select n.ID).FirstOrDefault();
            latestID = String.IsNullOrEmpty(latestID) ? "0" : latestID;
            latestID = (Int32.Parse(latestID) + 1).ToString();

            while (latestID.Length < 50)
            {
                latestID = "0" + latestID;
            }

            return latestID;
        }
        public string CheckIfExistsTheSame(string title, int _year)
        {
            string result = "false";
            if (this.Movies.Any(mov => mov.Title.Equals(title)))
            {
                var isExist = this.Movies.Any(mov => mov.Title.Equals(title) && mov.year == _year);
                if (isExist)
                {
                    result = this.Movies.Single(mov => mov.Title.Equals(title) && mov.year == _year).ID;
                }
            }
            return result;
        }

        public bool IsThereAnyError()
        {
            return this.Logs.Any(logs => logs.Desc.Contains("ERROR"));
        }

        public List<string> GetAllYearsInDatabase()
        {
            List<string> years = new List<string>();

            var tempyears = from movieData in this.Movies
                            group movieData by movieData.year into yearGrouping
                            select yearGrouping.Key;

            return years;
        }
    }
}
