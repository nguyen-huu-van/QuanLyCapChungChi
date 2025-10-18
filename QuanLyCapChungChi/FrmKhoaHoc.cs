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


namespace QuanLyCapChungChi
{
    public partial class FrmKhoaHoc : Form
    {
        SqlConnection MyCon = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456");
        public FrmKhoaHoc()
        {
            InitializeComponent();
        }
       
        private void FrmKhoaHoc_Load(object sender, EventArgs e)
        {
            Load_DataGridView();
            LamDep_DataGridView();
            if (dgvKhoahoc.Rows.Count > 0)
            {
                dgvKhoahoc.ClearSelection(); // Xóa chọn tất cả
                dgvKhoahoc.Rows[0].Selected = true; // Chọn hàng đầu tiên
                dgvKhoahoc.CurrentCell = dgvKhoahoc.Rows[0].Cells[0]; // Chọn ô đầu tiên
                dgvKhoahoc.Focus(); // Đưa focus vào DataGridView
                HienThiThongTinHocVien(dgvKhoahoc.Rows[0]);// Hiển thị dữ liệu lên các TextBox
            }
            LoadGiangvien();
        }
        private void LamDep_DataGridView()
        {
            dgvKhoahoc.Columns["SiSo"].Width = 250;  // Đặt chiều rộng cố định cho cột "Sĩ số"

            // Đặt chiều cao của tiêu đề
            dgvKhoahoc.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;//Ngăn không cho hệ thống và người dùng thay đổi chiều cao tiêu đề cột
            dgvKhoahoc.ColumnHeadersHeight = 40; // Chiều cao tiêu đề

            // Đặt màu nền và màu chữ cho tiêu đề
            dgvKhoahoc.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 212, 190);
            dgvKhoahoc.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvKhoahoc.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);

            // Áp dụng màu cho một cột cụ thể
            dgvKhoahoc.Columns["MAKHOAHOC"].HeaderCell.Style.BackColor = Color.FromArgb(234, 0, 1);

            // Cập nhật lại giao diện
            dgvKhoahoc.EnableHeadersVisualStyles = false;
        }

        private void HienThiThongTinHocVien(DataGridViewRow row)
        {
            if (row != null)
            {
                txtMakhoahoc.Text = dgvKhoahoc.CurrentRow.Cells["MAKHOAHOC"].Value.ToString();
                txtTenkhoahoc.Text = dgvKhoahoc.CurrentRow.Cells["TENKHOAHOC"].Value.ToString();
                txtMota.Text = dgvKhoahoc.CurrentRow.Cells["MOTA"].Value.ToString();
                txtSiso.Text = dgvKhoahoc.CurrentRow.Cells["SISO"].Value.ToString();
                dtpNgaybatdau.Text = dgvKhoahoc.CurrentRow.Cells["NGAYBATDAU"].Value.ToString();
                dtpNgayketthuc.Text = dgvKhoahoc.CurrentRow.Cells["NGAYKETTHUC"].Value.ToString();
                cmbGiangvien.Text = dgvKhoahoc.CurrentRow.Cells["HOTEN"].Value.ToString();
            }
        }

        private void Load_DataGridView()
        {
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();
            string sql = @"SELECT KHOAHOC.MAKHOAHOC,TENKHOAHOC,MOTA,NGAYBATDAU,NGAYKETTHUC,GIANGVIEN.HOTEN,SISO FROM KHOAHOC
                           INNER JOIN PHANCONG ON PHANCONG.MAKHOAHOC = KHOAHOC.MAKHOAHOC
                           INNER JOIN GIANGVIEN ON GIANGVIEN.MAGIANGVIEN = PHANCONG.MAGIANGVIEN";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, MyCon);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dgvKhoahoc.DataSource = dt;

            dgvKhoahoc.Columns[0].HeaderText = "Mã khóa học";
            dgvKhoahoc.Columns[1].HeaderText = "Tên khóa học";

            dgvKhoahoc.Columns[2].HeaderText = "Mô tả";

            dgvKhoahoc.Columns[3].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvKhoahoc.Columns[3].HeaderText = "Ngày bắt đầu";

            dgvKhoahoc.Columns[4].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvKhoahoc.Columns[4].HeaderText = "Ngày kết thúc";

            dgvKhoahoc.Columns[5].HeaderText = "Giảng viên";
            dgvKhoahoc.Columns[6].HeaderText = "Sĩ số";

            dgvKhoahoc.Columns["MOTA"].Width = 300;
            dgvKhoahoc.Columns["tenkhoahoc"].Width = 200;

            dgvKhoahoc.AllowUserToAddRows = false;
            dgvKhoahoc.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        private void dgvKhoahoc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtMakhoahoc.Text = dgvKhoahoc.CurrentRow.Cells["MAKHOAHOC"].Value.ToString();
            txtTenkhoahoc.Text = dgvKhoahoc.CurrentRow.Cells["TENKHOAHOC"].Value.ToString();
            txtMota.Text = dgvKhoahoc.CurrentRow.Cells["MOTA"].Value.ToString();
            txtSiso.Text=  dgvKhoahoc.CurrentRow.Cells["SISO"].Value.ToString();
            dtpNgaybatdau.Text = dgvKhoahoc.CurrentRow.Cells["NGAYBATDAU"].Value.ToString();
            dtpNgayketthuc.Text = dgvKhoahoc.CurrentRow.Cells["NGAYKETTHUC"].Value.ToString();
            cmbGiangvien.Text = dgvKhoahoc.CurrentRow.Cells["HOTEN"].Value.ToString();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // Mở kết nối đến CSDL nếu đang đóng
            if (MyCon.State == ConnectionState.Closed)
                MyCon.Open();
            MyCon.Close();

            // Xóa dữ liệu cũ trong các ô nhập liệu để chuẩn bị thêm học viên mới
            txtTenkhoahoc.ResetText();
            txtMakhoahoc.ResetText();
            txtMota.ResetText();
            txtSiso.ResetText();

            // Đưa con trỏ vào ô nhập Họ tên để người dùng nhập ngay
            txtTenkhoahoc.Focus();
        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra nhập liệu đầy đủ
            if (string.IsNullOrWhiteSpace(txtMakhoahoc.Text) ||
                string.IsNullOrWhiteSpace(txtTenkhoahoc.Text) ||
                string.IsNullOrWhiteSpace(txtMota.Text) || // Bỏ qua nếu Mô tả có thể trống
                string.IsNullOrWhiteSpace(txtSiso.Text) ||
                cmbGiangvien.SelectedIndex == -1) // Phải chọn một giảng viên
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin khóa học và chọn giảng viên.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Dừng lại nếu thiếu thông tin
            }

            // 2. Kiểm tra và chuyển đổi SiSo sang số (nếu cột SISO trong DB là số)
            int sisoValue;
            if (!int.TryParse(txtSiso.Text.Trim(), out sisoValue))
            {
                MessageBox.Show("Sĩ số phải là một số nguyên hợp lệ.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSiso.Focus();
                return; // Dừng lại nếu sĩ số không hợp lệ
            }

            // 3. Lấy Mã Giảng Viên từ ComboBox (Cách tốt nhất là dùng SelectedValue)
            object selectedGiangVienId = cmbGiangvien.SelectedValue;

            if (selectedGiangVienId == null)
            {
                MessageBox.Show("Không thể xác định Mã Giảng viên được chọn. Vui lòng kiểm tra lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Dừng lại nếu không lấy được mã giảng viên
            }

            // 4. Thực hiện thao tác CSDL với Transaction
            SqlTransaction transaction = null;
            try
            {
                if (MyCon.State == ConnectionState.Closed) MyCon.Open();
                transaction = MyCon.BeginTransaction();

                // --- Lệnh 1: INSERT vào bảng KHOAHOC ---
                string sqlInsertKhoaHoc = @"INSERT INTO KHOAHOC
                                    (MAKHOAHOC, TENKHOAHOC, MOTA, SISO, NGAYBATDAU, NGAYKETTHUC)
                                    VALUES (@MA, @TEN, @mt, @ss, @nbd, @nkt)";
                using (SqlCommand cmdKhoaHoc = new SqlCommand(sqlInsertKhoaHoc, MyCon, transaction))
                {
                    cmdKhoaHoc.Parameters.AddWithValue("@MA", txtMakhoahoc.Text.Trim());
                    cmdKhoaHoc.Parameters.AddWithValue("@TEN", txtTenkhoahoc.Text.Trim());
                    cmdKhoaHoc.Parameters.AddWithValue("@mt", txtMota.Text.Trim());
                    cmdKhoaHoc.Parameters.AddWithValue("@ss", sisoValue); // Dùng giá trị số đã parse
                    cmdKhoaHoc.Parameters.AddWithValue("@nbd", dtpNgaybatdau.Value); // Dùng .Value
                    cmdKhoaHoc.Parameters.AddWithValue("@nkt", dtpNgayketthuc.Value); // Dùng .Value

                    cmdKhoaHoc.ExecuteNonQuery(); // Thực thi lệnh INSERT khóa học
                }

                // --- Lệnh 2: INSERT vào bảng PHANCONG ---
                string sqlInsertPhanCong = @"INSERT INTO PHANCONG (MAKHOAHOC, MAGIANGVIEN)
                                     VALUES (@MA_PC, @MA_GV)";
                using (SqlCommand cmdPhanCong = new SqlCommand(sqlInsertPhanCong, MyCon, transaction))
                {
                    cmdPhanCong.Parameters.AddWithValue("@MA_PC", txtMakhoahoc.Text.Trim()); // Mã khóa học vừa thêm
                    cmdPhanCong.Parameters.AddWithValue("@MA_GV", selectedGiangVienId); // Mã giảng viên lấy từ ComboBox

                    cmdPhanCong.ExecuteNonQuery(); // Thực thi lệnh INSERT phân công
                }

                // --- Hoàn tất ---
                transaction.Commit(); // Xác nhận tất cả thay đổi nếu không có lỗi

                Load_DataGridView(); // Tải lại dữ liệu Grid
                MessageBox.Show("Đã lưu thông tin khóa học và phân công giảng viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Có thể thêm code để xóa trắng các ô nhập liệu sau khi lưu thành công
                // ClearInputFields();

            }
            catch (SqlException sqlEx) // Bắt lỗi SQL cụ thể (ví dụ: trùng khóa chính MAKHOAHOC)
            {
                try { if (transaction != null) transaction.Rollback(); } // Cố gắng rollback nếu có lỗi
                catch (Exception rbEx) { MessageBox.Show("Lỗi khi rollback: " + rbEx.Message); }

                MessageBox.Show("Lỗi SQL khi lưu khóa học: " + sqlEx.Message + "\nSố lỗi: " + sqlEx.Number, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Kiểm tra lỗi trùng khóa chính (số lỗi 2627 hoặc 2601 tùy phiên bản SQL Server)
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    MessageBox.Show("Mã khóa học '" + txtMakhoahoc.Text + "' đã tồn tại. Vui lòng nhập mã khác.", "Trùng mã khóa học", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMakhoahoc.Focus();
                    txtMakhoahoc.SelectAll();
                }
            }
            catch (Exception ex) // Bắt các lỗi khác (lỗi parse, lỗi logic...)
            {
                try { if (transaction != null) transaction.Rollback(); } // Cố gắng rollback nếu có lỗi
                catch (Exception rbEx) { MessageBox.Show("Lỗi khi rollback: " + rbEx.Message); }

                MessageBox.Show("Lỗi không xác định khi lưu khóa học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Đảm bảo kết nối được đóng
                if (MyCon.State == ConnectionState.Open)
                {
                    MyCon.Close();
                }
            }
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {

        }
        private void LoadGiangvien()
        {
            DataTable dt = new DataTable();
            try
            {
                // Sử dụng using để đảm bảo kết nối được đóng đúng cách
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456")) // Tạo kết nối mới hoặc dùng MyCon nếu quản lý cẩn thận
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    string sql = "SELECT magiangvien,hoten FROM giangvien ORDER BY hoten";
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                    adapter.Fill(dt);
                } // Kết nối sẽ tự động đóng ở đây khi ra khỏi khối using

                // Thêm dòng "Tất cả" vào đầu DataTable
                DataRow dr = dt.NewRow();
                // Chúng ta vẫn cần MAKHOAHOC để phân biệt, nhưng ValueMember sẽ là TENKHOAHOC
                dr["magiangvien"] = -1; // Giá trị đặc biệt vẫn hữu ích
                dr["hoten"] = "-- Chọn Tên GV --";
                dt.Rows.InsertAt(dr, 0); // Chèn vào vị trí đầu tiên

                // Gán DataSource cho ComboBox
                cmbGiangvien.DataSource = dt;
                cmbGiangvien.DisplayMember = "hoten"; // Cột hiển thị tên
                cmbGiangvien.ValueMember = "magiangvien";   // *** THAY ĐỔI QUAN TRỌNG: Lấy giá trị theo TÊN KHÓA HỌC ***

                // Đặt mục được chọn mặc định là "Tất cả"
                cmbGiangvien.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách khóa học: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Không cần finally để đóng kết nối nếu dùng using
    }
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMakhoahoc.Text) ||
                 string.IsNullOrWhiteSpace(txtTenkhoahoc.Text)||
                 cmbGiangvien.SelectedIndex == -1) // Kiểm tra ComboBox đã chọn chưa
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin khóa học và chọn giảng viên.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra SiSo có phải là số không (nếu cột SISO trong DB là số)
            int sisoValue;
            if (!int.TryParse(txtSiso.Text, out sisoValue))
            {
                MessageBox.Show("Sĩ số phải là một số nguyên.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSiso.Focus();
                return;
            }


            SqlTransaction transaction = null; // Khai báo transaction bên ngoài try

            try
            {
                if (MyCon.State == ConnectionState.Closed) MyCon.Open();
                transaction = MyCon.BeginTransaction(); // Bắt đầu transaction

                // --- Cập nhật bảng KHOAHOC ---
                // Sửa tên bảng thành KHOAHOC, sửa tên tham số @nbt thành @nbd, thêm @MA
                string sqlUpdateKhoaHoc = "UPDATE KHOAHOC SET TENKHOAHOC = @TEN, MOTA = @mt, SISO = @ss, NGAYBATDAU = @nbd, NGAYKETTHUC = @nkt WHERE MAKHOAHOC = @MA";
                using (SqlCommand cmdKhoaHoc = new SqlCommand(sqlUpdateKhoaHoc, MyCon, transaction)) // Gán transaction
                {
                    cmdKhoaHoc.Parameters.AddWithValue("@MA", txtMakhoahoc.Text); // Thêm tham số @MA còn thiếu
                    cmdKhoaHoc.Parameters.AddWithValue("@TEN", txtTenkhoahoc.Text);
                    cmdKhoaHoc.Parameters.AddWithValue("@mt", txtMota.Text);
                    // cmdKhoaHoc.Parameters.AddWithValue("@ss", txtSiso.Text); // Cách cũ nếu SISO là text
                    cmdKhoaHoc.Parameters.AddWithValue("@ss", sisoValue); // Sử dụng giá trị số đã parse
                    cmdKhoaHoc.Parameters.AddWithValue("@nbd", dtpNgaybatdau.Value); // Sử dụng .Value cho DateTimePicker
                    cmdKhoaHoc.Parameters.AddWithValue("@nkt", dtpNgayketthuc.Value); // Sử dụng .Value cho DateTimePicker

                    cmdKhoaHoc.ExecuteNonQuery();
                }

                // --- Cập nhật bảng PHANCONG ---
                string sqlUpdatePhanCong = @"UPDATE PHANCONG
                                            SET MAGIANGVIEN = (SELECT MAGIANGVIEN FROM GIANGVIEN WHERE HOTEN = @TenGV)
                                            WHERE MAKHOAHOC = @MA_PC"; // Dùng tên tham số khác để tránh trùng lặp nếu cần, hoặc dùng lại @MA nếu chắc chắn
                using (SqlCommand cmdPhanCong = new SqlCommand(sqlUpdatePhanCong, MyCon, transaction)) // Gán transaction
                {
                    // Lấy tên giảng viên từ ComboBox (cân nhắc dùng SelectedValue nếu có)
                    cmdPhanCong.Parameters.AddWithValue("@TenGV", cmbGiangvien.Text);
                    cmdPhanCong.Parameters.AddWithValue("@MA_PC", txtMakhoahoc.Text); // Điều kiện WHERE theo mã khóa học


                    int rowsAffected = cmdPhanCong.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("Cảnh báo: Không có dòng nào trong PHANCONG được cập nhật cho MAKHOAHOC = " + txtMakhoahoc.Text);
                    }

                }

                // Nếu cả hai lệnh thành công, commit transaction
                transaction.Commit();

                // Load lại danh sách để thấy thay đổi
                Load_DataGridView();

                MessageBox.Show("Bạn đã sửa thông tin khóa học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (SqlException sqlEx) // Bắt lỗi SQL cụ thể
            {
                MessageBox.Show("Lỗi SQL khi sửa khóa học: " + sqlEx.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    // Nếu có lỗi, rollback transaction
                    if (transaction != null) transaction.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    MessageBox.Show("Lỗi khi rollback transaction: " + rollbackEx.Message, "Lỗi nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex) // Bắt các lỗi khác (ví dụ: lỗi parse sĩ số nếu không kiểm tra trước)
            {
                MessageBox.Show("Lỗi không xác định khi sửa khóa học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    // Nếu có lỗi, rollback transaction
                    if (transaction != null) transaction.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    MessageBox.Show("Lỗi khi rollback transaction: " + rollbackEx.Message, "Lỗi nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                // Đảm bảo kết nối được đóng
                if (MyCon.State == ConnectionState.Open) MyCon.Close();
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            Load_DataGridView();
            LamDep_DataGridView();
            if (dgvKhoahoc.Rows.Count > 0)
            {
                dgvKhoahoc.ClearSelection(); // Xóa chọn tất cả
                dgvKhoahoc.Rows[0].Selected = true; // Chọn hàng đầu tiên
                dgvKhoahoc.CurrentCell = dgvKhoahoc.Rows[0].Cells[0]; // Chọn ô đầu tiên
                dgvKhoahoc.Focus(); // Đưa focus vào DataGridView
                HienThiThongTinHocVien(dgvKhoahoc.Rows[0]);// Hiển thị dữ liệu lên các TextBox
            }
            LoadGiangvien();
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
            txtMakhoahoc.ResetText();
            txtTenkhoahoc.ResetText();
            txtMota.ResetText();
            txtSiso.ResetText();

        }

        private void PerformSearch()
        {
            string searchValue = txtTimKiem.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchValue))
            {
                MessageBox.Show("Vui lòng nhập tên khóa học cần tìm.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- Lấy chuỗi kết nối ---
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456;Encrypt=False";

            // --- Câu lệnh SQL ---

            string query = @" SELECT KHOAHOC.MAKHOAHOC,TENKHOAHOC,MOTA,NGAYBATDAU,NGAYKETTHUC,GIANGVIEN.HOTEN,SISO FROM KHOAHOC
                           INNER JOIN PHANCONG ON PHANCONG.MAKHOAHOC = KHOAHOC.MAKHOAHOC
                           INNER JOIN GIANGVIEN ON GIANGVIEN.MAGIANGVIEN = PHANCONG.MAGIANGVIEN
                           WHERE tenkhoahoc LIKE @SearchValue";

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
                dgvKhoahoc.DataSource = null;
                dgvKhoahoc.DataSource = dt;
                LamDep_DataGridView(); // Làm đẹp grid (nếu cần)

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy khóa học nào có tên chứa '" + searchValue + "'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetDetailFields(); // Giả sử hàm này tồn tại
                }
                else
                {
                    dgvKhoahoc.ClearSelection();
                    if (dgvKhoahoc.Rows.Count > 0)
                    {
                        dgvKhoahoc.Rows[0].Selected = true;
                        // Quan trọng: Cập nhật txtMahocvien và các ô khác từ dòng được chọn
                        HienThiThongTinHocVien(dgvKhoahoc.Rows[0]);
                        dgvKhoahoc.Focus();
                    }
                    else
                    {
                        ResetDetailFields();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Lỗi SQL khi tìm kiếm khóa học: " + sqlEx.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetDetailFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không xác định khi tìm kiếm khoá học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetDetailFields();
            }
        }

        private void txtTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            // Kiểm tra xem phím được nhấn có phải là phím Enter không
            if (e.KeyCode == Keys.Enter)
            {
                // Gọi hành động tìm kiếm (giống Như khi nhấn nút btnTimKiem)
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

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (MyCon.State == ConnectionState.Closed) MyCon.Open();

                if (dgvKhoahoc.CurrentCell == null) // Kiểm tra xem có hàng nào đang được chọn không
                {
                    MessageBox.Show("Vui lòng chọn học viên cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int currentIndex = dgvKhoahoc.CurrentCell.RowIndex; // Lấy chỉ số hàng hiện tại
                string MAKHOAHOC = dgvKhoahoc.Rows[currentIndex].Cells[0].Value?.ToString(); // Lấy mã KHÓA HỌC

                if (string.IsNullOrEmpty(MAKHOAHOC)) // Kiểm tra xem mã học viên có hợp lệ không
                {
                    MessageBox.Show("Không thể xác định mã khóa học !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Xác nhận trước khi xóa
                DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa khóa học có mã {MAKHOAHOC} không?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;

                // Khai báo transaction trong using để đảm bảo tự động giải phóng tài nguyên
                using (SqlTransaction transaction = MyCon.BeginTransaction())
                {
                    try
                    {
                        // Xóa dữ liệu trong bảng PHANCONG trước
                        using (SqlCommand cmd1 = new SqlCommand("DELETE FROM PHANCONG WHERE MAKHOAHOC = @MAKHOAHOC", MyCon, transaction))
                        {
                            cmd1.Parameters.AddWithValue("@MAKHOAHOC", MAKHOAHOC);
                            cmd1.ExecuteNonQuery();
                        }

                        // Xóa KHÓA HỌC sau khi đã xóa các bản ghi liên quan
                        using (SqlCommand cmd2 = new SqlCommand("DELETE FROM KHOAHOC WHERE MAKHOAHOC = @MAKHOAHOC", MyCon, transaction))
                        {
                            cmd2.Parameters.AddWithValue("@MAKHOAHOC", MAKHOAHOC);
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
                        MessageBox.Show("Lỗi khi xóa khóa học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btnReset_Click(object sender, EventArgs e)
        {

            // Xóa dữ liệu cũ trong các ô nhập liệu để chuẩn bị thêm học viên mới
            txtTenkhoahoc.ResetText();
            txtMakhoahoc.ResetText();
            txtMota.ResetText();
            txtSiso.ResetText();

            // Đưa con trỏ vào ô nhập Họ tên để người dùng nhập ngay
            txtTenkhoahoc.Focus();
        }
    }
}
