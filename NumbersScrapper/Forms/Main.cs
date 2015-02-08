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
using NumbersScrapper.Helper;

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

        private void btnDownload_Click(object sender, EventArgs e)
        {
            int count = 0;
            if (rbAll.Checked)
            {
                // get all years listed
                List<String> years = new List<string>();

                var yearListPage = new HtmlAgilityPack.HtmlDocument();
                bool downloaded = false;
                while (!downloaded)
                {
                    try
                    {
                        yearListPage.LoadHtml(HelperClass.GetHTML(@"http://www.the-numbers.com/movies/#tab=year"));
                        downloaded = true;
                    }
                    catch
                    {
                        downloaded = false;
                        Task.Delay(5000); // add sleep for 1 sec to delay the fetch
                        count++;
                        if (count >= 3)
                            HelperClass.WriteToLog(ScrapperStatus.Error, "Fail download year list. Aborting.");
                        else
                        {
                            HelperClass.WriteToLog(ScrapperStatus.Error, "Fail download year list. Retry after 5 second...");
                            return;
                        }
                    }
                }
                // download all
                foreach (string year in years)
                {
                    var singleYear = new SingleYearPage();
                    singleYear.GetASingleYear(year);
                    foreach (SingleMovieLink sml in singleYear.Movies)
                    {
                        // get per movies
                        sml.GetMovie();
                    }
                }
            }
            else
            {
                // download by year
                var singleYear = new SingleYearPage();
                singleYear.GetASingleYear(txtYear.Text);
                foreach (SingleMovieLink sml in singleYear.Movies)
                {
                    // get per movies
                    sml.GetMovie();
                }
            }
        }

        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var howto = new HowTo();
            howto.Show();
        }
    }
}
