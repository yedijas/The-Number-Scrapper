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
                        yearListPage.LoadHtml(Helper.Helper.GetHTML(@"http://www.the-numbers.com/movies/#tab=year"));
                        downloaded = true;
                    }
                    catch
                    {
                        downloaded = false;
                        Task.Delay(1000); // add sleep for 1 sec to delay the fetch
                        count++;
                    }
                }
                if (count >= 3)
                {
                    MessageBox.Show("Failed three times to download Years List. Please try again later.");
                    return;
                }
                // download all
                foreach (string year in years)
                {
                    var singleYear = new SingleYearPage();
                    singleYear.GetASingleYear(year);
                    for (int i = 1; i <= 12; i++)
                    {
                        foreach (SingleMovieLink sml in singleYear.Monthly)
                        {
                            // get per movies
                            sml.GetMovie();
                        }

                    }
                }
            }
            else
            {
                // download by year
                var singleYear = new SingleYearPage();
                singleYear.GetASingleYear(txtYear.Text);
                for (int i = 1; i <= 12; i++)
                {
                    foreach (SingleMovieLink sml in singleYear.Monthly)
                    {
                        // get per movies
                        sml.GetMovie();
                    }
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
