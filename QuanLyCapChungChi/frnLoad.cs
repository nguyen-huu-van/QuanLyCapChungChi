using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyCapChungChi
{
    public partial class frnLoad : Form
    {
        public frnLoad()
        {
            InitializeComponent();
        }
        int startpoint = 0;

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void frnLoad_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void guna2ProgressBar1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            startpoint += 2;
            guna2ProgressBar1.Value = startpoint;
            if(guna2ProgressBar1.Value==100)
            {
                guna2ProgressBar1.Value = 0;
                timer1.Stop();
                this.Hide();

                FrmNen frm = new FrmNen();
                frm.Show();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
