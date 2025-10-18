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
    public partial class FrmKhoạThi : Form
    {
        SqlConnection MyCon = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456");
        public FrmKhoạThi()
        {
            InitializeComponent();
        }

        private void FrmKhoạThi_Load(object sender, EventArgs e)
        {
            Load_DataGridView();
            LamDep_DataGridView();
            if (dgvKhoathi.Rows.Count > 0)
            {
                dgvKhoathi.ClearSelection(); // Xóa chọn tất cả
                dgvKhoathi.Rows[0].Selected = true; // Chọn hàng đầu tiên
                dgvKhoathi.CurrentCell = dgvKhoathi.Rows[0].Cells[0]; // Chọn ô đầu tiên
                dgvKhoathi.Focus(); // Đưa focus vào DataGridView
                HienThiThongTinHocVien(dgvKhoathi.Rows[0]);// Hiển thị dữ liệu lên các TextBox
            }
        }
        private void HienThiThongTinHocVien(DataGridViewRow row)
        {
            if (row != null)
            {
                txtMakhoathi.Text = dgvKhoathi.CurrentRow.Cells["MAKHOATHI"].Value.ToString();
                txtTenkhoathi.Text = dgvKhoathi.CurrentRow.Cells["TENKHOATHI"].Value.ToString();
                txtPhongthi.Text = dgvKhoathi.CurrentRow.Cells["PHONGTHI"].Value.ToString();
                dtpNgaythi.Text = dgvKhoathi.CurrentRow.Cells["NGAYTHI"].Value.ToString();
                txtPhongthi.Text = dgvKhoathi.CurrentRow.Cells["PHONGTHI"].Value.ToString();
                cmbTenchungchi.Text = dgvKhoathi.CurrentRow.Cells["TENCHUNGCHI"].Value.ToString();
            }
        }
        private void LamDep_DataGridView()
        {
            
            // Đặt chiều cao của tiêu đề
            dgvKhoathi.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;//Ngăn không cho hệ thống và người dùng thay đổi chiều cao tiêu đề cột
            dgvKhoathi.ColumnHeadersHeight = 40; // Chiều cao tiêu đề

            // Đặt màu nền và màu chữ cho tiêu đề
            dgvKhoathi.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 212, 190);
            dgvKhoathi.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvKhoathi.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);

            // Áp dụng màu cho một cột cụ thể
            dgvKhoathi.Columns["MAKHOATHI"].HeaderCell.Style.BackColor = Color.FromArgb(234, 0, 1);

            // Cập nhật lại giao diện
            dgvKhoathi.EnableHeadersVisualStyles = false;
        }
        private void Load_DataGridView()
        {
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();
            string sql = @"SELECT KHOATHI.MAKHOATHI,TENKHOATHI,NGAYTHI,PHONGTHI,CHUNGCHI.TENCHUNGCHI FROM KHOATHI
                           INNER JOIN CHUNGCHI ON CHUNGCHI.MACHUNGCHI = KHOATHI.MACHUNGCHI";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, MyCon);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dgvKhoathi.DataSource = dt;

            dgvKhoathi.Columns[0].HeaderText = "Mã khóa thi";
            dgvKhoathi.Columns[1].HeaderText = "Tên khóa thi";

            dgvKhoathi.Columns[2].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvKhoathi.Columns[2].HeaderText = "Ngày thi";

            dgvKhoathi.Columns[3].HeaderText = "Phòng thi";
            dgvKhoathi.Columns[4].HeaderText = "Tên chứng chỉ";
            dgvKhoathi.Columns["PHONGTHI"].Width = 200;

            dgvKhoathi.AllowUserToAddRows = false;
            dgvKhoathi.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        private void dgvKhoathi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtMakhoathi.Text = dgvKhoathi.CurrentRow.Cells["MAKHOATHI"].Value.ToString();
            txtTenkhoathi.Text = dgvKhoathi.CurrentRow.Cells["TENKHOATHI"].Value.ToString();
            txtPhongthi.Text = dgvKhoathi.CurrentRow.Cells["PHONGTHI"].Value.ToString();
            dtpNgaythi.Text = dgvKhoathi.CurrentRow.Cells["NGAYTHI"].Value.ToString();
            txtPhongthi.Text = dgvKhoathi.CurrentRow.Cells["PHONGTHI"].Value.ToString();
            cmbTenchungchi.Text = dgvKhoathi.CurrentRow.Cells["TENCHUNGCHI"].Value.ToString();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private string GenerateNewKhoaThiId()
        {
            // Danh sách để lưu trữ phần SỐ của các mã khóa thi hiện có
            List<int> maKhoaThiNumbers = new List<int>();
            string prefix = "KT";

            try
            {
                // Luôn sử dụng 'using' để đảm bảo kết nối được quản lý tự động
                using (SqlConnection con = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456"))
                {
                    // Câu lệnh SQL để lấy TẤT CẢ mã khóa thi
                    string query = "SELECT MAKHOATHI FROM KHOATHI";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        if (con.State == ConnectionState.Closed)
                            con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Duyệt qua từng mã khóa thi đọc được
                            while (reader.Read())
                            {
                                string maKT = reader.GetString(0); // Lấy mã (VD: "KT05", "KT01")

                                // Kiểm tra xem mã có đúng định dạng "KT" + số không
                                if (!string.IsNullOrEmpty(maKT) && maKT.StartsWith(prefix))
                                {
                                    // Lấy phần số từ sau tiền tố "KT"
                                    string numberString = maKT.Substring(prefix.Length);

                                    // Cố gắng chuyển đổi phần số thành kiểu int
                                    if (int.TryParse(numberString, out int numberPart))
                                    {
                                        maKhoaThiNumbers.Add(numberPart); // Thêm số vào danh sách
                                    }
                                }
                            }
                        }
                    }
                } // Kết nối được tự động đóng ở đây

                // Nếu không có mã nào trong bảng, bắt đầu từ 1
                if (maKhoaThiNumbers.Count == 0)
                {
                    return prefix + "01";
                }

                // Sắp xếp danh sách các số thứ tự theo thứ tự tăng dần
                maKhoaThiNumbers.Sort();

                // Tìm số nhỏ nhất chưa được sử dụng (lấp chỗ trống)
                int nextNumber = 1;
                foreach (int existingNumber in maKhoaThiNumbers)
                {
                    if (existingNumber == nextNumber)
                    {
                        nextNumber++; // Nếu số 1 đã tồn tại, tìm số 2...
                    }
                    else
                    {
                        // Nếu tìm thấy một khoảng trống (ví dụ: có KT01 nhưng không có KT02),
                        // nextNumber chính là số còn thiếu.
                        break;
                    }
                }

                // Tạo mã khóa thi mới với định dạng KTxx (2 chữ số, có số 0 đứng đầu nếu cần)
                // Ví dụ: 1 -> KT01, 9 -> KT09, 10 -> KT10
                return prefix + nextNumber.ToString("D2");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi phát sinh mã khóa thi mới: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null; // Trả về null để báo hiệu có lỗi
            }
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            // 1. Xóa dữ liệu cũ trong các ô nhập liệu
            txtTenkhoathi.ResetText();
            txtPhongthi.ResetText();
            dtpNgaythi.Value = DateTime.Now;
            cmbTenchungchi.SelectedIndex = -1;

            // 2. Gọi hàm để phát sinh mã khóa thi mới
            string newId = GenerateNewKhoaThiId();

            // 3. Kiểm tra và hiển thị mã mới
            if (newId != null)
            {
                txtMakhoathi.Text = newId;

                // Quan trọng: Khóa ô mã khóa thi lại để người dùng không sửa được.
                // Mã đã được hệ thống tạo ra, không nên để người dùng thay đổi.
                txtMakhoathi.ReadOnly = true;

                // 4. Đưa con trỏ vào ô Tên khóa thi để người dùng nhập tiếp
                txtTenkhoathi.Focus();
            }
            // Nếu newId là null, thông báo lỗi đã được hiển thị trong hàm GenerateNewKhoaThiId()
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra đầu vào cơ bản (ít nhất là Mã Khóa Thi)
            if (string.IsNullOrWhiteSpace(txtMakhoathi.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã Khóa Thi cần sửa!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMakhoathi.Focus(); // Đặt con trỏ vào ô Mã Khóa Thi
                return;
            }

            // Thêm kiểm tra cho các trường khác nếu chúng không được phép rỗng khi sửa
            if (string.IsNullOrWhiteSpace(txtTenkhoathi.Text) ||
                string.IsNullOrWhiteSpace(txtPhongthi.Text))
            {
                MessageBox.Show("Tên khóa thi và Phòng thi không được để trống!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Bạn có thể thêm kiểm tra cho ComboBox nếu cần sửa cả MaChungChi ở đây

            try
            {
                if (MyCon.State == ConnectionState.Closed)
                {
                    MyCon.Open();
                }

                // Câu lệnh UPDATE thông tin học viên
                string sql = "UPDATE KHOATHI SET TENKHOATHI = @TEN, NGAYTHI = @NT, PHONGTHI = @PT WHERE MAKHOATHI = @MA";

                // 4. Sử dụng using cho SqlCommand
                using (SqlCommand Cmd = new SqlCommand(sql, MyCon))
                {
                    // 5. Trim() dữ liệu text
                    Cmd.Parameters.AddWithValue("@TEN", txtTenkhoathi.Text.Trim());
                    Cmd.Parameters.AddWithValue("@MA", txtMakhoathi.Text.Trim()); // Mã khóa thi là khóa, cũng nên Trim
                    Cmd.Parameters.AddWithValue("@PT", txtPhongthi.Text.Trim());

                    // 2. Sử dụng .Date nếu cột trong DB là DATE
                    Cmd.Parameters.AddWithValue("@NT", dtpNgaythi.Value.Date);
                    // Nếu cột là DATETIME, dùng: Cmd.Parameters.AddWithValue("@NT", dtpNgaythi.Value);

                    // 3. Kiểm tra số dòng bị ảnh hưởng
                    int rowsAffected = Cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Load lại danh sách để thấy thay đổi
                        Load_DataGridView();
                        MessageBox.Show("Bạn đã sửa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy mã khóa thi cần sửa", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                } // Cmd sẽ tự động được Dispose() ở đây
            }
            catch (SqlException ex) // Bắt lỗi SQL cụ thể
            {
                MessageBox.Show("Lỗi khi cập nhật dữ liệu: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) // Bắt các lỗi chung khác
            {
                MessageBox.Show("Đã có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 6. Đảm bảo kết nối được đóng
                if (MyCon.State == ConnectionState.Open)
                {
                    MyCon.Close();
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (MyCon.State == ConnectionState.Closed) MyCon.Open();

                if (dgvKhoathi.CurrentCell == null) // Kiểm tra xem có hàng nào đang được chọn không
                {
                    MessageBox.Show("Vui lòng chọn khóa thi cần xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int currentIndex = dgvKhoathi.CurrentCell.RowIndex; // Lấy chỉ số hàng hiện tại
                string MAHOCVIEN = dgvKhoathi.Rows[currentIndex].Cells[0].Value?.ToString(); // Lấy mã học viên

                if (string.IsNullOrEmpty(MAHOCVIEN)) // Kiểm tra xem mã học viên có hợp lệ không
                {
                    MessageBox.Show("Không thể xác định khóa thi!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Xác nhận trước khi xóa
                DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa khóa thi có mã {MAHOCVIEN} không?",
                    "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;

                // Khai báo transaction trong using để đảm bảo tự động giải phóng tài nguyên
                using (SqlTransaction transaction = MyCon.BeginTransaction())
                {
                    try
                    {
                        // Xóa KHOATHI sau khi đã xóa các bản ghi liên quan
                        using (SqlCommand cmd2 = new SqlCommand("DELETE FROM KHOATHI WHERE MAKHOATHI = @MA", MyCon, transaction))
                        {
                            cmd2.Parameters.AddWithValue("@MA", MAHOCVIEN);
                            cmd2.ExecuteNonQuery();
                        }

                        // Commit transaction sau khi xóa thành công
                        transaction.Commit();

                        MessageBox.Show("Bạn đã xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Load_DataGridView();
                        LamDep_DataGridView();
                        if (dgvKhoathi.Rows.Count > 0)
                        {
                            dgvKhoathi.ClearSelection(); // Xóa chọn tất cả
                            dgvKhoathi.Rows[0].Selected = true; // Chọn hàng đầu tiên
                            dgvKhoathi.CurrentCell = dgvKhoathi.Rows[0].Cells[0]; // Chọn ô đầu tiên
                            dgvKhoathi.Focus(); // Đưa focus vào DataGridView
                            HienThiThongTinHocVien(dgvKhoathi.Rows[0]);// Hiển thị dữ liệu lên các TextBox
                        }

                    }
                    catch (Exception ex)
                    {
                        // Nếu có lỗi, rollback lại trạng thái trước đó
                        transaction.Rollback();
                        MessageBox.Show("Lỗi khi xóa học viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMakhoathi.Text) ||
     string.IsNullOrWhiteSpace(txtTenkhoathi.Text) ||
     //cmbTenchungchi.SelectedIndex == -1 || // Đảm bảo người dùng đã chọn
     string.IsNullOrWhiteSpace(txtPhongthi.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin và chọn chứng chỉ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlTransaction transaction = null;
            string maChungChiValue = null; // Biến để lưu MaChungChi tìm được

            try
            {
                if (MyCon.State == ConnectionState.Closed)
                {
                    MyCon.Open();
                }

                transaction = MyCon.BeginTransaction();

                // --- BƯỚC 1: Lấy MaChungChi dựa trên TenChungChi được chọn ---
                string selectedTenChungChi = cmbTenchungchi.SelectedItem.ToString(); // Lấy tên chứng chỉ được chọn


                string sqlGetMaChungChi = "SELECT MaChungChi FROM CHUNGCHI WHERE TenChungChi = @TenChungChi";

                using (SqlCommand cmdGetMa = new SqlCommand(sqlGetMaChungChi, MyCon, transaction))
                {
                    cmdGetMa.Parameters.AddWithValue("@TenChungChi", selectedTenChungChi);
                    object result = cmdGetMa.ExecuteScalar(); // Lấy giá trị đầu tiên tìm được

                    if (result != null && result != DBNull.Value)
                    {
                        maChungChiValue = result.ToString();
                    }
                    else
                    {
                        // Không tìm thấy MaChungChi tương ứng -> Lỗi dữ liệu hoặc logic
                        transaction.Rollback(); // Hủy bỏ transaction vì không thể tiếp tục
                        MessageBox.Show($"Không tìm thấy Mã Chứng Chỉ tương ứng với Tên Chứng Chỉ '{selectedTenChungChi}'. Vui lòng kiểm tra lại dữ liệu.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Thoát khỏi hàm
                    }
                }

                // --- BƯỚC 2: Thêm DỮ LIỆU VÀO BẢNG KHOATHI với MaChungChi đã tìm được ---
                string sqlInsertKhoaThi = "INSERT INTO KHOATHI (MAKHOATHI, TENKHOATHI, NGAYTHI, MACHUNGCHI, PHONGTHI) " +
                                          "VALUES (@MA, @TEN, @NGAY, @MCC, @PT)";

                using (SqlCommand cmdInsert = new SqlCommand(sqlInsertKhoaThi, MyCon, transaction))
                {
                    cmdInsert.Parameters.AddWithValue("@MA", txtMakhoathi.Text.Trim());
                    cmdInsert.Parameters.AddWithValue("@TEN", txtTenkhoathi.Text.Trim());
                    cmdInsert.Parameters.AddWithValue("@NGAY", dtpNgaythi.Value.Date); // Giả sử cột NGAYTHI là DATE

                    // Sử dụng maChungChiValue đã tìm được ở Bước 1
                    cmdInsert.Parameters.AddWithValue("@MCC", maChungChiValue);

                    cmdInsert.Parameters.AddWithValue("@PT", txtPhongthi.Text.Trim());

                    cmdInsert.ExecuteNonQuery();
                }

                // Nếu mọi thứ thành công
                transaction.Commit();
                MessageBox.Show("Thêm khóa thi thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Load_DataGridView();
            }
            catch (Exception ex)
            {
                try
                {
                    // Cố gắng rollback nếu có lỗi xảy ra sau khi đã tìm được MaChungChi
                    if (transaction != null && transaction.Connection != null) // Kiểm tra transaction còn hợp lệ
                    {
                        transaction.Rollback();
                    }
                }
                catch (Exception rollbackEx)
                {
                    MessageBox.Show($"Lỗi khi rollback transaction: {rollbackEx.Message}", "Lỗi nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                MessageBox.Show($"Đã xảy ra lỗi khi thêm khóa thi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (MyCon.State == ConnectionState.Open) MyCon.Close();
            }

        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            Load_DataGridView();
            LamDep_DataGridView();
            if (dgvKhoathi.Rows.Count > 0)
            {
                dgvKhoathi.ClearSelection(); // Xóa chọn tất cả
                dgvKhoathi.Rows[0].Selected = true; // Chọn hàng đầu tiên
                dgvKhoathi.CurrentCell = dgvKhoathi.Rows[0].Cells[0]; // Chọn ô đầu tiên
                dgvKhoathi.Focus(); // Đưa focus vào DataGridView
                HienThiThongTinHocVien(dgvKhoathi.Rows[0]);// Hiển thị dữ liệu lên các TextBox
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Xóa dữ liệu cũ trong các ô nhập liệu để chuẩn bị thêm khóa thi mới
            txtMakhoathi.ResetText();
            txtTenkhoathi.ResetText();
            txtPhongthi.ResetText();
            // Đưa con trỏ vào ô nhập Họ tên để người dùng nhập ngay
            txtMakhoathi.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
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

        private void btnPrint_Click(object sender, EventArgs e)
        {

        }


        private void PerformSearch()
        {
            string searchValue = txtTimKiem.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchValue))
            {
                MessageBox.Show("Vui lòng nhập tên khóa thi cần tìm.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- Lấy chuỗi kết nối ---
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456;Encrypt=False";

            // --- Câu lệnh SQL ---

            string query = @"SELECT KHOATHI.MAKHOATHI,TENKHOATHI,NGAYTHI,PHONGTHI,CHUNGCHI.TENCHUNGCHI FROM KHOATHI
                           INNER JOIN CHUNGCHI ON CHUNGCHI.MACHUNGCHI = KHOATHI.MACHUNGCHI
                           WHERE tenkhoathi LIKE @SearchValue";
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
                dgvKhoathi.DataSource = null;
                dgvKhoathi.DataSource = dt;
                LamDep_DataGridView(); // Làm đẹp grid (nếu cần)

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy khóa học nào có tên chứa '" + searchValue + "'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                }
                else
                {
                    dgvKhoathi.ClearSelection();
                    if (dgvKhoathi.Rows.Count > 0)
                    {
                        dgvKhoathi.Rows[0].Selected = true;
                        // Quan trọng: Cập nhật txtMahocvien và các ô khác từ dòng được chọn
                        HienThiThongTinHocVien(dgvKhoathi.Rows[0]);
                        dgvKhoathi.Focus();
                    } 
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Lỗi SQL khi tìm kiếm khóa thi: " + sqlEx.Message, "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không xác định khi tìm kiếm khoá thi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
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
    }
}
