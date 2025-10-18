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
    public partial class FrmThi : Form
    {
        SqlConnection MyCon = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456"); 
        public FrmThi()
        {
            InitializeComponent();
           
        }
        private void PerformSearch()
        {
            string searchValue = txtTimKiem.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchValue))
            {
                MessageBox.Show("Vui lòng nhập Họ tên cần tìm.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // Optional: Load lại toàn bộ danh sách nếu muốn và nếu có hàm đó
                // LoadToanBoHocVien();
                return;
            }

            // --- Lấy chuỗi kết nối (NÊN lấy từ App.config hoặc nguồn cấu hình khác) ---
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456;Encrypt=False"; // Thay bằng cách lấy chuỗi kết nối của bạn

            // --- Xây dựng câu lệnh SQL (Thêm tiền tố bảng cho rõ ràng) ---
            // Sử dụng INNER JOIN nếu chỉ muốn HV có kết quả thi. Dùng LEFT JOIN nếu muốn tất cả HV.
            string query = @"SELECT
                         HOCVIEN.MAHOCVIEN,
                         HOCVIEN.HOTEN,
                         HOCVIEN.NGAYSINH,
                         HOCVIEN.GIOITINH,
                         HOCVIEN.DIACHI,
                         HOCVIEN.SODIENTHOAI,
                         HOCVIEN.EMAIL,
                         KHOATHI.TENKHOATHI
                     FROM HOCVIEN
                     INNER JOIN KETQUATHI ON KETQUATHI.MAHOCVIEN = HOCVIEN.MAHOCVIEN -- Hoặc LEFT JOIN
                     INNER JOIN KHOATHI ON KHOATHI.MAKHOATHI = KETQUATHI.MAKHOATHI -- Hoặc LEFT JOIN
                     WHERE HOCVIEN.HOTEN LIKE @SearchValue"; // Tìm tên chứa searchValue

            DataTable dt = new DataTable(); // Tạo DataTable ở ngoài để có thể sử dụng sau try-catch nếu cần

            try
            {
                // *** Sử dụng using cho SqlConnection và SqlCommand ***
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open(); // Mở kết nối ngay trước khi dùng
                    }

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Thêm tham số @SearchValue một cách an toàn
                        // Sử dụng Add thay vì AddWithValue để chỉ định kiểu rõ ràng hơn (tùy chọn)
                        cmd.Parameters.Add("@SearchValue", SqlDbType.NVarChar).Value = "%" + searchValue + "%";

                        // Sử dụng using cho SqlDataAdapter (tùy chọn nhưng là thực hành tốt)
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt); // Đổ dữ liệu vào DataTable
                        }
                    }
                } // Kết nối sẽ tự động đóng ở đây, kể cả khi có lỗi

                // --- Xử lý kết quả ---
                dgvThi.DataSource = null; // Xóa binding cũ
                dgvThi.DataSource = dt;   // Gán nguồn dữ liệu mới
                LamDep_DataGridView();        // Làm đẹp grid (nếu cần)

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy học viên nào có tên chứa '" + searchValue + "'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetDetailFields(); // Xóa thông tin chi tiết cũ
                }
                else
                {
                    // Chọn dòng đầu tiên và hiển thị thông tin
                    dgvThi.ClearSelection();
                    if (dgvThi.Rows.Count > 0) // Kiểm tra lại phòng trường hợp grid rỗng sau khi binding
                    {
                        dgvThi.Rows[0].Selected = true;
                        // Đảm bảo HienThiThongTinHocVien tồn tại và xử lý đúng DataGridViewRow
                        HienThiThongTinHocVien(dgvThi.Rows[0]);
                        dgvThi.Focus();
                    }
                    else
                    {
                        ResetDetailFields(); // Nếu grid vì lý do nào đó rỗng thì cũng reset
                    }
                }
            }
            catch (SqlException sqlEx) // Bắt lỗi SQL cụ thể
            {
                MessageBox.Show("Lỗi SQL khi tìm kiếm học viên: " + sqlEx.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetDetailFields();
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                MessageBox.Show("Lỗi không xác định khi tìm kiếm học viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetDetailFields();
            }
        }
        private void FrmThi_Load(object sender, EventArgs e)
        {
            //SetTextBox(false);
            //SetButton(true);
            Load_DataGridView();
            LamDep_DataGridView();
            if (dgvThi.Rows.Count > 0)
            {
                dgvThi.ClearSelection(); // Xóa chọn tất cả
                dgvThi.Rows[0].Selected = true; // Chọn hàng đầu tiên
                dgvThi.CurrentCell = dgvThi.Rows[0].Cells[0]; // Chọn ô đầu tiên
                dgvThi.Focus(); // Đưa focus vào DataGridView
                HienThiThongTinHocVien(dgvThi.Rows[0]);// Hiển thị dữ liệu lên các TextBox
            }
            LoadKhoaThi();
        }
        private void Load_DataGridView()
        {
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();
            string sql = @"SELECT HOCVIEN.MAHOCVIEN,HOTEN,NGAYSINH,GIOITINH,DIACHI,SODIENTHOAI,EMAIL,KHOATHI.TENKHOATHI FROM HOCVIEN 
                           INNER JOIN KETQUATHI ON KETQUATHI.MAHOCVIEN = HOCVIEN.MAHOCVIEN
                           INNER JOIN KHOATHI ON KHOATHI.MAKHOATHI= KETQUATHI.MAKHOATHI";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, MyCon);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dgvThi.DataSource = dt;

            dgvThi.Columns[0].HeaderText = "Mã Học Viên";
            dgvThi.Columns[1].HeaderText = "Họ Tên";

            dgvThi.Columns[2].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvThi.Columns[2].HeaderText = "Ngày Sinh";

            dgvThi.Columns[3].HeaderText = "Giới Tính";
            dgvThi.Columns[4].HeaderText = "Địa Chỉ";
            dgvThi.Columns[5].HeaderText = "Số Điện Thoại";

            dgvThi.Columns[6].HeaderText = "Emaii";
            dgvThi.Columns[7].HeaderText = "Tên Khóa Thi";
            dgvThi.Columns["TENKHOATHI"].Width = 400;
            dgvThi.Columns["DiaChi"].Width = 250;
            dgvThi.Columns["GIOITINH"].Width = 100;
            //dgvThi.Columns["NGAYSINH"].Width = 100;
            //dgvThi.Columns["SODIENTHOAI"].Width = 100;
            //dgvThi.Columns["MAHOCVIEN"].Width = 100;
            //dgvThi.Columns["EMAIL"].Width = 100;
            //dgvThi.Columns["HOTEN"].Width = 100;
            
            
            dgvThi.AllowUserToAddRows = false;
            dgvThi.EditMode = DataGridViewEditMode.EditProgrammatically;

        }
        private void LoadKhoaTHI()
        {
            DataTable dt = new DataTable();
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456";
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string sql = "SELECT MAKHOATHI, TENKHOATHI FROM KHOATHI ORDER BY TENKHOATHI";
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                    adapter.Fill(dt);
                }

                DataRow dr = dt.NewRow();
                // Sửa ở đây: dùng một giá trị đặc biệt cho mục placeholder
                dr["TENKHOATHI"] = "-- Chọn Khóa Thi --";
                dr["MAKHOATHI"] = "0"; // Hoặc một mã không tồn tại
                dt.Rows.InsertAt(dr, 0);

                cmbTenkhoathi.DataSource = dt;
                cmbTenkhoathi.DisplayMember = "TENKHOATHI"; // Hiển thị TÊN
                cmbTenkhoathi.ValueMember = "TENKHOATHI";   // Lấy giá trị là TÊN (để không hỏng chức năng lọc)

                cmbTenkhoathi.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách khóa thi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LamDep_DataGridView()
        {
            // Đặt chiều cao của tiêu đề
            dgvThi.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;//Ngăn không cho hệ thống và người dùng thay đổi chiều cao tiêu đề cột
            dgvThi.ColumnHeadersHeight = 40; // Chiều cao tiêu đề

            // Đặt màu nền và màu chữ cho tiêu đề
            dgvThi.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 212, 190);
            dgvThi.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvThi.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);

            // Áp dụng màu cho một cột cụ thể
            dgvThi.Columns["MAHOCVIEN"].HeaderCell.Style.BackColor = Color.FromArgb(234, 0, 1); 
            // Cập nhật lại giao diện
            dgvThi.EnableHeadersVisualStyles = false;
        }

        private void dgvThi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtMahocvien.Text = dgvThi.CurrentRow.Cells["MAHOCVIEN"].Value.ToString();
            txtHoten.Text = dgvThi.CurrentRow.Cells["HOTEN"].Value.ToString();
            cmbGioiTinh.Text = dgvThi.CurrentRow.Cells["GIOITINH"].Value.ToString();
            txtDiachi.Text = dgvThi.CurrentRow.Cells["DIACHI"].Value.ToString();
            txtEmail.Text = dgvThi.CurrentRow.Cells["EMAIL"].Value.ToString();
            txtSoDienthoai.Text = dgvThi.CurrentRow.Cells["SODIENTHOAI"].Value.ToString();
            dtpNgaysinh.Text = dgvThi.CurrentRow.Cells["NGAYSINH"].Value.ToString();
            //cmbTenkhoathi.Text = dgvThi.CurrentRow.Cells["TENKHOATHI"].Value.ToString();
        }
        private void HienThiThongTinHocVien(DataGridViewRow row)
        {
            if (row != null)
            {
                txtMahocvien.Text = dgvThi.CurrentRow.Cells["MAHOCVIEN"].Value.ToString();
                txtHoten.Text = dgvThi.CurrentRow.Cells["HOTEN"].Value.ToString();
                cmbGioiTinh.Text = dgvThi.CurrentRow.Cells["GIOITINH"].Value.ToString();
                txtDiachi.Text = dgvThi.CurrentRow.Cells["DIACHI"].Value.ToString();
                txtEmail.Text = dgvThi.CurrentRow.Cells["EMAIL"].Value.ToString();
                txtSoDienthoai.Text = dgvThi.CurrentRow.Cells["SODIENTHOAI"].Value.ToString();
                dtpNgaysinh.Text = dgvThi.CurrentRow.Cells["NGAYSINH"].Value.ToString();
                //cmbTenkhoathi.Text = dgvThi.CurrentRow.Cells["TENKHOATHI"].Value.ToString();
            }
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




                // 4. Đưa con trỏ vào ô Họ tên để người dùng nhập tiếp
                txtHoten.Focus();
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Lỗi cơ sở dữ liệu khi lấy mã học viên: {sqlEx.Message}", "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi không mong muốn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            // Không cần MyCon.Close() ở đây nếu bạn dùng 'using' cho SqlConnection


        }
        // Hàm phụ trợ để dọn dẹp form khi thêm mới
        private void ResetDetailFieldsForAdd()
        {
            txtHoten.Clear();
            txtDiachi.Clear();
            txtSoDienthoai.Clear();
            txtEmail.Clear();
            dtpNgaysinh.Value = DateTime.Now;
            cmbGioiTinh.SelectedIndex = -1; // Bỏ chọn
            cmbTenkhoathi.SelectedIndex = 0; // Chọn mục "-- Chọn Khóa Thi --"
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
        private bool MaHocVienDaTonTai(string maHocVien, SqlConnection connection)
        {
            // Đảm bảo kết nối đang mở để thực hiện truy vấn
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            // Câu lệnh SQL đếm số lượng bản ghi có mã học viên trùng khớp
            string query = "SELECT COUNT(*) FROM HocVien WHERE MaHocVien = @MA";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@MA", maHocVien);

                // ExecuteScalar trả về giá trị của cột đầu tiên, dòng đầu tiên (trong trường hợp này là số lượng)
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                // Nếu số lượng > 0, tức là mã đã tồn tại
                return count > 0;
            }
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            // === BƯỚC 1: KIỂM TRA DỮ LIỆU ĐẦU VÀO (Giống như trước) ===
            if (string.IsNullOrWhiteSpace(txtMahocvien.Text)) { /* ... */ return; }
            if (string.IsNullOrWhiteSpace(txtHoten.Text)) { /* ... */ return; }
            // ... các kiểm tra khác của bạn
            if (cmbTenkhoathi.SelectedValue == null || cmbTenkhoathi.SelectedValue.ToString() == "-- Chọn Khóa Thi --")
            {
                MessageBox.Show("Vui lòng chọn một Khóa thi.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTenkhoathi.Focus();
                return;
            }
            // ... các kiểm tra định dạng email, sđt

            string maHocVien = txtMahocvien.Text.Trim();
            string tenKhoaThiDaChon = cmbTenkhoathi.SelectedValue.ToString();
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456";

            // === BƯỚC 2: THỰC HIỆN LƯU VÀO CSDL VỚI TRANSACTION ===
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;
                try
                {
                    con.Open();

                    // Kiểm tra mã học viên đã tồn tại chưa
                    if (MaHocVienDaTonTai(maHocVien, con))
                    {
                        MessageBox.Show($"Mã học viên '{maHocVien}' đã tồn tại.", "Trùng lặp dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // *** BƯỚC MỚI & QUAN TRỌNG: LẤY MAKHOATHI TỪ TENKHOATHI ***
                    string maKhoaThiCanLuu = "";
                    string sqlGetMaKT = "SELECT MaKhoaThi FROM KhoaThi WHERE TenKhoaThi = @TenKT";
                    using (SqlCommand cmdGetMa = new SqlCommand(sqlGetMaKT, con))
                    {
                        cmdGetMa.Parameters.AddWithValue("@TenKT", tenKhoaThiDaChon);
                        object result = cmdGetMa.ExecuteScalar(); // Dùng ExecuteScalar để lấy một giá trị duy nhất

                        if (result != null)
                        {
                            maKhoaThiCanLuu = result.ToString();
                        }
                        else
                        {
                            // Trường hợp hiếm gặp: Tên khóa thi có trong ComboBox nhưng đã bị xóa khỏi CSDL
                            MessageBox.Show($"Không tìm thấy khóa thi '{tenKhoaThiDaChon}' trong cơ sở dữ liệu.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; // Dừng lại
                        }
                    }
                    // Tại đây, chúng ta đã có `maKhoaThiCanLuu` (ví dụ: "KT001")

                    // Bắt đầu transaction để đảm bảo toàn vẹn dữ liệu
                    transaction = con.BeginTransaction();

                    // 3.1. Thêm học viên vào bảng HOCVIEN
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

                    // 3.2. Đăng ký thi cho học viên, bây giờ đã có MÃ KHÓA THI đúng
                    string sqlInsertKetQuaThi = "INSERT INTO KetQuaThi (MaHocVien, MaKhoaThi) VALUES (@MA_HV, @MA_KT)";
                    using (SqlCommand cmdKetQuaThi = new SqlCommand(sqlInsertKetQuaThi, con, transaction))
                    {
                        cmdKetQuaThi.Parameters.AddWithValue("@MA_HV", maHocVien);
                        cmdKetQuaThi.Parameters.AddWithValue("@MA_KT", maKhoaThiCanLuu); // Dùng mã vừa tìm được
                        cmdKetQuaThi.ExecuteNonQuery();
                    }

                    // 3.3. Commit giao dịch nếu mọi thứ thành công
                    transaction.Commit();
                    MessageBox.Show("Đã lưu thông tin và đăng ký thi cho học viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    if (transaction != null) transaction.Rollback(); // Hoàn tác nếu có lỗi
                    MessageBox.Show("Đã có lỗi xảy ra trong quá trình lưu dữ liệu.\nChi tiết: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Tải lại dữ liệu để cập nhật GridView
                    Load_DataGridView();
                    LamDep_DataGridView();
                    if (dgvThi.Rows.Count > 0)
                    {
                        dgvThi.Rows[0].Selected = true;
                        HienThiThongTinHocVien(dgvThi.Rows[0]);
                    }
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvThi.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn học viên cần xóa!", "Chưa chọn học viên", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maHocVien = dgvThi.CurrentRow.Cells["MAHOCVIEN"].Value.ToString();

            DialogResult confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa vĩnh viễn học viên '{maHocVien}' không? Mọi dữ liệu liên quan sẽ bị mất.",
                                                  "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.No)
            {
                return;
            }

            // Sử dụng using để quản lý kết nối và transaction
            using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456"))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    // BƯỚC 1: Xóa các bản ghi liên quan trong bảng con (KETQUATHI) trước
                    string sqlDeleteKetQua = "DELETE FROM KETQUATHI WHERE MAHOCVIEN = @MaHocVien";
                    using (SqlCommand cmd1 = new SqlCommand(sqlDeleteKetQua, con, transaction))
                    {
                        cmd1.Parameters.AddWithValue("@MaHocVien", maHocVien);
                        cmd1.ExecuteNonQuery();
                    }

                    // BƯỚC 2: Xóa bản ghi trong bảng cha (HOCVIEN) sau
                    // **** ĐÂY LÀ PHẦN QUAN TRỌNG BỊ THIẾU ****
                    string sqlDeleteHocVien = "DELETE FROM HOCVIEN WHERE MAHOCVIEN = @MaHocVien";
                    using (SqlCommand cmd2 = new SqlCommand(sqlDeleteHocVien, con, transaction))
                    {
                        cmd2.Parameters.AddWithValue("@MaHocVien", maHocVien);
                        cmd2.ExecuteNonQuery();
                    }

                    transaction.Commit(); // Chỉ commit khi cả hai lệnh thành công
                    MessageBox.Show("Đã xóa học viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Hoàn tác tất cả các thay đổi nếu có lỗi
                    MessageBox.Show("Lỗi khi xóa học viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Tải lại dữ liệu sau khi thực hiện xong
            Load_DataGridView();
            if (dgvThi.Rows.Count > 0)
            {
                HienThiThongTinHocVien(dgvThi.Rows[0]);
            }
            else
            {
                ResetDetailFields();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (MyCon.State == ConnectionState.Closed) MyCon.Open();

                // Câu lệnh UPDATE thông tin học viên
                string sql = "UPDATE HOCVIEN SET HOTEN = @TEN, DIACHI = @DC, EMAIL = @E, SODIENTHOAI = @SDT, NGAYSINH = @NS,GIOITINH = @GT WHERE MAHOCVIEN = @MA";
                SqlCommand Cmd = new SqlCommand(sql, MyCon);
                Cmd.Parameters.AddWithValue("@TEN", txtHoten.Text);
                Cmd.Parameters.AddWithValue("@MA", txtMahocvien.Text);
                Cmd.Parameters.AddWithValue("@DC", txtDiachi.Text);
                Cmd.Parameters.AddWithValue("@E", txtEmail.Text);
                Cmd.Parameters.AddWithValue("@SDT", txtSoDienthoai.Text);
                Cmd.Parameters.AddWithValue("@GT", cmbGioiTinh.Text);
                Cmd.Parameters.AddWithValue("@NS", dtpNgaysinh.Value);
                Cmd.ExecuteNonQuery();

                // Load lại danh sách để thấy thay đổi
                

                MessageBox.Show("Bạn đã sửa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //refresh form
                Load_DataGridView();
               
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

        private void dgvThi_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        // Hàm phụ trợ để định dạng cột (tránh lặp code)
        private void FormatDataGridViewColumns()
        {
            // Thêm kiểm tra DataSource không null trước khi định dạng
            if (dgvThi.DataSource == null || dgvThi.Columns.Count == 0) return;

            // Kiểm tra số lượng cột để tránh lỗi IndexOutOfRangeException
            if (dgvThi.Columns.Count > 7)
            {
                dgvThi.Columns["MAHOCVIEN"].HeaderText = "Mã Học Viên"; // Nên dùng tên cột nếu biết rõ
                dgvThi.Columns["HOTEN"].HeaderText = "Họ Tên";

                dgvThi.Columns["NGAYSINH"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvThi.Columns["NGAYSINH"].HeaderText = "Ngày Sinh";

                dgvThi.Columns["GIOITINH"].HeaderText = "Giới Tính";
                dgvThi.Columns["DIACHI"].HeaderText = "Địa Chỉ";
                dgvThi.Columns["SODIENTHOAI"].HeaderText = "Số Điện Thoại";

                dgvThi.Columns["EMAIL"].HeaderText = "Email"; // Sửa lỗi chính tả "Emaii"
                dgvThi.Columns["TENKHOATHI"].HeaderText = "Khóa Thi";

                // Các cài đặt khác
                dgvThi.AllowUserToAddRows = false;
                dgvThi.EditMode = DataGridViewEditMode.EditProgrammatically;
                dgvThi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Ví dụ: Tự động fill chiều rộng
                                                                                       // dgvThi.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells); // Hoặc tự động điều chỉnh theo nội dung
            }
            else
            {
                // Có thể log lỗi hoặc thông báo nếu số cột không như mong đợi
                Console.WriteLine("Cảnh báo: Số lượng cột trong DataGridView không như mong đợi.");
            }
        }
        private void Load_DataGridView_Filtered(string tenKhoaThi)
        {
            try
            {
                // Sử dụng using để quản lý kết nối
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456"))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    // Parameterized Query để tránh SQL Injection và xử lý tên khóa học đúng
                    string sql = @"SELECT HOCVIEN.MAHOCVIEN, HOTEN, NGAYSINH, GIOITINH, DIACHI, SODIENTHOAI, EMAIL, KHOATHI.TENKHOATHI
                               FROM HOCVIEN
                               INNER JOIN KETQUATHI ON KETQUATHI.MAHOCVIEN = HOCVIEN.MAHOCVIEN
                               INNER JOIN KHOATHI ON KHOATHI.MAKHOATHI = KETQUATHI.MAKHOATHI
                               WHERE KHOATHI.TENKHOATHI = @tenKhoaThi"; // Lọc theo TENKHOAHOC

                    SqlCommand cmd = new SqlCommand(sql, con);
                    // Thêm Parameter với kiểu dữ liệu phù hợp (ví dụ: NVarChar nếu tên có Unicode)
                    cmd.Parameters.Add("@tenKhoaThi", SqlDbType.NVarChar).Value = tenKhoaThi;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvThi.DataSource = dt; // Gán DataSource
                } // Kết nối tự đóng

                // Định dạng cột sau khi gán DataSource
                FormatDataGridViewColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc danh sách học viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvThi.DataSource = null; // Xóa dữ liệu nếu có lỗi
            }
        }
        private void Load_DataGridView_All() // Đổi tên cho rõ ràng
        {
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456"))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    string sql = @"SELECT HOCVIEN.MAHOCVIEN, HOTEN, NGAYSINH, GIOITINH, DIACHI, SODIENTHOAI, EMAIL, KHOATHI.TENKHOATHI
                               FROM HOCVIEN
                               INNER JOIN KETQUATHI ON KETQUATHI.MAHOCVIEN = HOCVIEN.MAHOCVIEN
                               INNER JOIN KHOATHI ON KHOATHI.MAKHOATHI = KETQUATHI.MAKHOATHI";

                    SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvThi.DataSource = dt;
                } // Kết nối tự đóng

                // Định dạng cột
                FormatDataGridViewColumns(); // Gọi hàm định dạng chung
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách học viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvThi.DataSource = null; // Xóa dữ liệu nếu có lỗi
            }
        }
        private void cmbTenkhoathi_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kiểm tra xem có mục nào được chọn không và ComboBox đã được đổ dữ liệu chưa
            if (cmbTenkhoathi.SelectedIndex == -1 || cmbTenkhoathi.SelectedValue == null)
            {
                // dgvThi.DataSource = null; // Tùy chọn: Xóa dữ liệu cũ nếu muốn
                return;
            }

            try
            {
                // Lấy TENKHOAHOC từ mục được chọn trong ComboBox
                // Vì ValueMember đã là "TENKHOATHI", SelectedValue sẽ trả về chuỗi tên khóa học
                string selectedTenKhoaThi = cmbTenkhoathi.SelectedValue.ToString();

                // Kiểm tra nếu người dùng chọn "Tất cả"
                if (selectedTenKhoaThi == "-- Chọn Khóa Thi --")
                {
                    // Tải lại toàn bộ danh sách học viên
                    Load_DataGridView_All(); // Gọi hàm tải tất cả dữ liệu
                }
                else
                {
                    // Tải dữ liệu học viên theo TENKHOAHOC đã chọn
                    Load_DataGridView_Filtered(selectedTenKhoaThi);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu học viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Không cần đóng kết nối ở đây nếu dùng using trong các hàm tải dữ liệu
            }
        }
        private void LoadKhoaThi()
        {
            DataTable dt = new DataTable();
            try
            {
                // Sử dụng using để đảm bảo kết nối được đóng đúng cách
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456"))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    string sql = "SELECT MAKHOATHI, TENKHOATHI FROM KHOATHI ORDER BY TENKHOATHI";
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                    adapter.Fill(dt);
                }

                // Thêm dòng "Tất cả" hoặc "Chọn Khóa Thi" vào đầu DataTable
                DataRow dr = dt.NewRow();
                dr["MAKHOATHI"] = DBNull.Value; // Dùng DBNull.Value cho mục không hợp lệ
                dr["TENKHOATHI"] = "-- Chọn Khóa Thi --";
                dt.Rows.InsertAt(dr, 0);

                // Gán DataSource cho ComboBox
                cmbTenkhoathi.DataSource = dt;
                cmbTenkhoathi.DisplayMember = "TENKHOATHI"; // Cột hiển thị tên
                cmbTenkhoathi.ValueMember = "TENKHOATHI";   // *** SỬA LẠI THÀNH MAKHOATHI ***

                // Đặt mục được chọn mặc định
                cmbTenkhoathi.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách khóa thi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            Load_DataGridView();
            LamDep_DataGridView();
            if (dgvThi.Rows.Count > 0)
            {
                dgvThi.ClearSelection(); // Xóa chọn tất cả
                dgvThi.Rows[0].Selected = true; // Chọn hàng đầu tiên
                dgvThi.CurrentCell = dgvThi.Rows[0].Cells[0]; // Chọn ô đầu tiên
                dgvThi.Focus(); // Đưa focus vào DataGridView
                HienThiThongTinHocVien(dgvThi.Rows[0]);// Hiển thị dữ liệu lên các TextBox
            }
            LoadKhoaThi();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtMahocvien.ResetText();
            txtHoten.ResetText();
            txtDiachi.ResetText();
            txtSoDienthoai.ResetText();
            txtEmail.ResetText();
            // Đưa con trỏ vào ô nhập Họ tên để người dùng nhập ngay
            txtHoten.Focus();
           


        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có mục nào được chọn trong ComboBox không
            // *** Sử dụng đúng tên ComboBox của bạn (ví dụ: cmbTenKhoaThi) ***
            if (cmbTenkhoathi.SelectedIndex <= 0 || cmbTenkhoathi.SelectedValue == null) // Index 0 thường là mục placeholder
            {
                MessageBox.Show("Vui lòng chọn một khóa thi cụ thể từ danh sách để in báo cáo.", "Chưa chọn khóa thi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Dừng lại nếu chưa chọn khóa thi cụ thể
            }

            // Lấy tên khóa thi đang được chọn
            // Đảm bảo ValueMember của cmbTenKhoaThi được đặt là cột chứa tên khóa thi (ví dụ: "TenKhoaThi")
            string selectedTenKhoaThi = cmbTenkhoathi.SelectedValue.ToString();

            // (Tùy chọn) Thêm kiểm tra nếu giá trị lấy được có thể là chuỗi rỗng
            if (string.IsNullOrEmpty(selectedTenKhoaThi))
            {
                MessageBox.Show("Giá trị khóa thi được chọn không hợp lệ.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Dừng lại nếu giá trị không hợp lệ
            }

            // --- CHỈ CÓ MỘT LẦN KHAI BÁO VÀ SỬ DỤNG reportForm Ở ĐÂY ---
            // Tạo instance của FrmBaoCaoKhoaThi bằng constructor mới, truyền tên khóa thi vào
            FrmBaoCaoKhoaThi reportForm = new FrmBaoCaoKhoaThi(selectedTenKhoaThi); // Truyền tên khóa thi

            // Hiển thị form báo cáo
            // reportForm.ShowDialog(); // Hiển thị dạng modal
            reportForm.Show(); // Hiển thị dạng non-modal
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
        private void ResetDetailFields()
        {
            txtMahocvien.ResetText();
            txtHoten.ResetText();
            cmbGioiTinh.SelectedIndex = -1; // Hoặc về mặc định
            txtDiachi.ResetText();
            txtEmail.ResetText();
            txtSoDienthoai.ResetText();
            dtpNgaysinh.Value = DateTime.Now; // Hoặc ngày mặc định
            cmbTenkhoathi.SelectedIndex = -1; // Hoặc về mặc định
        }
        private void btnTimKiem_Click(object sender, EventArgs e)
        {

            PerformSearch();
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

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
