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
using System.Text.RegularExpressions;// dùng kiểm tra số điện thoại

namespace QuanLyCapChungChi
{
    public partial class FrmGiangvien : Form
    {
        SqlConnection MyCon = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456");

        public FrmGiangvien()
        {
            InitializeComponent();
        }
       
        private void Load_DataGridView()
        {
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();
            string sql = "SELECT MAGIANGVIEN,HOTEN,CHUYENMON,SODIENTHOAI,EMAIL FROM GIANGVIEN";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, MyCon);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dgvGiangvien.DataSource = dt;

            dgvGiangvien.Columns[0].HeaderText = "Mã giảng Viên";
            dgvGiangvien.Columns[1].HeaderText = "Họ Tên";

            dgvGiangvien.Columns[2].HeaderText = "Chuyên Môn";
            
            dgvGiangvien.Columns[3].HeaderText = "Số Điện Thoại";
            dgvGiangvien.Columns[4].HeaderText = "Email";

            dgvGiangvien.AllowUserToAddRows = false;
            dgvGiangvien.EditMode = DataGridViewEditMode.EditProgrammatically;

        }
        private void LamDep_DataGridView()
        {
            // Đặt chiều cao của tiêu đề
            dgvGiangvien.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;//Ngăn không cho hệ thống và người dùng thay đổi chiều cao tiêu đề cột
            dgvGiangvien.ColumnHeadersHeight = 40; // Chiều cao tiêu đề

            // Đặt màu nền và màu chữ cho tiêu đề
            dgvGiangvien.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 212, 190);
            dgvGiangvien.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGiangvien.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);

            // Áp dụng màu cho một cột cụ thể
            dgvGiangvien.Columns["MAGIANGVIEN"].HeaderCell.Style.BackColor = Color.FromArgb(234, 0, 1);

            // Cập nhật lại giao diện
            dgvGiangvien.EnableHeadersVisualStyles = false;
        }
        private void HienThiThongTinHocVien(DataGridViewRow row)
        {
            if (row != null)
            {
                txtMagiangvien.Text = dgvGiangvien.CurrentRow.Cells["MAGIANGVIEN"].Value.ToString();
                txtHoten.Text = dgvGiangvien.CurrentRow.Cells["HOTEN"].Value.ToString();
                txtChuyenmon.Text = dgvGiangvien.CurrentRow.Cells["CHUYENMON"].Value.ToString();
                txtSoDienthoai.Text = dgvGiangvien.CurrentRow.Cells["SODIENTHOAI"].Value.ToString();
                txtEmail.Text = dgvGiangvien.CurrentRow.Cells["EMAIL"].Value.ToString();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // Mở kết nối đến CSDL nếu đang đóng
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();

            // Xóa dữ liệu cũ trong các ô nhập liệu để chuẩn bị thêm học viên mới
            txtHoten.ResetText();
            txtSoDienthoai.ResetText();
            txtChuyenmon.ResetText();
            txtEmail.ResetText();
            txtMagiangvien.ResetText();
            // Đưa con trỏ vào ô nhập Họ tên để người dùng nhập ngay
            txtHoten.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (MyCon.State == ConnectionState.Closed) MyCon.Open();

                // Câu lệnh UPDATE thông tin học viên
                string sql = "UPDATE GIANGVIEN SET HOTEN = @TEN, CHUYENMON = @CM, SODIENTHOAI = @SDT, EMAIL = @E WHERE MAGIANGVIEN = @MA";
                SqlCommand Cmd = new SqlCommand(sql, MyCon);
                Cmd.Parameters.AddWithValue("@TEN", txtHoten.Text);
                Cmd.Parameters.AddWithValue("@CM", txtChuyenmon.Text);
                Cmd.Parameters.AddWithValue("@SDT", txtSoDienthoai.Text);
                Cmd.Parameters.AddWithValue("@E", txtEmail.Text);
                Cmd.Parameters.AddWithValue("@MA", txtMagiangvien.Text);
                Cmd.ExecuteNonQuery();

                // Load lại danh sách để thấy thay đổi
                Load_DataGridView();

                MessageBox.Show("Bạn đã sửa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                MyCon.Close();
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (MyCon.State == ConnectionState.Closed) MyCon.Open();

                if (dgvGiangvien.CurrentCell == null) // Kiểm tra xem có hàng nào đang được chọn không
                {
                    MessageBox.Show("Vui lòng chọn giảng viên cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int currentIndex = dgvGiangvien.CurrentCell.RowIndex; // Lấy chỉ số hàng hiện tại
                string MAGIANGVIEN = dgvGiangvien.Rows[currentIndex].Cells[0].Value?.ToString(); // Lấy mã học viên

                if (string.IsNullOrEmpty(MAGIANGVIEN)) // Kiểm tra xem mã học viên có hợp lệ không
                {
                    MessageBox.Show("Không thể xác định giảng viên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Xác nhận trước khi xóa
                DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa học viên có mã {MAGIANGVIEN} không?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;

                // Khai báo transaction trong using để đảm bảo tự động giải phóng tài nguyên
                using (SqlTransaction transaction = MyCon.BeginTransaction())
                {
                    try
                    {
                        // Xóa dữ liệu trong bảng PHANCONG trước
                        using (SqlCommand cmd1 = new SqlCommand("DELETE FROM PHANCONG WHERE MAGIANGVIEN = @MAGIANGVIEN", MyCon, transaction))
                        {
                            cmd1.Parameters.AddWithValue("@MAGIANGVIEN", MAGIANGVIEN);
                            cmd1.ExecuteNonQuery();
                        }

                        // Xóa học viên sau khi đã xóa các bản ghi liên quan
                        using (SqlCommand cmd2 = new SqlCommand("DELETE FROM GIANGVIEN WHERE MAGIANGVIEN = @MAGIANGVIEN", MyCon, transaction))
                        {
                            cmd2.Parameters.AddWithValue("@MAGIANGVIEN", MAGIANGVIEN);
                            cmd2.ExecuteNonQuery();
                        }

                        // Commit transaction sau khi xóa thành công
                        transaction.Commit();

                        Load_DataGridView();
                        MessageBox.Show("Bạn đã xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        // Nếu có lỗi, rollback lại trạng thái trước đó
                        transaction.Rollback();
                        MessageBox.Show("Lỗi khi giảng viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                MyCon.Close();
            }
        }
        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }
        private bool Is_Phone(string sdt)
        {
            // Kiểm tra xem số điện thoại có đúng 10 chữ số không
            if (!Regex.IsMatch(sdt, "^[0-9]{10}$"))
            {

                return false;
            }
            return true; // Nếu hợp lệ
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMagiangvien.Text) ||
        string.IsNullOrWhiteSpace(txtHoten.Text) ||
        string.IsNullOrWhiteSpace(txtChuyenmon.Text) ||
        string.IsNullOrWhiteSpace(txtSoDienthoai.Text) ||
        string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra email hợp lệ trước khi lưu
            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Email không hợp lệ! Vui lòng nhập đúng định dạng (example@domain.com)", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                return;
            }
            if (!Is_Phone(txtSoDienthoai.Text))
            {
                MessageBox.Show("Số điện thoại phải có đúng 10 chữ số!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSoDienthoai.Focus(); // Đưa con trỏ trở lại ô nhập số điện thoại
            }
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();
            SqlTransaction transaction = MyCon.BeginTransaction();

            try
            {
                // 1. Thêm học viên vào bảng HOCVIEN
                string sqlInsertHocVien = "INSERT INTO giangvien (MAGIANGVIEN,HOTEN,CHUYENMON,SODIENTHOAI,EMAIL) " +
                                          "VALUES (@MA, @TEN, @cm, @sdt, @E)";
                using (SqlCommand cmd = new SqlCommand(sqlInsertHocVien, MyCon, transaction))
                {
                    cmd.Parameters.AddWithValue("@MA",txtMagiangvien.Text);
                    cmd.Parameters.AddWithValue("@TEN", txtHoten.Text);
                    cmd.Parameters.AddWithValue("@cm",txtChuyenmon.Text);
                    cmd.Parameters.AddWithValue("@sdt", txtSoDienthoai.Text);
                    cmd.Parameters.AddWithValue("@e", txtEmail.Text);
                    cmd.ExecuteNonQuery();
                }
                // 4. Commit giao dịch
                transaction.Commit();

                Load_DataGridView();
                MessageBox.Show("Bạn đã lưu giảng viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Load_DataGridView();
                LamDep_DataGridView();
                if (dgvGiangvien.Rows.Count > 0)
                {
                    dgvGiangvien.ClearSelection();
                    dgvGiangvien.Rows[0].Selected = true;
                    dgvGiangvien.CurrentCell = dgvGiangvien.Rows[0].Cells[0];
                    dgvGiangvien.Focus();
                    HienThiThongTinHocVien(dgvGiangvien.Rows[0]);
                }
                MyCon.Close();
            }
        }

        private void dgvGiangvien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtMagiangvien.Text = dgvGiangvien.CurrentRow.Cells["MAGIANGVIEN"].Value.ToString();
            txtHoten.Text = dgvGiangvien.CurrentRow.Cells["HOTEN"].Value.ToString();
            txtChuyenmon.Text = dgvGiangvien.CurrentRow.Cells["CHUYENMON"].Value.ToString();
            txtSoDienthoai.Text = dgvGiangvien.CurrentRow.Cells["SODIENTHOAI"].Value.ToString();
            txtEmail.Text = dgvGiangvien.CurrentRow.Cells["EMAIL"].Value.ToString();
        }

        private void txtTimKiem_Leave(object sender, EventArgs e)
        {
            if (txtTimKiem.Text == "")
            {
                txtTimKiem.Text = "Nhập tên cần tìm kiếm ...";
                txtTimKiem.ForeColor = Color.Black;
            }
        }

        private void txtTimKiem_Enter(object sender, EventArgs e)
        {
            if (txtTimKiem.Text == "Nhập tên cần tìm kiếm ...")
            {
                txtTimKiem.Text = "";
                txtTimKiem.ForeColor = Color.Black;
            }
        }
        private void ResetDetailFields()
        {
            txtMagiangvien.ResetText();
            txtHoten.ResetText();
            txtSoDienthoai.ResetText();
            txtEmail.ResetText();
            txtChuyenmon.ResetText();
        }
        private void PerformSearch()
        {
            string searchValue = txtTimKiem.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchValue))
            {
                MessageBox.Show("Vui lòng nhập Họ tên cần tìm.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- Lấy chuỗi kết nối ---
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456;Encrypt=False";

            // --- Câu lệnh SQL ---
            string query = @"SELECT MAGIANGVIEN,HOTEN,CHUYENMON,SODIENTHOAI,EMAIL FROM GIANGVIEN
                     WHERE HOTEN LIKE @SearchValue";

            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@SearchValue", "%" + searchValue + "%");// tìm tất cả các bản ghi mà một cột văn bản có chứa chuỗi nhập vào
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                } // Kết nối tự đóng

                // --- Xử lý kết quả ---
                dgvGiangvien.DataSource = null;
                dgvGiangvien.DataSource = dt;
                LamDep_DataGridView(); // Làm đẹp grid (nếu cần)

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy học viên nào có tên chứa '" + searchValue + "'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetDetailFields(); // Giả sử hàm này tồn tại
                }
                else
                {
                    dgvGiangvien.ClearSelection();
                    if (dgvGiangvien.Rows.Count > 0)
                    {
                        dgvGiangvien.Rows[0].Selected = true;
                        // Quan trọng: Cập nhật txtMahocvien và các ô khác từ dòng được chọn
                        HienThiThongTinHocVien(dgvGiangvien.Rows[0]);
                        dgvGiangvien.Focus();
                    }
                    else
                    {
                        ResetDetailFields();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Lỗi SQL khi tìm kiếm học viên: " + sqlEx.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetDetailFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không xác định khi tìm kiếm học viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetDetailFields();
            }
        }
        private void FrmGiangvien_Load(object sender, EventArgs e)
        {
            //SetTextBox(false);
            //SetButton(true);
            Load_DataGridView();
            LamDep_DataGridView();
            if (dgvGiangvien.Rows.Count > 0)
            {
                dgvGiangvien.ClearSelection(); // Xóa chọn tất cả
                dgvGiangvien.Rows[0].Selected = true; // Chọn hàng đầu tiên
                dgvGiangvien.CurrentCell = dgvGiangvien.Rows[0].Cells[0]; // Chọn ô đầu tiên
                dgvGiangvien.Focus(); // Đưa focus vào DataGridView
                HienThiThongTinHocVien(dgvGiangvien.Rows[0]);// Hiển thị dữ liệu lên các TextBox
            }
        }
        private void txtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            // Kiểm tra xem phím được nhấn có phải là phím Enter không
            if (e.KeyCode == Keys.Enter)
            {
                // Gọi hành động tìm kiếm (giống như khi nhấn nút btnTimKiem)
                // Cách 1: Gọi trực tiếp sự kiện Click của nút (đơn giản)
                // btnTimKiem_Click(sender, e); // Hoặc dùng (this, EventArgs.Empty) nếu phù hợp

                // Cách 2: (TỐT HƠN) Tách logic tìm kiếm ra hàm riêng và gọi hàm đó
                PerformSearch();

                // Ngăn không cho TextBox xử lý tiếp phím Enter (ví dụ: tạo tiếng Beep)
                e.SuppressKeyPress = true; // Quan trọng để tránh tiếng 'ding'
                e.Handled = true;          // Đánh dấu là đã xử lý sự kiện
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }
    }
}
