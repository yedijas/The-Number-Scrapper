using NumbersScrapper.Downloader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using NumbersScrapper.DataModel;
using NumbersScrapper.HelperClasses;
using MoreLinq;

namespace NumbersScrapper.Forms
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void rbYear_CheckedChanged(object sender, EventArgs e)
        {
            txtYear.Enabled = true;
        }

        private void rbAll_CheckedChanged(object sender, EventArgs e)
        {
            txtYear.Enabled = false;
        }

        private void Redownload_Click(object sender, EventArgs e)
        {
            TheNumbersDataContext dc = new TheNumbersDataContext();
            var errors = dc.Errors.DistinctBy(x => x.link);

            foreach (var error in errors)
            {
                dc.ExecuteCommand("TRUNCATE TABLE [dbo].[log]"); // clears the log first.
                SingleMovieLink tempsml = new SingleMovieLink(error.link, error.year);
                dc.Connection.Close();
                tempsml.GetMovie();
                if (dc.Connection.State.Equals(ConnectionState.Closed))
                    dc.Connection.Open();
                if (!dc.IsThereAnyError())
                {
                    var tempdelete = dc.Errors.Where(err => err.link.Equals(error));
                    dc.Errors.DeleteAllOnSubmit(tempdelete);
                    dc.SubmitChanges();
                }
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            TheNumbersDataContext dc = new TheNumbersDataContext();
            dc.ExecuteCommand("TRUNCATE TABLE [dbo].[log]"); // clears the log first.

            Helper.WriteToLog(ProgramStatus.Start);
            if (rbAll.Checked)
            {
                // get all years listed
                List<String> years = new List<string>();

                var yearListPage = new HtmlAgilityPack.HtmlDocument();
                bool downloaded = false;
                int count = 0;
                while (!downloaded)
                {
                    try
                    {
                        yearListPage.LoadHtml(Helper.GetHTML(@"http://www.the-numbers.com/movies/#tab=year"));
                        downloaded = true;
                    }
                    catch
                    {
                        downloaded = false;
                        count++;
                        if (count >= 3)
                        {
                            Helper.WriteToLog(ProgramStatus.Error, "error loading the entire page");
                        }
                        else
                        {
                            Task.Delay(5000); // add sleep for 5 sec to delay the fetch
                        }
                    }
                }
                // download all
                if (downloaded)
                {
                    // get year list.
                    var table = yearListPage.DocumentNode.Descendants("div").Single(
                        o => o.HasAttributes && o.Attributes.Any(p => p.Name.Equals("id")) &&
                        o.Attributes["id"].Value.Equals("year"))
                        .Descendants("table").FirstOrDefault();
                    var trs = table.Descendants("tr").Reverse().ToList();
                    trs.Remove(trs.Last());

                    foreach (var tr in trs)
                    {
                        years.Add(tr.Descendants("td").FirstOrDefault().Descendants("b")
                            .FirstOrDefault().Descendants("a")
                            .FirstOrDefault().InnerText.Trim());
                    }

                    foreach (string year in years)
                    {
                        this.GetASingleYear(year);
                    }
                    MessageBox.Show("Whole database successfully stored to DB!");
                }
            }
            else
            {
                this.GetASingleYear(txtYear.Text);
            }
            if (dc.Errors.Count() > 0)
            {
                MessageBox.Show("Scrapping completed with error. Please send the entire log from DB to bananab9001@gmail.com");
            }
            Helper.WriteToLog(ProgramStatus.End);
        }

        private void GetASingleYear(string year)
        {
            // download by year
            var singleYear = new SingleYearPage();
            try // added extra checking for if fail to get the movie list.
            {
                singleYear.GetASingleYear(year);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            foreach (SingleMovieLink sml in singleYear.Movies)
            {
                // get per movies
                sml.GetMovie();
                //Console.WriteLine(sml.url);
            }
            MessageBox.Show("Year " + txtYear.Text + "successfully stored to DB!");
        }

        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var howto = new HowTo();
            howto.Show();
        }

        private void txtYear_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnDownload.PerformClick();
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // test modules
            //SingleMovieLink testsml = new SingleMovieLink(@"movie/Dark-Knight-The");
            //testsml.GetMovie();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToExcel export = new ExportToExcel();
            export.Show();
        }
    }
}
