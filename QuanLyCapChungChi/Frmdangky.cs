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
using System.Text.RegularExpressions; // Dùng kiểm tra số điện thoại
using System.Net.Mail; // Dùng để kiểm tra Email tốt hơn

namespace QuanLyCapChungChi
{
    public partial class Frmdangky : Form
    {
        SqlConnection MyCon = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456");

        public Frmdangky()
        {
            InitializeComponent();
        }

        private void Frmdangky_Load(object sender, EventArgs e)
        {
            LoadKhoaHoc();
            LoadKhoaTHI();
            // Đặt giá trị mặc định cho ComboBox Giới tính nếu cần
            // cmbGioiTinh.SelectedIndex = 0; // Hoặc -1 nếu muốn trống ban đầu
        }

        private void LoadKhoaTHI()
        {
            // Tạo kết nối cục bộ để tránh xung đột nếu MyCon đang mở ở nơi khác
            using (SqlConnection conn = new SqlConnection(MyCon.ConnectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MaKhoaThi, TenKhoaThi FROM KhoaThi ORDER BY TenKhoaThi"; // Thêm ORDER BY
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Thêm dòng "Chọn..." (tùy chọn)
                    DataRow dr = dt.NewRow();
                    dr["MaKhoaThi"] = DBNull.Value; // Hoặc giá trị không hợp lệ khác
                    dr["TenKhoaThi"] = "-- Chọn Khóa Thi --";
                    dt.Rows.InsertAt(dr, 0);

                    cmbTenkhoathi.DataSource = dt;
                    cmbTenkhoathi.DisplayMember = "TenKhoaThi";
                    cmbTenkhoathi.ValueMember = "MaKhoaThi";
                    cmbTenkhoathi.SelectedIndex = 0; // Chọn dòng đầu tiên
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải danh sách khóa thi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // conn sẽ tự động đóng khi ra khỏi using
            }
        }

        private void LoadKhoaHoc()
        {
            using (SqlConnection conn = new SqlConnection(MyCon.ConnectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MaKhoaHoc, TenKhoaHoc FROM KhoaHoc ORDER BY TenKhoaHoc"; // Thêm ORDER BY
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Thêm dòng "Chọn..."
                    DataRow dr = dt.NewRow();
                    dr["MaKhoaHoc"] = DBNull.Value;
                    dr["TenKhoaHoc"] = "-- Chọn Khóa Học --";
                    dt.Rows.InsertAt(dr, 0);

                    cmbTenKhoaHoc.DataSource = dt;
                    cmbTenKhoaHoc.DisplayMember = "TenKhoaHoc";
                    cmbTenKhoaHoc.ValueMember = "MaKhoaHoc";
                    cmbTenKhoaHoc.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải danh sách khóa học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --- Các hàm kiểm tra ---

        /// <summary>
        /// Kiểm tra tất cả các trường nhập liệu cơ bản.
        /// Hiển thị MessageBox và trả về false nếu có lỗi.
        /// </summary>
        /// <returns>True nếu hợp lệ, False nếu có lỗi.</returns>
        private bool ValidateInputFields()
        {
            // 1. Kiểm tra các trường không được để trống
            if (string.IsNullOrWhiteSpace(txtMahocvien.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã học viên!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMahocvien.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtHoten.Text))
            {
                MessageBox.Show("Vui lòng nhập Họ tên!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHoten.Focus();
                return false;
            }
            if (cmbGioiTinh.SelectedIndex == -1) // Kiểm tra xem đã chọn giới tính chưa
            {
                MessageBox.Show("Vui lòng chọn Giới tính!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbGioiTinh.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtDiachi.Text))
            {
                MessageBox.Show("Vui lòng nhập Địa chỉ!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDiachi.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtSoDienthoai.Text))
            {
                MessageBox.Show("Vui lòng nhập Số điện thoại!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoDienthoai.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập Email!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }


            // 2. Kiểm tra định dạng Email
            if (!IsValidEmail(txtEmail.Text.Trim())) // Trim() để loại bỏ khoảng trắng thừa
            {
                MessageBox.Show("Email không hợp lệ! Vui lòng nhập đúng định dạng (ví dụ: example@domain.com)", "Lỗi Định Dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                txtEmail.SelectAll(); // Chọn hết text để người dùng sửa dễ hơn
                return false;
            }

            // 3. Kiểm tra định dạng Số điện thoại
            if (!Is_Phone(txtSoDienthoai.Text.Trim()))
            {
                MessageBox.Show("Số điện thoại không hợp lệ! Phải có đúng 10 chữ số.", "Lỗi Định Dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSoDienthoai.Focus();
                txtSoDienthoai.SelectAll();
                return false;
            }

            // Nếu tất cả kiểm tra đều qua
            return true;
        }

        /// <summary>
        /// Kiểm tra định dạng email hợp lệ sử dụng lớp MailAddress.
        /// </summary>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false; // Email là bắt buộc trong trường hợp này

            try
            {
                // Thử tạo đối tượng MailAddress, nếu thành công là email hợp lệ về mặt cú pháp
                var addr = new System.Net.Mail.MailAddress(email);
                // Kiểm tra xem địa chỉ được phân tích có giống hệt chuỗi gốc không
                // (tránh trường hợp như "abc @domain.com" bị coi là hợp lệ)
                return addr.Address == email;
            }
            catch
            {
                // Nếu có lỗi khi tạo MailAddress -> không hợp lệ
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra số điện thoại có đúng 10 chữ số hay không.
        /// </summary>
        private bool Is_Phone(string sdt)
        {
            // Sử dụng Regex để kiểm tra chuỗi có phải là 10 ký tự số hay không
            // @"^\d{10}$":
            // ^ : Bắt đầu chuỗi
            // \d : Ký tự số (tương đương [0-9])
            // {10} : Lặp lại đúng 10 lần
            // $ : Kết thúc chuỗi
            return Regex.IsMatch(sdt, @"^\d{10}$");
        }

        // --- Hàm thực thi Database ---

        /// <summary>
        /// Thêm thông tin học viên vào bảng HocVien.
        /// </summary>
        /// <param name="connection">Đối tượng SqlConnection đang mở.</param>
        /// <param name="transaction">Đối tượng SqlTransaction đang hoạt động.</param>
        /// <returns>True nếu thêm thành công, False nếu thất bại.</returns>
        private bool InsertHocVien(SqlConnection connection, SqlTransaction transaction)
        {
            string sqlInsertHocVien = @"
                INSERT INTO HocVien
                       (MaHocVien, HoTen, NgaySinh, GioiTinh, DiaChi, SoDienThoai, Email)
                VALUES (@MA, @TEN, @NGAY, @GT, @DC, @DT, @E)";

            using (SqlCommand cmd = new SqlCommand(sqlInsertHocVien, connection, transaction))
            {
                // Thêm .Trim() để loại bỏ khoảng trắng thừa ở đầu/cuối
                cmd.Parameters.AddWithValue("@MA", txtMahocvien.Text.Trim());
                cmd.Parameters.AddWithValue("@TEN", txtHoten.Text.Trim());
                cmd.Parameters.AddWithValue("@NGAY", dtpNgaysinh.Value); // Ngày tháng không cần Trim
                cmd.Parameters.AddWithValue("@GT", cmbGioiTinh.SelectedItem.ToString()); // Lấy giá trị đã chọn
                cmd.Parameters.AddWithValue("@DC", txtDiachi.Text.Trim());
                cmd.Parameters.AddWithValue("@DT", txtSoDienthoai.Text.Trim()); // Đã kiểm tra là 10 số
                cmd.Parameters.AddWithValue("@E", txtEmail.Text.Trim()); // Đã kiểm tra định dạng

                // ExecuteNonQuery trả về số dòng bị ảnh hưởng
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0; // Trả về true nếu có ít nhất 1 dòng được thêm
            }
        }


        // --- Xử lý sự kiện Click Button ---

        // Lưu Học viên và Đăng ký Khóa Học
        private void button1_Click(object sender, EventArgs e)
        {
            // --- 1. Kiểm tra dữ liệu nhập ---
            if (!ValidateInputFields())
            {
                return; // Dừng lại nếu dữ liệu không hợp lệ
            }

            // --- 2. Kiểm tra lựa chọn Khóa Học ---
            if (cmbTenKhoaHoc.SelectedIndex <= 0 || cmbTenKhoaHoc.SelectedValue == null || cmbTenKhoaHoc.SelectedValue == DBNull.Value)
            {
                MessageBox.Show("Vui lòng chọn một Khóa học hợp lệ!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTenKhoaHoc.Focus();
                return;
            }

            // Khai báo transaction bên ngoài try để có thể rollback trong catch
            SqlTransaction transaction = null;
            try
            {
                // --- 3. Mở kết nối và Bắt đầu Transaction ---
                if (MyCon.State == ConnectionState.Closed)
                {
                    MyCon.Open();
                }
                transaction = MyCon.BeginTransaction();

                // --- 4. Thêm Học viên ---
                // Gọi hàm đã tách ra, truyền connection và transaction
                if (!InsertHocVien(MyCon, transaction))
                {
                    // Nếu hàm InsertHocVien trả về false (thường không xảy ra nếu không có lỗi)
                    // hoặc nếu bạn muốn có thông báo cụ thể hơn.
                    throw new Exception("Không thể thêm thông tin học viên vào hệ thống.");
                }

                // --- 5. Lấy Mã Khóa Học từ ComboBox ---
                // Lấy trực tiếp ValueMember đã được gán khi load ComboBox
                string maKhoaHoc = cmbTenKhoaHoc.SelectedValue.ToString();

                // --- 6. Thêm vào bảng Đăng Ký Học ---
                string sqlInsertDangKyHoc = "INSERT INTO DangKyHoc (MaHocVien, MaKhoaHoc) VALUES (@MA, @MKH)";
                using (SqlCommand cmdDangKy = new SqlCommand(sqlInsertDangKyHoc, MyCon, transaction))
                {
                    cmdDangKy.Parameters.AddWithValue("@MA", txtMahocvien.Text.Trim());
                    cmdDangKy.Parameters.AddWithValue("@MKH", maKhoaHoc);
                    cmdDangKy.ExecuteNonQuery();
                }

                // --- 7. Hoàn tất Transaction ---
                transaction.Commit(); // Lưu tất cả thay đổi vào DB
                MessageBox.Show("Đã lưu học viên và đăng ký khóa học thành công!", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tùy chọn: Xóa các trường sau khi lưu thành công
                // ClearInputFields();
            }
            catch (SqlException sqlEx) // Bắt lỗi SQL cụ thể (vd: Trùng Mã Học Viên)
            {
                // Nếu có lỗi SQL, hủy bỏ mọi thay đổi đã thực hiện trong transaction này
                if (transaction != null)
                {
                    try { transaction.Rollback(); } catch { /* Có thể bỏ qua lỗi khi rollback */ }
                }
                MessageBox.Show($"Lỗi CSDL khi đăng ký khóa học: {sqlEx.Message}\nSố lỗi: {sqlEx.Number}", "Lỗi Lưu Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                // Hủy bỏ transaction nếu có lỗi khác xảy ra
                if (transaction != null)
                {
                    try { transaction.Rollback(); } catch { /* Có thể bỏ qua lỗi khi rollback */ }
                }
                MessageBox.Show("Lỗi không xác định khi đăng ký khóa học: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Luôn đóng kết nối dù thành công hay thất bại
                if (MyCon.State == ConnectionState.Open)
                {
                    MyCon.Close();
                }
            }
        }

        // Lưu Học viên và Đăng ký Khóa Thi
        private void button2_Click(object sender, EventArgs e)
        {
            // --- 1. Kiểm tra dữ liệu nhập ---
            if (!ValidateInputFields())
            {
                return; // Dừng lại nếu dữ liệu không hợp lệ
            }

            // --- 2. Kiểm tra lựa chọn Khóa Thi ---
            if (cmbTenkhoathi.SelectedIndex <= 0 || cmbTenkhoathi.SelectedValue == null || cmbTenkhoathi.SelectedValue == DBNull.Value)
            {
                MessageBox.Show("Vui lòng chọn một Khóa thi hợp lệ!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTenkhoathi.Focus();
                return;
            }

            SqlTransaction transaction = null;
            try
            {
                // --- 3. Mở kết nối và Bắt đầu Transaction ---
                if (MyCon.State == ConnectionState.Closed)
                {
                    MyCon.Open();
                }
                transaction = MyCon.BeginTransaction();

                // --- 4. Thêm Học viên ---
                if (!InsertHocVien(MyCon, transaction))
                {
                    throw new Exception("Không thể thêm thông tin học viên vào hệ thống.");
                }

                // --- 5. Lấy Mã Khóa Thi từ ComboBox ---
                string maKhoaThi = cmbTenkhoathi.SelectedValue.ToString();

                // --- 6. Thêm vào bảng Kết Quả Thi ---
                // Giả sử điểm ban đầu là NULL hoặc bạn có thể thêm giá trị mặc định nếu cần
                // Kiểm tra cấu trúc bảng KetQuaThi xem các cột điểm có cho phép NULL không
                string sqlInsertKetQuaThi = "INSERT INTO KetQuaThi (MaHocVien, MaKhoaThi) VALUES (@MA, @MKT)";
                // Nếu điểm không được NULL và cần mặc định là 0:
                // string sqlInsertKetQuaThi = "INSERT INTO KetQuaThi (MaHocVien, MaKhoaThi, DiemSo, DiemSo2, DiemSo3) VALUES (@MA, @MKT, 0, 0, 0)";

                using (SqlCommand cmdKetQua = new SqlCommand(sqlInsertKetQuaThi, MyCon, transaction))
                {
                    cmdKetQua.Parameters.AddWithValue("@MA", txtMahocvien.Text.Trim());
                    cmdKetQua.Parameters.AddWithValue("@MKT", maKhoaThi);
                    cmdKetQua.ExecuteNonQuery();
                }

                // --- 7. Hoàn tất Transaction ---
                transaction.Commit();
                MessageBox.Show("Đã lưu học viên và đăng ký khóa thi thành công!", "Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tùy chọn: Xóa các trường
                // ClearInputFields();
            }
            catch (SqlException sqlEx)
            {
                if (transaction != null)
                {
                    try { transaction.Rollback(); } catch { /* Bỏ qua lỗi rollback */ }
                }
                MessageBox.Show($"Lỗi CSDL khi đăng ký khóa thi: {sqlEx.Message}\nSố lỗi: {sqlEx.Number}", "Lỗi Lưu Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    try { transaction.Rollback(); } catch { /* Bỏ qua lỗi rollback */ }
                }
                MessageBox.Show("Lỗi không xác định khi đăng ký khóa thi: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (MyCon.State == ConnectionState.Open)
                {
                    MyCon.Close();
                }
            }
        }

        // (Tùy chọn) Hàm xóa trắng các ô nhập liệu
        private void ClearInputFields()
        {
            txtMahocvien.Clear();
            txtHoten.Clear();
            dtpNgaysinh.Value = DateTime.Now; // Hoặc giá trị mặc định khác
            cmbGioiTinh.SelectedIndex = -1; // Bỏ chọn
            txtDiachi.Clear();
            txtSoDienthoai.Clear();
            txtEmail.Clear();
            cmbTenKhoaHoc.SelectedIndex = 0; // Chọn lại dòng "-- Chọn..."
            cmbTenkhoathi.SelectedIndex = 0; // Chọn lại dòng "-- Chọn..."
            txtMahocvien.Focus();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
            // Hàm này có thể không cần thiết, trừ khi bạn có logic cụ thể khi focus vào GroupBox
        }
    }
}