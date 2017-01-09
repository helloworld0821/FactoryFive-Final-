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
using DXApplication1.Utils;
using DXApplication1.DAL.TextDAL;
using DXApplication1.BLL;

namespace DXApplication1
{
    //public delegate void ShowTracerResultViewDelegate(string str);
    public partial class FrmTracerSelectedInfo : DevExpress.XtraEditors.XtraForm
    {
        public event ShowResultViewDelegate ShowResultView;
        OpenFileUtil ofu = new OpenFileUtil();
        public FrmTracerSelectedInfo()
        {
            InitializeComponent();
        }

        private void btnOpenTracerFile_Click(object sender, EventArgs e)
        {
            txtOpenTracerFile.Text = ofu.OpenSMFile();
        }

        private void btnOpenCWDY_Click(object sender, EventArgs e)
        {
            txtOpenCWDY.Text = FileDialogHelper.OpenExcel();
        }

        private void btnOpenJHDY_Click(object sender, EventArgs e)
        {
            txtOpenJHDY.Text = FileDialogHelper.OpenExcel();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtOpenTracerFile.Text) && !string.IsNullOrEmpty(txtOpenCWDY.Text) && !string.IsNullOrEmpty(txtOpenJHDY.Text))
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("正在导入示踪剂数据");
                splashScreenManager1.SetWaitFormDescription("请稍后...");
                GetTracerDAL.dtTracer.Clear();
                FrmLayerSelectedInfo.dtExcelCWDY.Clear();
                FrmLayerSelectedInfo.dtExcelJHDY.Clear();
                GetTracerDAL gtd = new GetTracerDAL();
                string[] filePath = txtOpenTracerFile.Text.Split(new char[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string[] tracePath = txtSJDY.Text.Split(new char[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < filePath.Count(); i++)
                {
                    gtd.GetTracer(filePath[i], tracePath[i]);
                }

                FrmLayerSelectedInfo.dtExcelCWDY = ExcelToDataTableUtil.ExcelToDataTable(txtOpenCWDY.Text);
                FrmLayerSelectedInfo.dtExcelJHDY = ExcelToDataTableUtil.ExcelToDataTable(txtOpenJHDY.Text);

                string selectedNodeName = MainForm.strTreeNodeText;
                ShowResultView(selectedNodeName);

                splashScreenManager1.CloseWaitForm();
            }
            else
            {
                MessageBox.Show("请检查文件是否选择");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSJDY_Click(object sender, EventArgs e)
        {
            txtSJDY.Text = ofu.OpenTextFile();
        }

        private void FrmTracerSelectedInfo_Load(object sender, EventArgs e)
        {

        }
    }
}