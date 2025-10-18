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

using OfficeOpenXml;//3 thư viện dùng nhập file vào excel
using OfficeOpenXml.Style;
using System.IO;


namespace QuanLyCapChungChi
{
    public partial class FrmThongKeChiTiet : Form
    {
        SqlConnection MyCon = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456");
        public FrmThongKeChiTiet()
        {
            InitializeComponent();
        }

        private void FrmThongKeChiTiet_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'DataSet1.Hocvien' table. You can move, or remove it, as needed.
            reportViewer1.Visible = false;
            this.HocvienTableAdapter.Fill(this.DataSet1.Hocvien);
            Load_DataGridView();
            LoadKhoaHoc();
            LamDep_DataGridView();
            this.reportViewer1.RefreshReport();
        }
        private void Load_DataGridView()
        {
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();
            string sql = @"SELECT HOCVIEN.MAHOCVIEN,HOTEN,NGAYSINH,GIOITINH,DIACHI,SODIENTHOAI,EMAIL FROM HOCVIEN";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, MyCon);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dgvTKHocVien.DataSource = dt;

            dgvTKHocVien.Columns[0].HeaderText = "Mã Học Viên";
            dgvTKHocVien.Columns[1].HeaderText = "Họ Tên";

            dgvTKHocVien.Columns[2].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvTKHocVien.Columns[2].HeaderText = "Ngày Sinh";

            dgvTKHocVien.Columns[3].HeaderText = "Giới Tính";
            dgvTKHocVien.Columns[4].HeaderText = "Địa Chỉ";
            dgvTKHocVien.Columns[5].HeaderText = "Số Điện Thoại";

            dgvTKHocVien.Columns[6].HeaderText = "Emaii";

            dgvTKHocVien.AllowUserToAddRows = false;
            dgvTKHocVien.EditMode = DataGridViewEditMode.EditProgrammatically;

        }
        private void LoadKhoaHoc()
        {
            SqlConnection MyCon = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456");

            try
            {
                {
                    MyCon.Open();
                    string query = "SELECT MaKhoaHoc, TenKhoaHoc FROM KhoaHoc";
                    SqlDataAdapter da = new SqlDataAdapter(query, MyCon);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbMaKH.DataSource = dt;
                    cmbMaKH.DisplayMember = "TenKhoaHoc";
                    cmbMaKH.ValueMember = "MaKhoaHoc";
                } // MyCon sẽ tự động được đóng khi ra khỏi `using`
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách khóa học: " + ex.Message);
            }
        }

        private void cmbTenKhoaHoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void btnTKKhoahoc_Click(object sender, EventArgs e)
        {
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();
            string sql2 = @"SELECT HOCVIEN.MAHOCVIEN,HOTEN,NGAYSINH,GIOITINH,DIACHI,SODIENTHOAI,EMAIL FROM HOCVIEN
                INNER JOIN DANGKYHOC ON HOCVIEN.MAHOCVIEN = DANGKYHOC.MAHOCVIEN 
                INNER JOIN KHOAHOC ON KHOAHOC.MAKHOAHOC = DANGKYHOC.MAKHOAHOC 
                WHERE KHOAHOC.MAKHOAHOC = @MA";
            SqlCommand cmd2 = new SqlCommand(sql2, MyCon);
            cmd2.Parameters.AddWithValue("@MA", cmbMaKH.SelectedValue.ToString());
            SqlDataAdapter adapter = new SqlDataAdapter(cmd2);
            DataTable dt = new DataTable();
            adapter.Fill(dt); // Nạp dữ liệu vào DataTable
            dgvTKHocVien.DataSource = dt;

            dgvTKHocVien.Columns[0].HeaderText = "Mã Học Viên";
            dgvTKHocVien.Columns[1].HeaderText = "Họ Tên";

            dgvTKHocVien.Columns[2].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvTKHocVien.Columns[2].HeaderText = "Ngày Sinh";

            dgvTKHocVien.Columns[3].HeaderText = "Giới Tính";
            dgvTKHocVien.Columns[4].HeaderText = "Địa Chỉ";
            dgvTKHocVien.Columns[5].HeaderText = "Số Điện Thoại";

            dgvTKHocVien.Columns[6].HeaderText = "Emaii";

            dgvTKHocVien.AllowUserToAddRows = false;
            dgvTKHocVien.EditMode = DataGridViewEditMode.EditProgrammatically;

            // Đóng kết nối
            MyCon.Close();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (MyCon.State == ConnectionState.Closed) MyCon.Open();
            string sql = @"SELECT HOCVIEN.MAHOCVIEN,HOTEN,NGAYSINH,GIOITINH,DIACHI,SODIENTHOAI,EMAIL FROM HOCVIEN";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, MyCon);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dgvTKHocVien.DataSource = dt;

            dgvTKHocVien.Columns[0].HeaderText = "Mã Học Viên";
            dgvTKHocVien.Columns[1].HeaderText = "Họ Tên";

            dgvTKHocVien.Columns[2].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvTKHocVien.Columns[2].HeaderText = "Ngày Sinh";

            dgvTKHocVien.Columns[3].HeaderText = "Giới Tính";
            dgvTKHocVien.Columns[4].HeaderText = "Địa Chỉ";
            dgvTKHocVien.Columns[5].HeaderText = "Số Điện Thoại";

            dgvTKHocVien.Columns[6].HeaderText = "Emaii";

            dgvTKHocVien.AllowUserToAddRows = false;
            dgvTKHocVien.EditMode = DataGridViewEditMode.EditProgrammatically;
        }
        private void LamDep_DataGridView()
        {
            // Đặt chiều cao của tiêu đề
            dgvTKHocVien.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;//Ngăn không cho hệ thống và người dùng thay đổi chiều cao tiêu đề cột
            dgvTKHocVien.ColumnHeadersHeight = 40; // Chiều cao tiêu đề

            // Đặt màu nền và màu chữ cho tiêu đề
            dgvTKHocVien.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 212, 190);
            dgvTKHocVien.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTKHocVien.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);

            // Áp dụng màu cho một cột cụ thể
            dgvTKHocVien.Columns["MAHOCVIEN"].HeaderCell.Style.BackColor = Color.FromArgb(234, 0, 1);

            // Cập nhật lại giao diện
            dgvTKHocVien.EnableHeadersVisualStyles = false;
        }
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
        private void btnPrint_Click(object sender, EventArgs e)
        {
            // --- Cấu hình cơ bản ---
            // Số cột dữ liệu chính (Mã HV -> Email)
            int dataColumnCount = 7;
            // Cột bắt đầu của phần bên phải (Trung tâm...) và phần ký tên
            // Điều chỉnh nếu cần, ví dụ: cột D hoặc E tùy vào độ rộng mong muốn
            int rightAlignStartColumn = 4; // Cột D 
                                           // Tổng số cột sử dụng cho tiêu đề và căn chỉnh (A->G)
            int totalColumns = dataColumnCount;

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel files (*.xlsx)|*.xlsx", FileName = "DanhSachHocVien.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                    try
                    {
                        using (ExcelPackage package = new ExcelPackage())
                        {
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Danh Sách Học Viên");

                            // --- Định dạng Header của Báo cáo ---

                            // 1. Cộng hòa xã hội chủ nghĩa Việt Nam (Trái)
                            worksheet.Cells["A1"].Value = "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM";
                            worksheet.Cells["A1:C1"].Merge = true; // Gộp cột A-C cho dòng 1
                            worksheet.Cells["A1:C1"].Style.Font.Bold = true;
                            worksheet.Cells["A1:C1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A1:C1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                            worksheet.Cells["A2"].Value = "Độc lập - Tự do - Hạnh phúc";
                            worksheet.Cells["A2:C2"].Merge = true; // Gộp cột A-C cho dòng 2
                            worksheet.Cells["A2:C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells["A2:C2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            // Thêm đường kẻ dưới (Optional)
                            worksheet.Cells["A2:C2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                            // 2. Tên Trung tâm (Phải)
                            string rightHeaderColStart = ExcelColumnFromNumber(rightAlignStartColumn); // Lấy tên cột (VD: "D")
                            string rightHeaderColEnd = ExcelColumnFromNumber(totalColumns);         // Lấy tên cột cuối (VD: "G")

                            worksheet.Cells[rightHeaderColStart + "1"].Value = "TRUNG TÂM NGOẠI NGỮ TIN HỌC";
                            worksheet.Cells[rightHeaderColStart + "1:" + rightHeaderColEnd + "1"].Merge = true;
                            worksheet.Cells[rightHeaderColStart + "1:" + rightHeaderColEnd + "1"].Style.Font.Bold = true;
                            worksheet.Cells[rightHeaderColStart + "1:" + rightHeaderColEnd + "1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rightHeaderColStart + "1:" + rightHeaderColEnd + "1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                            worksheet.Cells[rightHeaderColStart + "2"].Value = "ĐẠI HỌC XÂY DỰNG MIỀN TRUNG";
                            worksheet.Cells[rightHeaderColStart + "2:" + rightHeaderColEnd + "2"].Merge = true;
                            worksheet.Cells[rightHeaderColStart + "2:" + rightHeaderColEnd + "2"].Style.Font.Bold = true;
                            worksheet.Cells[rightHeaderColStart + "2:" + rightHeaderColEnd + "2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[rightHeaderColStart + "2:" + rightHeaderColEnd + "2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            // Thêm đường kẻ dưới (Optional)
                            worksheet.Cells[rightHeaderColStart + "2:" + rightHeaderColEnd + "2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            // 3. Tiêu đề chính của báo cáo
                            int titleRow = 4; // Dòng cho tiêu đề chính (để cách 1 dòng)
                            worksheet.Cells[titleRow, 1].Value = "DANH SÁCH HỌC VIÊN";
                            worksheet.Cells[titleRow, 1, titleRow, totalColumns].Merge = true; // Gộp từ cột A đến G
                            worksheet.Cells[titleRow, 1, titleRow, totalColumns].Style.Font.Bold = true;
                            worksheet.Cells[titleRow, 1, titleRow, totalColumns].Style.Font.Size = 14; // Cỡ chữ lớn hơn
                            worksheet.Cells[titleRow, 1, titleRow, totalColumns].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[titleRow, 1, titleRow, totalColumns].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                            // --- Định dạng Tiêu đề Cột Dữ liệu ---
                            int headerDataRow = titleRow + 2; // Dòng cho tiêu đề cột (cách 1 dòng)
                            worksheet.Cells[headerDataRow, 1].Value = "Mã Học Viên";
                            worksheet.Cells[headerDataRow, 2].Value = "Họ Tên";
                            worksheet.Cells[headerDataRow, 3].Value = "Ngày Sinh";
                            worksheet.Cells[headerDataRow, 4].Value = "Giới Tính";
                            worksheet.Cells[headerDataRow, 5].Value = "Địa Chỉ";
                            worksheet.Cells[headerDataRow, 6].Value = "Số Điện Thoại";
                            worksheet.Cells[headerDataRow, 7].Value = "Email";

                            // Định dạng cho dòng tiêu đề cột dữ liệu
                            using (ExcelRange headerRange = worksheet.Cells[headerDataRow, 1, headerDataRow, dataColumnCount])
                            {
                                headerRange.Style.Font.Bold = true; // in đậm tiêu đề cột
                                headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray); // Màu nền xám nhạt
                                headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa
                                headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                headerRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                headerRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                headerRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                headerRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            }

                            // --- Xuất dữ liệu từ DataGridView vào file Excel ---
                            int dataStartRow = headerDataRow + 1; // Dòng bắt đầu ghi dữ liệu
                            if (dgvTKHocVien.Rows.Count > 0)
                            {
                                for (int i = 0; i < dgvTKHocVien.Rows.Count; i++)
                                {
                                    // Kiểm tra nếu là dòng mới (không phải dòng header của dgv)
                                    if (!dgvTKHocVien.Rows[i].IsNewRow)
                                    {
                                        int currentRow = dataStartRow + i;
                                        worksheet.Cells[currentRow, 1].Value = dgvTKHocVien.Rows[i].Cells[0].Value?.ToString();
                                        worksheet.Cells[currentRow, 2].Value = dgvTKHocVien.Rows[i].Cells[1].Value?.ToString();

                                        // Xử lý Ngày Sinh - Chuyển đổi sang DateTime nếu có thể để định dạng đúng
                                        if (dgvTKHocVien.Rows[i].Cells[2].Value != null && DateTime.TryParse(dgvTKHocVien.Rows[i].Cells[2].Value.ToString(), out DateTime ngaySinh))
                                        {
                                            worksheet.Cells[currentRow, 3].Value = ngaySinh;
                                            worksheet.Cells[currentRow, 3].Style.Numberformat.Format = "dd/MM/yyyy"; // Định dạng ngày tháng
                                        }
                                        else
                                        {
                                            worksheet.Cells[currentRow, 3].Value = dgvTKHocVien.Rows[i].Cells[2].Value?.ToString(); // Giữ nguyên nếu không parse được
                                        }
                                        worksheet.Cells[currentRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa ngày sinh

                                        worksheet.Cells[currentRow, 4].Value = dgvTKHocVien.Rows[i].Cells[3].Value?.ToString();
                                        worksheet.Cells[currentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; // Căn giữa giới tính

                                        worksheet.Cells[currentRow, 5].Value = dgvTKHocVien.Rows[i].Cells[4].Value?.ToString();
                                        worksheet.Cells[currentRow, 6].Value = dgvTKHocVien.Rows[i].Cells[5].Value?.ToString();
                                        // Đảm bảo cột SĐT là text để không mất số 0 đầu
                                        worksheet.Cells[currentRow, 6].Style.Numberformat.Format = "@"; // Định dạng Text
                                        worksheet.Cells[currentRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                                        worksheet.Cells[currentRow, 7].Value = dgvTKHocVien.Rows[i].Cells[6].Value?.ToString();

                                        // Thêm viền cho các ô dữ liệu (Optional)
                                        using (ExcelRange dataRowRange = worksheet.Cells[currentRow, 1, currentRow, dataColumnCount])
                                        {
                                            dataRowRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                            dataRowRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                            dataRowRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                            dataRowRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                            dataRowRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center; // Căn giữa theo chiều dọc
                                        }
                                    }
                                }
                            }

                            // --- Định dạng Footer (Người lập, Ngày tháng) ---
                            int lastDataRow = dataStartRow + dgvTKHocVien.Rows.Count - (dgvTKHocVien.AllowUserToAddRows ? 1 : 0); // Xác định dòng dữ liệu cuối cùng
                            if (dgvTKHocVien.Rows.Count == 0) lastDataRow = headerDataRow; // Nếu không có dữ liệu

                            int footerStartRow = lastDataRow + 2; // Bắt đầu phần ký tên (cách 1 dòng)

                            // Ngày ... tháng ... năm ... (Phải)
                            worksheet.Cells[footerStartRow, rightAlignStartColumn].Value = $"Ngày {DateTime.Now.Day} tháng {DateTime.Now.Month} năm {DateTime.Now.Year}";
                            worksheet.Cells[footerStartRow, rightAlignStartColumn, footerStartRow, totalColumns].Merge = true;
                            worksheet.Cells[footerStartRow, rightAlignStartColumn, footerStartRow, totalColumns].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            //worksheet.Cells[footerStartRow, rightAlignStartColumn, footerStartRow, totalColumns].Style.Italic = true; // In nghiêng

                            // Người lập (Phải)
                            int nguoiLapRow = footerStartRow + 1;
                            worksheet.Cells[nguoiLapRow, rightAlignStartColumn].Value = "Người lập";
                            worksheet.Cells[nguoiLapRow, rightAlignStartColumn, nguoiLapRow, totalColumns].Merge = true;
                            worksheet.Cells[nguoiLapRow, rightAlignStartColumn, nguoiLapRow, totalColumns].Style.Font.Bold = true;
                            worksheet.Cells[nguoiLapRow, rightAlignStartColumn, nguoiLapRow, totalColumns].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                            // (Ký và ghi rõ họ tên) (Phải)
                            int kyTenRow = nguoiLapRow + 1;
                            worksheet.Cells[kyTenRow, rightAlignStartColumn].Value = "(Ký và ghi rõ họ tên)";
                            worksheet.Cells[kyTenRow, rightAlignStartColumn, kyTenRow, totalColumns].Merge = true;
                            //worksheet.Cells[kyTenRow, rightAlignStartColumn, kyTenRow, totalColumns].Style.Italic = true;
                            worksheet.Cells[kyTenRow, rightAlignStartColumn, kyTenRow, totalColumns].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                            // --- Tự động điều chỉnh độ rộng cột ---
                            // Chạy AutoFit sau khi đã thêm tất cả nội dung
                            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                            // worksheet.Cells["A:G"].AutoFitColumns(); // Hoặc chỉ định rõ các cột cần AutoFit

                            // Điều chỉnh lại độ rộng cột nếu AutoFit chưa ưng ý (ví dụ cột Địa chỉ)
                            worksheet.Column(5).Width = 40; // Cột E (Địa chỉ) - đặt độ rộng cố định nếu muốn
                            worksheet.Column(2).Width = 25; // Cột B (Họ tên)
                            worksheet.Column(6).Width = 15; // Cột F (SĐT)
                            worksheet.Column(7).Width = 25; // Cột G (Email)

                            // --- Lưu file Excel ---
                            File.WriteAllBytes(sfd.FileName, package.GetAsByteArray());

                            MessageBox.Show("Xuất danh sách học viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xuất file: " + ex.Message + (ex.InnerException != null ? "\nInner Exception: " + ex.InnerException.Message : ""), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        private void button1_Click(object sender, EventArgs e)
        {
            reportViewer1.Visible = true;
        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }
        private void set_report(bool enable)
        {
            reportViewer1.Visible = false;

        }
    }
    }

