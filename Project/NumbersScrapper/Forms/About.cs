using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NumbersScrapper.Forms
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void About_Load(object sender, EventArgs e)
        {
            var appsetting = Properties.Settings.Default;
            label1.Text = appsetting.appname;
            label2.Text = appsetting.version;
            label3.Text = appsetting.contact;
        }
    }
}
