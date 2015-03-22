using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NumbersScrapper.DataModel;
using System.IO;
using MoreLinq;
using Excel = Microsoft.Office.Interop.Excel;

namespace NumbersScrapper.Forms
{
    public partial class ExportToExcel : Form
    {
        string mainpath = @"C:\temp\NumbersScrapper";
        public ExportToExcel()
        {
            InitializeComponent();
        }

        private void ExportToExcel_Load(object sender, EventArgs e)
        {
            txtPath.Text = mainpath;
            LoadListBox();
        }

        private void LoadListBox()
        {
            TheNumbersDataContext dc = new TheNumbersDataContext();

            var years = dc.Movies.GroupBy(x => x.year).Select(y => y.Key);
            lstYear.DataSource = years;
            lstYear.Update();
        }

        private void SelectAllInList()
        {
            for (int i = 0; i < lstYear.Items.Count; i++)
            {
                lstYear.SetSelected(i, true);
            }
        }

        private void ExportData()
        {
            TheNumbersDataContext dc = new TheNumbersDataContext();
            foreach (var item in lstYear.SelectedItems)
            {
                if (!Directory.Exists(mainpath + @"\" + item.ToString()))
                {
                    Directory.CreateDirectory(mainpath + @"\" + item.ToString());
                }

                foreach (var movie in dc.Movies.Where(x => x.year == Int32.Parse(item.ToString())))
                {
                    CreateExcel(dc, movie, mainpath + @"\" + item.ToString());
                }

            }
        }

        private void CreateExcel(TheNumbersDataContext dc, Movy movie, string path)
        {
            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = false;
            Excel.Workbook wb = excelApp.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
            Excel.Worksheet summ = (Excel.Worksheet)wb.Sheets[1];
            try
            {
                if (!Directory.Exists(path + @"\" + movie.Title))
                {
                    Directory.CreateDirectory(path + @"\" + movie.Title);
                }

                summ.Name = "@Summary";
                summ.get_Range("A1", "A1").Value2 = "Title";
                summ.get_Range("A2", "A2").Value2 = "Budget";
                summ.get_Range("A3", "A3").Value2 = "MPAA Rating";
                summ.get_Range("A4", "A4").Value2 = "Running Time";
                summ.get_Range("A5", "A5").Value2 = "Franchise";
                summ.get_Range("A6", "A6").Value2 = "Genre";
                summ.get_Range("A7", "A7").Value2 = "Company";
                summ.get_Range("A8", "A8").Value2 = "RT Critics";
                summ.get_Range("A9", "A9").Value2 = "RT Audiences";
                summ.get_Range("A10", "A10").Value2 = "Released on";

                summ.get_Range("B1", "B1").Value2 = movie.Title;
                summ.get_Range("B2", "B2").Value2 = movie.Budget;
                summ.get_Range("B3", "B3").Value2 = movie.MPAARating;
                summ.get_Range("B4", "B4").Value2 = movie.RunningTime;
                summ.get_Range("B5", "B5").Value2 = movie.Franchise;
                summ.get_Range("B6", "B6").Value2 = movie.Genre;
                summ.get_Range("B7", "B7").Value2 = movie.Company;
                summ.get_Range("B8", "B8").Value2 = movie.RTCRating;
                summ.get_Range("B9", "B9").Value2 = movie.RTARating;

                int counter = 0;
                foreach (var reldate in dc.ReleaseDates.Where(o => o.MovieID == movie.ID))
                {
                    counter++;
                    summ.get_Range("B" + (counter + 9), "B" + (counter + 9)).Value2 = reldate.ReleaseDate1 + reldate.Desc + " by " + reldate.Company;
                }


                Excel.Worksheet DBO = (Excel.Worksheet)excelApp.Worksheets.Add();

                DBO.Name = @"BOX OFFICE";
                DBO.get_Range("A1", "A1").Value2 = @"Rank";
                DBO.get_Range("B1", "B1").Value2 = @"Gross";
                DBO.get_Range("C1", "C1").Value2 = @"Theatre Count";
                DBO.get_Range("D1", "D1").Value2 = @"Total Gross";
                DBO.get_Range("E1", "E1").Value2 = @"Num Days";
                DBO.get_Range("F1", "F1").Value2 = @"Date";

                counter = 0;
                foreach (var BO in dc.DailyBOs.Where(o => o.MovieID == movie.ID))
                {
                    counter++;
                    DBO.get_Range("A" + counter, "A" + counter).Value2 = BO.Rank;
                    DBO.get_Range("B" + counter, "B" + counter).Value2 = BO.Gross;
                    DBO.get_Range("C" + counter, "C" + counter).Value2 = BO.TheatersCount;
                    DBO.get_Range("D" + counter, "D" + counter).Value2 = BO.TotalGross;
                    DBO.get_Range("E" + counter, "E" + counter).Value2 = BO.NumDays;
                    DBO.get_Range("F" + counter, "F" + counter).Value2 = BO.DateCounted;
                }

                Excel.Worksheet ROLE = (Excel.Worksheet)excelApp.Worksheets.Add();

                ROLE.Name = @"Cast & Crew";
                ROLE.get_Range("A1", "A1").Value2 = @"Role";
                ROLE.get_Range("B1", "B1").Value2 = @"Name";

                counter = 0;
                foreach (var Ro in dc.Roles.Where(o => o.IDMovie == movie.ID))
                {
                    counter++;
                    ROLE.get_Range("A" + counter, "A" + counter).Value2 = Ro.Role1;
                    ROLE.get_Range("B" + counter, "B" + counter).Value2 = Ro.Name;
                }

                Excel.Worksheet VidSales = (Excel.Worksheet)excelApp.Worksheets.Add();

                VidSales.Name = @"Video Sales";
                VidSales.get_Range("A1", "A1").Value2 = @"Rank";
                VidSales.get_Range("B1", "B1").Value2 = @"Units";
                VidSales.get_Range("C1", "C1").Value2 = @"Spending";
                VidSales.get_Range("D1", "D1").Value2 = @"Week";
                VidSales.get_Range("E1", "E1").Value2 = @"Total Spending";
                VidSales.get_Range("F1", "F1").Value2 = @"Type";
                VidSales.get_Range("G1", "G1").Value2 = @"Date";

                counter = 0;
                foreach (var VS in dc.Videos.Where(o => o.MovieID == movie.ID))
                {
                    counter++;
                    VidSales.get_Range("A" + counter, "A" + counter).Value2 = VS.Rank;
                    VidSales.get_Range("B" + counter, "B" + counter).Value2 = VS.Units;
                    VidSales.get_Range("C" + counter, "C" + counter).Value2 = VS.Spending;
                    VidSales.get_Range("D" + counter, "D" + counter).Value2 = VS.Week;
                    VidSales.get_Range("E" + counter, "E" + counter).Value2 = VS.TotalSpending;
                    VidSales.get_Range("F" + counter, "F" + counter).Value2 = VS.Type;
                    VidSales.get_Range("G" + counter, "G" + counter).Value2 = VS.DateCounted;
                }
                wb.SaveAs(path + @"\" + movie.Title + @"\" + movie.ID + "-" + movie.Title + @".xls");
                wb.Close();
                excelApp.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
            finally
            {
                excelApp.Quit();
            }
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Export all will be time comsumpting. Better be done from SQL management studio.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dr == DialogResult.Yes)
            {
                SelectAllInList();
                // export
                ExportData();
            }
        }

        private void btnSome_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("This will export some years.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dr == DialogResult.Yes)
            {
                // export
                ExportData();
            }
        }

        private void txtPath_Click(object sender, EventArgs e)
        {
            var result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                mainpath = folderDlg.SelectedPath.ToString() + @"\NumbersScrapper";
            }
            else
            {
                MessageBox.Show(@"The main folder will now be set into C:\temp!");
            }
            if (!Directory.Exists(mainpath))
            {
                Directory.CreateDirectory(mainpath);
            }
            txtPath.Text = mainpath;
        }
    }
}
