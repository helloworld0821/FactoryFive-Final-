using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts;
using DevExpress.Utils;

namespace DXApplication1
{
    public partial class FrmDrawCYPM : DevExpress.XtraEditors.XtraForm
    {
        public FrmDrawCYPM()
        {
            InitializeComponent();
        }

        private void DrawResult_Load(object sender, EventArgs e)
        {


            Series series1 = new Series("含水率", ViewType.Bar);
            Series series2 = new Series("产液量", ViewType.Bar);
            chartCYPM.DataSource = MainForm.dtResult;
            series1.ValueDataMembers.AddRange(new string[] { "HS" });
            series2.ValueDataMembers.AddRange(new string[] { "MNCYE" });
            series1.ArgumentDataMember = "CW";
            series2.ArgumentDataMember = "CW";
            series1.ValueScaleType = ScaleType.Numerical;
            //series1.PointOptions.ValueNumericOptions.Format = NumericFormat.Percent;
            //series1.Label.TextPattern = "{VP:P0}";
            List<Series> list = new List<Series>() { series1, series2 };
            chartCYPM.Series.AddRange(list.ToArray());
            chartCYPM.Legend.Visibility = DefaultBoolean.True;
            chartCYPM.Titles.Clear();
            ChartTitle titles = new ChartTitle();
            titles.Text = "油井产液剖面图";
            titles.TextColor = System.Drawing.Color.Black;
            titles.Indent = 1; //设置距离  值越小柱状图就越大
            titles.Font = new Font("黑体", 15, FontStyle.Bold);  //设置字体
            titles.Dock = ChartTitleDockStyle.Right;//设置对齐方式
            titles.Alignment = StringAlignment.Center;//居中对齐
            chartCYPM.Titles.Add(titles);
            chartCYPM.SeriesTemplate.LabelsVisibility = DefaultBoolean.True;

            for (int i = 0; i < list.Count; i++)
            {
                //list[i].View.Color = colorList[i];

                CreateAxisY(list[i]);
            }

            //chartCYPM.Series.Add(series1);
            //chartCYPM.Series.Add(series2);
            XYDiagram diagram1 = (XYDiagram)chartCYPM.Diagram;
            //SecondaryAxisY seAxisY = new SecondaryAxisY("产液量");
            //diagram1.SecondaryAxesY.Add(seAxisY);
            //((BarSeriesView)series2.View).AxisY = seAxisY;

            //diagram1.AxisY.Label.TextPattern = 

            //diagram1.DefaultPane.BorderVisible = false;
            diagram1.Rotated = true;
            diagram1.AxisX.Title.Text = "层位";
            diagram1.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.Default;
            diagram1.AxisX.Title.Alignment = StringAlignment.Center;
            diagram1.AxisX.Title.Font = new Font(new FontFamily("黑体"), 11);
            diagram1.AxisX.Alignment = AxisAlignment.Zero;
            
            diagram1.AxisY.GridLines.Visible = false;
        }
        private SecondaryAxisY CreateAxisY(Series series)
        {
            SecondaryAxisY myAxis = new SecondaryAxisY(series.Name);
            ((XYDiagram)chartCYPM.Diagram).SecondaryAxesY.Add(myAxis);
            ((XYDiagram)chartCYPM.Diagram).SecondaryAxesY.Clear();
            ((XYDiagram)chartCYPM.Diagram).SecondaryAxesY.Add(myAxis);
            ((BarSeriesView)series.View).AxisY = myAxis;
            myAxis.Title.Text = series.Name;
            myAxis.Title.Alignment = StringAlignment.Center; //顶部对齐
            myAxis.Title.Visibility = DevExpress.Utils.DefaultBoolean.Default; //显示标题
            myAxis.Title.Font = new Font(new FontFamily("黑体"), 11);

            Color color = series.View.Color;//设置坐标的颜色和图表线条颜色一致

            myAxis.Title.TextColor = color;
            myAxis.Label.TextColor = color;
            myAxis.Color = color;

            return myAxis;
        }
    }
}