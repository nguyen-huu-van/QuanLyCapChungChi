using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace QuanLyCapChungChi
{
    public partial class FrmTrangChu : Form
    {
        SqlConnection MyCon = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456");
        public FrmTrangChu()
        {
            InitializeComponent();
        }

        private void FrmTrangChu_Load(object sender, EventArgs e)
        {
            System.Timers.Timer Timer = new System.Timers.Timer();
            Timer.Interval = 1000;
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();

            //Điếm số lượng học viên
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();
            string sql3 = "SELECT COUNT(MAHOCVIEN) FROM HOCVIEN";
            SqlCommand cmd3 = new SqlCommand(sql3, MyCon);
            int sl = (int)cmd3.ExecuteScalar();
            txtTongHocVien.Text = sl.ToString();
            MyCon.Close();

            //Điếm số lượng giảng viên
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();
            string sql4 = "SELECT COUNT(MAGIANGVIEN) FROM GIANGVIEN";
            SqlCommand cmd4 = new SqlCommand(sql4, MyCon);
            int slgv = (int)cmd4.ExecuteScalar();
            txtTongGiangVien.Text = slgv.ToString();
            MyCon.Close();

            //Điếm số lượng khóa học
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();
            string sql5 = "SELECT COUNT(MAKHOAHOC) FROM KHOAHOC";
            SqlCommand cmd5 = new SqlCommand(sql5, MyCon);
            int slkh = (int)cmd5.ExecuteScalar();
            txtTongKhoaHoc.Text = slkh.ToString();
            MyCon.Close();

            //Điếm số lượng chứng chỉ
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();
            string sql6 = "SELECT COUNT(MAKHOATHI) FROM KHOATHI";
            SqlCommand cmd6 = new SqlCommand(sql6, MyCon);
            int slcc = (int)cmd6.ExecuteScalar();
            txtTongChungChi.Text = slcc.ToString();
            MyCon.Close();
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (btnDangXuat.InvokeRequired)
            {
                btnDangXuat.Invoke((MethodInvoker)delegate
                {
                    btnDangXuat.Text = DateTime.Now.ToString("hh:mm:ss");
                    btnDangXuat.Value = DateTime.Now.Second;
                });
            }
            else
            {
                btnDangXuat.Text = DateTime.Now.ToString("hh:mm:ss");
                btnDangXuat.Value = DateTime.Now.Second;
            }
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
