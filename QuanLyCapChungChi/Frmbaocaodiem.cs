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

    public partial class Frmbaocaodiem : Form
    {
        private readonly object _maKhoaThi;
        public Frmbaocaodiem(object maKhoaThi)
        {
            if (maKhoaThi == null || maKhoaThi == DBNull.Value)
            {
                // Có thể hiển thị lỗi và đóng form hoặc ném exception
                MessageBox.Show("Mã khóa thi không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Cân nhắc đóng form nếu không có mã khóa thi:
                // this.Load += (s, e) => this.Close(); // Đóng form ngay khi Load nếu có lỗi
                // Hoặc ném lỗi để FrmDiem xử lý:
                throw new ArgumentNullException(nameof(maKhoaThi), "Mã khóa thi không được rỗng khi tạo form báo cáo.");
            }
            _maKhoaThi = maKhoaThi;
            InitializeComponent();
        }

        private void Frmbaocaodiem_Load(object sender, EventArgs e)
        {
            LoadReportData(_maKhoaThi);
        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }

        // Hàm tải và hiển thị dữ liệu báo cáo
        private void LoadReportData(object maKhoaThi) // Nhận MaKhoaThi
        {
            string connectionString = "Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456;Encrypt=False";
            DataTable dt = new DataTable();

            // --- Câu lệnh SQL chỉ lấy những học viên ĐẬU của khóa thi cụ thể ---
            string sql = @"
                SELECT
                    kqt.MaHocVien,
                    h.HOTEN,
                    kqt.DIEMSO,
                    kqt.DIEMSO2,
                    kqt.DIEMSO3,
                    kt.TENKHOATHI,
                    -- Có thể lấy cột ketqua từ DB nếu bạn đã tạo Trigger/Computed Column
                    -- kqt.ketqua AS KetQuaDB
                    -- Hoặc tính lại ở đây để chắc chắn đúng logic báo cáo
                    CASE
                        WHEN kt.TENKHOATHI LIKE N'%Tin học%' AND kqt.DIEMSO >= 5.0 AND kqt.DIEMSO2 >= 5.0 THEN N'Đậu'
                        WHEN kt.TENKHOATHI LIKE N'%Tiếng Anh%' AND kqt.DIEMSO >= 5.0 AND kqt.DIEMSO2 >= 5.0 AND kqt.DIEMSO3 >= 5.0 THEN N'Đậu'
                        ELSE N'Rớt' -- Về lý thuyết không nên có dòng Rớt ở đây do mệnh đề WHERE, nhưng để cho đủ CASE
                    END AS KetQuaReport -- Đặt tên khác nếu bạn có cả cột từ DB
                FROM KETQUATHI kqt
                INNER JOIN HOCVIEN h ON kqt.MAHOCVIEN = h.MAHOCVIEN
                INNER JOIN KHOATHI kt ON kqt.MAKHOATHI = kt.MAKHOATHI
                WHERE kqt.MAKHOATHI = @MaKhoaThi -- Lọc theo khóa thi được chọn
                  AND ( -- Thêm điều kiện lọc chỉ những người ĐẬU
                        (kt.TENKHOATHI LIKE N'%Tin học%' AND kqt.DIEMSO >= 5.0 AND kqt.DIEMSO2 >= 5.0)
                        OR
                        (kt.TENKHOATHI LIKE N'%Tiếng Anh%' AND kqt.DIEMSO >= 5.0 AND kqt.DIEMSO2 >= 5.0 AND kqt.DIEMSO3 >= 5.0)
                      )
                ORDER BY h.HOTEN; -- Sắp xếp theo tên học viên cho dễ nhìn";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        // Thêm Parameter MaKhoaThi vào câu lệnh SQL
                        // Quan trọng: Sử dụng AddWithValue hoặc Add với đúng SqlDbType
                        cmd.Parameters.AddWithValue("@MaKhoaThi", maKhoaThi);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            conn.Open();
                            adapter.Fill(dt);
                        }
                    }
                }

                // --- Cấu hình ReportViewer ---
                this.reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.LocalReport.DataSources.Clear();

                // *** Quan trọng: Đảm bảo tên này đúng với tên file report và namespace của bạn ***
                // Ví dụ: Nếu file là ReportDiemDau.rdlc trong project QuanLyCapChungChi
                // thì đường dẫn là "QuanLyCapChungChi.ReportDiemDau.rdlc"
                this.reportViewer1.LocalReport.ReportEmbeddedResource = "QuanLyCapChungChi.Report3.rdlc"; // Sửa lại nếu tên file report khác

                if (dt.Rows.Count > 0)
                {
                    // *** Quan trọng: Đảm bảo tên "DataSet1" khớp với tên DataSet trong file RDLC ***
                    ReportDataSource rds = new ReportDataSource("DataSet1", dt); // Sửa "DataSet1" nếu cần
                    this.reportViewer1.LocalReport.DataSources.Add(rds);

                    // Làm mới báo cáo để hiển thị dữ liệu
                    this.reportViewer1.RefreshReport();
                }
                else
                {
                    // Không có ai đậu trong khóa thi này
                    this.reportViewer1.Clear(); // Xóa hiển thị cũ
                    MessageBox.Show($"Không có học viên nào đậu trong khóa thi này.", "Không có dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Không gọi RefreshReport khi không có dữ liệu
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Lỗi SQL khi tải danh sách đậu: {sqlEx.Message}\nSố lỗi: {sqlEx.Number}", "Lỗi Cơ Sở Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.reportViewer1.Clear();
            }
            catch (LocalProcessingException lpEx)
            {
                MessageBox.Show($"Lỗi xử lý báo cáo: {lpEx.Message}\nKiểm tra lại tên DataSet ('DataSet1'?) và đường dẫn ReportEmbeddedResource ('QuanLyCapChungChi.Report1.rdlc'?) trong code và file RDLC.", "Lỗi Báo Cáo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.reportViewer1.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi khi tải báo cáo danh sách đậu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.reportViewer1.Clear();
            }
        }
    }
}
