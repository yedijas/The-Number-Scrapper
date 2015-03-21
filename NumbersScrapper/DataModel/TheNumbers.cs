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
            return latestID;
        }
        public string CheckIfExistsTheSame(string title, List<ReleaseDate> rds)
        {
            string result = "false";
            if (this.Movies.Any(mov => mov.Title.Equals(title)))
            {
                var tempMovies = this.Movies.Where(mov => mov.Title.Equals(title));
                foreach (var tempMovie in tempMovies)
                {
                    foreach (var rd in rds)
                    {
                        int year = rd.ReleaseDate1.Year;
                        if (this.ReleaseDates.Any(rd1 => rd1.MovieID.Equals(tempMovie.ID) && rd1.ReleaseDate1.Year == year))
                        {
                            result = tempMovie.ID;
                        }
                        if (!result.Equals("false")) ;
                    }
                }
            }
            return result;
        }
    }
}
