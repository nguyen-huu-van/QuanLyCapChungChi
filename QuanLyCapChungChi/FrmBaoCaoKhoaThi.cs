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
    public partial class FrmBaoCaoKhoaThi : Form
    {
        private string _selectedKhoaThi = null;
        public FrmBaoCaoKhoaThi(string tenKhoaThi)
        {
            InitializeComponent();
            // Lưu tên khóa học được chọn
            _selectedKhoaThi = tenKhoaThi;
        }

        private void FrmBaoCaoKhoaThi_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedKhoaThi))
            {
                LoadReportData(_selectedKhoaThi);
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
        private void LoadReportData(string tenKhoaThi)
        {
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456;Encrypt=False";
            DataTable dt = new DataTable();
            string sql = @"SELECT HocVien.MaHocVien, HocVien.HoTen, HocVien.NgaySinh, HocVien.GioiTinh, HocVien.DiaChi, HocVien.SoDienThoai, HocVien.Email, KhoaThi.TenKhoaThi
                           FROM HocVien INNER JOIN
                           ketquathi ON HocVien.MaHocVien = ketquathi.MaHocVien INNER JOIN
                           Khoathi ON ketquathi.MaKhoathi = Khoathi.MaKhoathi
                           WHERE KhoaThi.TenKhoaThi = @tenKhoaThiParam";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@tenKhoaThiParam", SqlDbType.NVarChar).Value = tenKhoaThi;
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
                this.reportViewer1.LocalReport.ReportEmbeddedResource = "QuanLyCapChungChi.Report2.rdlc";

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
                    MessageBox.Show($"Không tìm thấy học viên nào cho khóa học '{tenKhoaThi}'.", "Không có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}
