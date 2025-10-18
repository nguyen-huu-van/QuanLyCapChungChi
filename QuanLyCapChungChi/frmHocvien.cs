using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing; // đổi màu nền ô
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

using OfficeOpenXml;//3 thư viện dùng nhập file vào excel
using OfficeOpenXml.Style;
using System.IO;

using System.Text.RegularExpressions;// dùng kiểm tra số điện thoại

using System.Drawing.Printing;// dùng để in

using Microsoft.Reporting.WinForms;

namespace QuanLyCapChungChi
{
    public partial class frmHocvien : Form
    {
        SqlConnection MyCon = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456");



        public frmHocvien()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }
        private void SetButton(bool enabled)
        {
            btnLuu.Enabled = !enabled;
            btnReset.Enabled = !enabled;
            btnThem.Enabled = enabled;
            btnXoa.Enabled = enabled;
            btnSua.Enabled = enabled;
            btnPrint.Enabled = enabled;
        }

        private void LamDep_DataGridView()
        {
            // Đặt chiều cao của tiêu đề
            dgvHocvien.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;//Ngăn không cho hệ thống và người dùng thay đổi chiều cao tiêu đề cột
            dgvHocvien.ColumnHeadersHeight = 40; // Chiều cao tiêu đề

            // Đặt màu nền và màu chữ cho tiêu đề
            dgvHocvien.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 212, 190);
            dgvHocvien.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHocvien.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);

            // Áp dụng màu cho một cột cụ thể
            dgvHocvien.Columns["MAHOCVIEN"].HeaderCell.Style.BackColor = Color.FromArgb(234, 0, 1);

            // Cập nhật lại giao diện
            dgvHocvien.EnableHeadersVisualStyles = false;
        }
        private void HienThiThongTinHocVien(DataGridViewRow row)
        {
            if (row != null)
            {
                txtMahocvien.Text = dgvHocvien.CurrentRow.Cells["MAHOCVIEN"].Value.ToString();
                txtHoten.Text = dgvHocvien.CurrentRow.Cells["HOTEN"].Value.ToString();
                cmbGioiTinh.Text = dgvHocvien.CurrentRow.Cells["GIOITINH"].Value.ToString();
                txtDiachi.Text = dgvHocvien.CurrentRow.Cells["DIACHI"].Value.ToString();
                txtEmail.Text = dgvHocvien.CurrentRow.Cells["EMAIL"].Value.ToString();
                txtSoDienthoai.Text = dgvHocvien.CurrentRow.Cells["SODIENTHOAI"].Value.ToString();
                dtpNgaysinh.Text = dgvHocvien.CurrentRow.Cells["NGAYSINH"].Value.ToString();
                //cmbTenKhoaHoc.Text = dgvHocvien.CurrentRow.Cells["TENKHOAHOC"].Value.ToString();
            }
        }
        private void LoadKhoaHoc1()
        {

        }

        private void frmHocvien_Load(object sender, EventArgs e)
        {

            SetButton(true);
            Load_DataGridView();
            LamDep_DataGridView();
            //if (dgvHocvien.Rows.Count > 0)
            //{
            //    dgvHocvien.ClearSelection(); // Xóa chọn tất cả
            //    dgvHocvien.Rows[0].Selected = true; // Chọn hàng đầu tiên
            //    dgvHocvien.CurrentCell = dgvHocvien.Rows[0].Cells[0]; // Chọn ô đầu tiên
            //    dgvHocvien.Focus(); // Đưa focus vào DataGridView
            //    HienThiThongTinHocVien(dgvHocvien.Rows[0]);// Hiển thị dữ liệu lên các TextBox

            //}

            LoadKhoaHoc();
            txtMahocvien.ResetText();
            txtHoten.ResetText();
            txtDiachi.ResetText();
            txtSoDienthoai.ResetText();
            txtEmail.ResetText();
            cmbTenKhoaHoc.Focus();
            LoadKhoaThi();

        }
        private void dgvHocvien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void LoadKhoaThi()
        {
            DataTable dt = new DataTable();
            try
            {
                // Sử dụng using để đảm bảo kết nối được đóng đúng cách
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456")) // Tạo kết nối mới hoặc dùng MyCon nếu quản lý cẩn thận
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    string sql = "SELECT MAKHOATHI, TENKHOATHI FROM KHOATHI ORDER BY TENKHOATHI";
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                    adapter.Fill(dt);
                } // Kết nối sẽ tự động đóng ở đây khi ra khỏi khối using

                // Thêm dòng "Tất cả" vào đầu DataTable
                DataRow dr = dt.NewRow();
                // Chúng ta vẫn cần MAKHOAHOC để phân biệt, nhưng ValueMember sẽ là TENKHOAHOC
                dr["MAKHOATHI"] = -1; // Giá trị đặc biệt vẫn hữu ích
                dr["TENKHOATHI"] = "-- Chọn Khóa Thi --";
                dt.Rows.InsertAt(dr, 0); // Chèn vào vị trí đầu tiên

                // Gán DataSource cho ComboBox
                cmbTenkhoathi.DataSource = dt;
                cmbTenkhoathi.DisplayMember = "TENKHOATHI"; // Cột hiển thị tên
                cmbTenkhoathi.ValueMember = "TENKHOATHI";   // *** THAY ĐỔI QUAN TRỌNG: Lấy giá trị theo TÊN KHÓA HỌC ***

                // Đặt mục được chọn mặc định là "Tất cả"
                cmbTenkhoathi.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách khóa học: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Không cần finally để đóng kết nối nếu dùng using
        }
        private void dgvHocvien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtMahocvien.Text = dgvHocvien.CurrentRow.Cells["MAHOCVIEN"].Value.ToString();
            txtHoten.Text = dgvHocvien.CurrentRow.Cells["HOTEN"].Value.ToString();
            cmbGioiTinh.Text = dgvHocvien.CurrentRow.Cells["GIOITINH"].Value.ToString();
            txtDiachi.Text = dgvHocvien.CurrentRow.Cells["DIACHI"].Value.ToString();
            txtEmail.Text = dgvHocvien.CurrentRow.Cells["EMAIL"].Value.ToString();
            txtSoDienthoai.Text = dgvHocvien.CurrentRow.Cells["SODIENTHOAI"].Value.ToString();
            dtpNgaysinh.Text = dgvHocvien.CurrentRow.Cells["NGAYSINH"].Value.ToString();
            cmbTenKhoaHoc.Text = dgvHocvien.CurrentRow.Cells["TENKHOAHOC"].Value.ToString();
        }
        // Định nghĩa Enum để quản lý kiểu tìm kiếm cho rõ ràng
        private enum SearchType { ByName, ByTAlast4, ByGender, LoadAll }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            PerformSearch(); // Gọi hàm tìm kiếm
        }
   
        // Hàm tiện ích để xóa trắng các ô chi tiết
        private void ResetDetailFields()
        {
            txtMahocvien.ResetText();
            txtHoten.ResetText();
            cmbGioiTinh.SelectedIndex = -1; // Hoặc về mặc định
            txtDiachi.ResetText();
            txtEmail.ResetText();
            txtSoDienthoai.ResetText();
            dtpNgaysinh.Value = DateTime.Now; // Hoặc ngày mặc định
            cmbTenKhoaHoc.SelectedIndex = -1; // Hoặc về mặc định
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            // Danh sách để lưu trữ phần SỐ của các mã học viên hiện có
            List<int> maHocVienNumbers = new List<int>();
            // Chuỗi kết nối - Đảm bảo bạn sử dụng chuỗi kết nối đúng
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456;Encrypt=False"; // Thêm Encrypt=False nếu cần

            try
            {
                // Sử dụng 'using' để đảm bảo tài nguyên được giải phóng đúng cách
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // Câu lệnh SQL để lấy TẤT CẢ mã học viên
                    // Không cần ORDER BY ở đây vì chúng ta sẽ sắp xếp danh sách số sau
                    string query = "SELECT MAHOCVIEN FROM HOCVIEN";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (con.State == ConnectionState.Closed)
                            con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Duyệt qua từng mã học viên đọc được từ CSDL
                            while (reader.Read())
                            {
                                string maHV = reader.GetString(0); // Lấy mã học viên (VD: "HV020", "HV005")

                                // Kiểm tra xem mã có đúng định dạng "HV" + số không
                                if (maHV != null && maHV.Length > 2 && maHV.StartsWith("HV"))
                                {
                                    // Lấy phần số từ vị trí thứ 2 trở đi (bỏ "HV")
                                    string numberString = maHV.Substring(2);

                                    // Cố gắng chuyển đổi phần số thành kiểu int
                                    // Sử dụng TryParse để tránh lỗi nếu phần sau "HV" không phải là số
                                    if (int.TryParse(numberString, out int numberPart))
                                    {
                                        maHocVienNumbers.Add(numberPart); // Thêm số vào danh sách
                                    }
                                    else
                                    {
                                        // Tùy chọn: Ghi log hoặc thông báo nếu có mã không đúng định dạng
                                        Console.WriteLine($"Mã học viên không hợp lệ được bỏ qua: {maHV}");
                                    }
                                }
                                else
                                {
                                    // Tùy chọn: Ghi log hoặc thông báo nếu có mã không đúng định dạng
                                    Console.WriteLine($"Mã học viên không hợp lệ được bỏ qua: {maHV}");
                                }
                            }
                        } // SqlDataReader được tự động đóng ở đây
                    }
                } // SqlConnection được tự động đóng ở đây

                // Sắp xếp danh sách các số thứ tự đã lấy được theo thứ tự tăng dần
                maHocVienNumbers.Sort();

                // Tìm số nhỏ nhất chưa được sử dụng
                int nextNumber = 1; // Bắt đầu kiểm tra từ số 1 (cho HV001)
                foreach (int existingNumber in maHocVienNumbers)
                {
                    if (existingNumber == nextNumber)
                    {
                        // Nếu số hiện tại khớp với số đang tìm, tăng số cần tìm lên 1
                        nextNumber++;
                    }
                    else if (existingNumber > nextNumber)
                    {
                        // Nếu số hiện tại lớn hơn số đang tìm, nghĩa là đã tìm thấy khoảng trống
                        // nextNumber chính là số nhỏ nhất còn thiếu. Thoát vòng lặp.
                        break;
                    }
                    // Trường hợp existingNumber < nextNumber không nên xảy ra nếu danh sách đã sắp xếp
                }

                // Tạo mã học viên mới với định dạng HVxxx (3 chữ số, có số 0 đứng đầu nếu cần)
                // Ví dụ: 1 -> HV001, 25 -> HV025, 123 -> HV123
                string newMaHV = "HV" + nextNumber.ToString("D3");

                // --- Chuẩn bị giao diện cho việc thêm mới ---

                // 1. Hiển thị mã học viên mới lên TextBox (QUAN TRỌNG!)
                txtMahocvien.Text = newMaHV;
                

                // 2. Xóa dữ liệu cũ trong các ô nhập liệu khác
                txtHoten.ResetText();
                txtDiachi.ResetText();
                txtSoDienthoai.ResetText();
                txtEmail.ResetText();
                dtpNgaysinh.Value = DateTime.Now; // Đặt lại ngày sinh về ngày hiện tại (hoặc giá trị mặc định khác)
                cmbGioiTinh.SelectedIndex = -1; // Bỏ chọn giới tính
               

                // 3. Kích hoạt/Vô hiệu hóa các nút phù hợp
                SetButton(false); // Kích hoạt nút Lưu, Hủy; Vô hiệu hóa Thêm, Sửa, Xóa, In

                // 4. Đưa con trỏ vào ô Họ tên để người dùng nhập tiếp
                txtHoten.Focus();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Lỗi cơ sở dữ liệu khi lấy mã học viên: {sqlEx.Message}", "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Có thể xem xét đặt lại các nút về trạng thái ban đầu nếu lỗi
                SetButton(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi không mong muốn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Có thể xem xét đặt lại các nút về trạng thái ban đầu nếu lỗi
                SetButton(true);
            }
            // Không cần MyCon.Close() ở đây nếu bạn dùng 'using' cho SqlConnection
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
          

            try
            {
                if (MyCon.State == ConnectionState.Closed) MyCon.Open();

                // Câu lệnh UPDATE thông tin học viên
                string sql = "UPDATE HOCVIEN SET HOTEN = @TEN, DIACHI = @DC, EMAIL = @E, SODIENTHOAI = @SDT, GIOITINH = @GT WHERE MAHOCVIEN = @MA";
                SqlCommand Cmd = new SqlCommand(sql, MyCon);
                Cmd.Parameters.AddWithValue("@TEN", txtHoten.Text);
                Cmd.Parameters.AddWithValue("@MA", txtMahocvien.Text);
                Cmd.Parameters.AddWithValue("@DC", txtDiachi.Text);
                Cmd.Parameters.AddWithValue("@E", txtEmail.Text);
                Cmd.Parameters.AddWithValue("@SDT", txtSoDienthoai.Text);
                Cmd.Parameters.AddWithValue("@GT", cmbGioiTinh.Text);
                Cmd.ExecuteNonQuery();

                // **Cập nhật khóa học của học viên**
                string sqlUpdateKhoaHoc = "UPDATE DANGKYHOC SET MAKHOAHOC = (SELECT MAKHOAHOC FROM KHOAHOC WHERE TENKHOAHOC = @TENKH) WHERE MAHOCVIEN = @MA";
                SqlCommand cmdUpdateKhoaHoc = new SqlCommand(sqlUpdateKhoaHoc, MyCon);
                cmdUpdateKhoaHoc.Parameters.AddWithValue("@TenKH", cmbTenKhoaHoc.Text); // Lấy giá trị từ ComboBox
                cmdUpdateKhoaHoc.Parameters.AddWithValue("@MA", txtMahocvien.Text);
                cmdUpdateKhoaHoc.ExecuteNonQuery();

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
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) // Thêm kiểm tra null hoặc trống
                return false;
            string pattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$"; // Mẫu Regex phổ biến hơn một chút
                                                                                                      // Hoặc giữ mẫu cũ của bạn: string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);

        }
        private bool KiemTraEmailTrung(string email, string maHocVienHienTai)
        {
            bool isDuplicate = false;
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456"))
            {
                try
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    string sqlCheck = "SELECT COUNT(*) FROM HOCVIEN WHERE EMAIL = @Email AND MAHOCVIEN <> @MaHienTai";
                    using (SqlCommand cmdCheck = new SqlCommand(sqlCheck, con))
                    {
                        cmdCheck.Parameters.AddWithValue("@Email", email);
                        cmdCheck.Parameters.AddWithValue("@MaHienTai", maHocVienHienTai);
                        int count = (int)cmdCheck.ExecuteScalar();
                        if (count > 0)
                        {
                            isDuplicate = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi kiểm tra email trong CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isDuplicate = true; // Coi là trùng nếu không kiểm tra được để an toàn
                }
            }
            return isDuplicate;
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
            // === BƯỚC 1: KIỂM TRA DỮ LIỆU ĐẦU VÀO NGHIÊM NGẶT HƠN ===
            if (string.IsNullOrWhiteSpace(txtMahocvien.Text))
            {
                MessageBox.Show("Mã học viên không được để trống. Vui lòng nhấn 'Thêm' để tạo mã mới.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtHoten.Text))
            {
                MessageBox.Show("Vui lòng nhập Họ tên học viên.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHoten.Focus();
                return;
            }
            if (cmbGioiTinh.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn Giới tính.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbGioiTinh.Focus();
                return;
            }
            // Sửa lại kiểm tra ComboBox khóa học
            if (cmbTenKhoaHoc.SelectedValue == null || cmbTenKhoaHoc.SelectedValue.ToString() == "-- Chọn Khóa Học --")
            {
                MessageBox.Show("Vui lòng chọn một Khóa học.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTenKhoaHoc.Focus();
                return;
            }
            if (!IsValidEmail(txtEmail.Text.Trim()))
            {
                MessageBox.Show("Email không hợp lệ. Vui lòng kiểm tra lại.", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }
            if (!Is_Phone(txtSoDienthoai.Text.Trim()))
            {
                MessageBox.Show("Số điện thoại không hợp lệ. Phải là 10 chữ số.", "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoDienthoai.Focus();
                return;
            }

            string maHocVien = txtMahocvien.Text.Trim();
            string tenKhoaHocDaChon = cmbTenKhoaHoc.SelectedValue.ToString();
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456";

            // === BƯỚC 2: SỬ DỤNG KẾT NỐI CỤC BỘ VÀ TRANSACTION ĐỂ LƯU DỮ LIỆU ===
            // Dùng 'using' là cách làm tốt nhất, nó tự động quản lý việc mở và đóng kết nối.
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;
                try
                {
                    con.Open();

                    // Kiểm tra mã học viên đã tồn tại chưa
                    if (MaHocVienDaTonTai(maHocVien, con)) // Giả sử bạn có hàm này
                    {
                        MessageBox.Show($"Mã học viên '{maHocVien}' đã tồn tại trong hệ thống. Vui lòng nhấn 'Thêm' để tạo mã khác.", "Trùng lặp dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // *** BƯỚC QUAN TRỌNG NHẤT: LẤY MAKHOAHOC TỪ TENKHOAHOC ĐÃ CHỌN ***
                    string maKhoaHocCanLuu = "";
                    string sqlGetMaKH = "SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = @TenKH";
                    using (SqlCommand cmdGetMa = new SqlCommand(sqlGetMaKH, con))
                    {
                        cmdGetMa.Parameters.AddWithValue("@TenKH", tenKhoaHocDaChon);
                        object result = cmdGetMa.ExecuteScalar(); // Lấy một giá trị duy nhất

                        if (result != null)
                        {
                            maKhoaHocCanLuu = result.ToString();
                        }
                        else
                        {
                            MessageBox.Show($"Không tìm thấy khóa học '{tenKhoaHocDaChon}' trong CSDL. Vui lòng kiểm tra lại.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; // Dừng lại nếu không tìm thấy
                        }
                    }
                    // Giờ chúng ta đã có `maKhoaHocCanLuu` chính xác để lưu.

                    // Bắt đầu transaction
                    transaction = con.BeginTransaction();

                    // 2.1. Thêm học viên vào bảng HOCVIEN
                    string sqlInsertHocVien = @"INSERT INTO HocVien (MaHocVien, HoTen, NgaySinh, GioiTinh, DiaChi, SoDienThoai, Email) 
                                      VALUES (@MA, @TEN, @NGAY, @GT, @DC, @DT, @E)";
                    using (SqlCommand cmdHocVien = new SqlCommand(sqlInsertHocVien, con, transaction))
                    {
                        cmdHocVien.Parameters.AddWithValue("@MA", maHocVien);
                        cmdHocVien.Parameters.AddWithValue("@TEN", txtHoten.Text.Trim());
                        cmdHocVien.Parameters.AddWithValue("@NGAY", dtpNgaysinh.Value);
                        cmdHocVien.Parameters.AddWithValue("@GT", cmbGioiTinh.SelectedItem.ToString());
                        cmdHocVien.Parameters.AddWithValue("@DC", txtDiachi.Text.Trim());
                        cmdHocVien.Parameters.AddWithValue("@DT", txtSoDienthoai.Text.Trim());
                        cmdHocVien.Parameters.AddWithValue("@E", txtEmail.Text.Trim());
                        cmdHocVien.ExecuteNonQuery();
                    }

                    // 2.2. Đăng ký học viên vào khóa học trong bảng DANGKYHOC
                    string sqlInsertDangKyHoc = "INSERT INTO DangKyHoc (MaHocVien, MaKhoaHoc) VALUES (@MA_HV, @MA_KH)";
                    using (SqlCommand cmdDangKy = new SqlCommand(sqlInsertDangKyHoc, con, transaction))
                    {
                        cmdDangKy.Parameters.AddWithValue("@MA_HV", maHocVien);
                        cmdDangKy.Parameters.AddWithValue("@MA_KH", maKhoaHocCanLuu); // Dùng MÃ vừa tìm được
                        cmdDangKy.ExecuteNonQuery();
                    }

                    // 2.3. Commit giao dịch nếu mọi thứ thành công
                    transaction.Commit();
                    MessageBox.Show("Đã lưu thông tin và đăng ký khóa học cho học viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (SqlException sqlEx)
                {
                    if (transaction != null) transaction.Rollback();
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Lỗi trùng khóa
                    {
                        MessageBox.Show("Dữ liệu bị trùng lặp. Mã học viên này có thể đã tồn tại hoặc đã được đăng ký vào khóa học này.", "Lỗi trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Lỗi cơ sở dữ liệu: " + sqlEx.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null) transaction.Rollback();
                    MessageBox.Show("Đã có lỗi xảy ra: " + ex.Message, "Lỗi Hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // === BƯỚC 3: CẬP NHẬT GIAO DIỆN SAU KHI LƯU ===
                    SetButton(true); // Thiết lập lại trạng thái các nút
                    Load_DataGridView_All(); // Tải lại toàn bộ dữ liệu (hoặc Load_DataGridView() của bạn)
                    LamDep_DataGridView();

                    if (dgvHocvien.Rows.Count > 0)
                    {
                        dgvHocvien.ClearSelection();
                        dgvHocvien.Rows[0].Selected = true;
                        HienThiThongTinHocVien(dgvHocvien.Rows[0]);
                    }
                }
                // 'using (SqlConnection...)' sẽ tự động đóng kết nối, không cần con.Close()
            }
        }
        // Bạn cần hàm kiểm tra mã học viên tồn tại này (nếu chưa có)
        private bool MaHocVienDaTonTai(string maHocVien, SqlConnection connection)
        {
            // Câu lệnh SQL đếm số lượng bản ghi có mã học viên trùng khớp
            string query = "SELECT COUNT(*) FROM HocVien WHERE MaHocVien = @MA";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                // Phải truyền transaction vào command nếu nó đang được thực thi bên trong transaction
                // Nhưng ở đây ta kiểm tra trước khi transaction bắt đầu nên không cần
                cmd.Parameters.AddWithValue("@MA", maHocVien);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (MyCon.State == ConnectionState.Closed)
                {
                    MyCon.Open();
                }

                // 1. Kiểm tra xem có hàng nào đang được chọn không
                if (dgvHocvien.CurrentCell == null)
                {
                    MessageBox.Show("Vui lòng chọn học viên cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Lấy chỉ số hàng hiện tại và Mã Học Viên
                int currentIndex = dgvHocvien.CurrentCell.RowIndex;
                // Sử dụng ?.ToString() để an toàn hơn nếu giá trị cell là null
                string MAHOCVIEN = dgvHocvien.Rows[currentIndex].Cells[0].Value?.ToString();

                // 3. Kiểm tra xem mã học viên có hợp lệ không
                if (string.IsNullOrEmpty(MAHOCVIEN))
                {
                    MessageBox.Show("Không thể xác định mã học viên từ hàng đã chọn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 4. Xác nhận trước khi xóa
                DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa học viên có mã '{MAHOCVIEN}' và tất cả dữ liệu liên quan (đăng ký học, kết quả thi) không?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    return; // Người dùng chọn không xóa
                }

                // 5. Bắt đầu Transaction để đảm bảo tính toàn vẹn
                // Khai báo transaction bên ngoài try để có thể truy cập trong catch (nếu cần rollback thủ công)
                // nhưng using sẽ tự động quản lý dispose
                using (SqlTransaction transaction = MyCon.BeginTransaction())
                {
                    try
                    {
                        // --- Xóa các bản ghi phụ thuộc TRƯỚC ---

                        // 5a. Xóa dữ liệu trong bảng DANGKYHOC
                        using (SqlCommand cmdDeleteDangKy = new SqlCommand("DELETE FROM DANGKYHOC WHERE MAHOCVIEN = @MAHOCVIEN", MyCon, transaction))
                        {
                            cmdDeleteDangKy.Parameters.AddWithValue("@MAHOCVIEN", MAHOCVIEN);
                            cmdDeleteDangKy.ExecuteNonQuery(); // Thực thi lệnh xóa
                        }

                        // 5b. Xóa dữ liệu trong bảng KETQUATHI
                        using (SqlCommand cmdDeleteKetQua = new SqlCommand("DELETE FROM KETQUATHI WHERE MAHOCVIEN = @MAHOCVIEN", MyCon, transaction))
                        {
                            cmdDeleteKetQua.Parameters.AddWithValue("@MAHOCVIEN", MAHOCVIEN);
                            cmdDeleteKetQua.ExecuteNonQuery(); // Thực thi lệnh xóa
                        }

                        // --- Xóa bản ghi chính SAU CÙNG ---

                        // 5c. Xóa học viên trong bảng HOCVIEN
                        using (SqlCommand cmdDeleteHocVien = new SqlCommand("DELETE FROM HOCVIEN WHERE MAHOCVIEN = @MAHOCVIEN", MyCon, transaction))
                        {
                            cmdDeleteHocVien.Parameters.AddWithValue("@MAHOCVIEN", MAHOCVIEN);
                            int rowsAffected = cmdDeleteHocVien.ExecuteNonQuery(); // Thực thi lệnh xóa

                            // Kiểm tra xem có thực sự xóa được học viên không (có thể đã bị xóa bởi tiến trình khác)
                            if (rowsAffected == 0)
                            {
                                // Có thể không cần báo lỗi ở đây nếu việc không tìm thấy HV là chấp nhận được
                                // transaction.Rollback(); // Nếu muốn hủy bỏ nếu không tìm thấy HV
                                // MessageBox.Show("Không tìm thấy học viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                // return;
                            }
                        }

                        // 6. Nếu tất cả các lệnh xóa thành công, commit transaction
                        transaction.Commit();

                        MessageBox.Show("Xóa học viên và dữ liệu liên quan thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 7. Tải lại dữ liệu DataGridView
                        Load_DataGridView(); // Gọi hàm tải lại dữ liệu của bạn
                    }
                    catch (SqlException sqlEx) // Bắt lỗi SQL cụ thể hơn
                    {
                        // Nếu có lỗi xảy ra trong quá trình xóa, rollback lại transaction
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception rollbackEx)
                        {
                            // Ghi log hoặc xử lý lỗi khi rollback thất bại (hiếm khi xảy ra)
                            MessageBox.Show("Lỗi nghiêm trọng khi rollback transaction: " + rollbackEx.Message, "Lỗi Rollback", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        MessageBox.Show($"Lỗi SQL khi xóa dữ liệu: {sqlEx.Message}\nSố lỗi: {sqlEx.Number}", "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex) // Bắt các lỗi chung khác
                    {
                        // Rollback transaction nếu có lỗi khác
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception rollbackEx)
                        {
                            MessageBox.Show("Lỗi nghiêm trọng khi rollback transaction: " + rollbackEx.Message, "Lỗi Rollback", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        MessageBox.Show("Lỗi khi xóa học viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } // using transaction sẽ tự động Dispose transaction
            }
            catch (Exception ex) // Bắt lỗi ở cấp độ cao hơn (ví dụ: lỗi kết nối, lỗi lấy dữ liệu từ dgv)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Đảm bảo kết nối luôn được đóng
                if (MyCon != null && MyCon.State == ConnectionState.Open)
                {
                    MyCon.Close();
                }
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtMahocvien.ResetText();
            txtHoten.ResetText();
            txtDiachi.ResetText();
            txtSoDienthoai.ResetText();
            txtEmail.ResetText();
            txtMahocvien.ResetText();
        }

        

        // Hàm trợ giúp để chuyển đổi số cột thành tên cột Excel (1 -> A, 2 -> B, ...)
        private string ExcelColumnFromNumber(int column)
        {
            string columnString = "";
            decimal columnNumber = column;
            while (columnNumber > 0)
            {
                decimal currentLetterNumber = (columnNumber - 1) % 26;
                char currentLetter = (char)(currentLetterNumber + 65);
                columnString = currentLetter + columnString;
                columnNumber = (columnNumber - (currentLetterNumber + 1)) / 26;
            }
            return columnString;
        }

        private void txtTimKiem_Enter(object sender, EventArgs e)
        {
            if (txtTimKiem.Text == "Nhập tên cần tìm kiếm ...")
            {
                txtTimKiem.Text = "";
                txtTimKiem.ForeColor = Color.Black;
            }
        }

        private void txtTimKiem_Leave(object sender, EventArgs e)
        {
            if (txtTimKiem.Text == "")
            {
                txtTimKiem.Text = "Nhập tên cần tìm kiếm ...";
                txtTimKiem.ForeColor = Color.Black;
            }
        }

        private void btnThem_Leave(object sender, EventArgs e)
        {
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {

            SetButton(true);
            Load_DataGridView();
            LamDep_DataGridView();
            if (dgvHocvien.Rows.Count > 0)
            {
                dgvHocvien.ClearSelection(); // Xóa chọn tất cả
                dgvHocvien.Rows[0].Selected = true; // Chọn hàng đầu tiên
                dgvHocvien.CurrentCell = dgvHocvien.Rows[0].Cells[0]; // Chọn ô đầu tiên
                dgvHocvien.Focus(); // Đưa focus vào DataGridView
                HienThiThongTinHocVien(dgvHocvien.Rows[0]);// Hiển thị dữ liệu lên các TextBox
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void txtSoDienthoai_TextChanged(object sender, EventArgs e)
        {
            // Kiểm tra nếu chuỗi nhập vào không phải toàn số
            if (!Regex.IsMatch(txtSoDienthoai.Text, "^[0-9]*$"))
            {
                // Hiển thị hộp thoại cảnh báo nếu nhập ký tự không hợp lệ
                //MessageBox.Show("Số điện thoại chỉ được nhập số!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Xóa ký tự cuối cùng (ký tự không hợp lệ) khỏi ô nhập số điện thoại
                txtSoDienthoai.Text = txtSoDienthoai.Text.Remove(txtSoDienthoai.Text.Length - 1);
            }
            if (!string.IsNullOrWhiteSpace(txtSoDienthoai.Text)&& !Regex.IsMatch(txtSoDienthoai.Text, "^[0-9]{10}$"))
            {
                txtSoDienthoai.BackColor = Color.LightPink;
            }
            else
            {
                txtSoDienthoai.BackColor = Color.White;
            }
        }

        private void txtSoDienthoai_Leave(object sender, EventArgs e)
        {
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !IsValidEmail(txtEmail.Text))
            {
                txtEmail.BackColor = Color.LightPink; // Đổi màu nền nếu sai
            }
            else
            {
                txtEmail.BackColor = Color.White; // Trả lại màu nền bình thường
            }
        }

        private void txtSoDienthoai_MouseLeave(object sender, EventArgs e)
        {
            if(txtTimKiem.Text == "")
            {
                txtTimKiem.Text = "Phải có đúng 10 chữ số!";
                txtTimKiem.ForeColor = Color.Silver;
            }
        }

        private void txtSoDienthoai_Enter(object sender, EventArgs e)
        {
            if (txtTimKiem.Text == "Phải có đúng 10 chữ số!")
            {
                txtTimKiem.Text = "";
                txtTimKiem.ForeColor = Color.Black;
            }
        }

        private void cmbTieuChiTimKiem_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Cập nhật placeholder và xóa text cũ khi đổi tiêu chí
            //UpdateSearchPlaceholder();
            txtTimKiem.Clear(); // Xóa nội dung tìm kiếm cũ
            txtTimKiem.Focus(); // Đặt focus vào ô nhập liệu
        }
       

        private void btnTimKiem_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void btnTimKiem_Enter(object sender, EventArgs e)
        {
           
        }
        private void btnTimKiem_Leave(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
          
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
      
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMahocvien.Text) ||
       string.IsNullOrWhiteSpace(txtHoten.Text) ||
       string.IsNullOrWhiteSpace(txtDiachi.Text) ||
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
               
                //  Lấy Makhoathi từ khoathi
                string selectedKhoathi = cmbTenkhoathi.Text.Trim(); // Lấy chính xác tên khóa học
                if (string.IsNullOrEmpty(selectedKhoathi))
                {
                    throw new Exception("Vui lòng chọn chứng chỉ!");
                }

                string sqlGetMakhoathi = "SELECT Makhoathi FROM khoathi WHERE tenkhoathi = @Tenkhoathi";
                string makhoathi = "";

                using (SqlCommand cmd = new SqlCommand(sqlGetMakhoathi, MyCon, transaction))
                {
                    cmd.Parameters.AddWithValue("@Tenkhoathi", selectedKhoathi);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        makhoathi = result.ToString();
                    else
                        throw new Exception("Không tìm thấy khóa học trong hệ thống!");
                }


                // 3. Đăng ký học viên vào khóa học trong bảng DANGKyThi
                string sqlInsertDangKyHoc = "INSERT INTO ketquathi (MaHocVien, Makhoathi) VALUES (@MA, @Makt)";

                using (SqlCommand cmd = new SqlCommand(sqlInsertDangKyHoc, MyCon, transaction))
                {
                    cmd.Parameters.AddWithValue("@MA", txtMahocvien.Text);
                    cmd.Parameters.AddWithValue("@Makt", makhoathi);
                    cmd.ExecuteNonQuery();
                }

                // 4. Commit giao dịch
                transaction.Commit();

                Load_DataGridView();
                MessageBox.Show("Bạn đã lưu học viên và đăng ký khóa thi thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                MyCon.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            // Kiểm tra xem có mục nào được chọn trong ComboBox không
            if (cmbTenKhoaHoc.SelectedIndex <= 0 || cmbTenKhoaHoc.SelectedValue == null) // Index 0 là "-- Hiển thị tất cả --"
            {
                MessageBox.Show("Vui lòng chọn một khóa học cụ thể từ danh sách để in báo cáo.", "Chưa chọn khóa học", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Dừng lại nếu chưa chọn khóa học cụ thể
            }

            // Lấy tên khóa học đang được chọn
            // Vì ValueMember là "TENKHOAHOC", SelectedValue trả về tên khóa học
            string selectedTenKhoaHoc = cmbTenKhoaHoc.SelectedValue.ToString();

            // Tạo instance của FrmBaocao bằng constructor mới, truyền tên khóa học vào
            FrmBaocao reportForm = new FrmBaocao(selectedTenKhoaHoc);

            // Hiển thị form báo cáo
            // reportForm.ShowDialog(); // Hiển thị dạng modal (chặn tương tác với form Học viên)
            reportForm.Show();
        }
        private void LoadKhoaHoc()
        {
            DataTable dt = new DataTable();
            try
            {
                // Sử dụng using để đảm bảo kết nối được đóng đúng cách
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456")) // Tạo kết nối mới hoặc dùng MyCon nếu quản lý cẩn thận
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    string sql = "SELECT MAKHOAHOC, TENKHOAHOC FROM KHOAHOC ORDER BY TENKHOAHOC";
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                    adapter.Fill(dt);
                } // Kết nối sẽ tự động đóng ở đây khi ra khỏi khối using

                // Thêm dòng "Tất cả" vào đầu DataTable
                DataRow dr = dt.NewRow();
                // Chúng ta vẫn cần MAKHOAHOC để phân biệt, nhưng ValueMember sẽ là TENKHOAHOC
                dr["MAKHOAHOC"] = -1; // Giá trị đặc biệt vẫn hữu ích
                dr["TENKHOAHOC"] = "-- Chọn Khóa Học --";
                dt.Rows.InsertAt(dr, 0); // Chèn vào vị trí đầu tiên

                // Gán DataSource cho ComboBox
                cmbTenKhoaHoc.DataSource = dt;
                cmbTenKhoaHoc.DisplayMember = "TENKHOAHOC"; // Cột hiển thị tên
                cmbTenKhoaHoc.ValueMember = "TENKHOAHOC";   // *** THAY ĐỔI QUAN TRỌNG: Lấy giá trị theo TÊN KHÓA HỌC ***

                // Đặt mục được chọn mặc định là "Tất cả"
                cmbTenKhoaHoc.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách khóa học: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Không cần finally để đóng kết nối nếu dùng using
        }
        private void cmbTenKhoaHoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kiểm tra xem có mục nào được chọn không và ComboBox đã được đổ dữ liệu chưa
            if (cmbTenKhoaHoc.SelectedIndex == -1 || cmbTenKhoaHoc.SelectedValue == null)
            {
                // dgvHocvien.DataSource = null; // Tùy chọn: Xóa dữ liệu cũ nếu muốn
                return;
            }

            try
            {
                // Lấy TENKHOAHOC từ mục được chọn trong ComboBox
                // Vì ValueMember đã là "TENKHOAHOC", SelectedValue sẽ trả về chuỗi tên khóa học
                string selectedTenKhoaHoc = cmbTenKhoaHoc.SelectedValue.ToString();

                // Kiểm tra nếu người dùng chọn "Tất cả"
                if (selectedTenKhoaHoc == "-- Chọn Khóa Học --")
                {
                    // Tải lại toàn bộ danh sách học viên
                    Load_DataGridView_All(); // Gọi hàm tải tất cả dữ liệu
                }
                else
                {
                    // Tải dữ liệu học viên theo TENKHOAHOC đã chọn
                    Load_DataGridView_Filtered(selectedTenKhoaHoc);

                    if (dgvHocvien.Rows.Count > 0)
                    {
                        dgvHocvien.ClearSelection(); // Xóa chọn tất cả
                        dgvHocvien.Rows[0].Selected = true; // Chọn hàng đầu tiên
                        dgvHocvien.CurrentCell = dgvHocvien.Rows[0].Cells[0]; // Chọn ô đầu tiên
                        dgvHocvien.Focus(); // Đưa focus vào DataGridView
                        HienThiThongTinHocVien(dgvHocvien.Rows[0]);// Hiển thị dữ liệu lên các TextBox

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu học viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Không cần đóng kết nối ở đây nếu dùng using trong các hàm tải dữ liệu
            }
        }

        // Hàm tải dữ liệu ĐÃ LỌC theo TENKHOAHOC
        private void Load_DataGridView_Filtered(string tenKhoaHoc)
        {
            try
            {
                // Sử dụng using để quản lý kết nối
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456"))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    // Parameterized Query để tránh SQL Injection và xử lý tên khóa học đúng
                    string sql = @"SELECT HOCVIEN.MAHOCVIEN, HOTEN, NGAYSINH, GIOITINH, DIACHI, SODIENTHOAI, EMAIL, KHOAHOC.TENKHOAHOC
                               FROM HOCVIEN
                               INNER JOIN DANGKYHOC ON DANGKYHOC.MAHOCVIEN = HOCVIEN.MAHOCVIEN
                               INNER JOIN KHOAHOC ON KHOAHOC.MAKHOAHOC = DANGKYHOC.MAKHOAHOC
                               WHERE KHOAHOC.TENKHOAHOC = @tenKhoaHoc"; // Lọc theo TENKHOAHOC

                    SqlCommand cmd = new SqlCommand(sql, con);
                    // Thêm Parameter với kiểu dữ liệu phù hợp (ví dụ: NVarChar nếu tên có Unicode)
                    cmd.Parameters.Add("@tenKhoaHoc", SqlDbType.NVarChar).Value = tenKhoaHoc;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvHocvien.DataSource = dt; // Gán DataSource
                } // Kết nối tự đóng

                // Định dạng cột sau khi gán DataSource
                FormatDataGridViewColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc danh sách học viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvHocvien.DataSource = null; // Xóa dữ liệu nếu có lỗi
            }
        }

        // Hàm tải TẤT CẢ dữ liệu học viên (tên hàm gốc: Load_DataGridView)
        private void Load_DataGridView_All() // Đổi tên cho rõ ràng
        {
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456"))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    string sql = @"SELECT HOCVIEN.MAHOCVIEN, HOTEN, NGAYSINH, GIOITINH, DIACHI, SODIENTHOAI, EMAIL, KHOAHOC.TENKHOAHOC
                               FROM HOCVIEN
                               INNER JOIN DANGKYHOC ON DANGKYHOC.MAHOCVIEN = HOCVIEN.MAHOCVIEN
                               INNER JOIN KHOAHOC ON KHOAHOC.MAKHOAHOC = DANGKYHOC.MAKHOAHOC";

                    SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvHocvien.DataSource = dt;
                } // Kết nối tự đóng

                // Định dạng cột
                FormatDataGridViewColumns(); // Gọi hàm định dạng chung
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách học viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvHocvien.DataSource = null; // Xóa dữ liệu nếu có lỗi
            }
        }

        // Hàm phụ trợ để định dạng cột (tránh lặp code)
        private void FormatDataGridViewColumns()
        {
            // Thêm kiểm tra DataSource không null trước khi định dạng
            if (dgvHocvien.DataSource == null || dgvHocvien.Columns.Count == 0) return;

            // Kiểm tra số lượng cột để tránh lỗi IndexOutOfRangeException
            if (dgvHocvien.Columns.Count > 7)
            {
                dgvHocvien.Columns["MAHOCVIEN"].HeaderText = "Mã Học Viên"; // Nên dùng tên cột nếu biết rõ
                dgvHocvien.Columns["HOTEN"].HeaderText = "Họ Tên";

                dgvHocvien.Columns["NGAYSINH"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvHocvien.Columns["NGAYSINH"].HeaderText = "Ngày Sinh";

                dgvHocvien.Columns["GIOITINH"].HeaderText = "Giới Tính";
                dgvHocvien.Columns["DIACHI"].HeaderText = "Địa Chỉ";
                dgvHocvien.Columns["SODIENTHOAI"].HeaderText = "Số Điện Thoại";

                dgvHocvien.Columns["EMAIL"].HeaderText = "Email"; // Sửa lỗi chính tả "Emaii"
                dgvHocvien.Columns["TENKHOAHOC"].HeaderText = "Khóa Học";

                // Các cài đặt khác
                dgvHocvien.AllowUserToAddRows = false;
                dgvHocvien.EditMode = DataGridViewEditMode.EditProgrammatically;
                dgvHocvien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Ví dụ: Tự động fill chiều rộng
                                                                                       // dgvHocvien.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells); // Hoặc tự động điều chỉnh theo nội dung
            }
            else
            {
                // Có thể log lỗi hoặc thông báo nếu số cột không như mong đợi
                Console.WriteLine("Cảnh báo: Số lượng cột trong DataGridView không như mong đợi.");
            }
        }

        private void Load_DataGridView()
        {
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();
            string sql = @"SELECT HOCVIEN.MAHOCVIEN,HOTEN,NGAYSINH,GIOITINH,DIACHI,SODIENTHOAI,EMAIL,KHOAHOC.TENKHOAHOC FROM HOCVIEN 
                           INNER JOIN DANGKYHOC ON DANGKYHOC.MAHOCVIEN = HOCVIEN.MAHOCVIEN
                           INNER JOIN KHOAHOC ON KHOAHOC.MAKHOAHOC = DANGKYHOC.MAKHOAHOC";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, MyCon);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dgvHocvien.DataSource = dt;

            dgvHocvien.Columns[0].HeaderText = "Mã Học Viên";
            dgvHocvien.Columns[1].HeaderText = "Họ Tên";

            dgvHocvien.Columns[2].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvHocvien.Columns[2].HeaderText = "Ngày Sinh";

            dgvHocvien.Columns[3].HeaderText = "Giới Tính";
            dgvHocvien.Columns[4].HeaderText = "Địa Chỉ";
            dgvHocvien.Columns[5].HeaderText = "Số Điện Thoại";

            dgvHocvien.Columns[6].HeaderText = "Emaii";
            dgvHocvien.Columns[7].HeaderText = "Khóa Học";

            dgvHocvien.Columns["EMAIL"].Width = 150;
            dgvHocvien.Columns["DIACHI"].Width = 250;
            dgvHocvien.Columns["GIOITINH"].Width = 50;
            dgvHocvien.Columns["SODIENTHOAI"].Width = 100;
            dgvHocvien.Columns["NGAYSINH"].Width = 70;
            dgvHocvien.AllowUserToAddRows = false;
            dgvHocvien.EditMode = DataGridViewEditMode.EditProgrammatically;

        }

        // Sự kiện CellClick xảy ra khi click vào BẤT KỲ ô nào trong DataGridView.
        private void dgvHocvien_CellClick_1(object sender, DataGridViewCellEventArgs e)
        { 
            // e.RowIndex là chỉ số của dòng được click. Dòng tiêu đề có chỉ số là -1.
            // dgvHocvien.Rows.Count là tổng số dòng đang có trong bảng (bao gồm cả dòng mới nếu có).
            if (e.RowIndex >= 0 && e.RowIndex < dgvHocvien.Rows.Count)
            {
                // Nếu điều kiện trên đúng, nghĩa là đã click vào một dòng dữ liệu hợp lệ.
                // --- BƯỚC 2: LẤY RA DÒNG ĐƯỢC CLICK ---
                // Lấy ra đối tượng DataGridViewRow tương ứng với chỉ số dòng vừa click.
                // Biến 'row' bây giờ sẽ chứa toàn bộ thông tin của dòng đó.
                DataGridViewRow row = dgvHocvien.Rows[e.RowIndex];

                // --- BƯỚC 3: LẤY DỮ LIỆU TỪNG Ô TRONG DÒNG VÀ HIỂN THỊ LÊN CÁC CONTROL ---
                // Sử dụng tên cột (ví dụ: "HOTEN") để truy cập vào ô (Cell) tương ứng trong dòng 'row'.
                // Sử dụng toán tử ?. (null-conditional) và ?? (null-coalescing) để lấy dữ liệu AN TOÀN,
                // tránh lỗi nếu ô đó không có dữ liệu (null).

                // Lấy Họ tên:
                // 1. row.Cells["HOTEN"] : Lấy ô ở cột "HOTEN".
                // 2. ?.Value           : Nếu ô đó tồn tại, lấy giá trị (Value) của nó. Nếu không, kết quả là null.
                // 3. ?.ToString()      : Nếu giá trị vừa lấy được không phải null, chuyển nó thành chuỗi. Nếu là null, kết quả là null.
                // 4. ?? ""             : Nếu kết quả cuối cùng của chuỗi trên là null, thì dùng chuỗi rỗng "" thay thế.
                // => Gán kết quả (là chuỗi họ tên hoặc chuỗi rỗng) vào TextBox txtHoten.

                //THÊM DÒNG NÀY ĐỂ CẬP NHẬT MÃ HỌC VIÊN
                txtMahocvien.Text = row.Cells["MAHOCVIEN"]?.Value?.ToString() ?? "";
                txtHoten.Text = row.Cells["HOTEN"]?.Value?.ToString() ?? "";

                // Lấy Ngày sinh (Kiểm tra kiểu dữ liệu trước khi gán cho DateTimePicker nếu cần)
                // Nếu dtpNgaysinh là DateTimePicker thực sự, nên chuyển đổi giá trị:
                if (row.Cells["NGAYSINH"]?.Value != null && row.Cells["NGAYSINH"].Value != DBNull.Value)
                {
                    if (DateTime.TryParse(row.Cells["NGAYSINH"].Value.ToString(), out DateTime ngaySinh))
                    {
                        dtpNgaysinh.Value = ngaySinh;
                    }
                    else
                    {
                        // Xử lý nếu không parse được ngày tháng (có thể đặt giá trị mặc định)
                        dtpNgaysinh.Value = DateTime.Now; // Hoặc giữ nguyên giá trị cũ
                        // Hoặc gán Text nếu dtpNgaysinh không phải DateTimePicker chuẩn:
                        // dtpNgaysinh.Text = row.Cells["NGAYSINH"]?.Value?.ToString() ?? "";
                    }
                }
                else
                {
                    dtpNgaysinh.Value = DateTime.Now; // Giá trị mặc định nếu NULL
                                                      // Hoặc gán Text nếu dtpNgaysinh không phải DateTimePicker chuẩn:
                                                      // dtpNgaysinh.Text = "";
                }

                // Lấy Địa chỉ
                txtDiachi.Text = row.Cells["DIACHI"]?.Value?.ToString() ?? "";

                // Lấy Số Điện Thoại
                txtSoDienthoai.Text = row.Cells["SODIENTHOAI"]?.Value?.ToString() ?? "";

                // Lấy Email
                txtEmail.Text = row.Cells["EMAIL"]?.Value?.ToString() ?? "";

                // Lấy Giới Tính (Gán giá trị cho ComboBox)
                cmbGioiTinh.Text = row.Cells["GIOITINH"]?.Value?.ToString() ?? "";
                // Lấy Tên Khóa Học (Gán giá trị cho ComboBox)
                //cmbTenKhoaHoc.Text = row.Cells["TENKHOAHOC"]?.Value?.ToString() ?? "";

                // --- KẾT THÚC: Dữ liệu từ dòng được click đã được hiển thị lên các ô nhập liệu ---
            }
            // Nếu điều kiện if ở trên không đúng (click vào header hoặc vùng trống), thì không làm gì cả.
        }

        private void txtHoten_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

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
            string query = @"SELECT HOCVIEN.MAHOCVIEN, HOTEN, NGAYSINH, HOCVIEN.GIOITINH, DIACHI, SODIENTHOAI, EMAIL, KHOAHOC.TENKHOAHOC
                     FROM HOCVIEN
                     INNER JOIN DANGKYHOC ON DANGKYHOC.MAHOCVIEN = HOCVIEN.MAHOCVIEN
                     INNER JOIN KHOAHOC ON KHOAHOC.MAKHOAHOC = DANGKYHOC.MAKHOAHOC
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
                dgvHocvien.DataSource = null;
                dgvHocvien.DataSource = dt;
                LamDep_DataGridView(); // Làm đẹp grid (nếu cần)

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy học viên nào có tên chứa '" + searchValue + "'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetDetailFields(); // Giả sử hàm này tồn tại
                }
                else
                {
                    dgvHocvien.ClearSelection();
                    if (dgvHocvien.Rows.Count > 0)
                    {
                        dgvHocvien.Rows[0].Selected = true;
                        // Quan trọng: Cập nhật txtMahocvien và các ô khác từ dòng được chọn
                        HienThiThongTinHocVien(dgvHocvien.Rows[0]);
                        dgvHocvien.Focus();
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
    }
}
