using DevExpress.XtraBars;
using DevExpress.XtraCharts;
using DevExpress.XtraPrinting;
using DXApplication1.BLL;
using DXApplication1.BLL.DatabaseBLL;
using DXApplication1.DAL;
using DXApplication1.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors;
using DXApplication1.DAL.TextDAL;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Drawing.Imaging;
using System.IO;

namespace DXApplication1
{
    //public delegate void MouseWheelHandle(object sender, MouseEventArgs e);
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        //public event MouseWheelHandle MouseWheelHandleEvent;
        public static DataTable dtResult = new DataTable();
        public static int ChooseWaterItem = -1;
        public static int ChooseOilItem = -1;
        public static readonly string strLTKName = "LTK";
        public static string strStartDate = string.Empty;
        public static string strEndDate = string.Empty;
        public static string strCSMBPath = String.Empty;
        DataTable dtCM = new DataTable();
        DataTable dt = new DataTable();
        public static DataTable dtShowResultView = new DataTable();
        public static string strTreeNodeText = string.Empty;
        TreeNode tn = new TreeNode();
        TreeNode tnp = new TreeNode();

        string strTnpText = string.Empty;
        string strTnText = string.Empty;
        private static readonly int WIDTH = 175;
        private static readonly int HEIGHT = 70;
        //public static Bitmap img = new Bitmap(WIDTH, HEIGHT);

        //public static Graphics gs = Graphics.FromImage(img);

        //Metafile mf = new Metafile("img.emf", gs.GetHdc());

        int zoomStep = 40;
        Bitmap myBmp;
        bool isMove = false;    //判断鼠标在pictureEdit上移动时，是否处于拖拽过程(鼠标左键是否按下)
        Point mouseDownPoint = new Point(); //记录拖拽过程鼠标位置

        [DllImport("User32.dll")]
        private static extern IntPtr WindowFromPoint(Point p);
        IntPtr h;
        Control currentControl;
        TreeNodeMouseClickBLL tnmc = new TreeNodeMouseClickBLL();

        public static string[] strNodes = { "数据前处理", "低效循环层位识别", "低效循环油水井识别" };
        public MainForm()
        {
            InitializeComponent();
        }

        public void TreeViewBind(DataTable dt, TreeNode parentNode, string parentID)
        {
            gridView1.OptionsView.ShowGroupPanel = false;
            DataRow[] rows = dt.Select(string.Format("parentID={0}", parentID));
            foreach (DataRow row in rows)
            {
                TreeNode node = new TreeNode()
                {
                    Text = row["NAME"].ToString()
                };
                TreeViewBind(dt, node, row["ID"].ToString());
                if (parentNode == null)
                {
                    treeView1.Nodes.Add(node);
                }
                else
                {
                    parentNode.Nodes.Add(node);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.pictureEdit1.MouseWheel += PictureEdit1_MouseWheel;
            ReadFileDAL.ReadPermX();
            ribbonPageHidden.Visible = false;
            GetJJ.GetJJMethod();

            GetTreeList.GetTreeListMethod();
            //SlpfDAL slf = new SlpfDAL();
            //slf.GetSlpfData();

            InitializeBLL ib = new InitializeBLL();
            TreeViewBind(GetTreeList.dtTreeList, null, "-1");
            DataTable dtDate = ib.LoadDate();
            ib.AmendDaa074();
            //ib.AmendDCB01();
            //ib.AmendDCB02();
            for (int i = 0; i < dtDate.Rows.Count; i++)
            {
                cmbStartDate.Properties.Items.Add(dtDate.Rows[i]["NY"]);
                //cmbEndDate.Properties.Items.Add(dtDate.Rows[i]["NY"]);
            }

            layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            radioSelected.SelectedIndex = -1;
        }


        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            tn = e.Node;

            // TreeNode tn = treeView1.SelectedNode;


            if (!strNodes.Contains(tn.Text))
            {
                tnp = e.Node.Parent;
                if (tnp.Text.Equals("数据前处理"))
                {
                    layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dockPanelDate.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                    dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;

                    gridView1.Columns.Clear();
                    gridDatabs.DataSource = tnmc.ShowDataTable(tn.Text);

                }
                else if (tnp.Text.Equals("低效循环层位识别"))
                {
                    layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dockPanelDate.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;

                    gridView1.Columns.Clear();
                    gridResult.Visible = true;
                    //gridDatabs.DataSource = tnmc.Compute(tn.Text);
                    if (ComputeHIDAL.dtOilHI.Rows.Count > 0 && ComputeHIDAL.dtWaterHI.Rows.Count > 0)
                    {
                        if ((tn.Text.Equals("层位识别")))
                        {
                            dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                        }
                        if (tn.Text.Equals("层位展示"))
                        {
                            dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                            if (dt.Rows.Count > 0)
                            {
                                layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                                gridViewLayer.Columns.Clear();
                                gridLayer.DataSource = dt.DefaultView.ToTable(true, new string[] { "CW", "K" });
                                gridViewLayer.Columns[0].Caption = "层位";
                                gridViewLayer.Columns[0].FieldName = "CW";
                                gridViewLayer.Columns[1].VisibleIndex = -1;
                            }
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("请先进行油水井筛选");
                    }

                }
                else if (tnp.Text.Equals("射孔库"))
                {
                    layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                }
                else if (tnp.Text.Equals("低效循环油井识别") || tnp.Text.Equals("低效循环水井识别"))
                {
                    layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dockPanelDate.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                    dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                    radioSelected.SelectedIndex = -1;

                }
                else if (tnp.Text.Equals("低效循环油水井识别"))
                {
                    layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dockPanelDate.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                    dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                    radioSelected.SelectedIndex = -1;

                    if (tn.Text.Equals("低效循环油水井列表"))
                    {
                        if (ComputeHIDAL.dtOilFirstFilterHI1.Rows.Count > 0 && ComputeHIDAL.dtWaterFirstFilterHI1.Rows.Count > 0)
                        {
                            layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            gridViewDXSJ.Columns.Clear();
                            gridViewDXYJ.Columns.Clear();
                            gridDXYJ.DataSource = ComputeHIDAL.DXOilMethod();
                            gridViewDXYJ.Columns[0].Caption = "低效油井:  共" + gridViewDXYJ.RowCount + "口";
                            gridViewDXYJ.Columns[0].FieldName = "JH";
                            gridDXSJ.DataSource = ComputeHIDAL.DXWaterMethod();
                            gridViewDXSJ.Columns[0].Caption = "低效水井:  共" + gridViewDXSJ.RowCount + "口";
                            gridViewDXSJ.Columns[0].FieldName = "JH";
                        }
                        else
                        {
                            XtraMessageBox.Show("请先进行油水井筛选");
                        }
                    }
                }
            }

        }
        private void cmbStartDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            strStartDate = cmbStartDate.Text;
            cmbEndDate.Properties.Items.Clear();
            OprDatabsBLL odb = new OprDatabsBLL();
            DataTable dtEndList = odb.EndDateList();
            foreach (DataRow dr in dtEndList.Rows)
            {
                cmbEndDate.Properties.Items.Add(dr["NY"]);
            }
        }

        private void cmbEndDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            strEndDate = cmbEndDate.Text;
        }



        private void chartControl1_Click_1(object sender, EventArgs e)
        {

            MouseEventArgs me = (MouseEventArgs)e;
            ChartHitInfo hitInfo = chartControl1.CalcHitInfo(me.Location);
            if (hitInfo.SeriesPoint != null)
            //if (hitInfo.InDiagram)
            {
                XtraMessageBox.Show(Convert.ToDouble(hitInfo.SeriesPoint.Argument).ToString("f2") + "," + hitInfo.SeriesPoint.Values[0].ToString("f2"));
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            splashScreenManager1.ShowWaitForm();
            splashScreenManager1.SetWaitFormCaption("正在导入模板信息");
            splashScreenManager1.SetWaitFormDescription("请稍后...");

            OprDatabsBLL odb = new OprDatabsBLL();
            odb.UpDatabs(dtCM);


            splashScreenManager1.CloseWaitForm();
        }

        private void btnOpenCCCSMBFile_Click(object sender, EventArgs e)
        {
            OpenFileUtil ofu = new OpenFileUtil();
            strCSMBPath = ofu.OpenTextFile();

            if (!string.IsNullOrEmpty(strCSMBPath))
            {
                CCCSMBUtil cu = new CCCSMBUtil();
                string[] filePath = strCSMBPath.Split(new char[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < filePath.Count(); i++)
                {
                    dtCM = cu.GetCCCSMB(filePath[i]);
                }

            }

            if (CCCSMBUtil.isCCCSMB)
            {
                gridViewCCCSMB.Columns.Clear();
                gridCCCSMB.DataSource = dtCM;
                gridViewCCCSMB.Columns[0].Caption = "油层组名称";
                gridViewCCCSMB.Columns[0].FieldName = "yczmc";
                gridViewCCCSMB.Columns[1].Caption = "砂体类型";
                gridViewCCCSMB.Columns[1].FieldName = "stlx";
                gridViewCCCSMB.Columns[2].Caption = "孔隙度";
                gridViewCCCSMB.Columns[2].FieldName = "kxd";
                gridViewCCCSMB.Columns[3].Caption = "饱和度";
                gridViewCCCSMB.Columns[3].FieldName = "bhd";
                gridViewCCCSMB.Columns[4].Caption = "渗透率";
                gridViewCCCSMB.Columns[4].FieldName = "stl";
            }
            else
            {
                XtraMessageBox.Show("储层参数模板文件格式不正确，请检查后再导入");
            }
        }

        private void radioSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectWellIfmBLL swi = new SelectWellIfmBLL();
            switch (radioSelected.SelectedIndex)
            {
                case 0:
                    splashScreenManager1.ShowWaitForm();
                    splashScreenManager1.SetWaitFormCaption("正在查询信息");
                    splashScreenManager1.SetWaitFormDescription("请稍后...");
                    if (chartControl1.Titles[0].ToString().Equals("日产水与日产油HI图"))
                    {
                        DataTable dt = ComputeHIDAL.dtOilFirstFilterHI1.Copy();
                        dt.Merge(ComputeHIDAL.dtOilFirstFilterHI2);
                        gridControl2.DataSource = swi.GetOilWellIfm(dt, 1);
                    }
                    else if (chartControl1.Titles[0].ToString().Equals("含水与流压HI图"))
                    {
                        gridControl2.DataSource = swi.GetOilWellIfm(ComputeHIDAL.dtOilSecondFilterHI1, 2);
                    }
                    else if (chartControl1.Titles[0].ToString().Equals("注入强度与视吸水指数HI图"))
                    {
                        gridControl2.DataSource = swi.GetWaterWellIfm(ComputeHIDAL.dtWaterFirstFilterHI1);
                    }
                    splashScreenManager1.CloseWaitForm();
                    break;
                case 1:
                    splashScreenManager1.ShowWaitForm();
                    splashScreenManager1.SetWaitFormCaption("正在查询信息");
                    splashScreenManager1.SetWaitFormDescription("请稍后...");
                    if (chartControl1.Titles[0].ToString().Equals("日产水与日产油HI图"))
                    {
                        DataTable dt = ComputeHIDAL.dtOilFirstFilterHI3.Copy();
                        dt.Merge(ComputeHIDAL.dtOilFirstFilterHI4);
                        gridControl2.DataSource = swi.GetOilWellIfm(dt, 1);
                    }
                    else if (chartControl1.Titles[0].ToString().Equals("含水与流压HI图"))
                    {
                        gridControl2.DataSource = swi.GetOilWellIfm(ComputeHIDAL.dtOilSecondFilterHI2, 2);
                    }
                    else if (chartControl1.Titles[0].ToString().Equals("注入强度与视吸水指数HI图"))
                    {
                        gridControl2.DataSource = swi.GetWaterWellIfm(ComputeHIDAL.dtWaterFirstFilterHI2);
                    }
                    splashScreenManager1.CloseWaitForm();
                    break;
                case 2:
                    splashScreenManager1.ShowWaitForm();
                    splashScreenManager1.SetWaitFormCaption("正在查询信息");
                    splashScreenManager1.SetWaitFormDescription("请稍后...");
                    if (chartControl1.Titles[0].ToString().Equals("日产水与日产油HI图"))
                    {
                        DataTable dt = ComputeHIDAL.dtOilFirstFilterHI5.Copy();
                        dt.Merge(ComputeHIDAL.dtOilFirstFilterHI6);
                        gridControl2.DataSource = swi.GetOilWellIfm(dt, 1);
                    }
                    else if (chartControl1.Titles[0].ToString().Equals("含水与流压HI图"))
                    {
                        gridControl2.DataSource = swi.GetOilWellIfm(ComputeHIDAL.dtOilSecondFilterHI3, 2);
                    }
                    else if (chartControl1.Titles[0].ToString().Equals("注入强度与视吸水指数HI图"))
                    {
                        gridControl2.DataSource = swi.GetWaterWellIfm(ComputeHIDAL.dtWaterFirstFilterHI3);
                    }
                    splashScreenManager1.CloseWaitForm();
                    break;
                case 3:
                    splashScreenManager1.ShowWaitForm();
                    splashScreenManager1.SetWaitFormCaption("正在查询信息");
                    splashScreenManager1.SetWaitFormDescription("请稍后...");
                    if (chartControl1.Titles[0].ToString().Equals("日产水与日产油HI图"))
                    {
                        DataTable dt = ComputeHIDAL.dtOilFirstFilterHI7.Copy();
                        dt.Merge(ComputeHIDAL.dtOilFirstFilterHI8);
                        gridControl2.DataSource = swi.GetOilWellIfm(dt, 1);
                    }
                    else if (chartControl1.Titles[0].ToString().Equals("含水与流压HI图"))
                    {
                        gridControl2.DataSource = swi.GetOilWellIfm(ComputeHIDAL.dtOilSecondFilterHI4, 2);
                    }
                    else if (chartControl1.Titles[0].ToString().Equals("注入强度与视吸水指数HI图"))
                    {
                        gridControl2.DataSource = swi.GetWaterWellIfm(ComputeHIDAL.dtWaterFirstFilterHI4);
                    }
                    splashScreenManager1.CloseWaitForm();
                    break;
                default:
                    gridControl2.DataSource = null;
                    break;

            }
            GridColumn gc = gridView2.Columns["井号"];
            if (gc == null)
                return;
            gc.GroupIndex = 0;
            gridView2.GroupFormat = "{0}: [#image]{1}";
            GridGroupSummaryItem item = new GridGroupSummaryItem();
            item.DisplayFormat = "(井号总计: {0})";
            item.SummaryType = DevExpress.Data.SummaryItemType.Count;
            gridView2.GroupSummary.Add(item);

        }

        private void btnDataSelected_Click(object sender, EventArgs e)
        {
            ChooseWaterItem = cmbWaterSelected.SelectedIndex;
            ChooseOilItem = cmbOilSelected.SelectedIndex;
            strTreeNodeText = "层位识别";
            if (ChooseWaterItem == 0)
            {
                TreeNodeMouseClickBLL tnmc = new TreeNodeMouseClickBLL();
                gridViewResult.Columns.Clear();
                dt = tnmc.IdeDxLayer(strTreeNodeText);
                //dt.Columns.Add("", System.Type.GetType("System.Boolean"));
                gridResult.DataSource = dt;
                dtResult = dt.Clone();
                #region gridControl
                gridViewResult.Columns[0].Caption = "油井井号";
                gridViewResult.Columns[0].FieldName = "JHY";
                gridViewResult.Columns[1].Caption = "层位";
                gridViewResult.Columns[1].FieldName = "CW";
                gridViewResult.Columns[2].Caption = "含水";
                gridViewResult.Columns[2].FieldName = "HS";
                gridViewResult.Columns[3].Caption = "产液量";
                gridViewResult.Columns[3].FieldName = "MNCYE";
                gridViewResult.Columns[4].Caption = "产液强度";
                gridViewResult.Columns[4].FieldName = "CYEQD";
                gridViewResult.Columns[5].Caption = "渗透率";
                gridViewResult.Columns[5].FieldName = "YSTL";
                gridViewResult.Columns[6].Caption = "砂岩厚度";
                gridViewResult.Columns[6].FieldName = "YSYHD";
                gridViewResult.Columns[7].Caption = "微相";
                gridViewResult.Columns[7].FieldName = "YWX";
                gridViewResult.Columns[8].Caption = "水井井号";
                gridViewResult.Columns[8].FieldName = "JHS";
                gridViewResult.Columns[9].Caption = "注入强度";
                gridViewResult.Columns[9].FieldName = "ZRQD";
                gridViewResult.Columns[10].Caption = "渗透率";
                gridViewResult.Columns[10].FieldName = "SSTL";
                gridViewResult.Columns[11].Caption = "砂岩厚度";
                gridViewResult.Columns[11].FieldName = "SSYHD";
                gridViewResult.Columns[12].Caption = "微相";
                gridViewResult.Columns[12].FieldName = "SWX";
                gridViewResult.Columns[13].VisibleIndex = -1;
                gridViewResult.Columns[14].VisibleIndex = -1;
                gridViewResult.Columns[15].VisibleIndex = -1;
                gridViewResult.Columns[16].VisibleIndex = -1;
                gridViewResult.Columns[17].VisibleIndex = -1;
                gridViewResult.Columns[18].VisibleIndex = -1;
                gridViewResult.Columns[19].VisibleIndex = -1;
                gridViewResult.Columns[20].VisibleIndex = -1;
                gridViewResult.Columns[21].VisibleIndex = -1;
                gridViewResult.Columns[22].VisibleIndex = -1;
                gridViewResult.Columns[23].VisibleIndex = -1;
                gridViewResult.Columns[24].VisibleIndex = -1;
                gridViewResult.Columns[25].VisibleIndex = -1;
                gridViewResult.Columns[26].VisibleIndex = -1;
                gridViewResult.Columns[27].VisibleIndex = -1;
                gridViewResult.Columns[28].VisibleIndex = -1;
                gridViewResult.Columns[29].VisibleIndex = -1;
                gridViewResult.Columns[30].VisibleIndex = -1;
                gridViewResult.Columns[31].VisibleIndex = -1;
                gridViewResult.Columns[32].VisibleIndex = -1;
                gridViewResult.Columns[33].VisibleIndex = -1;
                #endregion
            }
            else if (ChooseWaterItem == 1)
            {
                FrmTracerSelectedInfo ftc = new FrmTracerSelectedInfo();
                ftc.ShowResultView += GirdViewResultSource;
                ftc.Show();
            }
            else if (ChooseWaterItem == 2)
            {
                FrmLayerSelectedInfo fic = new FrmLayerSelectedInfo();
                fic.ShowResultView += GirdViewResultSource;
                fic.Show();

            }
            else if (ChooseWaterItem == 3)
            {
                FrmLayerSelectedInfo fic = new FrmLayerSelectedInfo();
                fic.ShowResultView += GirdViewResultSource;
                fic.Show();

            }
            else
            {
                DxxhcwsbDAL dd = new DxxhcwsbDAL();
                gridResult.DataSource = dd.DxxhcesbMethod();
            }
        }

        public void GirdViewResultSource(string selectedNodeName)
        {
            layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            TreeNodeMouseClickBLL tnmc = new TreeNodeMouseClickBLL();
            gridViewResult.Columns.Clear();
            dt = tnmc.IdeDxLayer(selectedNodeName);
            //dt.Columns.Add("", System.Type.GetType("System.Boolean"));
            gridResult.DataSource = dt;
            dtResult = dt.Clone();
            #region gridViewResult
            gridViewResult.Columns[0].Caption = "油井井号";
            gridViewResult.Columns[0].FieldName = "SJJHY";
            gridViewResult.Columns[2].Caption = "层位";
            gridViewResult.Columns[2].FieldName = "CW";
            gridViewResult.Columns[3].Caption = "含水";
            gridViewResult.Columns[3].FieldName = "HS";
            gridViewResult.Columns[4].Caption = "产液量";
            gridViewResult.Columns[4].FieldName = "MNCYE";
            gridViewResult.Columns[5].Caption = "产液强度";
            gridViewResult.Columns[5].FieldName = "CYEQD";
            gridViewResult.Columns[6].Caption = "渗透率";
            gridViewResult.Columns[6].FieldName = "YSTL";
            gridViewResult.Columns[7].Caption = "砂岩厚度";
            gridViewResult.Columns[7].FieldName = "YSYHD";
            gridViewResult.Columns[8].Caption = "微相";
            gridViewResult.Columns[8].FieldName = "YWX";
            gridViewResult.Columns[9].Caption = "水井井号";
            gridViewResult.Columns[9].FieldName = "SJJHS";
            gridViewResult.Columns[11].Caption = "注入强度";
            gridViewResult.Columns[11].FieldName = "ZRQD";
            gridViewResult.Columns[12].Caption = "渗透率";
            gridViewResult.Columns[12].FieldName = "SSTL";
            gridViewResult.Columns[13].Caption = "砂岩厚度";
            gridViewResult.Columns[13].FieldName = "SSYHD";
            gridViewResult.Columns[14].Caption = "微相";
            gridViewResult.Columns[14].FieldName = "SWX";
            gridViewResult.Columns[1].VisibleIndex = -1;
            gridViewResult.Columns[10].VisibleIndex = -1;
            gridViewResult.Columns[15].VisibleIndex = -1;
            gridViewResult.Columns[16].VisibleIndex = -1;
            gridViewResult.Columns[17].VisibleIndex = -1;
            gridViewResult.Columns[18].VisibleIndex = -1;
            gridViewResult.Columns[19].VisibleIndex = -1;
            gridViewResult.Columns[20].VisibleIndex = -1;
            gridViewResult.Columns[21].VisibleIndex = -1;
            gridViewResult.Columns[22].VisibleIndex = -1;
            gridViewResult.Columns[23].VisibleIndex = -1;
            gridViewResult.Columns[24].VisibleIndex = -1;
            gridViewResult.Columns[25].VisibleIndex = -1;
            gridViewResult.Columns[26].VisibleIndex = -1;
            gridViewResult.Columns[27].VisibleIndex = -1;
            gridViewResult.Columns[28].VisibleIndex = -1;
            gridViewResult.Columns[29].VisibleIndex = -1;
            gridViewResult.Columns[30].VisibleIndex = -1;
            gridViewResult.Columns[31].VisibleIndex = -1;
            gridViewResult.Columns[32].VisibleIndex = -1;
            gridViewResult.Columns[33].VisibleIndex = -1;
            gridViewResult.Columns[34].VisibleIndex = -1;
            //gridViewResult.Columns[33].VisibleIndex = -1;
            gridViewResult.Columns[35].Caption = "关联系数";
            gridViewResult.Columns[35].FieldName = "GLXS";
            #endregion
        }

        private void btnDate_Click(object sender, EventArgs e)
        {
            layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            dockPanelDate.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            radioSelected.SelectedIndex = -1;
            layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

            if (cmbStartDate.Text.CompareTo(cmbEndDate.Text) <= 0)
            {
                TreeNodeMouseClickBLL tnmc = new TreeNodeMouseClickBLL();


                if (tnp.Text.Equals("低效循环油井识别") || strTnpText.Equals("低效循环油井识别"))
                {
                    strTnpText = "";
                    if (tn.Text.Equals("一次筛选") || strTnText.Equals("油井一次筛选"))
                    {
                        strTnText = "";
                        splashScreenManager1.ShowWaitForm();
                        splashScreenManager1.SetWaitFormCaption("正在计算");
                        splashScreenManager1.SetWaitFormDescription("请稍后...");

                        gridView2.Columns.Clear();
                        chartControl1.Series.Clear();
                        tnmc.ShowHIImg("低效循环油井识别", "一次筛选");

                        #region Title
                        chartControl1.Titles.Clear();
                        ChartTitle titles = new ChartTitle();
                        titles.Text = "日产水与日产油HI图";
                        titles.TextColor = System.Drawing.Color.Black;
                        titles.Indent = 1; //设置距离  值越小柱状图就越大
                        titles.Font = new Font("黑体", 15, FontStyle.Bold);  //设置字体
                        titles.Dock = ChartTitleDockStyle.Bottom;//设置对齐方式
                        titles.Indent = 0;
                        titles.Alignment = StringAlignment.Center;//居中对齐
                        chartControl1.Titles.Add(titles);
                        #endregion

                        #region Series
                        if (ComputeHIDAL.dtOilFirstFilterHI1.Rows.Count > 0)
                        {
                            Series sr1 = new Series("高产水高产油高注入量", ViewType.Point);
                            sr1.ArgumentScaleType = ScaleType.Numerical;
                            sr1.DataSource = ComputeHIDAL.dtOilFirstFilterHI1.DefaultView;
                            sr1.ArgumentDataMember = "RCSLHI";
                            sr1.ValueDataMembers[0] = "RCYLHI";
                            sr1.View.Color = Color.Red;//颜色
                            this.chartControl1.Series.Add(sr1);
                        }
                        if (ComputeHIDAL.dtOilFirstFilterHI2.Rows.Count > 0)
                        {
                            Series sr2 = new Series("高产水高产油低注入量", ViewType.Point);
                            sr2.ArgumentScaleType = ScaleType.Numerical;
                            sr2.DataSource = ComputeHIDAL.dtOilFirstFilterHI2.DefaultView;
                            sr2.ArgumentDataMember = "RCSLHI";
                            sr2.ValueDataMembers[0] = "RCYLHI";
                            sr2.View.Color = Color.DarkRed;//颜色
                            this.chartControl1.Series.Add(sr2);
                        }
                        if (ComputeHIDAL.dtOilFirstFilterHI3.Rows.Count > 0)
                        {
                            Series sr3 = new Series("低产水高产油高注入量", ViewType.Point);
                            sr3.ArgumentScaleType = ScaleType.Numerical;
                            sr3.DataSource = ComputeHIDAL.dtOilFirstFilterHI3.DefaultView;
                            sr3.ArgumentDataMember = "RCSLHI";
                            sr3.ValueDataMembers[0] = "RCYLHI";
                            sr3.View.Color = Color.Blue;//颜色
                            this.chartControl1.Series.Add(sr3);
                        }
                        if (ComputeHIDAL.dtOilFirstFilterHI4.Rows.Count > 0)
                        {
                            Series sr4 = new Series("低产水高产油低注入量", ViewType.Point);
                            sr4.ArgumentScaleType = ScaleType.Numerical;
                            sr4.DataSource = ComputeHIDAL.dtOilFirstFilterHI4.DefaultView;
                            sr4.ArgumentDataMember = "RCSLHI";
                            sr4.ValueDataMembers[0] = "RCYLHI";
                            sr4.View.Color = Color.DeepSkyBlue;//颜色
                            this.chartControl1.Series.Add(sr4);
                        }
                        if (ComputeHIDAL.dtOilFirstFilterHI5.Rows.Count > 0)
                        {
                            Series sr5 = new Series("低产水低产油高注入量", ViewType.Point);
                            sr5.ArgumentScaleType = ScaleType.Numerical;
                            sr5.DataSource = ComputeHIDAL.dtOilFirstFilterHI5.DefaultView;
                            sr5.ArgumentDataMember = "RCSLHI";
                            sr5.ValueDataMembers[0] = "RCYLHI";
                            sr5.View.Color = Color.OrangeRed;//颜色
                            this.chartControl1.Series.Add(sr5);
                        }
                        if (ComputeHIDAL.dtOilFirstFilterHI6.Rows.Count > 0)
                        {
                            Series sr6 = new Series("低产水低产油低注入量", ViewType.Point);
                            sr6.ArgumentScaleType = ScaleType.Numerical;
                            sr6.DataSource = ComputeHIDAL.dtOilFirstFilterHI6.DefaultView;
                            sr6.ArgumentDataMember = "RCSLHI";
                            sr6.ValueDataMembers[0] = "RCYLHI";
                            sr6.View.Color = Color.DarkOrange;//颜色
                            this.chartControl1.Series.Add(sr6);
                        }
                        if (ComputeHIDAL.dtOilFirstFilterHI7.Rows.Count > 0)
                        {
                            Series sr7 = new Series("高产水低产油高注入量", ViewType.Point);
                            sr7.ArgumentScaleType = ScaleType.Numerical;
                            sr7.DataSource = ComputeHIDAL.dtOilFirstFilterHI7.DefaultView;
                            sr7.ArgumentDataMember = "RCSLHI";
                            sr7.ValueDataMembers[0] = "RCYLHI";
                            sr7.View.Color = Color.Brown;//颜色
                            this.chartControl1.Series.Add(sr7);
                        }
                        if (ComputeHIDAL.dtOilFirstFilterHI8.Rows.Count > 0)
                        {
                            Series sr8 = new Series("高产水低产油低注入量", ViewType.Point);
                            sr8.ArgumentScaleType = ScaleType.Numerical;
                            sr8.DataSource = ComputeHIDAL.dtOilFirstFilterHI8.DefaultView;
                            sr8.ArgumentDataMember = "RCSLHI";
                            sr8.ValueDataMembers[0] = "RCYLHI";
                            sr8.View.Color = Color.DarkGray;//颜色
                            this.chartControl1.Series.Add(sr8);
                        }

                        #endregion

                        #region XYDiagram
                        if (chartControl1.Series.Count() > 0)
                        {
                            XYDiagram diagram1 = (XYDiagram)chartControl1.Diagram;
                            diagram1.DefaultPane.BorderVisible = false;
                            diagram1.AxisX.Title.Text = "日产水HI";
                            diagram1.AxisY.Title.Text = "日产油HI";
                            diagram1.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.Default;
                            diagram1.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.Default;
                            diagram1.AxisX.Title.Alignment = StringAlignment.Far;
                            diagram1.AxisY.Title.Alignment = StringAlignment.Far;
                            diagram1.AxisX.Title.Font = new Font(new FontFamily("黑体"), 11);
                            diagram1.AxisY.Title.Font = new Font(new FontFamily("黑体"), 11);
                            diagram1.AxisX.Title.Alignment = StringAlignment.Far;
                            diagram1.AxisY.Title.Alignment = StringAlignment.Far;
                            diagram1.AxisX.Alignment = AxisAlignment.Zero;
                            diagram1.AxisY.Alignment = AxisAlignment.Zero;
                            diagram1.AxisY.GridLines.Visible = false;
                            diagram1.EnableAxisXScrolling = true;
                            diagram1.EnableAxisXZooming = true;
                            diagram1.EnableAxisYScrolling = true;
                            diagram1.EnableAxisYZooming = true;
                        }
                        else
                        {
                            XtraMessageBox.Show("当前日期下油井列表为空");
                        }
                        #endregion
                        splashScreenManager1.CloseWaitForm();
                        layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                    if (tn.Text.Equals("二次筛选") || strTnText.Equals("二次筛选"))
                    {
                        strTnText = "";
                        gridView2.Columns.Clear();
                        chartControl1.Series.Clear();
                        if (ComputeHIDAL.dtOilHI.Rows.Count > 0)
                        {

                            tnmc.ShowHIImg("低效循环油井识别", "二次筛选");
                            #region Title
                            chartControl1.Titles.Clear();
                            ChartTitle titles = new ChartTitle();
                            titles.Text = "含水与流压HI图";
                            titles.TextColor = System.Drawing.Color.Black;
                            titles.Indent = 1; //设置距离  值越小柱状图就越大
                            titles.Font = new Font("黑体", 15, FontStyle.Bold);  //设置字体
                            titles.Dock = ChartTitleDockStyle.Bottom;//设置对齐方式
                            titles.Indent = 0;
                            titles.Alignment = StringAlignment.Center;//居中对齐
                            chartControl1.Titles.Add(titles);
                            #endregion

                            #region Series
                            if (ComputeHIDAL.dtOilSecondFilterHI1.Rows.Count > 0)
                            {
                                Series sr1 = new Series("高含水高流压", ViewType.Point);
                                sr1.ArgumentScaleType = ScaleType.Numerical;
                                sr1.DataSource = ComputeHIDAL.dtOilSecondFilterHI1.DefaultView;
                                sr1.ArgumentDataMember = "HSHI";
                                sr1.ValueDataMembers[0] = "LYHI";
                                sr1.View.Color = Color.Red;//颜色
                                this.chartControl1.Series.Add(sr1);
                            }
                            if (ComputeHIDAL.dtOilSecondFilterHI2.Rows.Count > 0)
                            {
                                Series sr2 = new Series("低含水高流压", ViewType.Point);
                                sr2.ArgumentScaleType = ScaleType.Numerical;
                                sr2.DataSource = ComputeHIDAL.dtOilSecondFilterHI2.DefaultView;
                                sr2.ArgumentDataMember = "HSHI";
                                sr2.ValueDataMembers[0] = "LYHI";
                                sr2.View.Color = Color.DarkRed;//颜色
                                this.chartControl1.Series.Add(sr2);
                            }
                            if (ComputeHIDAL.dtOilSecondFilterHI3.Rows.Count > 0)
                            {
                                Series sr3 = new Series("低含水低流压", ViewType.Point);
                                sr3.ArgumentScaleType = ScaleType.Numerical;
                                sr3.DataSource = ComputeHIDAL.dtOilSecondFilterHI3.DefaultView;
                                sr3.ArgumentDataMember = "HSHI";
                                sr3.ValueDataMembers[0] = "LYHI";
                                sr3.View.Color = Color.Blue;//颜色
                                this.chartControl1.Series.Add(sr3);
                            }
                            if (ComputeHIDAL.dtOilSecondFilterHI4.Rows.Count > 0)
                            {
                                Series sr4 = new Series("高含水低流压", ViewType.Point);
                                sr4.ArgumentScaleType = ScaleType.Numerical;
                                sr4.DataSource = ComputeHIDAL.dtOilSecondFilterHI4.DefaultView;
                                sr4.ArgumentDataMember = "HSHI";
                                sr4.ValueDataMembers[0] = "LYHI";
                                sr4.View.Color = Color.DeepSkyBlue;//颜色
                                this.chartControl1.Series.Add(sr4);
                            }


                            #endregion
                            #region XYDiagram
                            XYDiagram diagram1 = (XYDiagram)chartControl1.Diagram;
                            diagram1.DefaultPane.BorderVisible = false;
                            diagram1.AxisX.Title.Text = "含水HI";
                            diagram1.AxisY.Title.Text = "流压HI";
                            diagram1.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.Default;
                            diagram1.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.Default;
                            diagram1.AxisX.Title.Alignment = StringAlignment.Far;
                            diagram1.AxisY.Title.Alignment = StringAlignment.Far;
                            diagram1.AxisX.Title.Font = new Font(new FontFamily("黑体"), 11);
                            diagram1.AxisY.Title.Font = new Font(new FontFamily("黑体"), 11);
                            diagram1.AxisX.Title.Alignment = StringAlignment.Far;
                            diagram1.AxisY.Title.Alignment = StringAlignment.Far;
                            diagram1.AxisX.Alignment = AxisAlignment.Zero;
                            diagram1.AxisY.Alignment = AxisAlignment.Zero;
                            diagram1.AxisY.GridLines.Visible = false;
                            diagram1.EnableAxisXScrolling = true;
                            diagram1.EnableAxisXZooming = true;
                            diagram1.EnableAxisYScrolling = true;
                            diagram1.EnableAxisYZooming = true;
                            #endregion
                            layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        }
                        else
                        {
                            XtraMessageBox.Show("请先进行油井的第一次筛选");
                        }
                    }
                }
                else if (tnp.Text.Equals("低效循环水井识别") || strTnpText.Equals("低效循环水井识别"))
                {
                    strTnpText = "";
                    layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dockPanelDate.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
                    dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;

                    radioSelected.SelectedIndex = -1;

                    if (tn.Text.Equals("一次筛选") || strTnText.Equals("水井一次筛选"))
                    {
                        strTnText = "";
                        layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        gridView2.Columns.Clear();
                        chartControl1.Series.Clear();
                        tnmc.ShowHIImg("低效循环水井识别", "一次筛选");

                        #region Title
                        chartControl1.Titles.Clear();
                        ChartTitle titles = new ChartTitle();
                        titles.Text = "注入强度与视吸水指数HI图";
                        titles.TextColor = System.Drawing.Color.Black;
                        titles.Indent = 1; //设置距离  值越小柱状图就越大
                        titles.Font = new Font("黑体", 15, FontStyle.Bold);  //设置字体
                        titles.Dock = ChartTitleDockStyle.Bottom;//设置对齐方式
                        titles.Indent = 0;
                        titles.Alignment = StringAlignment.Center;//居中对齐
                        chartControl1.Titles.Add(titles);
                        #endregion

                        #region Series
                        if (ComputeHIDAL.dtWaterFirstFilterHI1.Rows.Count > 0)
                        {
                            Series sr1 = new Series("高注入量高视吸水指数", ViewType.Point);
                            sr1.ArgumentScaleType = ScaleType.Numerical;
                            sr1.DataSource = ComputeHIDAL.dtWaterFirstFilterHI1.DefaultView;
                            sr1.ArgumentDataMember = "ZRQDHI";
                            sr1.ValueDataMembers[0] = "SXSZSHI";
                            sr1.View.Color = Color.Red;//颜色
                            this.chartControl1.Series.Add(sr1);
                        }
                        if (ComputeHIDAL.dtWaterFirstFilterHI2.Rows.Count > 0)
                        {
                            Series sr2 = new Series("低注入量高视吸水指数", ViewType.Point);
                            sr2.ArgumentScaleType = ScaleType.Numerical;
                            sr2.DataSource = ComputeHIDAL.dtWaterFirstFilterHI2.DefaultView;
                            sr2.ArgumentDataMember = "ZRQDHI";
                            sr2.ValueDataMembers[0] = "SXSZSHI";
                            sr2.View.Color = Color.DarkRed;//颜色
                            this.chartControl1.Series.Add(sr2);
                        }
                        if (ComputeHIDAL.dtWaterFirstFilterHI3.Rows.Count > 0)
                        {
                            Series sr3 = new Series("低注入量低视吸水指数", ViewType.Point);
                            sr3.ArgumentScaleType = ScaleType.Numerical;
                            sr3.DataSource = ComputeHIDAL.dtWaterFirstFilterHI3.DefaultView;
                            sr3.ArgumentDataMember = "ZRQDHI";
                            sr3.ValueDataMembers[0] = "SXSZSHI";
                            sr3.View.Color = Color.Blue;//颜色
                            this.chartControl1.Series.Add(sr3);
                        }
                        if (ComputeHIDAL.dtWaterFirstFilterHI4.Rows.Count > 0)
                        {
                            Series sr4 = new Series("高注入量低视吸水指数", ViewType.Point);
                            sr4.ArgumentScaleType = ScaleType.Numerical;
                            sr4.DataSource = ComputeHIDAL.dtWaterFirstFilterHI4.DefaultView;
                            sr4.ArgumentDataMember = "ZRQDHI";
                            sr4.ValueDataMembers[0] = "SXSZSHI";
                            sr4.View.Color = Color.DeepSkyBlue;//颜色
                            this.chartControl1.Series.Add(sr4);
                        }

                        #endregion
                        #region XYDiagram
                        XYDiagram diagram1 = (XYDiagram)chartControl1.Diagram;
                        diagram1.DefaultPane.BorderVisible = false;
                        diagram1.AxisX.Title.Text = "注入强度HI";
                        diagram1.AxisY.Title.Text = "视吸水指数HI";
                        diagram1.AxisX.Title.Visibility = DevExpress.Utils.DefaultBoolean.Default;
                        diagram1.AxisY.Title.Visibility = DevExpress.Utils.DefaultBoolean.Default;
                        diagram1.AxisX.Title.Alignment = StringAlignment.Far;
                        diagram1.AxisY.Title.Alignment = StringAlignment.Far;
                        diagram1.AxisX.Title.Font = new Font(new FontFamily("黑体"), 11);
                        diagram1.AxisY.Title.Font = new Font(new FontFamily("黑体"), 11);
                        diagram1.AxisX.Title.Alignment = StringAlignment.Far;
                        diagram1.AxisY.Title.Alignment = StringAlignment.Far;
                        diagram1.AxisX.Alignment = AxisAlignment.Zero;
                        diagram1.AxisY.Alignment = AxisAlignment.Zero;
                        diagram1.AxisY.GridLines.Visible = false;
                        diagram1.EnableAxisXScrolling = true;
                        diagram1.EnableAxisXZooming = true;
                        diagram1.EnableAxisYScrolling = true;
                        diagram1.EnableAxisYZooming = true;
                        #endregion

                    }
                }
            }
            else
            {
                XtraMessageBox.Show("起始年月大于终止年月，请重新选择");
            }
        }
        private void cmbWaterSelected_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbOilSelected.Properties.Items.Clear();
            switch (cmbWaterSelected.SelectedIndex)
            {
                case 0:
                    string[] strType = { "DCB01", "示踪剂", "数值模拟分层数据", "静态数据" };
                    for (int i = 0; i < strType.Count(); i++)
                    {
                        cmbOilSelected.Properties.Items.Add(strType[i]);
                    }
                    break;
                case 1:
                    //strType = { "DCB01", "示踪剂", "数值模拟分层数据", "静态数据" };
                    cmbOilSelected.Properties.Items.Add("示踪剂");
                    cmbOilSelected.SelectedIndex = 0;
                    break;
                case 2:
                    cmbOilSelected.Properties.Items.Add("数值模拟分层数据");
                    cmbOilSelected.SelectedIndex = 0;
                    break;
                case 3:
                    cmbOilSelected.Properties.Items.Add("静态数据");
                    cmbOilSelected.SelectedIndex = 0;
                    break;
            }
        }

        private void gridResult_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                System.Drawing.Point point = new System.Drawing.Point(Control.MousePosition.X, Control.MousePosition.Y); //右键菜单弹出的位置
                PopupMenu popupMenu = new PopupMenu();

                popupMenu.AddItems(new BarItem[] { barBtnResult, barBtnPrint, barBtnExport });
                popupMenu.ShowPopup(barManager1, point);
            }
        }



        private void barBtnClose_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Close();
        }

        private void barBtnDataList_ItemClick(object sender, ItemClickEventArgs e)
        {
            layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            dockPanelDate.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            radioSelected.SelectedIndex = -1;
            if (ComputeHIDAL.dtOilHI.Rows.Count > 0 && ComputeHIDAL.dtWaterHI.Rows.Count > 0)
            {
                layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                gridViewDXSJ.Columns.Clear();
                gridViewDXYJ.Columns.Clear();
                gridDXYJ.DataSource = ComputeHIDAL.DXOilMethod();
                gridViewDXYJ.Columns[0].Caption = "低效油井:  共" + gridViewDXYJ.RowCount + "口";
                gridViewDXYJ.Columns[0].FieldName = "JH";
                gridDXSJ.DataSource = ComputeHIDAL.DXWaterMethod();
                gridViewDXSJ.Columns[0].Caption = "低效水井:  共" + gridViewDXSJ.RowCount + "口";
                gridViewDXSJ.Columns[0].FieldName = "JH";
            }
            else
            {

                XtraMessageBox.Show("请先进行油水井筛选");
            }
        }

        private void barBtnAreaSelected_ItemClick(object sender, ItemClickEventArgs e)
        {
            FrmAreaSelected fas = new FrmAreaSelected();
            fas.Show();
        }

        private void barBtnResult_ItemClick(object sender, ItemClickEventArgs e)
        {

            DataRow dr = gridViewResult.GetFocusedDataRow();



            if (ChooseWaterItem == 2)
            {
                DataRow[] drOil = GetLayer.dtOil.Select("JHY = '" + dr["JHY"] + "'");
                if (drOil.Count() > 0)
                {
                    dtResult.Clear();
                    DataTable dtDJY = drOil.CopyToDataTable();
                    dtResult = DxxhcwsbDAL.TracerOilGetLayer(dr["JHY"].ToString(), dtDJY);//计算数模情况下
                }
            }
            if (ChooseWaterItem == 1)
            {
                DataRow[] drOil = GetTracerDAL.dtOil.Select("JHY = '" + dr["JHY"] + "'");
                if (drOil.Count() > 0)
                {
                    dtResult.Clear();
                    DataTable dtDJY = drOil.CopyToDataTable();
                    dtResult = DxxhcwsbDAL.TracerOilGetLayer(dr["JHY"].ToString(), dtDJY);//计算数模情况下
                }
            }
            //dtResult.Rows.Add(dr.ItemArray);

            FrmDrawCYPM fdr = new FrmDrawCYPM();
            fdr.Show();
        }

        private void barBtnExport_ItemClick(object sender, ItemClickEventArgs e)
        {
            Point p = Cursor.Position;
            h = WindowFromPoint(p);
            EnumControls(this);

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "导出Excel";
            fileDialog.Filter = "Excel文件(*.xls)|*.xls";
            DialogResult dialogResult = fileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions();
                (currentControl as GridControl).ExportToXls(fileDialog.FileName);
                DevExpress.XtraEditors.XtraMessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        void EnumControls(Control container)
        {
            foreach (Control c in container.Controls)
            {
                if (c.Handle == h)
                {
                    currentControl = c;
                    break;
                }
                //c is the child control here
                EnumControls(c);
            }
        }

        private void barBtnPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            Point p = Cursor.Position;
            h = WindowFromPoint(p);
            EnumControls(this);

            PrintingSystem print = new DevExpress.XtraPrinting.PrintingSystem();
            PrintableComponentLink link = new PrintableComponentLink(print);
            print.Links.Add(link);
            link.Component = (currentControl as GridControl);//这里可以是可打印的部件
            PageHeaderFooter phf = link.PageHeaderFooter as PageHeaderFooter;
            phf.Header.Content.Clear();
            phf.Header.Content.AddRange(new string[] { "", "", "" });
            phf.Header.Font = new System.Drawing.Font("宋体", 14, System.Drawing.FontStyle.Bold);
            phf.Header.LineAlignment = BrickAlignment.Center;
            link.CreateDocument(); //建立文档
            print.PreviewFormEx.Show();//进行预览

        }

        private void gridDatabs_MouseClick(object sender, MouseEventArgs e)
        {
            GridMouseClick(sender, e);
        }

        private void gridControl2_MouseClick(object sender, MouseEventArgs e)
        {
            GridMouseClick(sender, e);
        }

        private void gridDXYJ_MouseClick(object sender, MouseEventArgs e)
        {
            GridMouseClick(sender, e);
        }

        private void gridDXSJ_MouseClick(object sender, MouseEventArgs e)
        {
            GridMouseClick(sender, e);
        }
        private void GridMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                System.Drawing.Point point = new System.Drawing.Point(Control.MousePosition.X, Control.MousePosition.Y); //右键菜单弹出的位置
                PopupMenu popupMenu = new PopupMenu();

                popupMenu.AddItems(new BarItem[] { barBtnPrint, barBtnExport });
                popupMenu.ShowPopup(barManager1, point);
            }
        }

        #region RibbonItemClick
        private void barBtnOilFirstSelected_ItemClick(object sender, ItemClickEventArgs e)
        {
            layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            dockPanelDate.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            radioSelected.SelectedIndex = -1;
            layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            strTnpText = "低效循环油井识别";
            strTnText = "油井一次筛选";


        }

        private void barBtnOilSecondSelected_ItemClick(object sender, ItemClickEventArgs e)
        {
            layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            dockPanelDate.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            radioSelected.SelectedIndex = -1;
            layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (ComputeHIDAL.dtOilHI.Rows.Count > 0)
            {
                strTnpText = "低效循环油井识别";
                strTnText = "二次筛选";
            }
            else
            {
                XtraMessageBox.Show("请先进行油井的第一次筛选");
            }
        }

        private void barBtnWaterFirstSelected_ItemClick(object sender, ItemClickEventArgs e)
        {
            layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            dockPanelDate.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            radioSelected.SelectedIndex = -1;
            strTnpText = "低效循环水井识别";
            strTnText = "水井一次筛选";
        }

        private void barBtnJH_ItemClick(object sender, ItemClickEventArgs e)
        {
            BarBtnClick(barBtnJH.Caption);
        }

        private void barBtnZB_ItemClick(object sender, ItemClickEventArgs e)
        {
            BarBtnClick(barBtnZB.Caption);
        }

        private void barBtnBZXC_ItemClick(object sender, ItemClickEventArgs e)
        {
            BarBtnClick(barBtnBZXC.Caption);
        }

        private void barBtnLTK_ItemClick(object sender, ItemClickEventArgs e)
        {
            BarBtnClick(barBtnLTK.Caption);
        }

        private void barBtnYJS_ItemClick(object sender, ItemClickEventArgs e)
        {
            BarBtnClick(barBtnYJS.Caption);
        }

        private void barBtnSJS_ItemClick(object sender, ItemClickEventArgs e)
        {
            BarBtnClick(barBtnSJS.Caption);
        }

        private void barBtnCYPM_ItemClick(object sender, ItemClickEventArgs e)
        {
            BarBtnClick(barBtnCYPM.Caption);
        }

        private void barBtnXSPM_ItemClick(object sender, ItemClickEventArgs e)
        {
            BarBtnClick(barBtnXSPM.Caption);
        }

        private void barBtnXZ_ItemClick(object sender, ItemClickEventArgs e)
        {
            layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
        }

        private void barBtnCK_ItemClick(object sender, ItemClickEventArgs e)
        {
            BarBtnClick("射孔库");
        }


        private void BarBtnClick(string strItemName)
        {
            layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            dockPanelDate.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;

            gridView1.Columns.Clear();
            gridDatabs.DataSource = tnmc.ShowDataTable(strItemName);
        }
        private void barBtnCWSB_ItemClick(object sender, ItemClickEventArgs e)
        {
            layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            dockPanelDate.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;

            gridView1.Columns.Clear();
            gridResult.Visible = true;
            if (ComputeHIDAL.dtOilHI.Rows.Count > 0 && ComputeHIDAL.dtWaterHI.Rows.Count > 0)
            {
                layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Visible;
            }
            else
            {
                dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
                layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                XtraMessageBox.Show("请先进行油水井筛选");
            }
        }


        #endregion

        private void barBtnShowLayer_ItemClick(object sender, ItemClickEventArgs e)
        {
            dockPanelSelectedData.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            dockPanelDate.Visibility = DevExpress.XtraBars.Docking.DockVisibility.Hidden;
            layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCWSB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDatabs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutSplitImgSubs.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutCCCSMB.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            layoutDXYSJ.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            if (ComputeHIDAL.dtOilHI.Rows.Count > 0 && ComputeHIDAL.dtWaterHI.Rows.Count > 0)
            {
                if (dt.Rows.Count > 0)
                {
                    layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    gridViewLayer.Columns.Clear();
                    gridLayer.DataSource = dt.DefaultView.ToTable(true, new string[] { "CW", "K" });
                    gridViewLayer.Columns[0].Caption = "层位";
                    gridViewLayer.Columns[0].FieldName = "CW";
                    gridViewLayer.Columns[1].VisibleIndex = -1;
                }
            }
            else
            {
                layoutShowLayer.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                XtraMessageBox.Show("请先进行油水井筛选");
            }


        }

        private void gridViewLayer_Click(object sender, EventArgs e)
        {

            DataTable dtYSJ = dt.Copy();
            dtYSJ.Columns.Add("X1", System.Type.GetType("System.Int32"));
            dtYSJ.Columns.Add("Y1", System.Type.GetType("System.Int32"));
            DataRow dr = gridViewLayer.GetFocusedDataRow();
            DataRow[] drAll = dtYSJ.Select("CW = '" + dr["CW"] + "'");
            foreach (DataRow drDJ in drAll)
            {
                DataRow[] drTemp = GetLayer.dtOil.Select("JHY = '" + drDJ["JHY"] + "' and k = " + dr["K"]);

                drDJ["X"] = Convert.ToInt32(drTemp[0]["X"]);
                drDJ["Y"] = Convert.ToInt32(drTemp[0]["Y"]);
                drTemp = GetLayer.dtWater.Select("JHS = '" + drDJ["JHS"] + "' and k = " + dr["K"]);
                drDJ["X1"] = Convert.ToInt32(drTemp[0]["X"]);
                drDJ["Y1"] = Convert.ToInt32(drTemp[0]["Y"]);
            }
            Draw(Convert.ToInt32(dr["K"]), drAll.CopyToDataTable());

        }

        private void Draw(int k, DataTable dtYSJ)
        {
            Bitmap img = new Bitmap(WIDTH, HEIGHT);

            Graphics gs = Graphics.FromImage(img);
            List<decimal> lst = new List<decimal>();
            int times = 1;
            myBmp = (Bitmap)img;
            if (File.Exists("img.emf"))
            {
                //mf.Dispose();
                File.Delete("img.emf");
            }
            //gs.ReleaseHdc();
            Metafile mf = new Metafile("img.emf", gs.GetHdc());

            Graphics g = Graphics.FromImage(mf);
            Pen p = null;


            int start = (k - 1) * WIDTH * HEIGHT;
            int end = (k) * WIDTH * HEIGHT;

            //lst = ReadFile.lstPermx.Skip(start).Take(175 * 70).ToList();
            lst = ReadFileDAL.lstPermx.GetRange(start, WIDTH * HEIGHT);
            int diffColor = 1020;//255*4 = 1020; 起始 红255 随后绿0++255，红255--0，蓝0++255，绿255--0
            decimal max = lst.Max();
            decimal min = lst.Min();
            Color c = new Color();

            while (start < end)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    for (int x = 0; x < WIDTH; x++)
                    {
                        if (Convert.ToDouble(ReadFileDAL.lstPermx[start]) >= 0.1)
                        {
                            double dblRatio = Convert.ToDouble((ReadFileDAL.lstPermx[start] - min) / (max - min));
                            //int intArgb = blue + (int)(dblRatio * diffColor);
                            int intArgb = (int)(dblRatio * diffColor) + 255;
                            int n = (int)Math.Floor(Convert.ToDecimal(intArgb / 255));//向下取整
                            switch (n)
                            {
                                case 1:
                                    c = Color.FromArgb(0, intArgb - 255, 255);
                                    break;
                                case 2:
                                    c = Color.FromArgb(0, 255, 255 * 3 - intArgb);
                                    break;
                                case 3:
                                    c = Color.FromArgb(intArgb - 255 * 3, 255, 0);
                                    break;
                                case 4:
                                    c = Color.FromArgb(255, 255 * 5 - intArgb, 0);
                                    break;
                                case 5:
                                    c = Color.FromArgb(255, 0, 0);
                                    break;
                            }

                            //p = new Pen(c, times);
                            //Rectangle r = new Rectangle(x * times, y * times, times, times);

                            //g.DrawRectangle(p, r);

                            //g.DrawRectangle(p, x, y, 1, 1);
                        }
                        else
                        {
                            c = Color.White;

                        }
                        p = new Pen(c, times);
                        Rectangle r = new Rectangle(x * times, y * times, times, times);

                        g.DrawRectangle(p, r);
                        start++;
                    }
                }

            }

            DataTable dtJHS = dtYSJ.DefaultView.ToTable(true, "JHS");
            for (int i = 0; i < dtJHS.Rows.Count; i++)
            {
                DataTable dtLTJHY = dtYSJ.Select("JHS = '" + dtJHS.Rows[i]["JHS"] + "'").CopyToDataTable();
                float flMaxHS = float.Parse(dtLTJHY.Compute("max(HS)", "").ToString());
                float flMinHS = float.Parse(dtLTJHY.Compute("min(HS)", "").ToString());
                float flMaxCYE = float.Parse(dtLTJHY.Compute("max(MNCYE)", "").ToString());
                float flMinCYE = float.Parse(dtLTJHY.Compute("min(MNCYE)", "").ToString());
                float ratio = 0;
                int cyRatio = 0;
                int penWidth = 1;
                Color nc = new Color();
                foreach (DataRow dr in dtLTJHY.Rows)
                {
                    c = GetPixelColor(Convert.ToInt32(dr["X1"]) * times, Convert.ToInt32(dr["Y1"]) * times);
                    if ((flMaxHS == flMinHS))
                    {
                        ratio = 1;

                    }
                    else
                    {
                        ratio = (float.Parse((dr["HS"]).ToString()) - flMinHS) / (flMaxHS - flMinHS);
                    }
                    if (flMaxCYE == flMinCYE)
                    {
                        nc = Color.DarkViolet;
                        // nc = ReColor(c);
                    }
                    else
                    {
                        cyRatio = Convert.ToInt32((float.Parse((dr["MNCYE"]).ToString()) - flMinCYE) / (flMaxCYE - flMinCYE));
                        nc = Color.FromArgb((255 - 107 * cyRatio), 0, 211);

                        // nc = ReColor(c);
                    }
                    p = new Pen(nc, ratio * penWidth + 1);
                    System.Drawing.Drawing2D.AdjustableArrowCap lineArrow = new System.Drawing.Drawing2D.AdjustableArrowCap(ratio * penWidth + 1, ratio * penWidth + 1, true);
                    p.CustomEndCap = lineArrow;

                    g.DrawLine(p, Convert.ToInt32(dr["X1"]) * times, Convert.ToInt32(dr["Y1"]) * times, Convert.ToInt32(dr["X"]) * times, Convert.ToInt32(dr["Y"]) * times);
                    PointF py = new PointF(Convert.ToInt32(dr["X"]) * times - 1, Convert.ToInt32(dr["Y"]) * times - 1);
                    PointF ps = new PointF(Convert.ToInt32(dr["X1"]) * times, Convert.ToInt32(dr["Y1"]) * times);

                    // Font f = new Font("Arial", 12);
                    Font f = new Font("Times New Roman", 2, FontStyle.Bold);
                    g.DrawString(dr["sjjhy"].ToString(), f, Brushes.Black, py);
                    g.DrawString(dr["sjjhs"].ToString(), f, Brushes.DarkRed, ps);
                }
            }

            g.Save();
            g.Dispose();
            mf.Dispose();

            FileStream fs = new FileStream("img.emf", FileMode.OpenOrCreate);
            BinaryReader br = new BinaryReader(fs);

            byte[] b = br.ReadBytes((int)fs.Length);
            br.Close();
            fs.Close(); //文件流操作时一定要记住关。
            if (b != null)
            {
                Stream ms = new MemoryStream(b);
                pictureEdit1.Image = Image.FromStream(ms);
                ms.Close();
            }
            pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;

        }

        /// 获取指定窗口的设备场景
        ///
        /// 将获取其设备场景的窗口的句柄。若为0，则要获取整个屏幕的DC
        /// 指定窗口的设备场景句柄，出错则为0
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        ///
        /// 释放由调用GetDC函数获取的指定设备场景
        ///
        /// 要释放的设备场景相关的窗口句柄
        /// 要释放的设备场景句柄
        /// 执行成功为1，否则为0
        [DllImport("user32.dll")]
        public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        ///
        /// 在指定的设备场景中取得一个像素的RGB值
        ///
        /// 一个设备场景的句柄
        /// 逻辑坐标中要检查的横坐标
        /// 逻辑坐标中要检查的纵坐标
        /// 指定点的颜色
        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        public System.Drawing.Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
            (int)(pixel & 0x0000FF00) >> 8,
            (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }
        //得到互补色
        public Color ReColor(Color color)
        {
            byte[] colortemp = new byte[3];
            colortemp[0] = (byte)(255 - color.R); //也可以直接取非 colortemp[0] = ~(byte)color.R;
            colortemp[1] = (byte)(255 - color.G);
            colortemp[2] = (byte)(255 - color.B);
            return Color.FromArgb(colortemp[0], colortemp[1], colortemp[2]);
        }
        private void PictureEdit1_MouseWheel(object sender, MouseEventArgs e)
        {

            int x = e.Location.X;
            int y = e.Location.Y;
            int ow = pictureEdit1.Width;
            int oh = pictureEdit1.Height;
            int VX, VY;
            if (e.Delta > 0)
            {
                pictureEdit1.Width += zoomStep;
                this.pictureEdit1.Height += zoomStep;
                PropertyInfo pInfo = pictureEdit1.GetType().GetProperty("ImageRectangle", BindingFlags.Instance |
                    BindingFlags.NonPublic);
                //Rectangle rect = (Rectangle)pInfo.GetValue(pictureEdit1);
                //pictureEdit1.Width = rect.Width;
                //pictureEdit1.Height = rect.Height;
            }
            if (e.Delta < 0)
            {

                if (pictureEdit1.Width < WIDTH / 10)
                    return;

                pictureEdit1.Width -= zoomStep;
                pictureEdit1.Height -= zoomStep;
                //PropertyInfo pInfo = pictureEdit1.GetType().GetProperty("ImageRectangle", BindingFlags.Instance |
                //    BindingFlags.NonPublic);
                //Rectangle rect = (Rectangle)pInfo.GetValue(pictureEdit1);
                //pictureEdit1.Width = rect.Width;
                //pictureEdit1.Height = rect.Height;
            }

            VX = (int)((double)x * (ow - pictureEdit1.Width) / ow);
            VY = (int)((double)y * (oh - pictureEdit1.Height) / oh);
            pictureEdit1.Location = new Point(pictureEdit1.Location.X + VX, pictureEdit1.Location.Y + VY);
        }

        private void pictureEdit1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
                isMove = true;
                pictureEdit1.Focus();
            }
        }

        private void pictureEdit1_MouseMove(object sender, MouseEventArgs e)
        {
            pictureEdit1.Focus();
            if (isMove)
            {
                int x, y;
                int moveX, moveY;
                moveX = Cursor.Position.X - mouseDownPoint.X;
                moveY = Cursor.Position.Y - mouseDownPoint.Y;
                x = pictureEdit1.Location.X + moveX;
                y = pictureEdit1.Location.Y + moveY;
                pictureEdit1.Location = new Point(x, y);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
            }
        }

        private void pictureEdit1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMove = false;
            }
        }
    }
}
