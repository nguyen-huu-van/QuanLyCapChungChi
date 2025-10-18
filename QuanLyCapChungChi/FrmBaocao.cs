using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Data.SqlClient;
namespace QuanLyCapChungChi
{
    public partial class FrmBaocao : Form
    {
        private string _selectedKhoaHoc = null;
        public FrmBaocao(string tenKhoaHoc)
        {
            InitializeComponent();

            // Lưu tên khóa học được chọn
            _selectedKhoaHoc = tenKhoaHoc;
        }
        // Biến để lưu tên khóa học được truyền từ frmHocvien
       

        private void Form1_Load(object sender, EventArgs e)
        {
            // Chỉ load dữ liệu và refresh nếu có tên khóa học hợp lệ
            if (!string.IsNullOrEmpty(_selectedKhoaHoc))
            {
                LoadReportData(_selectedKhoaHoc);
            }
            else
            {
                // Xử lý khi form được mở mà không có khóa học (ví dụ: mở từ menu chính)
                this.reportViewer1.LocalReport.DataSources.Clear(); // Xóa datasource cũ
                this.reportViewer1.Clear(); // Xóa hiển thị report viewer
                                            // Không gọi RefreshReport() ở đây vì không có dữ liệu
                MessageBox.Show("Chưa có khóa học nào được chọn để tạo báo cáo.", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Cân nhắc đóng form nếu không có dữ liệu: this.Close();
            }
        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {
   
           
        }
        private void LoadReportData(string tenKhoaHoc)
        {
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456;Encrypt=False";
            DataTable dt = new DataTable();
            string sql = @"SELECT HocVien.MaHocVien AS Expr1, HocVien.HoTen, HocVien.NgaySinh, HocVien.GioiTinh, HocVien.DiaChi, HocVien.SoDienThoai, HocVien.Email, KhoaHoc.TenKhoaHoc
                           FROM HocVien INNER JOIN
                           DangKyHoc ON HocVien.MaHocVien = DangKyHoc.MaHocVien INNER JOIN
                           KhoaHoc ON DangKyHoc.MaKhoaHoc = KhoaHoc.MaKhoaHoc
                           WHERE KhoaHoc.TenKhoaHoc = @tenKhoaHocParam";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@tenKhoaHocParam", SqlDbType.NVarChar).Value = tenKhoaHoc;
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            conn.Open();
                            adapter.Fill(dt);
                        }
                    }
                }

                // --- Cấu hình ReportViewer ---
                this.reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.DataSources.Clear(); // Luôn xóa datasource cũ trước khi thêm mới
                // *** Đảm bảo tên này chính xác ***
                this.reportViewer1.LocalReport.ReportEmbeddedResource = "QuanLyCapChungChi.Report1.rdlc";

                if (dt.Rows.Count > 0)
                {
                    // *** Đảm bảo tên "DataSet1" khớp với tên trong RDLC ***
                    ReportDataSource rds = new ReportDataSource("DataSet1", dt);
                    this.reportViewer1.LocalReport.DataSources.Add(rds);

                    // !!! Chỉ gọi RefreshReport SAU KHI đã thêm DataSource thành công !!!
                    this.reportViewer1.RefreshReport();
                }
                else
                {
                    // Không có dữ liệu -> Không thêm DataSource và không gọi RefreshReport
                    this.reportViewer1.Clear(); // Xóa hiển thị cũ nếu có
                    MessageBox.Show($"Không tìm thấy học viên nào cho khóa học '{tenKhoaHoc}'.", "Không có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Không gọi RefreshReport ở đây
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Lỗi SQL: {sqlEx.Message}\nSố lỗi: {sqlEx.Number}", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.reportViewer1.Clear(); // Xóa hiển thị nếu có lỗi
            }
            catch (LocalProcessingException lpEx) // Bắt lỗi cụ thể của ReportViewer
            {
                MessageBox.Show($"Lỗi xử lý báo cáo cục bộ: {lpEx.Message}\nKiểm tra lại tên DataSet và cấu hình báo cáo.", "Lỗi Báo Cáo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.reportViewer1.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tải báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.reportViewer1.Clear(); // Xóa hiển thị nếu có lỗi
            }
        }

    }
}
