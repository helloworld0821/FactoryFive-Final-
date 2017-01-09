using DXApplication1.DAL.TextDAL;
using DXApplication1.Model;
using DXApplication1.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.DAL
{
    /// <summary>
    /// 低效循环层位识别
    /// </summary>
    class DxxhcwsbDAL
    {

        public static readonly double HsQuality = 0.95;
        public static readonly double RcylQuality = 5;
        public static readonly double RcylBfsQuality = 0.1;
        public static readonly double CyqdQuality = 2;
        public static readonly double ZrqdQuality = 2;
        public static readonly double ZrlQuality = 5;
        public static readonly double ZrBfsQuality = 0.1;

        DateUtil du = new DateUtil();
        DateTime dtStartDate = DateTime.ParseExact(MainForm.strStartDate, "yyyyMM", null);
        DateTime dtEndDate = DateTime.ParseExact(MainForm.strEndDate, "yyyyMM", null);


        public DataTable DxxhcesbMethod()
        {

            DataView dv;

            DataTable dtDXOilJH = ComputeHIDAL.DXOilMethod();
            DataTable dtDXWaterJH = ComputeHIDAL.DXWaterMethod();


            ConnDatabaseUtil cdu = new ConnDatabaseUtil();

            string strSQL = string.Empty;

            List<WellModel> lstWM = new List<WellModel>();
            WellModel wm = new WellModel();

            string str = string.Empty;
            int diffMonth = Calc(dtStartDate, dtEndDate);

            DataTable dtWater = new DataTable();//当前日期下的数模水井
            DataTable dtOil = new DataTable();//当前日期下的数模油井
            DataTable dtDJY = new DataTable();//单井油
            DataTable dtDJS = new DataTable();//单井水
            DataRow[] drWater;
            DataRow[] drOil;
            DataRow[] drTracer;
            DataTable dtTracer = new DataTable();

            if (MainForm.ChooseWaterItem == 2)
            {
                dtWater = GetLayer.dtWater.Copy();
                dtOil = GetLayer.dtOil.Copy();
            }

            if (MainForm.ChooseWaterItem == 1)
            {
                dtTracer = GetTracerDAL.dtTracer.Clone();

                var query =
                    from rTracer in GetTracerDAL.dtTracer.AsEnumerable()
                    join rOil in GetTracerDAL.dtOil.AsEnumerable()
                    on new { JHY = rTracer.Field<string>("JHY"), DTDATE = rTracer.Field<DateTime>("DTDATE"), K = rTracer.Field<Int32>("K") } equals new { JHY = rOil.Field<string>("JHY"), DTDATE = rOil.Field<DateTime>("DTDATE"), K = rOil.Field<Int32>("K") }

                    select new
                    {
                        JHY = rTracer.Field<string>("JHY"),
                        DTDATE = rTracer.Field<DateTime>("DTDATE"),
                        K = rTracer.Field<Int32>("K"),
                        SZJ = rTracer.Field<string>("SZJ"),
                        SZJND = rTracer.Field<double>("SZJND"),
                        MNCS = rOil.Field<double>("MNCS"),
                        MNCY = rOil.Field<double>("MNCY"),
                        MNCYE = rOil.Field<double>("MNCYE")
                    };

                foreach (var obj in query)
                {
                    dtTracer.Rows.Add(obj.JHY, null, obj.SZJ, obj.SZJND, null, obj.DTDATE, null, null, null, obj.K, obj.MNCY, obj.MNCS, obj.MNCYE);
                }
                dtTracer.Columns.Add("ZRL", System.Type.GetType("System.Double"));
                dtTracer.Columns["ZRL"].Expression = "MNCS*SZJND";
            }
            DataTable dtOilCW = new DataTable();//油井存在的层位
            DataTable dtWaterCW = new DataTable();//水井存在层位
            DataTable dtResult = dtOil.Clone();
            dtResult.Columns.Add("GLXS", System.Type.GetType("System.Double"));
            for (int i = 0; i < dtDXOilJH.Rows.Count; i++)
            {
                strSQL = "select distinct * from " + MainForm.strLTKName + " where jhy = '" + dtDXOilJH.Rows[i]["JH"] + "'";
                DataTable dtLTWater = cdu.SelectVFP(strSQL);
                switch (MainForm.ChooseWaterItem)
                {
                    case 0:
                        dtOilCW = dtDCB01(dtDXOilJH.Rows[i]["JH"].ToString());
                        break;
                    case 1:
                        DataRow[] dr = FrmLayerSelectedInfo.dtExcelJHDY.Select("实际井 = '" + dtDXOilJH.Rows[i]["JH"] + "'");
                        if (dr.Count() > 0)
                        {
                            drOil = GetTracerDAL.dtOil.Select("JHY = '" + dr[0]["数模井"] + "'");
                            if (drOil.Count() > 0)
                            {
                                dtDJY = drOil.CopyToDataTable();
                                dtOilCW = TracerOilGetLayer(dr[0]["数模井"].ToString(), dtDJY);//计算示踪剂情况下
                            }
                            else
                            {
                                dtOilCW = new DataTable();//计算数模情况下
                            }
                        }
                        else
                        {
                            dtOilCW = new DataTable();//计算数模情况下
                        }
                        break;
                    case 2:

                        dr = FrmLayerSelectedInfo.dtExcelJHDY.Select("实际井 = '" + dtDXOilJH.Rows[i]["JH"] + "'");
                        if (dr.Count() > 0)
                        {
                            drOil = dtOil.Select("JHY = '" + dr[0]["数模井"] + "'");
                            if (drOil.Count() > 0)
                            {
                                dtDJY = drOil.CopyToDataTable();
                                dtOilCW = NewOilGetLayer(dr[0]["数模井"].ToString(), dtDJY);//计算数模情况下
                            }
                            else
                            {
                                dtOilCW = new DataTable();//计算数模情况下
                            }
                        }
                        else
                        {
                            dtOilCW = new DataTable();//计算数模情况下
                        }
                        break;
                    case 3:
                        dtOilCW = Jt(dtDXOilJH.Rows[i]["JH"].ToString());//计算静态情况下
                        break;
                }

                if (dtOilCW.Rows.Count > 0)
                {
                    #region 示踪剂对示踪剂
                    if (MainForm.ChooseWaterItem == 1)
                    {
                        for (int j = 0; j < dtOilCW.Rows.Count; j++)
                        {
                            double djSzj = 0;
                            double totalSzj = 0;
                            DataTable dtTempSZJ = dtTracer.Clone();
                            dtTempSZJ.Columns.Add("DJSZJ", System.Type.GetType("System.Double"));
                            dtTempSZJ.Columns.Add("totalSzj", System.Type.GetType("System.Double"));
                            dtTempSZJ.Columns.Add("SZJBFS", System.Type.GetType("System.Double"));
                            DataRow[] drTemp = dtTracer.Select("JHY = '" + dtOilCW.Rows[j]["JHY"] + "' AND K = " + dtOilCW.Rows[j]["K"]);
                            if (drTemp.Count() > 0)
                            {
                                if (dtOilCW.Rows[j]["JHY"].Equals("1B361"))
                                {
                                }
                                DataTable dtTemp = drTemp.CopyToDataTable();
                                totalSzj = Convert.ToDouble(dtTemp.Compute("sum(ZRL)", "")) / dtTemp.DefaultView.ToTable(true, "DTDATE").Rows.Count;
                                DataTable dtSZJ = dtTemp.DefaultView.ToTable(true, "szj");
                                foreach (DataRow drtp in dtSZJ.Rows)
                                {
                                    DataRow[] drSJJHS = FrmLayerSelectedInfo.dtExcelJHDY.Select("数模井 = '" + drtp["SZJ"] + "'");//drsjjhs实际井号水
                                    if (drSJJHS.Count() > 0)
                                    {
                                        DataRow[] drLTWater = dtDXWaterJH.Select("JH = '" + drSJJHS[0]["实际井"] + "'");
                                        if (drLTWater.Count() > 0)
                                        {
                                            djSzj = Convert.ToDouble(dtTemp.Compute("AVG(ZRL)", "SZJ = '" + drtp["SZJ"] + "'"));
                                            DataTable dtSJCS = cdu.SelectDatabase("select a.*, round(a.砂岩厚度 * a.渗透率, 4) KH from T_WELL_DAA074 a where a.井号 = '" + drSJJHS[0]["实际井"] + "' AND a.层位 = '" + dtOilCW.Rows[j]["CW"] + "'");//水井参数
                                            DataRow[] drSMCS = GetTracerDAL.dtWater.Select("JHS = '" + drSJJHS[0]["数模井"] + "' AND K = " + dtOilCW.Rows[j]["K"]);
                                            if (dtSJCS.Rows.Count > 0 && drSMCS.Count() > 0)
                                            {
                                                DataRow drSZJ = dtTempSZJ.NewRow();
                                                drSZJ["JHS"] = dtSJCS.Rows[0]["井号"];
                                                drSZJ["MNCS"] = Convert.ToDouble(drSMCS[0]["MNCS"]);
                                                drSZJ["ZRQD"] = Convert.ToDouble(drSMCS[0]["MNCS"]) / Convert.ToDouble(dtSJCS.Rows[0]["砂岩厚度"]);
                                                drSZJ["SKH"] = dtSJCS.Rows[0]["KH"];
                                                drSZJ["SSTL"] = Convert.ToDouble(dtSJCS.Rows[0]["渗透率"]);
                                                drSZJ["SSYHD"] = Convert.ToDouble(dtSJCS.Rows[0]["砂岩厚度"]);
                                                drSZJ["SWX"] = Convert.ToDouble(dtSJCS.Rows[0]["微相"]);

                                                drSZJ["JHY"] = dtOilCW.Rows[j]["JHY"];
                                                drSZJ["K"] = Convert.ToInt32(dtOilCW.Rows[j]["K"]);
                                                drSZJ["CW"] = dtOilCW.Rows[j]["CW"];
                                                drSZJ["YSYHD"] = Convert.ToDouble(dtOilCW.Rows[j]["YSYHD"]);
                                                drSZJ["YSTL"] = Convert.ToDouble(dtOilCW.Rows[j]["YSTL"]);
                                                drSZJ["YWX"] = dtOilCW.Rows[j]["YWX"];
                                                drSZJ["YKH"] = Convert.ToDouble(dtOilCW.Rows[j]["YKH"]);
                                                drSZJ["HS"] = Convert.ToDouble(dtOilCW.Rows[j]["HS"]);
                                                drSZJ["CYEQD"] = Convert.ToDouble(dtOilCW.Rows[j]["CYEQD"]);
                                                drSZJ["MNCYE"] = Convert.ToDouble(dtOilCW.Rows[j]["MNCYE"]);
                                                drSZJ["SZJ"] = drSJJHS[0]["实际井"];
                                                drSZJ["totalSzj"] = totalSzj;
                                                drSZJ["DJSZJ"] = djSzj;
                                                drSZJ["SZJBFS"] = djSzj / totalSzj;
                                                dtTempSZJ.Rows.Add(drSZJ);
                                            }
                                        }
                                    }
                                }
                                if (dtTempSZJ.Rows.Count > 0)
                                {
                                    dv = dtTempSZJ.DefaultView;
                                    dv.Sort = "SZJBFS DESC";
                                    dtTempSZJ = dv.ToTable();
                                    totalSzj = 0;
                                    //foreach (DataRow item in dtTempSZJ.Rows)
                                    //{
                                    //    sw.WriteLine(item["JHY"] + " " + item["CW"] + " " + item["HS"] + " " + item["MNCYE"] + " " + item["CYEQD"] + " " + item["YSYHD"] + " " + item["YSTL"] + " " + item["YWX"] + " " + item["YKH"] + " " + item["SZJ"] + " " + item["DJSZJ"] + " " + item["totalSzj"] + " " + Convert.ToDouble(item["SZJBFS"]).ToString("P") + " " + item["MNCS"] + " " + item["ZRQD"] + " " + item["SSYHD"] + " " + item["SKH"] + " " + item["SSTL"] + " " + item["SWX"]);
                                    //}
                                }
                                dtResult.Merge(dtTempSZJ);
                            }
                        }
                    }
                    #endregion
                    else
                    {
                        foreach (DataRow drLTRow in dtLTWater.Rows)
                        {
                            DataRow[] drJJ = GetJJ.dtJJ.Select("JHY = '" + dtDXOilJH.Rows[i]["JH"] + "' AND JHS = '" + drLTRow["JHS"] + "'");
                            if (drJJ.Count() > 0)
                                if (Convert.ToInt32(drJJ[0]["JJ"]) > 650)
                                    continue;
                            DataRow[] drLTWater = dtDXWaterJH.Select("JH = '" + drLTRow["JHS"] + "'");
                            if (drLTWater.Count() > 0)
                            {

                                switch (MainForm.ChooseWaterItem)
                                {
                                    case 0:
                                        dtWaterCW = dtDCB02(drLTRow["JHS"].ToString());
                                        break;
                                    case 2:
                                        DataRow[] dr = FrmLayerSelectedInfo.dtExcelJHDY.Select("实际井 = '" + drLTRow["JHS"] + "'");

                                        if (dr.Count() > 0)
                                        {
                                            drWater = dtWater.Select("JHS = '" + dr[0]["数模井"] + "'");
                                            if (drWater.Count() > 0)
                                            {
                                                dtDJS = drWater.CopyToDataTable();
                                                dtWaterCW = NewWaterGetLayer(dr[0]["数模井"].ToString(), dtDJS);
                                            }
                                            else
                                            {
                                                dtWaterCW = new DataTable();//计算数模情况下
                                            }
                                        }
                                        else
                                        {
                                            dtWaterCW = new DataTable();//计算数模情况下
                                        }
                                        break;
                                    case 3:
                                        dtWaterCW = Jt(drLTRow["JHS"].ToString());
                                        break;
                                }
                                if (dtWaterCW.Rows.Count > 0)
                                {
                                    for (int k = 0; k < dtOilCW.Rows.Count; k++)
                                    {
                                        DataRow[] drWaterCW = dtWaterCW.Select("CW = '" + dtOilCW.Rows[k]["CW"] + "'");
                                        if (drWaterCW.Count() > 0)
                                        {
                                            wm = new WellModel();
                                            if (MainForm.ChooseOilItem == 0 || MainForm.ChooseOilItem == 2)
                                            {
                                                wm.jhy = dtOilCW.Rows[k]["JHY"].ToString();
                                                wm.cw = dtOilCW.Rows[k]["CW"].ToString();
                                                wm.CYEQD = Convert.ToDouble(Convert.ToDouble(dtOilCW.Rows[k]["CYEQD"]).ToString("f2"));
                                                wm.YSTL = Convert.ToDouble(dtOilCW.Rows[k]["YSTL"]);
                                                wm.YSYHD = Convert.ToDouble(dtOilCW.Rows[k]["YSYHD"]);
                                                wm.YWX = Convert.ToDouble(dtOilCW.Rows[k]["YWX"]);
                                                wm.HS = Convert.ToDouble(Convert.ToDouble(dtOilCW.Rows[k]["HS"]).ToString("f4"));
                                                //wm.YKH = Convert.ToDouble(Convert.ToDouble(dtOilCW.Rows[k]["YKH"]).ToString("f2"));
                                                wm.mncye = Convert.ToDouble(Convert.ToDouble(dtOilCW.Rows[k]["MNCYE"]).ToString("f2"));
                                                if (MainForm.ChooseOilItem == 2)
                                                    wm.k = Convert.ToInt32(dtOilCW.Rows[k]["K"]);
                                            }
                                            if (MainForm.ChooseWaterItem == 2 || MainForm.ChooseWaterItem == 0)
                                            {
                                                wm.jhy = dtOilCW.Rows[k]["JHY"].ToString();
                                                wm.CYEQD = Convert.ToDouble(Convert.ToDouble(dtOilCW.Rows[k]["CYEQD"]).ToString("f2"));
                                                wm.YSTL = Convert.ToDouble(dtOilCW.Rows[k]["YSTL"]);
                                                wm.YSYHD = Convert.ToDouble(dtOilCW.Rows[k]["YSYHD"]);
                                                wm.YWX = Convert.ToDouble(dtOilCW.Rows[k]["YWX"]);
                                                wm.HS = Convert.ToDouble(Convert.ToDouble(dtOilCW.Rows[k]["HS"]).ToString("f4"));
                                                //wm.YKH = Convert.ToDouble(Convert.ToDouble(dtOilCW.Rows[k]["YKH"]);
                                                wm.mncye = Convert.ToDouble(Convert.ToDouble(dtOilCW.Rows[k]["MNCYE"]).ToString("f2"));

                                                wm.jhs = drWaterCW[0]["JHS"].ToString();
                                                wm.cw = drWaterCW[0]["CW"].ToString();
                                                wm.mncs = Convert.ToDouble(Convert.ToDouble(drWaterCW[0]["MNCS"]).ToString("f2"));
                                                //wm.SKH = Convert.ToDouble(Convert.ToDouble(drWaterCW[0]["SKH"]);
                                                wm.ZRQD = Convert.ToDouble(Convert.ToDouble(drWaterCW[0]["ZRQD"]).ToString("f2"));
                                                wm.SSTL = Convert.ToDouble(drWaterCW[0]["SSTL"]);
                                                wm.SSYHD = Convert.ToDouble(drWaterCW[0]["SSYHD"]);
                                                wm.SWX = Convert.ToDouble(drWaterCW[0]["SWX"]);
                                                if (MainForm.ChooseWaterItem == 2)
                                                    wm.k = Convert.ToInt32(drWaterCW[0]["K"]);
                                            }
                                            else if (MainForm.ChooseWaterItem == 3)
                                            {
                                                wm.jhy = dtOilCW.Rows[k]["井号"].ToString();
                                                wm.cw = dtOilCW.Rows[k]["层位"].ToString();
                                                wm.YSTL = Convert.ToDouble(dtOilCW.Rows[k]["渗透率"]);
                                                wm.YSYHD = Convert.ToDouble(dtOilCW.Rows[k]["砂岩厚度"]);
                                                //wm.YKH = Convert.ToDouble(dtOilCW.Rows[k]["油KH"]);
                                                wm.YWX = Convert.ToDouble(dtOilCW.Rows[k]["微相"]);

                                                wm.jhs = drWaterCW[0]["井号"].ToString();
                                                wm.cw = drWaterCW[0]["层位"].ToString();
                                                wm.ZRQD = Convert.ToDouble(Convert.ToDouble(drWaterCW[0]["注入强度"]).ToString("f2"));
                                                wm.SSTL = Convert.ToDouble(drWaterCW[0]["渗透率"]);
                                                //wm.SKH = Convert.ToDouble(drWaterCW[0]["水KH"]);
                                                wm.SSYHD = Convert.ToDouble(drWaterCW[0]["砂岩厚度"]);
                                                wm.SWX = Convert.ToDouble(drWaterCW[0]["微相"]);
                                            }
                                            lstWM.Add(wm);
                                        }
                                    }
                                }
                            }

                            #region 静态对静态
                            if (MainForm.ChooseWaterItem == 3)
                            {
                                DataTable dt = JTMethod(dtDXOilJH.Rows[i]["JH"].ToString(), drLTRow["JHS"].ToString());
                                dtResult.Merge(dt);
                            }
                            #endregion
                        }
                    }
                    DataTable dtGL = ListToDataTableUtil.ListToDataTable(lstWM);
                    dtGL.Columns.Add("GLXS", System.Type.GetType("System.Double"));

                    lstWM.Clear();
                    #region 数模对数模
                    if (MainForm.ChooseWaterItem == 2)
                    {
                        DataTable dtLTCW = dtGL.DefaultView.ToTable(true, "K");
                        for (int m = 0; m < dtLTCW.Rows.Count; m++)
                        {
                            DataRow[] drGL = dtGL.Select("K = " + dtLTCW.Rows[m]["K"]);
                            if (drGL.Count() == 1)
                            {
                                drGL[0]["GLXS"] = 1;
                                dtResult.Rows.Add(drGL[0].ItemArray);
                            }
                            else if (drGL.Count() > 1)
                            {
                                foreach (DataRow dr in drGL)
                                {


                                    double dblZrlAvg = Convert.ToDouble(dtWater.Compute("Avg(MNCS)", "K = " + dtLTCW.Rows[m]["K"]));
                                    double dblRcylAvg = Convert.ToDouble(dtOil.Compute("Avg(MNCYE)", "K = " + dtLTCW.Rows[m]["K"]));

                                    double fz = 0;
                                    double sfm = 0, yfm = 0;
                                    double cye = 0, zrl = 0;

                                    for (DateTime date = dtStartDate; date <= dtEndDate; date = date.AddMonths(1))
                                    {

                                        DataTable dtOilLayer = dtOil.Select("JHY = '" + dtGL.Rows[0]["JHY"] + "' AND K = " + Convert.ToInt32(dtLTCW.Rows[m]["K"])).CopyToDataTable();
                                        DataTable dtWaterLayer = dtWater.Select("JHS = '" + dr["JHS"] + "' AND K = " + Convert.ToInt32(dtLTCW.Rows[m]["K"])).CopyToDataTable();
                                        DataRow[] drOilLayer = dtOilLayer.Select("DTDATE = #" + date + "#");
                                        DataRow[] drWaterLayer = dtWaterLayer.Select("DTDATE = #" + date + "#");
                                        if (drOilLayer.Count() == 0)
                                        {
                                            cye = 0;
                                        }
                                        else
                                        {
                                            cye = Convert.ToDouble(drOilLayer[0]["MNCYE"]);
                                        }
                                        if (drWaterLayer.Count() == 0)
                                        {
                                            zrl = 0;
                                        }
                                        else
                                        {
                                            zrl = Convert.ToDouble(dtWaterLayer.Rows[0]["MNCS"]);
                                        }

                                        fz += (cye - dblRcylAvg) * (zrl - dblZrlAvg);
                                        sfm += Math.Pow((cye - dblRcylAvg), 2);
                                        yfm += Math.Pow((zrl - dblZrlAvg), 2);

                                    }

                                    sfm = Math.Pow(sfm, 0.5);
                                    yfm = Math.Pow(yfm, 0.5);
                                    double r = fz / (sfm * yfm);
                                    fz = 0;
                                    sfm = 0;
                                    yfm = 0;

                                    dr["GLXS"] = r;
                                   // dtResult.Rows.Add(dr.ItemArray);
                                }
                                double dblGlxsMin = Convert.ToDouble(drGL.CopyToDataTable().Compute("min(GLXS)", ""));
                                double dblGlxsMax = Convert.ToDouble(drGL.CopyToDataTable().Compute("max(GLXS)", ""));

                                foreach (DataRow dr in drGL)
                                {
                                    if (dblGlxsMin != dblGlxsMax)
                                    {
                                        dr["GLXS"] = ((Convert.ToDouble(dr["GLXS"]) - dblGlxsMin) / (dblGlxsMax - dblGlxsMin)).ToString("f4");
                                    }
                                    else
                                    {
                                        dr["GLXS"] = 1;
                                    }
                                    dtResult.Rows.Add(dr.ItemArray);
                                }
                            }
                        }

                    }
                    #endregion
                    else if (MainForm.ChooseWaterItem == 0)
                    {
                        dtResult.Merge(dtGL);
                    }
                    //else
                    //{
                    //    
                    //}

                }
            }

            dv = dtResult.DefaultView;
            dtResult.Columns["sjjhy"].Expression = "'X10-'+substring(jhy,1,1)+'-'+substring(jhy,2,len(jhy)-1)";
            dtResult.Columns["sjjhs"].Expression = "'X10-'+substring(jhs,1,1)+'-'+substring(jhs,2,len(jhs)-1)";
            dv.Sort = "SJJHY ASC, CW ASC, GLXS DESC";
            dtResult = dv.ToTable();
            //foreach (DataRow drT in dtResult.Rows)
            //{
            //    sw.WriteLine(drT["JHY"].ToString() + " " + drT["CW"].ToString() + " " + drT["hs"].ToString() + " " + drT["mncye"].ToString() + " " + drT["CYEQD"].ToString() + " " + drT["ystl"].ToString() + " " + drT["ysyhd"].ToString() + " " + drT["ywx"].ToString() + " " + drT["JHS"].ToString() + " " + drT["zrqd"].ToString() + " " + drT["sstl"].ToString() + " " + drT["ssyhd"].ToString() + " " + drT["swx"].ToString());
            //}


            //sw.Close();
            //fs.Close();
            return dtResult;
        }

        public static DataTable TracerOilGetLayer(string strJHY, DataTable dtDJY)//示踪剂油井
        {
            DataTable dtTempOil = dtDJY.Clone();

            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            List<double> lstHsAvg = new List<double>();
            List<double> lstYcylAvg = new List<double>();
            List<double> lstYcslAvg = new List<double>();
            DataTable dtMNCW = dtDJY.DefaultView.ToTable(true, "K");//模拟层位
            for (int k = 0; k < dtMNCW.Rows.Count; k++)
            {
                double dblCsSum = Convert.ToDouble(dtDJY.Compute("SUM(MNCS)", "K = " + dtMNCW.Rows[k]["K"]));
                double dblCyeSum = Convert.ToDouble(dtDJY.Compute("SUM(MNCYE)", "K = " + dtMNCW.Rows[k]["K"]));

                //double dblHsAvg = Convert.ToDouble(dtDJY.Compute("AVG(HS)", "K = " + dtMNCW.Rows[k]["K"]));
                double dblHsAvg = dblCsSum / dblCyeSum;
                double dblYcslAvg = Convert.ToDouble(dtDJY.Compute("AVG(MNCS)", "K = " + dtMNCW.Rows[k]["K"]));
                double dblYcylAvg = Convert.ToDouble(dtDJY.Compute("AVG(MNCYE)", "K = " + dtMNCW.Rows[k]["K"]));
                lstHsAvg.Add(dblHsAvg);
                lstYcylAvg.Add(dblYcylAvg);
                lstYcslAvg.Add(dblYcslAvg);
            }
            //层位参数
            DataRow[] drSJJHY = FrmLayerSelectedInfo.dtExcelJHDY.Select("数模井 = '" + strJHY + "'");
            DataTable dtCWCS = cdu.SelectVFP("select a.*, round(a.砂岩厚度 * a.渗透率, 4) as KH from T_DAA074 as a where a.井号 = '" + drSJJHY[0]["实际井"] + "'");

            for (int i = 0; i < dtMNCW.Rows.Count; i++)
            {
                DataRow[] drXC = FrmLayerSelectedInfo.dtExcelCWDY.Select("序号 = '" + dtMNCW.Rows[i]["K"] + "'");
                DataRow[] drCW = dtCWCS.Select("层位 = '" + drXC[0]["沉积单元"] + "'");
                if (drCW.Count() > 0)
                {
                    DataRow dr = dtTempOil.NewRow();
                    dr["JHY"] = strJHY;
                    dr["K"] = Convert.ToInt32(dtMNCW.Rows[i]["K"]);
                    dr["CW"] = drCW[0]["层位"].ToString();
                    dr["HS"] = lstHsAvg[i];
                    dr["MNCS"] = lstYcslAvg[i];
                    dr["MNCYE"] = lstYcylAvg[i];
                    dr["CYEQD"] = lstYcylAvg[i] / Convert.ToDouble(drCW[0]["砂岩厚度"]);
                    dr["YKH"] = Convert.ToDouble(drCW[0]["KH"]);
                    dr["YSYHD"] = Convert.ToDouble(drCW[0]["砂岩厚度"]);
                    dr["YSTL"] = Convert.ToDouble(drCW[0]["渗透率"]);
                    dr["YWX"] = Convert.ToDouble(drCW[0]["微相"]);
                    dtTempOil.Rows.Add(dr);
                }

            }
            return dtTempOil;
        }
        public DataTable NewOilGetLayer(string strJHY, DataTable dtDJY)//数模油井
        {
            DataTable dtTempOil = dtDJY.Clone();

            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            List<double> lstHsAvg = new List<double>();
            List<double> lstYcslAvg = new List<double>();
            List<double> lstYcylAvg = new List<double>();
            //dtDJY.Columns["HS"].Expression = "MNCS/MNCYE";
            DataTable dtMNCW = dtDJY.DefaultView.ToTable(true, "K");//模拟层位
            for (int k = 0; k < dtMNCW.Rows.Count; k++)
            {
                double dblCsSum = Convert.ToDouble(dtDJY.Compute("SUM(MNCS)", "K = " + dtMNCW.Rows[k]["K"]));
                double dblCyeSum = Convert.ToDouble(dtDJY.Compute("SUM(MNCYE)", "K = " + dtMNCW.Rows[k]["K"]));

                //double dblHsAvg = Convert.ToDouble(dtDJY.Compute("AVG(HS)", "K = " + dtMNCW.Rows[k]["K"]));
                double dblHsAvg = dblCsSum / dblCyeSum;
                double dblYcslAvg = Convert.ToDouble(dtDJY.Compute("AVG(MNCS)", "K = " + dtMNCW.Rows[k]["K"]));
                double dblYcylAvg = Convert.ToDouble(dtDJY.Compute("AVG(MNCYE)", "K = " + dtMNCW.Rows[k]["K"]));
                lstHsAvg.Add(dblHsAvg);
                lstYcylAvg.Add(dblYcylAvg);
                lstYcslAvg.Add(dblYcslAvg);
            }

            //层位参数
            DataRow[] drSJJHY = FrmLayerSelectedInfo.dtExcelJHDY.Select("数模井 = '" + strJHY + "'");

            //DataTable dtCWCS = cdu.SelectDatabase("select n.*, round(n.渗透率 * n.砂岩厚度, 4) KH from (select t.井号, avg(t.渗透率 * t.砂岩厚度) as avgkh, avg(t.渗透率) as avgk from T_WELL_DAA074 t group by 井号) a, T_WELL_DAA074 n where a.井号 = n.井号 and (n.渗透率 * n.砂岩厚度) > a.avgkh and n.渗透率 > a.avgk and a.井号 = '" + drSJJHY[0]["实际井"] + "'");//按K、KH限制取大于平均值的层
            DataTable dtCWCS = cdu.SelectVFP("select n.* from (select 井号, avg(渗透率) as avgk from T_DAA074 group by 井号) as a, T_DAA074 as n where a.井号 = n.井号 and n.渗透率 > a.avgk and a.井号 = '" + drSJJHY[0]["实际井"] + "'");//按K限制取大于平均值的层
            //DataTable dtCWCS = cdu.SelectDatabase("select a.*, round(a.砂岩厚度 * a.渗透率, 4) KH from T_WELL_DAA074 a where a.井号 = '" + drSJJHY[0]["实际井"] + "'");
            for (int i = 0; i < dtMNCW.Rows.Count; i++)
            {
                DataRow[] drXC = FrmLayerSelectedInfo.dtExcelCWDY.Select("序号 = '" + dtMNCW.Rows[i]["K"] + "'");
                DataRow[] drCW = dtCWCS.Select("层位 = '" + drXC[0]["沉积单元"] + "'");
                if (drCW.Count() > 0)
                {
                    DataRow dr = dtTempOil.NewRow();
                    dr["JHY"] = strJHY;
                    dr["K"] = Convert.ToInt32(dtMNCW.Rows[i]["K"]);
                    dr["CW"] = drCW[0]["层位"].ToString();
                    dr["HS"] = lstHsAvg[i];
                    dr["MNCS"] = lstYcslAvg[i];
                    dr["MNCYE"] = lstYcylAvg[i];
                    dr["CYEQD"] = lstYcylAvg[i] / Convert.ToDouble(drCW[0]["砂岩厚度"]);
                    //dr["YKH"] = Convert.ToDouble(drCW[0]["KH"]);
                    dr["YSYHD"] = Convert.ToDouble(drCW[0]["砂岩厚度"]);
                    dr["YSTL"] = Convert.ToDouble(drCW[0]["渗透率"]);
                    dr["YWX"] = Convert.ToDouble(drCW[0]["微相"]);
                    dtTempOil.Rows.Add(dr);
                }

            }
            if (dtTempOil.Rows.Count > 0)
            {
                double dblAllCsSum = Convert.ToDouble(dtTempOil.Compute("SUM(MNCS)", ""));
                double dblAllCyeSum = Convert.ToDouble(dtTempOil.Compute("SUM(MNCYE)", ""));
                double dblAllSyhdSum = Convert.ToDouble(dtCWCS.Compute("sum(砂岩厚度)", ""));
               
                //double dblAllHsAvg = (dblAllCsSum / dblAllCyeSum )* 0.8;
                //double dblAllCYEQDAvg = (dblAllCyeSum / dblAllSyhdSum) * 0.8;
                double dblAllHsAvg = dblAllCsSum / dblAllCyeSum;
                double dblAllCYEQDAvg = dblAllCyeSum / dblAllSyhdSum;
                DataRow[] drTemp = dtTempOil.Select("HS > " + dblAllHsAvg + "AND CYEQD > " + dblAllCYEQDAvg);
                if (drTemp.Count() > 0)
                {
                    DataTable dt = drTemp.CopyToDataTable();
                    //DataView dv = dt.DefaultView;
                    //dv.Sort = "YSTL DESC";
                    //dt = dv.ToTable();
                    return dt;
                }
            }
            return new DataTable();
            //return dtTempOil;
        }

        public static DataTable NewWaterGetLayer(string strJHS, DataTable dtDJS)//数模水井
        {
            DataTable dtTempWater = dtDJS.Clone();

            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            List<double> lstZrlAvg = new List<double>();

            DataTable dtMNCW = dtDJS.DefaultView.ToTable(true, "K");
            for (int k = 0; k < dtMNCW.Rows.Count; k++)
            {
                double dblZrlAvg = Convert.ToDouble(dtDJS.Compute("AVG(MNCS)", "JHS = '" + strJHS + "' AND K = " + dtMNCW.Rows[k]["K"]));
                lstZrlAvg.Add(dblZrlAvg);
            }
            DataRow[] drSJJHS = FrmLayerSelectedInfo.dtExcelJHDY.Select("数模井 = '" + strJHS + "'");
            //层位参数
            //DataTable dtCWCS = cdu.SelectDatabase("select n.*, round(n.渗透率 * n.砂岩厚度, 4) KH from (select t.井号, avg(t.渗透率 * t.砂岩厚度) as avgkh, avg(t.渗透率) as avgk from T_WELL_DAA074 t group by 井号) a, T_WELL_DAA074 n where a.井号 = n.井号 and (n.渗透率 * n.砂岩厚度) > a.avgkh and n.渗透率 > a.avgk and a.井号 = '" + drSJJHS[0]["实际井"] + "'");
            DataTable dtCWCS = cdu.SelectVFP("select a.*, round(a.砂岩厚度 * a.渗透率, 4) as KH from T_DAA074 as a where a.井号 = '" + drSJJHS[0]["实际井"] + "'");
            for (int i = 0; i < dtMNCW.Rows.Count; i++)
            {
                DataRow[] drXC = FrmLayerSelectedInfo.dtExcelCWDY.Select("序号 = '" + dtMNCW.Rows[i]["K"] + "'");

                DataRow[] drCW = dtCWCS.Select("层位 = '" + drXC[0]["沉积单元"] + "'");
                if (drCW.Count() > 0)
                {
                    DataRow dr = dtTempWater.NewRow();
                    dr["JHS"] = strJHS;
                    dr["K"] = Convert.ToInt32(dtMNCW.Rows[i]["K"]);
                    dr["CW"] = drCW[0]["层位"].ToString();
                    dr["MNCS"] = lstZrlAvg[i];
                    dr["ZRQD"] = lstZrlAvg[i] / Convert.ToDouble(drCW[0]["砂岩厚度"]);
                    dr["SKH"] = Convert.ToDouble(drCW[0]["KH"]);
                    dr["SSYHD"] = Convert.ToDouble(drCW[0]["砂岩厚度"]);
                    dr["SSTL"] = Convert.ToDouble(drCW[0]["渗透率"]);
                    dr["SWX"] = Convert.ToDouble(drCW[0]["微相"]);
                    dtTempWater.Rows.Add(dr);
                }
            }
            return dtTempWater;
        }
        /*之前 按KH和K取前一半
        public DataTable NewOilGetLayer(string strJHY, DataTable dtDJY)//数模油井
        {
            DataTable dtTempOil = dtDJY.Clone();

            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            List<double> lstHsAvg = new List<double>();
            List<double> lstYcylAvg = new List<double>();
            dtDJY.Columns["HS"].Expression = "MNCS/MNCYE";
            DataTable dtMNCW = dtDJY.DefaultView.ToTable(true, "K");//模拟层位
            for (int k = 0; k < dtMNCW.Rows.Count; k++)
            {
                double dblHsAvg = Convert.ToDouble(dtDJY.Compute("AVG(HS)", "K = " + dtMNCW.Rows[k]["K"]));
                double dblYcylAvg = Convert.ToDouble(dtDJY.Compute("AVG(MNCYE)", "K = " + dtMNCW.Rows[k]["K"]));
                lstHsAvg.Add(dblHsAvg);
                lstYcylAvg.Add(dblYcylAvg);
            }
            //层位参数
            DataRow[] drSJJHY = FrmLayerSelectedInfo.dtExcelJHDY.Select("数模井 = '" + strJHY + "'");

            DataTable dtCWCS = cdu.SelectDatabase("select n.*, round(n.渗透率 * n.砂岩厚度, 4) KH from (select t.井号, avg(t.渗透率 * t.砂岩厚度) as avgkh, avg(t.渗透率) as avgk from T_WELL_DAA074 t group by 井号) a, T_WELL_DAA074 n where a.井号 = n.井号 and (n.渗透率 * n.砂岩厚度) > a.avgkh and n.渗透率 > a.avgk and a.井号 = '" + drSJJHY[0]["实际井"] + "'");
            //DataTable dtCWCS = cdu.SelectDatabase("select a.*, round(a.砂岩厚度 * a.渗透率, 4) KH from T_WELL_DAA074 a where a.井号 = '" + drSJJHY[0]["实际井"] + "'");
            for (int i = 0; i < dtMNCW.Rows.Count; i++)
            {
                DataRow[] drXC = FrmLayerSelectedInfo.dtExcelCWDY.Select("序号 = '" + dtMNCW.Rows[i]["K"] + "'");
                DataRow[] drCW = dtCWCS.Select("层位 = '" + drXC[0]["沉积单元"] + "'");
                if (drCW.Count() > 0)
                {
                    DataRow dr = dtTempOil.NewRow();
                    dr["JHY"] = strJHY;
                    dr["K"] = Convert.ToInt32(dtMNCW.Rows[i]["K"]);
                    dr["CW"] = drCW[0]["层位"].ToString();
                    dr["HS"] = lstHsAvg[i];
                    dr["MNCYE"] = lstYcylAvg[i];
                    dr["YKH"] = Convert.ToDouble(drCW[0]["KH"]);
                    dr["YSYHD"] = Convert.ToDouble(drCW[0]["砂岩厚度"]);
                    dr["YSTL"] = Convert.ToDouble(drCW[0]["渗透率"]);
                    dr["YWX"] = Convert.ToDouble(drCW[0]["微相"]);
                    dtTempOil.Rows.Add(dr);
                }

            }
            if (dtTempOil.Rows.Count > 0)
            {
                double dblAllHsAvg = Convert.ToDouble(dtTempOil.Compute("avg(HS)", ""));
                double dblAllYcylAvg = Convert.ToDouble(dtTempOil.Compute("avg(MNCYE)", ""));
                DataRow[] drTemp = dtTempOil.Select("HS > " + dblAllHsAvg + "AND MNCYE > " + dblAllYcylAvg);
                if (drTemp.Count() > 0)
                    return drTemp.CopyToDataTable();
            }
            return new DataTable();
            //return dtTempOil;
        }
        
        public DataTable NewWaterGetLayer(string strJHS, DataTable dtDJS)//数模水井
        {
            DataTable dtTempWater = dtDJS.Clone();

            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            List<double> lstZrlAvg = new List<double>();

            DataTable dtMNCW = dtDJS.DefaultView.ToTable(true, "K");
            for (int k = 0; k < dtMNCW.Rows.Count; k++)
            {
                double dblZrlAvg = Convert.ToDouble(dtDJS.Compute("AVG(MNCS)", "JHS = '" + strJHS + "' AND K = " + dtMNCW.Rows[k]["K"]));
                lstZrlAvg.Add(dblZrlAvg);
            }
            DataRow[] drSJJHS = FrmLayerSelectedInfo.dtExcelJHDY.Select("数模井 = '" + strJHS + "'");
            //层位参数
            DataTable dtCWCS = cdu.SelectDatabase("select n.*, round(n.渗透率 * n.砂岩厚度, 4) KH from (select t.井号, avg(t.渗透率 * t.砂岩厚度) as avgkh, avg(t.渗透率) as avgk from T_WELL_DAA074 t group by 井号) a, T_WELL_DAA074 n where a.井号 = n.井号 and (n.渗透率 * n.砂岩厚度) > a.avgkh and n.渗透率 > a.avgk and a.井号 = '" + drSJJHS[0]["实际井"] + "'");
            //DataTable dtCWCS = cdu.SelectDatabase("select a.*, round(a.砂岩厚度 * a.渗透率, 4) KH from T_WELL_DAA074 a where a.井号 = '" + drSJJHS[0]["实际井"] + "'");
            for (int i = 0; i < dtMNCW.Rows.Count; i++)
            {
                DataRow[] drXC = FrmLayerSelectedInfo.dtExcelCWDY.Select("序号 = '" + dtMNCW.Rows[i]["K"] + "'");

                DataRow[] drCW = dtCWCS.Select("层位 = '" + drXC[0]["沉积单元"] + "'");
                if (drCW.Count() > 0)
                {
                    DataRow dr = dtTempWater.NewRow();
                    dr["JHS"] = strJHS;
                    dr["K"] = Convert.ToInt32(dtMNCW.Rows[i]["K"]);
                    dr["CW"] = drCW[0]["层位"].ToString();
                    dr["MNCS"] = lstZrlAvg[i];
                    dr["ZRQD"] = lstZrlAvg[i] / Convert.ToDouble(drCW[0]["砂岩厚度"]);
                    dr["SKH"] = Convert.ToDouble(drCW[0]["KH"]);
                    dr["SSYHD"] = Convert.ToDouble(drCW[0]["砂岩厚度"]);
                    dr["SSTL"] = Convert.ToDouble(drCW[0]["渗透率"]);
                    dr["SWX"] = Convert.ToDouble(drCW[0]["微相"]);
                    dtTempWater.Rows.Add(dr);
                }
            }
            return dtTempWater;
        }
        */
        /// <summary>
        /// 关联井数及层位
        /// </summary>
        /// <param name=""></param>
       
        public DataTable JTMethod(string strJHY, string strJHS)
        {
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            //油水井连通层参数
            return cdu.SelectVFP("select a.井号 as 油井号, a.层位, a.微相 as 油微相, a.渗透率 as 油渗透率, a.砂岩厚度 as 油砂岩厚度, b.井号 as 水井号, b.微相 as 水微相, b.渗透率 as 水渗透率, b.砂岩厚度 as 水砂岩厚度 from (select n.* from (select 井号, avg(渗透率 * 砂岩厚度) as avgkh, avg(渗透率) as avgk from T_DAA074 group by 井号) as a, T_DAA074 as n where a.井号 = n.井号 and (n.渗透率 * n.砂岩厚度) > a.avgkh and n.渗透率 > a.avgk and a.井号='" + strJHY + "') as a, (select n.* from (select 井号, avg(渗透率 * 砂岩厚度) as avgkh, avg(渗透率) as avgk from T_DAA074 group by 井号) as a, T_DAA074 as n where a.井号 = n.井号 and (n.渗透率 * n.砂岩厚度) > a.avgkh and n.渗透率 > a.avgk and a.井号 = '" + strJHS + "') as b where a.层位 = b.层位");

        }
        public DataTable dtDCB02(string strJHS)//数据库DCB02
        {
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            //string strSQL = "select new.*, s.zrl1, s.zrbfs1, s.csrq from T_WELL_DAA074 new, (select f.*, g.syhd, round((f.sumsyhd * f.zrl/ g.syhd), 4) as zrl1, round((f.sumsyhd * f.zrbfs/ g.syhd), 4) as zrbfs1 from (select jh, csrq, zrl, zrbfs, jdds1, jdds2, sum(sumsyhd) as syhd from (select d.*,  e.csrq, e.zrl, e.zrbfs, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc||dymc as cw from DAA074 t where t.syds <> 0) group by jh, cw) b, (select t.*, yczmc||dymc as cw from DAA074 t where skqk = 1) c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds)d, (select jh, csrq, zrl, zrbfs, jdds1, jdds2 from DCB02 where zrl <> 0 and zrbfs <> 0) e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2)) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2)))) f group by jh, csrq, zrl, zrbfs, jdds1, jdds2)g, (select d.*, e.csrq, e.zrl, e.zrbfs, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc||dymc as cw from DAA074 t where t.syds <> 0 and t.skqk = 1) group by jh, cw) b, (select t.*, yczmc||dymc as cw from DAA074 t) c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds)d, (select jh, csrq, zrl, zrbfs, jdds1, jdds2 from DCB02 where zrl <> 0 and zrbfs <> 0 and csrq > to_date('2012/01/01', 'yyyy/mm/dd')) e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2))) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2)))f where g.jh = f.jh and g.zrl = f.zrl and g.csrq = f.csrq and g.zrbfs = f.zrbfs and g.jdds1 = f.jdds1 and g.jdds2 = f.jdds2) s where s.jh = new.jh and s.cw = new.cw order by new.jh, s.csrq, new.cw ";

            string strSQL = "select t.*, a.* from (select f.jh as 井号, f.cw as 层位, f.csrq as 测试日期, f.sumsyhd as 砂岩厚度, round((f.sumsyhd * f.zrl/ g.syhd), 4) as 注入量, round((f.sumsyhd * f.zrbfs/ g.syhd), 4) as 注水百分数 from (select jh, csrq, zrl, zrbfs, jdds1, jdds2, sum(sumsyhd) as syhd from (select d.*,  e.csrq, e.zrl, e.zrbfs, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc+dymc as cw from DAA074 as t where t.syds <> 0) group by jh, cw) as b, (select t.*, yczmc+dymc as cw from DAA074 as t where skqk = '1') as c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds) as d, (select jh, csrq, zrl, zrbfs, jdds1, jdds2 from DCB02 where zrl <> 0 and zrbfs <> 0) as e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2)) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2)))) as f group by jh, csrq, zrl, zrbfs, jdds1, jdds2) as g, (select d.*, e.csrq, e.zrl, e.zrbfs, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc+dymc as cw from DAA074 as t where t.syds <> 0 and t.skqk = '1') group by jh, cw) as b, (select t.*, yczmc+dymc as cw from DAA074 as t) as c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds) as d, (select jh, csrq, zrl, zrbfs, jdds1, jdds2 from DCB02 where zrl <> 0 and zrbfs <> 0) as e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2))) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2))) as f where g.jh = f.jh and g.zrl = f.zrl and g.csrq = f.csrq and g.zrbfs = f.zrbfs and g.jdds1 = f.jdds1 and g.jdds2 = f.jdds2) as t, (select n.* from (select 井号, avg(渗透率 * 砂岩厚度) as avgkh, avg(渗透率) as avgk from T_DAA074 group by 井号) as a, T_DAA074 as n where a.井号 = n.井号 and (n.渗透率 * n.砂岩厚度) > a.avgkh and n.渗透率 > a.avgk and a.井号 = '" + strJHS + "') as a where a.井号 = t.井号 and a.层位 = t.层位";
            DataTable dtSCS = cdu.SelectVFP(strSQL);

            DataRow[] drWater = dtSCS.Select("测试日期 > #" + dtStartDate.AddMonths(-2) + "# AND 测试日期 < #" + dtEndDate.AddMonths(2) + "#");
            List<WellModel> lstWM = new List<WellModel>();
            DataTable dtWM = ListToDataTableUtil.ListToDataTable(lstWM);

            if (drWater.Count() > 0)
            {
                DataTable dtWater = drWater.CopyToDataTable();
                DataTable dtCW = dtWater.DefaultView.ToTable(true, "层位");
                for (int k = 0; k < dtCW.Rows.Count; k++)
                {
                    double dblHsAvg = Convert.ToDouble(dtWater.Compute("AVG(注入量)", "层位 = '" + dtCW.Rows[k]["层位"] + "'"));
                    
                    DataRow[] drTemp = dtWater.Select("层位 = '" + dtCW.Rows[k]["层位"] + "'");
                    DataRow drWM = dtWM.NewRow();
                    drWM["jhs"] = strJHS;
                    drWM["cw"] = dtCW.Rows[k]["层位"].ToString();
                    drWM["SSTL"] = Convert.ToDouble(drTemp[0]["渗透率"]);
                    drWM["SSYHD"] = Convert.ToDouble(drTemp[0]["砂岩厚度"]);
                    drWM["SWX"] = Convert.ToDouble(drTemp[0]["微相"]);
                    drWM["mncs"] = dblHsAvg;
                    dtWM.Rows.Add(drWM);

                }


            }
            return dtWM;
        }
        public DataTable Jt(string strJH)//静态井
        {
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            //油水井连通层参数
            return cdu.SelectVFP("select n.* from (select 井号, avg(渗透率 * 砂岩厚度) as avgkh, avg(渗透率) as avgk from T_DAA074 group by 井号) as a, T_DAA074 as n where a.井号 = n.井号 and (n.渗透率 * n.砂岩厚度) > a.avgkh and n.渗透率 > a.avgk and a.井号='" + strJH + "'");
        }

        public DataTable dtDCB01(string strJHY)//数据库DCB01
        {
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            string strSQL = "select t.井号, t.层位, to_date(t.测试日期,'mm/dd/yy') as 测试日期, t.砂岩厚度, t.含水, t.日产液量, a.* from (select f.jh as 井号, f.cw as 层位, f.csrq as 测试日期, f.sumsyhd as 砂岩厚度, f.hs as 含水, round((f.sumsyhd * f.rcyl1/ g.syhd), 4) as 日产液量 from (select jh, csrq, hs, rcyl1, jdds1, jdds2, sum(sumsyhd) as syhd from (select d.*,  e.csrq, e.hs, e.rcyl1, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc+dymc as cw from DAA074 as t where t.syds <> 0) group by jh, cw) as b, (select t.*, yczmc+dymc as cw from DAA074 as t where skqk = '1') as c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds) as d, (select jh, csrq, hs, rcyl1, jdds1, jdds2 from DCB01 where hs <> 0 and rcyl1 <> 0) as e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2)) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2)))) as f group by jh, csrq, hs, rcyl1, jdds1, jdds2) as g, (select d.*, e.csrq, e.hs, e.rcyl1, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc+dymc as cw from DAA074 as t where t.syds <> 0 and t.skqk = '1') group by jh, cw) as b, (select t.*, yczmc+dymc as cw from DAA074 t) as c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds) as d, (select jh, csrq, hs, rcyl1, jdds1, jdds2 from DCB01 where hs <> 0 and rcyl1 <> 0) as e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2))) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2))) as f where g.jh = f.jh and g.hs = f.hs and g.csrq = f.csrq and g.rcyl1 = f.rcyl1 and g.jdds1 = f.jdds1 and g.jdds2 = f.jdds2) as t, (select n.* from (select 井号, avg(渗透率 * 砂岩厚度) as avgkh, avg(渗透率) as avgk from T_DAA074 group by 井号) as a, T_DAA074 as n where a.井号 = n.井号 and (n.渗透率 * n.砂岩厚度) > a.avgkh and n.渗透率 > a.avgk and a.井号 = '" + strJHY + "') as a where a.井号 = t.井号 and a.层位 = t.层位";
            DataTable dtYCS = cdu.SelectVFP(strSQL);//基础参数

            List<WellModel> lstWM = new List<WellModel>();
            DataTable dtWM = ListToDataTableUtil.ListToDataTable(lstWM);
            
            DataRow[] drOil = dtYCS.Select("测试日期 > #" + dtStartDate.AddMonths(-2) + "# AND 测试日期 < #" + dtEndDate.AddMonths(2) + "#");
            if (drOil.Count() > 0)
            {
                DataTable dtOil = drOil.CopyToDataTable();
                DataTable dtCW = dtOil.DefaultView.ToTable(true, "层位");

                for (int i = 0; i < dtCW.Rows.Count; i++)
                {
                    double dblRcylAvg = Convert.ToDouble(dtOil.Compute("AVG(日产液量)", "层位 = '" + dtCW.Rows[i]["层位"] + "'"));
                    double dblHsAvg = Convert.ToDouble(dtOil.Compute("AVG(含水)", "层位 = '" + dtCW.Rows[i]["层位"] + "'"));

                    DataRow drWM = dtWM.NewRow();
                   
                    DataRow[] drTemp = dtOil.Select("层位 = '" + dtCW.Rows[i]["层位"] + "'");
                    drWM["jhy"] = strJHY;
                    drWM["cw"] = dtCW.Rows[i]["层位"].ToString();
                    drWM["ystl"] = Convert.ToDouble(drTemp[0]["渗透率"]);
                    drWM["ysyhd"] = Convert.ToDouble(drTemp[0]["砂岩厚度"]);
                    drWM["ywx"] = Convert.ToDouble(drTemp[0]["微相"]);
                    drWM["mncye"] = dblRcylAvg;
                    drWM["HS"] = dblHsAvg;
                    dtWM.Rows.Add(drWM);
                }
            }
            return dtWM;
        }

        /// <summary>
        /// 计算两个日期的月份差值
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns>月份差</returns>
        public static int Calc(DateTime d1, DateTime d2)
        {
            DateTime max = d1 > d2 ? d1 : d2;
            DateTime min = d1 > d2 ? d2 : d1;

            int yeardiff = max.Year - min.Year;
            int monthdiff = max.Month - min.Month;

            return yeardiff * 12 + monthdiff + 1;
        }

    }
}
