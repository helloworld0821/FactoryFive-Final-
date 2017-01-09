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
    public delegate void  ShowResultViewDelegate(string str);
    public partial class FrmLayerSelectedInfo : DevExpress.XtraEditors.XtraForm
    {

        public event ShowResultViewDelegate ShowResultView;
        public static DataTable dtExcelCWDY = new DataTable ();
        public static DataTable dtExcelJHDY = new DataTable();

        OpenFileUtil ofu = new OpenFileUtil();
        public FrmLayerSelectedInfo()
        {
            InitializeComponent();
        }

        private void btnOpenSMFile_Click(object sender, EventArgs e)
        {
            txtOpenSMFile.Text = ofu.OpenSMFile();
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
            if (!string.IsNullOrEmpty(txtOpenSMFile.Text) && !string.IsNullOrEmpty(txtOpenCWDY.Text) && !string.IsNullOrEmpty(txtOpenJHDY.Text))
            {
                splashScreenManager1.ShowWaitForm();
                splashScreenManager1.SetWaitFormCaption("正在导入数模数据");
                splashScreenManager1.SetWaitFormDescription("请稍后...");
                GetLayer.dtWater.Clear();
                GetLayer.dtOil.Clear();
                dtExcelCWDY.Clear();
                dtExcelJHDY.Clear();
                GetLayer gl = new GetLayer();
                string[] filePath = txtOpenSMFile.Text.Split(new char[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < filePath.Count(); i++)
                {
                    gl.GetMnLayer(filePath[i]);
                }

                dtExcelCWDY = ExcelToDataTableUtil.ExcelToDataTable(txtOpenCWDY.Text);
                dtExcelJHDY = ExcelToDataTableUtil.ExcelToDataTable(txtOpenJHDY.Text);
                
                //MainForm mf = new MainForm();
                string selectedNodeName = MainForm.strTreeNodeText;
                //mf.ShowResultView += GetViewData;
                //mf.GirdViewResultSource();
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
        public DataTable GetViewData(string selectedNodeName)
        {
            
            TreeNodeMouseClickBLL tnmc = new TreeNodeMouseClickBLL();
            return tnmc.IdeDxLayer(selectedNodeName);
        }
        
    }
}