using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using BCrypt.Net;

namespace QuanLyCapChungChi
{
    public partial class FrmDangNhap : Form
    {
        SqlConnection MyCon = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456");

        public FrmDangNhap()
        {
            InitializeComponent();
        }
        public static string QUYEN = "";
        private string LAYQUYEN()
        {
            string phanQuyenResult = null; // Kết quả trả về, mặc định là null (thất bại)
            string matKhauHashedFromDb = null;

            // Lấy thông tin người dùng nhập từ các TextBox (cần đảm bảo chúng có thể truy cập được từ hàm này)
            string taiKhoanNhap = txtTaiKhoan.Text.Trim(); // Nên là txtTaiKhoan (tên control trên form đăng nhập)
            string matKhauNhapPlainText = txtMatkhau.Text; // Nên là txtMatkhau (tên control trên form đăng nhập)

            // Chuỗi kết nối của bạn
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456;Encrypt=False";

            // Sử dụng using để đảm bảo kết nối được đóng đúng cách
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Câu lệnh SQL để lấy MẬT KHẨU ĐÃ BĂM và PHANQUYEN dựa trên TAIKHOAN
                string sql = "SELECT MATKHAU, PHANQUYEN FROM NGUOIDUNG WHERE TAIKHOAN = @TK";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@TK", taiKhoanNhap);

                    try
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) // Nếu tìm thấy người dùng với TAIKHOAN này
                            {
                                // Lấy mật khẩu đã băm và phân quyền từ database
                                matKhauHashedFromDb = reader["MATKHAU"]?.ToString();
                                string quyenFromDb = reader["PHANQUYEN"]?.ToString();

                                // *** BƯỚC QUAN TRỌNG: Xác thực mật khẩu ***
                                // Kiểm tra xem có lấy được chuỗi hash không và dùng BCrypt.Verify
                                if (!string.IsNullOrEmpty(matKhauHashedFromDb) && BCrypt.Net.BCrypt.Verify(matKhauNhapPlainText, matKhauHashedFromDb))
                                {
                                    // Mật khẩu khớp! Gán quyền lấy được từ DB vào kết quả trả về
                                    phanQuyenResult = quyenFromDb;
                                }
                                // Nếu Verify trả về false (mật khẩu sai), phanQuyenResult vẫn là null
                            }
                            // Nếu reader.Read() là false (không tìm thấy TAIKHOAN), phanQuyenResult vẫn là null
                        } // SqlDataReader được đóng và dispose tại đây
                    }
                    catch (SqlException sqlEx)
                    {
                        MessageBox.Show($"Lỗi SQL khi kiểm tra đăng nhập: {sqlEx.Message}", "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        phanQuyenResult = null; // Đảm bảo trả về null khi có lỗi
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi không xác định khi kiểm tra đăng nhập: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        phanQuyenResult = null; // Đảm bảo trả về null khi có lỗi
                    }
                    // Kết nối sẽ được đóng tự động khi ra khỏi khối using (SqlConnection)
                } // SqlCommand được dispose tại đây
            }

            return phanQuyenResult;
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtTaiKhoan_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void txtTaiKhoan_MouseEnter(object sender, EventArgs e)
        {

        }

        private void txtTaiKhoan_Enter(object sender, EventArgs e)
        {
            if(txtTaiKhoan.Text == "Username")
            {
                txtTaiKhoan.Text = "";
                txtTaiKhoan.ForeColor = Color.Black;
            }    
        }

        private void txtTaiKhoan_Leave(object sender, EventArgs e)
        {
            if (txtTaiKhoan.Text == "")
            {
                txtTaiKhoan.Text = "Username";
                txtTaiKhoan.ForeColor = Color.Silver;
            }
        }

        private void txtMatkhau_Enter(object sender, EventArgs e)
        {
            if(txtMatkhau.Text == "Password")
            {
                txtMatkhau.Text = "";
                txtMatkhau.ForeColor = Color.Black;
            }
        }

        private void txtMatkhau_Leave(object sender, EventArgs e)
        {
            if (txtMatkhau.Text == "")
            {
                txtMatkhau.Text = "Password";
                txtMatkhau.ForeColor = Color.Silver;
            }
        }

        private void txtTaiKhoan_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            DialogResult traloi = MessageBox.Show("Bạn có muốn thoát không ?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (traloi == DialogResult.Yes) this.Close();
        }

        private void FrmDangNhap_Load(object sender, EventArgs e)
        {

        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            // --- Kiểm tra ô nhập liệu trống ---
            if (string.IsNullOrWhiteSpace(txtTaiKhoan.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên tài khoản!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTaiKhoan.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMatkhau.Text))
            {
                MessageBox.Show("Vui lòng nhập Mật khẩu!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatkhau.Focus();
                return;
            }

            // --- Gọi hàm LAYQUYEN (đã được sửa đổi) ---
            // Hàm này sẽ tự mở/đóng kết nối và trả về quyền nếu đăng nhập thành công, null nếu thất bại
            string QUYEN = LAYQUYEN();

            // --- Xử lý kết quả đăng nhập ---
            if (!string.IsNullOrEmpty(QUYEN)) // Kiểm tra xem có lấy được quyền không (đăng nhập thành công)
            {
                // Đăng nhập thành công
                MessageBox.Show("Bạn đã đăng nhập với quyền " + QUYEN, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                FrmGiaoDienChinh frmM = new FrmGiaoDienChinh();

                // --- Phân quyền giao diện ---
                if (QUYEN != "admin")
                {
                    frmM.button6.Visible = false;
                    frmM.button7.Visible = false;
                    frmM.button13.Visible = false;
                    frmM.button14.Visible = false;
                    frmM.button1.Visible = false;
                    frmM.button3.Visible = false;
                    frmM.menuContainer1.Visible = false;
                    frmM.menuContainer.Visible = false;
                    frmM.button10.Visible = true;
                }
                else // Là admin
                {
                    frmM.button10.Visible = false; // ẩn nút đăng ký thi
                }
                // -----------------------------

                frmM.Show();
                this.Hide();
            }
            else
            {
                // Đăng nhập thất bại (do hàm LAYQUYEN trả về null)
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTaiKhoan.Text = ""; // Hoặc txtTaiKhoan.Clear();
                txtMatkhau.Text = "";  // Hoặc txtMatkhau.Clear();
                txtTaiKhoan.Focus();
                // Không cần gọi lại sự kiện Leave ở đây trừ khi bạn có logic đặc biệt
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtTaiKhoan.ResetText();
            txtMatkhau.ResetText();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            // Mở kết nối nếu đang đóng (an toàn hơn khi gọi LAYQUYEN)
            if (MyCon.State == ConnectionState.Closed)
            {
                try
                {
                    MyCon.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi kết nối CSDL: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Không thể tiếp tục nếu không kết nối được
                }
            }

            // Lấy quyền của người dùng
            QUYEN = LAYQUYEN(); // Hàm LAYQUYEN đã tự đóng kết nối trong finally

            // Kiểm tra kết quả lấy quyền
            if (!string.IsNullOrEmpty(QUYEN)) // Dùng string.IsNullOrEmpty để kiểm tra an toàn hơn
            {
                // Đăng nhập thành công
                MessageBox.Show("Bạn đã đăng nhập với quyền " + QUYEN, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tạo một instance của Form chính
                FrmGiaoDienChinh frmM = new FrmGiaoDienChinh();

                // --- QUAN TRỌNG: Thiết lập trạng thái Button13 TRƯỚC KHI Show form ---
                if (QUYEN != "admin")
                {
                    // Nếu không phải admin

                    frmM.button6.Visible = false;
                    frmM.button7.Visible = false;
                    frmM.button13.Visible = false;
                    frmM.button14.Visible = false;
                    frmM.button1.Visible = false;
                    frmM.button3.Visible = false;
                    frmM.menuContainer1.Visible = false;
                    frmM.menuContainer.Visible = false;
                    frmM.button10.Visible = true;


                }
                else
                {
                    // Nếu là admin
                    frmM.button10.Visible = false;// ẩn nút đăng ký
                }
                // --------------------------------------------------------------------

                // Hiển thị Form chính
                frmM.Show();

                // Ẩn Form đăng nhập hiện tại
                this.Hide();
            }
            else
            {
                // Đăng nhập thất bại
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Thay đổi Icon và Buttons cho phù hợp
                // Xóa trắng ô nhập liệu (ResetText sẽ trả về giá trị mặc định, Text = "" tốt hơn nếu không có giá trị mặc định)
                txtTaiKhoan.Text = "";
                txtMatkhau.Text = "";
                // Đặt lại placeholder text nếu bạn muốn
                txtTaiKhoan_Leave(txtTaiKhoan, EventArgs.Empty); // Gọi lại sự kiện Leave để hiện placeholder
                txtMatkhau_Leave(txtMatkhau, EventArgs.Empty);   // Gọi lại sự kiện Leave để hiện placeholder

                // Đưa con trỏ về ô Tên tài khoản
                this.txtTaiKhoan.Focus();

                // Đảm bảo kết nối được đóng nếu trước đó mở mà không vào được hàm LAYQUYEN (ít xảy ra)
                if (MyCon.State == ConnectionState.Open)
                {
                    MyCon.Close();
                }
            }
        }
    }
}
