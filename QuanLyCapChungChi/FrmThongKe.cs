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
using System.Windows.Forms.DataVisualization.Charting; // Thêm thư viện Chart

namespace QuanLyCapChungChi
{

    public partial class FrmThongKe : Form
    {
        SqlConnection MyCon = new SqlConnection("Data Source=DESKTOP-U7HLODO\\SQLEXPRESS;Initial Catalog=QuanLyCapChungChi;User ID=sa;Password=123456");
        public FrmThongKe()
        {
            InitializeComponent();
        }
        private int GetCount(string query)
        {
            SqlCommand cmd = new SqlCommand(query, MyCon);
            return (int)cmd.ExecuteScalar();
        }
        private void FrmThongKe_Load(object sender, EventArgs e)
        {

            if (MyCon.State == ConnectionState.Closed) MyCon.Open();

            // Lấy dữ liệu từ SQL
            int tongHocVien = GetCount("SELECT COUNT(MAHOCVIEN) FROM HOCVIEN");
            int tongGiangVien = GetCount("SELECT COUNT(MAGIANGVIEN) FROM GIANGVIEN");
            int tongKhoaHoc = GetCount("SELECT COUNT(MAKHOAHOC) FROM KHOAHOC");
            int tongChungChi = GetCount("SELECT COUNT(MAKHOATHI) FROM KHOATHI");

            // Hiển thị dữ liệu lên TextBox
            txtTongHocVien.Text = tongHocVien.ToString();
            txtTongGiangVien.Text = tongGiangVien.ToString();
            txtTongKhoaHoc.Text = tongKhoaHoc.ToString();
            txtTongChungChi.Text = tongChungChi.ToString();

            // Hiển thị dữ liệu lên biểu đồ (chart1)
            ShowChart(tongHocVien, tongGiangVien, tongKhoaHoc, tongChungChi);

            MyCon.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }
        // Hàm hiển thị dữ liệu lên Chart
        private void ShowChart(int hocVien, int giangVien, int khoaHoc, int chungChi)
        {
            // Xóa dữ liệu cũ
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Legends.Clear();

            // Xóa các hiệu ứng nền không mong muốn
            chart1.BackHatchStyle = ChartHatchStyle.None;
            chart1.BackImage = "";
            chart1.BackColor = Color.White;
            chart1.BackGradientStyle = GradientStyle.None;

            // Tạo vùng hiển thị cho biểu đồ
            ChartArea chartArea = new ChartArea();
            chartArea.BackColor = Color.Transparent; // Xóa màu nền vùng biểu đồ
            chartArea.AxisX.Title = "Danh mục";
            chartArea.AxisY.Title = "Số lượng";
            chartArea.AxisY.Interval = 1;
            chartArea.AxisX.MajorGrid.LineWidth = 0; // Ẩn lưới trục X
            chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot; // Lưới Y kiểu chấm
            chart1.ChartAreas.Add(chartArea);

            // Thêm màu nền Gradient cho biểu đồ
            chart1.BackColor = Color.LightSteelBlue; // Màu nền chính
            chart1.BackGradientStyle = GradientStyle.TopBottom; // Hiệu ứng chuyển màu
            chart1.BackSecondaryColor = Color.WhiteSmoke; // Màu chuyển dần xuống dưới

            // Tạo Series mới
            Series series = new Series("Thống kê");
            series.ChartType = SeriesChartType.Column; // Biểu đồ cột
            series.Font = new Font("Arial", 12, FontStyle.Bold); // Font chữ to đẹp hơn
            series.IsValueShownAsLabel = true; // Hiển thị số trên cột
            series.LabelForeColor = Color.Black; // Màu chữ số liệu trên cột

            // Mảng tên và giá trị
            string[] categories = { "Học viên", "Giảng viên", "Khóa học", "Khóa thi" };
            int[] values = { hocVien, giangVien, khoaHoc, chungChi };

            // Màu sắc cho từng cột
            Color[] colors = { Color.Blue, Color.Green, Color.Orange, Color.Red };

            for (int i = 0; i < categories.Length; i++)
            {
                DataPoint point = new DataPoint();
                point.SetValueXY(categories[i], values[i]);
                point.Color = colors[i]; // Gán màu cho cột
                point.Label = values[i].ToString(); // Hiển thị số liệu trên cột
                point.Font = new Font("Arial", 10, FontStyle.Bold);

                // Bo góc cột
                point.BorderWidth = 2;
                point.BorderColor = Color.Black;

                series.Points.Add(point);
            }

            // Thêm Series vào Chart
            chart1.Series.Add(series);

            // Thêm chú thích (Legend)
            Legend legend = new Legend();
            legend.Title = "Chú thích";
            legend.Font = new Font("Arial", 10, FontStyle.Bold);
            legend.BackColor = Color.Transparent; // Nền trong suốt
            legend.Docking = Docking.Top; // Đặt vị trí trên cùng
            legend.Alignment = StringAlignment.Far; // Canh phải

            chart1.Legends.Add(legend);

            // Thêm từng mục vào Legend
            for (int i = 0; i < categories.Length; i++)
            {
                LegendItem legendItem = new LegendItem();
                legendItem.Name = categories[i];
                legendItem.Color = colors[i];
                chart1.Legends[0].CustomItems.Add(legendItem);
            }

        }
        private void panel1_Click(object sender, EventArgs e)
        {
            FrmThongKeChiTiet frm = new FrmThongKeChiTiet();
            frm.Show();
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
