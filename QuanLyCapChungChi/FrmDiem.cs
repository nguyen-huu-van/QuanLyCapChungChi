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
    public partial class FrmDiem : Form
    {
        // --- Biến thành viên của lớp ---
        SqlConnection MyCon = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456");
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;
        private BindingSource bindingSource;
        private bool isComboBoxLoaded = false;
        // -----------------------------

        public FrmDiem()
        {
            InitializeComponent();
        }

        // ... (Các hàm LamDep_DataGridView, FrmChungchi_Load, LoadKhoaTHI giữ nguyên) ...
        private void LamDep_DataGridView()
        {
            // Đặt chiều cao của tiêu đề
            dgvdiem.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvdiem.ColumnHeadersHeight = 40;

            // Đặt màu nền và màu chữ cho tiêu đề
            dgvdiem.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 212, 190);
            dgvdiem.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvdiem.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            dgvdiem.EnableHeadersVisualStyles = false; // Quan trọng để màu tùy chỉnh có hiệu lực

            // Áp dụng màu cho một cột cụ thể (nếu cần - sau khi DataSource được gán)
            if (dgvdiem.Columns.Contains("hoten") && dgvdiem.Columns["hoten"] != null) // Kiểm tra cột tồn tại
            {
                dgvdiem.Columns["hoten"].HeaderCell.Style.BackColor = Color.FromArgb(234, 0, 1);
                // Đảm bảo màu chữ vẫn là trắng nếu nền thay đổi
                dgvdiem.Columns["hoten"].HeaderCell.Style.ForeColor = Color.White;
            }
        }

        private void FrmChungchi_Load(object sender, EventArgs e)
        {
            LoadKhoaTHI(); // Tải ComboBox trước
            LamDep_DataGridView(); // Làm đẹp cấu trúc ban đầu
            dgvdiem.DataSource = null; // Xóa dữ liệu cũ nếu có
            ClearTextBoxes(); // Xóa các ô text box
        }

        private void LoadKhoaTHI()
        {
            // Sử dụng biến kết nối MyCon của lớp
            string query = "SELECT MaKhoaThi, TenKhoaThi FROM KhoaThi ORDER BY TenKhoaThi"; // Thêm ORDER BY cho dễ nhìn
            SqlDataAdapter da = new SqlDataAdapter(query, MyCon);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);

                // Thêm một dòng trống hoặc "Chọn khóa thi" vào đầu danh sách (tùy chọn)
                DataRow dr = dt.NewRow();
                dr["MaKhoaThi"] = DBNull.Value; // Hoặc giá trị đặc biệt như 0, -1
                dr["TenKhoaThi"] = "-- Chọn Khóa Thi --";
                dt.Rows.InsertAt(dr, 0);


                cmbTenkhoathi.DataSource = dt;
                cmbTenkhoathi.DisplayMember = "TenKhoaThi";
                cmbTenkhoathi.ValueMember = "MaKhoaThi";

                cmbTenkhoathi.SelectedIndex = 0; // Chọn dòng "-- Chọn Khóa Thi --"
                isComboBoxLoaded = true; // Đánh dấu ComboBox đã load xong
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách khóa thi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Không cần đóng mở kết nối ở đây nếu chỉ dùng cho DataAdapter Fill
        }


        // Sửa đổi Load_DataGridView để tải theo MaKhoaThi và định nghĩa UpdateCommand
        // Sửa đổi Load_DataGridView để chấp nhận maKhoaThi là null và tải tất cả dữ liệu
        private void Load_DataGridView(object maKhoaThi) // Giữ nguyên chữ ký hàm
        {
            dataTable = new DataTable();
            bindingSource = new BindingSource();

            // Xây dựng phần cơ bản của câu lệnh SQL
            string baseSql = @"SELECT
           KETQUATHI.MaHocVien,
           KETQUATHI.MaKhoaThi,
           HOCVIEN.HOTEN,
           KETQUATHI.DIEMSO,
           KETQUATHI.DIEMSO2,
           KETQUATHI.DIEMSO3,
           KHOATHI.TENKHOATHI,
           CASE
               WHEN KHOATHI.TENKHOATHI LIKE N'%Tin học%' AND KETQUATHI.DIEMSO >= 5.0 AND KETQUATHI.DIEMSO2 >= 5.0 THEN N'Đậu'
               WHEN KHOATHI.TENKHOATHI LIKE N'%Tiếng Anh%' AND KETQUATHI.DIEMSO >= 5.0 AND KETQUATHI.DIEMSO2 >= 5.0 AND KETQUATHI.DIEMSO3 >= 5.0 THEN N'Đậu'
               ELSE N'Rớt'
           END AS ketqua
       FROM KETQUATHI
       INNER JOIN HOCVIEN ON KETQUATHI.MAHOCVIEN = HOCVIEN.MAHOCVIEN
       INNER JOIN KHOATHI ON KHOATHI.MAKHOATHI = KETQUATHI.MAKHOATHI";

            string finalSql; // Câu lệnh SQL cuối cùng sẽ được thực thi

            try
            {
                // Kiểm tra xem có cần lọc theo MaKhoaThi không
                if (maKhoaThi != null && maKhoaThi != DBNull.Value)
                {
                    // Nếu có MaKhoaThi, thêm mệnh đề WHERE
                    finalSql = baseSql + " WHERE KETQUATHI.MAKHOATHI = @MaKhoaThi";
                    dataAdapter = new SqlDataAdapter(finalSql, MyCon);
                    // Thêm Parameter cho mệnh đề WHERE
                    dataAdapter.SelectCommand.Parameters.AddWithValue("@MaKhoaThi", maKhoaThi);
                }
                else
                {
                    // Nếu maKhoaThi là null (chọn "-- Chọn Khóa Thi --"), dùng câu lệnh cơ bản không có WHERE
                    finalSql = baseSql;
                    dataAdapter = new SqlDataAdapter(finalSql, MyCon);
                    // Không cần thêm parameter vì không có mệnh đề WHERE
                }

                // --- PHẦN ĐỊNH NGHĨA LỆNH UPDATE VẪN GIỮ NGUYÊN ---
                // Lệnh Update vẫn cần MaHocVien và MaKhoaThi để xác định đúng dòng cần cập nhật trong bảng KETQUATHI
                string updateSql = @"UPDATE KETQUATHI
                             SET DIEMSO = @DiemSo, DIEMSO2 = @DiemSo2, DIEMSO3 = @DiemSo3
                             WHERE MaHocVien = @Original_MaHocVien AND MaKhoaThi = @Original_MaKhoaThi";
                SqlCommand updateCommand = new SqlCommand(updateSql, MyCon);

                updateCommand.Parameters.Add("@DiemSo", SqlDbType.Float, 0, "DIEMSO").SourceVersion = DataRowVersion.Current;
                updateCommand.Parameters.Add("@DiemSo2", SqlDbType.Float, 0, "DIEMSO2").SourceVersion = DataRowVersion.Current;
                updateCommand.Parameters.Add("@DiemSo3", SqlDbType.Float, 0, "DIEMSO3").SourceVersion = DataRowVersion.Current;
                updateCommand.Parameters.Add("@Original_MaHocVien", SqlDbType.NVarChar, 50, "MaHocVien").SourceVersion = DataRowVersion.Original;
                updateCommand.Parameters.Add("@Original_MaKhoaThi", SqlDbType.NVarChar, 50, "MaKhoaThi").SourceVersion = DataRowVersion.Original;

                dataAdapter.UpdateCommand = updateCommand;
                // --- KẾT THÚC ĐỊNH NGHĨA LỆNH UPDATE ---

                // Fill dữ liệu vào DataTable
                dataTable.Clear(); // Xóa dữ liệu cũ trước khi fill mới
                dataAdapter.Fill(dataTable);

                bindingSource.DataSource = dataTable;
                dgvdiem.DataSource = bindingSource;

                // --- Cấu hình cột (giữ nguyên) ---
                if (dgvdiem.Columns.Count > 0)
                {
                    // ... (phần cấu hình cột như cũ) ...
                    if (dgvdiem.Columns.Contains("MaHocVien")) dgvdiem.Columns["MaHocVien"].Visible = false;
                    if (dgvdiem.Columns.Contains("MaKhoaThi")) dgvdiem.Columns["MaKhoaThi"].Visible = false;

                    if (dgvdiem.Columns.Contains("HOTEN"))
                    {
                        dgvdiem.Columns["HOTEN"].HeaderText = "Họ Tên";
                        dgvdiem.Columns["HOTEN"].ReadOnly = true;
                        dgvdiem.Columns["HOTEN"].Width = 200; // Điều chỉnh lại nếu cần
                    }
                    if (dgvdiem.Columns.Contains("DIEMSO"))
                    {
                        dgvdiem.Columns["DIEMSO"].HeaderText = "Điểm số 1";
                        dgvdiem.Columns["DIEMSO"].Width = 90;
                        dgvdiem.Columns["DIEMSO"].DefaultCellStyle.Format = "N2";
                    }
                    if (dgvdiem.Columns.Contains("DIEMSO2"))
                    {
                        dgvdiem.Columns["DIEMSO2"].HeaderText = "Điểm số 2";
                        dgvdiem.Columns["DIEMSO2"].Width = 90;
                        dgvdiem.Columns["DIEMSO2"].DefaultCellStyle.Format = "N2";
                    }
                    if (dgvdiem.Columns.Contains("DIEMSO3"))
                    {
                        dgvdiem.Columns["DIEMSO3"].HeaderText = "Điểm số 3";
                        dgvdiem.Columns["DIEMSO3"].Width = 90;
                        dgvdiem.Columns["DIEMSO3"].DefaultCellStyle.Format = "N2";
                    }
                    if (dgvdiem.Columns.Contains("ketqua")) // Thêm cột kết quả nếu chưa có
                    {
                        dgvdiem.Columns["ketqua"].HeaderText = "Kết quả";
                        dgvdiem.Columns["ketqua"].ReadOnly = true; // Kết quả tính toán, không cho sửa
                        dgvdiem.Columns["ketqua"].Width = 80;
                    }
                    if (dgvdiem.Columns.Contains("TENKHOATHI"))
                    {
                        dgvdiem.Columns["TENKHOATHI"].HeaderText = "Tên Khóa Thi";
                        dgvdiem.Columns["TENKHOATHI"].ReadOnly = true;
                        dgvdiem.Columns["TENKHOATHI"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    }
                }

                dgvdiem.EditMode = DataGridViewEditMode.EditOnEnter;
                dgvdiem.AllowUserToAddRows = false;

                LamDep_DataGridView(); // Áp dụng lại làm đẹp

                // Xóa lựa chọn và text box, chọn dòng đầu nếu có dữ liệu
                ClearTextBoxes();
                dgvdiem.ClearSelection();
                if (dgvdiem.Rows.Count > 0)
                {
                    dgvdiem.Rows[0].Selected = true;
                    // Kích hoạt sự kiện CellClick cho dòng đầu tiên để hiển thị thông tin lên TextBox
                    dgvdiem_CellClick(dgvdiem, new DataGridViewCellEventArgs(0, 0));
                }

            }
            catch (SqlException sqlEx)
            {
                string commandText = dataAdapter?.SelectCommand?.CommandText ?? "N/A";
                MessageBox.Show($"Lỗi SQL khi tải dữ liệu:\n{sqlEx.Message}\nSố lỗi: {sqlEx.Number}\nCâu lệnh: {commandText}", "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Cân nhắc xóa dữ liệu trên grid nếu lỗi
                dgvdiem.DataSource = null;
                ClearTextBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Cân nhắc xóa dữ liệu trên grid nếu lỗi
                dgvdiem.DataSource = null;
                ClearTextBoxes();
            }
        }

        // ... (cmbTenkhoathi_SelectedIndexChanged_1, dgvdiem_CellClick giữ nguyên) ...
        private void cmbTenkhoathi_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (!isComboBoxLoaded) return; // Thoát nếu ComboBox chưa load xong

            if (cmbTenkhoathi.SelectedIndex > 0 && cmbTenkhoathi.SelectedValue != null && cmbTenkhoathi.SelectedValue != DBNull.Value)
            {
                // Trường hợp chọn một khóa thi cụ thể
                object selectedMaKhoaThi = cmbTenkhoathi.SelectedValue;
                Load_DataGridView(selectedMaKhoaThi); // Tải dữ liệu lọc theo MaKhoaThi
            }
            else // Trường hợp chọn "-- Chọn Khóa Thi --" (SelectedIndex <= 0)
            {
                Load_DataGridView(null); // Gọi Load_DataGridView với tham số null để tải tất cả
            }
        }

        private void dgvdiem_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Đảm bảo click vào một dòng hợp lệ (không phải header và có trong danh sách Rows)
            if (e.RowIndex >= 0 && e.RowIndex < dgvdiem.Rows.Count)
            {
                DataGridViewRow row = dgvdiem.Rows[e.RowIndex];

                // Sử dụng indexer và toán tử null-conditional (?.) / null-coalescing (??) để lấy giá trị an toàn
                // Không cần dùng row.Cells.Contains(...) nữa

                // Lấy giá trị từ các ô của dòng được chọn
                txtTenthisinh.Text = row.Cells["HOTEN"]?.Value?.ToString() ?? ""; // Dòng ~234 (sửa lại)
                txtDiem.Text = row.Cells["DIEMSO"]?.Value?.ToString() ?? "";     // Dòng ~235 (sửa lại)
                txtDiem2.Text = row.Cells["DIEMSO2"]?.Value?.ToString() ?? "";    // Dòng ~236 (sửa lại)
                txtDiem3.Text = row.Cells["DIEMSO3"]?.Value?.ToString() ?? "";    // Dòng ~237 (sửa lại - đây có thể là dòng 238 gây lỗi)

                

            }
            else // Nếu click vào header hoặc vùng trống
            {
            }
        }
        /*
        Dữ liệu được sửa trên DataGridView -
        > Thay đổi được ghi vào DataTable(trong bộ nhớ) và RowState của dòng đổi thành Modified
        -> Khi Lưu, SqlDataAdapter dùng lệnh UpdateCommand(đã định nghĩa trước) 
        -> DataAdapter tự động lấy giá trị mới(cho SET) và gốc(cho WHERE) từ dòng Modified trong DataTable để điền vào tham số 
        -> Gửi lệnh UPDATE đến DB -> Nếu thành công, AcceptChanges() reset RowState về Unchanged
        */
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (dgvdiem.DataSource == null || bindingSource.DataSource == null || dataTable == null)
            {
                MessageBox.Show("Chưa có dữ liệu để lưu. Vui lòng chọn một khóa thi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                this.Validate();
                bindingSource.EndEdit();

                DataTable changes = dataTable.GetChanges(DataRowState.Modified); // Chỉ lấy các dòng đã sửa đổi
                if (changes == null)
                {
                    MessageBox.Show("Không có thay đổi nào để lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Kiểm tra xem UpdateCommand đã được gán chưa
                if (dataAdapter == null || dataAdapter.UpdateCommand == null)
                {
                    MessageBox.Show("Lỗi: Lệnh cập nhật chưa được định nghĩa cho DataAdapter.", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // DataAdapter.Update sẽ tự động mở và đóng kết nối khi cần
                int rowsAffected = dataAdapter.Update(changes); // Chỉ gửi các dòng đã sửa đổi

                dataTable.AcceptChanges(); // Quan trọng: chấp nhận thay đổi sau khi update thành công

                MessageBox.Show($"Đã lưu thành công {rowsAffected} thay đổi vào cơ sở dữ liệu!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
 
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Lỗi SQL khi lưu dữ liệu: {sqlEx.Message}\nSố lỗi: {sqlEx.Number}\nVui lòng kiểm tra lại dữ liệu nhập (ví dụ: điểm phải là số).", "Lỗi Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataTable.RejectChanges(); // Hủy các thay đổi trên DataTable nếu lưu lỗi
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi không xác định khi lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataTable.RejectChanges(); // Hủy các thay đổi
            }
            // Không cần finally để đóng connection vì Update tự quản lý
        }

        private void ClearTextBoxes()
        {
            txtTenthisinh.Text = "";
            txtDiem.Text = "";
            txtDiem2.Text = "";
            txtDiem3.Text = "";
        }
        private void dgvdiem_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Context == DataGridViewDataErrorContexts.Commit) // Chỉ xử lý lỗi khi commit giá trị
            {
                MessageBox.Show($"Lỗi nhập liệu tại cột '{dgvdiem.Columns[e.ColumnIndex].HeaderText}' dòng {e.RowIndex + 1}:\n{e.Exception.Message}\nVui lòng nhập lại giá trị hợp lệ (ví dụ: kiểu số).",
                           "Lỗi Nhập Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // Ngăn không cho exception mặc định hiển thị và giữ lại giá trị cũ
            e.ThrowException = false;
            e.Cancel = true;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có khóa thi nào được chọn không
            if (cmbTenkhoathi.SelectedIndex <= 0 || cmbTenkhoathi.SelectedValue == null || cmbTenkhoathi.SelectedValue == DBNull.Value)
            {
                MessageBox.Show("Vui lòng chọn một khóa thi cụ thể để in danh sách đậu.", "Chưa chọn khóa thi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy MaKhoaThi đang được chọn
            object selectedMaKhoaThi = cmbTenkhoathi.SelectedValue;
            string tenKhoaThi = cmbTenkhoathi.Text; // Lấy tên để hiển thị (tùy chọn)

            try
            {
                // Khởi tạo form báo cáo và truyền MaKhoaThi vào
                Frmbaocaodiem frmReport = new Frmbaocaodiem(selectedMaKhoaThi);
                // frmReport.Text = $"Danh sách học viên ĐẬU - Khóa thi: {tenKhoaThi}"; // Đặt tiêu đề form báo cáo (tùy chọn)
                frmReport.ShowDialog(); // Hiển thị form báo cáo (dạng Dialog để chặn tương tác với FrmDiem)
                                        // Hoặc dùng frmReport.Show(); nếu muốn cả 2 form cùng tương tác được
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có khóa thi nào được chọn không
            if (cmbTenkhoathi.SelectedIndex <= 0 || cmbTenkhoathi.SelectedValue == null || cmbTenkhoathi.SelectedValue == DBNull.Value)
            {
                MessageBox.Show("Vui lòng chọn một khóa thi cụ thể để in danh sách.", "Chưa chọn khóa thi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy MaKhoaThi đang được chọn
            object selectedMaKhoaThi = cmbTenkhoathi.SelectedValue;
            string tenKhoaThi = cmbTenkhoathi.Text; // Lấy tên để hiển thị (tùy chọn)

            try
            {
                // Khởi tạo form báo cáo và truyền MaKhoaThi vào
                Frmbaocaorot frmReport = new Frmbaocaorot(selectedMaKhoaThi);
                // frmReport.Text = $"Danh sách học viên ĐẬU - Khóa thi: {tenKhoaThi}"; // Đặt tiêu đề form báo cáo (tùy chọn)
                frmReport.ShowDialog(); // Hiển thị form báo cáo (dạng Dialog để chặn tương tác với FrmDiem)
                                        // Hoặc dùng frmReport.Show(); nếu muốn cả 2 form cùng tương tác được
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở form báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}