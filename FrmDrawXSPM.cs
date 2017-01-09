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
    public partial class FrmDrawXSPM : DevExpress.XtraEditors.XtraForm
    {
        public FrmDrawXSPM()
        {
            InitializeComponent();
        }

        private SecondaryAxisY CreateAxisY(Series series)
        {
            SecondaryAxisY myAxis = new SecondaryAxisY(series.Name);
            ((XYDiagram)chartXSPM.Diagram).SecondaryAxesY.Add(myAxis);
            ((XYDiagram)chartXSPM.Diagram).SecondaryAxesY.Clear();
            ((XYDiagram)chartXSPM.Diagram).SecondaryAxesY.Add(myAxis);
            ((StackedBarSeriesView)series.View).AxisY = myAxis;
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

        private void FrmDrawXSPM_Load(object sender, EventArgs e)
        {

            Series series1 = new Series("含水率", ViewType.StackedBar);
            Series series2 = new Series("产液量", ViewType.StackedBar);
            chartXSPM.DataSource = MainForm.dtResult;
            series1.ValueDataMembers.AddRange(new string[] { "HS" });
            series2.ValueDataMembers.AddRange(new string[] { "MNCYE" });
            series1.ArgumentDataMember = "CW";
            series2.ArgumentDataMember = "CW";

            List<Series> list = new List<Series>() { series1, series2 };
            chartXSPM.Series.AddRange(list.ToArray());
            chartXSPM.Legend.Visibility = DefaultBoolean.True;
            chartXSPM.Titles.Clear();
            ChartTitle titles = new ChartTitle();
            titles.Text = "油井吸水剖面图";
            titles.TextColor = System.Drawing.Color.Black;
            titles.Indent = 1; //设置距离  值越小柱状图就越大
            titles.Font = new Font("黑体", 15, FontStyle.Bold);  //设置字体
            titles.Dock = ChartTitleDockStyle.Right;//设置对齐方式
            titles.Alignment = StringAlignment.Center;//居中对齐
            chartXSPM.Titles.Add(titles);
            chartXSPM.SeriesTemplate.LabelsVisibility = DefaultBoolean.True;

            for (int i = 0; i < list.Count; i++)
            {
                //list[i].View.Color = colorList[i];

                CreateAxisY(list[i]);
            }
            XYDiagram diagram1 = (XYDiagram)chartXSPM.Diagram;
            //diagram1.DefaultPane.BorderVisible = false;
            diagram1.Rotated = true;
            diagram1.AxisX.Title.Text = "层位";
            diagram1.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.Default;
            diagram1.AxisX.Title.Alignment = StringAlignment.Center;
            diagram1.AxisX.Title.Font = new Font(new FontFamily("黑体"), 11);
            diagram1.AxisX.Alignment = AxisAlignment.Zero;
            diagram1.AxisY.GridLines.Visible = false;
        }
    }
}