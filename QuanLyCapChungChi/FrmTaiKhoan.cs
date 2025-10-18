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
    public partial class FrmTaiKhoan : Form
    {
        SqlConnection MyCon = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456");
        public FrmTaiKhoan()
        {
            InitializeComponent();
        }

        private void FrmTaiKhoan_Load(object sender, EventArgs e)
        {
            Load_DataGridView();
            LamDep_DataGridView();
            if (dgvTaikhoan.Rows.Count > 0)
            {
                dgvTaikhoan.ClearSelection(); // Xóa chọn tất cả
                dgvTaikhoan.Rows[0].Selected = true; // Chọn hàng đầu tiên
                dgvTaikhoan.CurrentCell = dgvTaikhoan.Rows[0].Cells[0]; // Chọn ô đầu tiên
                dgvTaikhoan.Focus(); // Đưa focus vào DataGridView
                HienThiThongTinHocVien(dgvTaikhoan.Rows[0]);// Hiển thị dữ liệu lên các TextBox
            }
            LoadQuyen();
        }
        private void LoadQuyen()
        {
            DataTable dt = new DataTable();
            // Thay bằng chuỗi kết nối thực tế hoặc biến chứa chuỗi kết nối của bạn
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456;Encrypt=False";

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // Cải thiện SQL: Lấy quyền duy nhất, bỏ qua NULL/rỗng, sắp xếp
                    string sql = "SELECT DISTINCT phanquyen FROM nguoidung WHERE phanquyen IS NOT NULL AND phanquyen <> '' ORDER BY phanquyen";

                    con.Open();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(sql, con))
                    {
                        // 1. Đổ dữ liệu quyền từ CSDL vào DataTable TRƯỚC
                        adapter.Fill(dt);
                    }
                } // Kết nối tự động đóng

                // --- THÊM PLACEHOLDER ---
                // 2. Tạo dòng mới
                DataRow dr = dt.NewRow();
                // 3. Gán giá trị placeholder
                dr["phanquyen"] = "-- Chọn Quyền --"; // Tên cột phải khớp chính xác
                                                      // 4. Chèn vào vị trí đầu tiên (index 0)
                dt.Rows.InsertAt(dr, 0);

                // --- Binding ComboBox ---
                // 5. Xóa binding cũ (quan trọng)
                cmbQuyen.DataSource = null;
                // 6. Gán DataTable đã có placeholder
                cmbQuyen.DataSource = dt;
                cmbQuyen.DisplayMember = "phanquyen"; // Cột hiển thị
                cmbQuyen.ValueMember = "phanquyen";   // Cột lấy giá trị (giống DisplayMember)

                // 7. Đặt mục chọn mặc định là placeholder (luôn là index 0)
                cmbQuyen.SelectedIndex = 0;

                // 8. (Tùy chọn) Vô hiệu hóa nếu chỉ có placeholder
                cmbQuyen.Enabled = (dt.Rows.Count > 1);

            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Lỗi SQL khi tải danh sách quyền: {sqlEx.Message}", "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetComboBoxOnError(cmbQuyen, "-- Lỗi tải quyền --"); // Hàm reset (xem lại code trước)
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi không xác định khi tải danh sách quyền: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetComboBoxOnError(cmbQuyen, "-- Lỗi tải quyền --"); // Hàm reset (xem lại code trước)
            }
        }
        // Hàm tiện ích để reset ComboBox khi có lỗi 
        private void ResetComboBoxOnError(ComboBox comboBox, string errorMessage)
        {
            comboBox.DataSource = null;
            comboBox.Items.Clear();
            comboBox.Items.Add(errorMessage);
            comboBox.SelectedIndex = 0;
            comboBox.Enabled = false;
        }
        private void HienThiThongTinHocVien(DataGridViewRow row)
        {
            if (row != null)
            { 
                txtTaikhoan.Text = dgvTaikhoan.CurrentRow.Cells["TAIKHOAN"].Value.ToString();
                txtMatkhau.Text = dgvTaikhoan.CurrentRow.Cells["MATKHAU"].Value.ToString();
                cmbQuyen.Text = dgvTaikhoan.CurrentRow.Cells["PHANQUYEN"].Value.ToString();
            }
        }
        private void LamDep_DataGridView()
        {

            // Đặt chiều cao của tiêu đề
            dgvTaikhoan.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;//Ngăn không cho hệ thống và người dùng thay đổi chiều cao tiêu đề cột
            dgvTaikhoan.ColumnHeadersHeight = 40; // Chiều cao tiêu đề

            // Đặt màu nền và màu chữ cho tiêu đề
            dgvTaikhoan.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 212, 190);
            dgvTaikhoan.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTaikhoan.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);

            // Áp dụng màu cho một cột cụ thể
            dgvTaikhoan.Columns["taikhoan"].HeaderCell.Style.BackColor = Color.FromArgb(234, 0, 1);

            // Cập nhật lại giao diện
            dgvTaikhoan.EnableHeadersVisualStyles = false;
        }
        private void Load_DataGridView()
        {
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();
            string sql = @"SELECT TAIKHOAN,MATKHAU,PHANQUYEN FROM NGUOIDUNG";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, MyCon);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dgvTaikhoan.DataSource = dt;

            dgvTaikhoan.Columns[0].HeaderText = "Tài Khoản";
            dgvTaikhoan.Columns[1].HeaderText = "Mật khẩu";

            dgvTaikhoan.Columns[2].HeaderText = "Phân Quyền";

            dgvTaikhoan.AllowUserToAddRows = false;
            dgvTaikhoan.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            txtTaikhoan.ResetText();
            txtMatkhau.ResetText();
            txtTaikhoan.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // --- 1. Validation đầu vào ---
            // Giả định txtTaikhoan đang chứa tài khoản cần sửa và không được phép sửa đổi (ReadOnly = true)
            if (string.IsNullOrWhiteSpace(txtTaikhoan.Text))
            {
                MessageBox.Show("Không xác định được tài khoản cần sửa.", "Lỗi Logic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Không thể sửa nếu không biết sửa ai
            }
            if (string.IsNullOrWhiteSpace(txtMatkhau.Text))
            {
                MessageBox.Show("Mật khẩu không được để trống khi sửa!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatkhau.Focus();
                return;
            }
            if (cmbQuyen.SelectedIndex <= 0) // Chỉ kiểm tra nếu mục placeholder (index 0) được chọn
            {
                MessageBox.Show("Vui lòng chọn Phân quyền hợp lệ!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbQuyen.Focus();
                return;
            }

                // Lấy dữ liệu (đã được trim và kiểm tra)
             string taiKhoanCanSua = txtTaikhoan.Text; // Lấy từ textbox (đã được set ReadOnly)
            string matKhauPlainText = txtMatkhau.Text;
            string phanQuyenMoi = cmbQuyen.Text.Trim(); // Hoặc SelectedValue

            // --- Băm mật khẩu mới ---
            string matKhauHashed = BCrypt.Net.BCrypt.HashPassword(matKhauPlainText);

            // --- 2. Thực hiện thao tác CSDL với using ---
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456;Encrypt=False";
            // Sử dụng using cho cả Connection và Command
            try // Thêm try bao ngoài để bắt lỗi chung
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open(); // Mở kết nối

                    string sql = "UPDATE NGUOIDUNG SET MATKHAU = @MK, PHANQUYEN = @PQ WHERE TAIKHOAN = @TK";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        // Sử dụng mật khẩu đã băm
                        cmd.Parameters.AddWithValue("@MK", matKhauHashed);
                        cmd.Parameters.AddWithValue("@PQ", phanQuyenMoi);
                        cmd.Parameters.AddWithValue("@TK", taiKhoanCanSua);

                        // --- 3. Kiểm tra kết quả ExecuteNonQuery ---
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Bạn đã sửa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Load_DataGridView(); // Load lại để thấy thay đổi
                                                 // Tùy chọn: Xóa các ô sau khi sửa thành công hoặc chuyển sang trạng thái xem/thêm mới
                                                 // ClearUserFormFields();
                                                 // SetControlsForAddMode();
                        }
                        else
                        {
                            // Không có dòng nào được cập nhật -> tài khoản không tồn tại?
                            MessageBox.Show("Không tìm thấy tài khoản cần sửa trong CSDL.", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    } // SqlCommand được Dispose tự động ở đây
                } // SqlConnection được Close và Dispose tự động ở đây
            }
            catch (SqlException sqlEx) // Bắt lỗi SQL cụ thể
            {
                MessageBox.Show($"Lỗi SQL khi sửa người dùng: {sqlEx.Message}\nSố lỗi: {sqlEx.Number}", "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) // Bắt các lỗi chung khác
            {
                MessageBox.Show($"Đã xảy ra lỗi không xác định khi sửa: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Không cần khối finally để đóng kết nối khi đã dùng using
        }

        private void dgvTaikhoan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtTaikhoan.Text = dgvTaikhoan.CurrentRow.Cells["TAIKHOAN"].Value.ToString();
            txtMatkhau.Text = dgvTaikhoan.CurrentRow.Cells["MATKHAU"].Value.ToString();
            cmbQuyen.Text = dgvTaikhoan.CurrentRow.Cells["PHANQUYEN"].Value.ToString();
        }
        private void ClearUserFormFields()
        {
            txtTaikhoan.Clear();
            txtMatkhau.Clear();
            cmbQuyen.SelectedIndex = 0; // Reset về mục "-- Chọn Quyền --"
            txtTaikhoan.Focus();
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            // --- 1. Kiểm tra dữ liệu đầu vào ---
            if (string.IsNullOrWhiteSpace(txtTaikhoan.Text))
            {
                MessageBox.Show("Tên tài khoản không được để trống!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTaikhoan.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMatkhau.Text))
            {
                MessageBox.Show("Mật khẩu không được để trống!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatkhau.Focus();
                return;
            }
            if (cmbQuyen.SelectedIndex <= 0)
            {
                MessageBox.Show("Vui lòng chọn Phân quyền!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbQuyen.Focus();
                return;
            }

            // Lấy dữ liệu từ các control
            string taiKhoan = txtTaikhoan.Text.Trim();
            string matKhauPlainText = txtMatkhau.Text; // Lấy mật khẩu gốc người dùng nhập
            string phanQuyen = cmbQuyen.Text.Trim();

            // --- BƯỚC BĂM MẬT KHẨU 
            // Sử dụng BCrypt.Net-Next để băm mật khẩu
            // Hàm HashPassword sẽ tự động tạo salt và băm mật khẩu
            string matKhauHashed = BCrypt.Net.BCrypt.HashPassword(matKhauPlainText);

            // Bây giờ 'matKhauHashed' chứa chuỗi đã được băm an toàn để lưu

            // --- 2. Thực hiện thao tác với CSDL ---
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456;Encrypt=False";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    // --- 2.1 KIỂM TRA TÀI KHOẢN TỒN TẠI ---
                    string sqlCheckExist = "SELECT COUNT(*) FROM NGUOIDUNG WHERE TAIKHOAN = @TK_Check";
                    using (SqlCommand cmdCheck = new SqlCommand(sqlCheckExist, con))
                    {
                        cmdCheck.Parameters.AddWithValue("@TK_Check", taiKhoan);
                        int count = (int)cmdCheck.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Tên tài khoản này đã tồn tại. Vui lòng chọn tên khác.", "Trùng tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txtTaikhoan.Focus();
                            txtTaikhoan.SelectAll();
                            return;
                        }
                    }

                    // --- 2.2 THÊM NGƯỜI DÙNG MỚI ---
                    string sqlInsert = "INSERT INTO NGUOIDUNG (TAIKHOAN, MATKHAU, PHANQUYEN) VALUES (@TK, @MK, @PQ)";
                    using (SqlCommand cmdInsert = new SqlCommand(sqlInsert, con))
                    {
                        cmdInsert.Parameters.AddWithValue("@TK", taiKhoan);
                        // *** QUAN TRỌNG: Sử dụng mật khẩu đã băm ('matKhauHashed') để lưu ***
                        cmdInsert.Parameters.AddWithValue("@MK", matKhauHashed);
                        cmdInsert.Parameters.AddWithValue("@PQ", phanQuyen);

                        int rowsAffected = cmdInsert.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Thêm người dùng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Load_DataGridView(); // Load lại danh sách người dùng
                            ClearUserFormFields(); // Xóa trắng các ô nhập liệu (nên có hàm này)
                        }
                        else
                        {
                            MessageBox.Show("Thêm người dùng không thành công. Không có dòng nào bị ảnh hưởng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show($"Lỗi SQL khi thêm người dùng: {sqlEx.Message}\nSố lỗi: {sqlEx.Number}", "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi không xác định: {ex.Message}", "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // Kết nối tự động đóng ở đây
            }
        }
    }
}
