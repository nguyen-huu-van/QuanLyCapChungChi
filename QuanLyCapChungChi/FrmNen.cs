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
    public partial class FrmNen : Form
    {
        public FrmNen()
        {
            InitializeComponent();
        }

        private void FrmNen_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void bunifuThinButton21_Click(object sender, EventArgs e)
        {
            FrmDangNhap frm = new FrmDangNhap();
            frm.MdiParent = this;
            frm.Show();
        }
    }
}
