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
    public partial class FrmGiaoDienChinh : Form
    {
        SqlConnection MyCon = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456");
        public FrmGiaoDienChinh()
        {
            InitializeComponent();
            // Đảm bảo menuContainer có chiều cao ban đầu là chiều cao khi thu gọn
            menuContainer.Height = 38;
            // Đảm bảo trạng thái menuExpand ban đầu khớp với chiều cao ban đầu
            menuExpand = false;
            // Thiết lập Interval cho Timer (ví dụ 15ms cho mượt)
            menuTransition.Interval = 15;

            // Đảm bảo menuContainer có chiều cao ban đầu là chiều cao khi thu gọn
            menuContainer1.Height = 43;
            // Đảm bảo trạng thái menuExpand ban đầu khớp với chiều cao ban đầu
            menuExpand = false;
            // Thiết lập Interval cho Timer (ví dụ 15ms cho mượt)
            menuTransition1.Interval = 15;

       


        }
        private void FrmGiaoDienChinh_Load(object sender, EventArgs e)
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

        private void btnGiangVien_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            frmHocvien guest = new frmHocvien();
            guest.TopLevel = false;
            guest.Dock = DockStyle.Fill;
            guest.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(guest);
            guest.Show();

           
        }

        private void panel_chinh_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnThongKe_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmThongKe guest2 = new FrmThongKe();
            guest2.TopLevel = false;
            guest2.Dock = DockStyle.Fill;
            guest2.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(guest2);
            guest2.Show();
        }

        private void txtDemHocVien_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel_slide_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnDangXuat_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

       

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btn_TrangChu_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmTrangChu home = new FrmTrangChu();
            home.TopLevel = false;
            home.Dock = DockStyle.Fill;
            home.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(home);
            home.Show();
        }

        private void btnHocVien_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            frmHocvien home = new frmHocvien();
            home.TopLevel = false;
            home.Dock = DockStyle.Fill;
            home.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(home);
            home.Show();
        }

        private void btnGiangVien_Click_1(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmGiangvien guest1 = new FrmGiangvien();
            guest1.TopLevel = false;
            guest1.Dock = DockStyle.Fill;
            guest1.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(guest1);
            guest1.Show();
        }

        private void btnKhoaHoc_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmKhoaHoc guest1 = new FrmKhoaHoc();
            guest1.TopLevel = false;
            guest1.Dock = DockStyle.Fill;
            guest1.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(guest1);
            guest1.Show();
        }

        private void btnChungchi_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmDiem home = new FrmDiem();
            home.TopLevel = false;
            home.Dock = DockStyle.Fill;
            home.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(home);
            home.Show();
        }
        bool menuExpand = false;

        private void menuTransition_Tick(object sender, EventArgs e)
        {
            if (menuExpand == false) // Trạng thái hiện tại là đang thu gọn -> Mục tiêu: Mở rộng
            {
                menuContainer.Height += 10;
                // Kiểm tra xem đã mở rộng đủ chưa
                if (menuContainer.Height >= 185)
                {
                    menuTransition.Stop(); // Dừng animation
                    menuExpand = true;    // Cập nhật trạng thái: Đã mở rộng xong
                }
            }
            else // menuExpand == true // Trạng thái hiện tại là đang mở rộng -> Mục tiêu: Thu gọn
            {
                menuContainer.Height -= 10;
                // Kiểm tra xem đã thu gọn đủ chưa
                if (menuContainer.Height <= 46) // Sử dụng <= để đảm bảo dừng đúng
                {
                    menuTransition.Stop(); // Dừng animation
                    menuExpand = false;   // Cập nhật trạng thái: Đã thu gọn xong
                }
            }
        }

        private void btnmenu_Click(object sender, EventArgs e)
        {
            menuTransition.Start();
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            FrmThongKeChiTiet frm = new FrmThongKeChiTiet();
            frm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            frmHocvien home = new frmHocvien();
            home.TopLevel = false;
            home.Dock = DockStyle.Fill;
            home.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(home);
            home.Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmGiangvien guest1 = new FrmGiangvien();
            guest1.TopLevel = false;
            guest1.Dock = DockStyle.Fill;
            guest1.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(guest1);
            guest1.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmKhoaHoc guest1 = new FrmKhoaHoc();
            guest1.TopLevel = false;
            guest1.Dock = DockStyle.Fill;
            guest1.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(guest1);
            guest1.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            menuTransition1.Start();
        }

        private void MenuTransition1_Tick(object sender, EventArgs e)
        {
            if (menuExpand == false) // Trạng thái hiện tại là đang thu gọn -> Mục tiêu: Mở rộng
            {
                menuContainer1.Height += 10;
                // Kiểm tra xem đã mở rộng đủ chưa
                if (menuContainer1.Height >= 160)
                {
                    menuTransition1.Stop(); // Dừng animation
                    menuExpand = true;    // Cập nhật trạng thái: Đã mở rộng xong
                }
            }
            else // menuExpand == true // Trạng thái hiện tại là đang mở rộng -> Mục tiêu: Thu gọn
            {
                menuContainer1.Height -= 10;
                // Kiểm tra xem đã thu gọn đủ chưa
                if (menuContainer1.Height <= 48) // Sử dụng <= để đảm bảo dừng đúng
                {
                    menuTransition1.Stop(); // Dừng animation
                    menuExpand = false;   // Cập nhật trạng thái: Đã thu gọn xong
                }
            }
        }

        private void menuContainer1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void menuContainer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel_main_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtTongChungChi_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtTongKhoaHoc_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txtTongGiangVien_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmThi home = new FrmThi();
            home.TopLevel = false;
            home.Dock = DockStyle.Fill;
            home.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(home);
            home.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmDiem home = new FrmDiem();
            home.TopLevel = false;
            home.Dock = DockStyle.Fill;
            home.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(home);
            home.Show();
        }

        private void btnTaiKhoan_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmDiem home = new FrmDiem();
            home.TopLevel = false;
            home.Dock = DockStyle.Fill;
            home.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(home);
            home.Show();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmTrangChu home = new FrmTrangChu();
            home.TopLevel = false;
            home.Dock = DockStyle.Fill;
            home.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(home);
            home.Show();
        }

        private void button7_Click_2(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmDiem home = new FrmDiem();
            home.TopLevel = false;
            home.Dock = DockStyle.Fill;
            home.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(home);
            home.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmThongKe guest2 = new FrmThongKe();
            guest2.TopLevel = false;
            guest2.Dock = DockStyle.Fill;
            guest2.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(guest2);
            guest2.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmTrangChu home = new FrmTrangChu();
            home.TopLevel = false;
            home.Dock = DockStyle.Fill;
            home.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(home);
            home.Show();
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
        }

        private void button13_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmThongKe guest2 = new FrmThongKe();
            guest2.TopLevel = false;
            guest2.Dock = DockStyle.Fill;
            guest2.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(guest2);
            guest2.Show();
        }
        bool sidebarExpand = true;
        private void slidebarTransition_Tick(object sender, EventArgs e)
        {
            if (sidebarExpand)
            {
                sidebar.Width -= 10;
                if (sidebar.Width <= 46)
                {
                    sidebarExpand = false;
                    sidebarTransition.Stop();
                }
            }
            else
            {
                sidebar.Width += 10;
                if (sidebar.Width >= 263)
                {
                    sidebarExpand = true;
                    sidebarTransition.Stop();
                }
            }       
        }

        private void btnHam_Click(object sender, EventArgs e)
        {
            sidebarTransition.Start();
        }

        private void button7_Click_3(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmKhoạThi home = new FrmKhoạThi();
            home.TopLevel = false;
            home.Dock = DockStyle.Fill;
            home.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(home);
            home.Show();
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmThongKe guest2 = new FrmThongKe();
            guest2.TopLevel = false;
            guest2.Dock = DockStyle.Fill;
            guest2.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(guest2);
            guest2.Show();
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            menuTransition2.Start();
        }

        private void button10_Click_2(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            Frmdangky guest2 = new Frmdangky();
            guest2.TopLevel = false;
            guest2.Dock = DockStyle.Fill;
            guest2.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(guest2);
            guest2.Show();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            panel_chinh.Controls.Clear();
            FrmTaiKhoan guest2 = new FrmTaiKhoan();
            guest2.TopLevel = false;
            guest2.Dock = DockStyle.Fill;
            guest2.FormBorderStyle = FormBorderStyle.None;
            panel_chinh.Controls.Add(guest2);
            guest2.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {

            FrmNen frm = new FrmNen();
            this.Hide();
            frm.Show();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
