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
    /// 计算日产油HI, 日产水HI, 日注水量HI
    /// </summary>
    class ComputeHIDAL
    {

        public static DataTable dtOilHI = new DataTable("Table_OilHI");
        public static DataTable dtWaterHI = new DataTable("Table_WaterHI");
        public static DataTable dtOilFirstFilterHI1 = new DataTable();//高产水高产油高注入量
        public static DataTable dtOilFirstFilterHI2 = new DataTable();//高产水高产油低注入量
        public static DataTable dtOilFirstFilterHI3 = new DataTable();//低产水高产油高注入量
        public static DataTable dtOilFirstFilterHI4 = new DataTable();//低产水高产油低注入量
        public static DataTable dtOilFirstFilterHI5 = new DataTable();//低产水低产油高注入量
        public static DataTable dtOilFirstFilterHI6 = new DataTable();//低产水低产油低注入量
        public static DataTable dtOilFirstFilterHI7 = new DataTable();//高产水低产油高注入量
        public static DataTable dtOilFirstFilterHI8 = new DataTable();//高产水低产油低注入量
        public static DataTable dtOilSecondFilterHI1 = new DataTable();//高含水高流压
        public static DataTable dtOilSecondFilterHI2 = new DataTable();//低含水高流压
        public static DataTable dtOilSecondFilterHI3 = new DataTable();//低含水低流压
        public static DataTable dtOilSecondFilterHI4 = new DataTable();//高含水低流压
        public static DataTable dtWaterFirstFilterHI1 = new DataTable();
        public static DataTable dtWaterFirstFilterHI2 = new DataTable();
        public static DataTable dtWaterFirstFilterHI3 = new DataTable();
        public static DataTable dtWaterFirstFilterHI4 = new DataTable();


        public void ComputeOilHIMethod()
        {
            //FileStream fs = new FileStream(@"F:\研究生\油工数据\五厂项目\20161107\一次筛选.txt", FileMode.Append);
            //StreamWriter sw = new StreamWriter(fs);
            if (dtOilHI.Rows.Count == 0)
            {
                if (dtOilHI.Columns.Count == 0)
                {
                    dtOilHI.Columns.Add("JH", System.Type.GetType("System.String"));
                    dtOilHI.Columns.Add("RCYLHI", System.Type.GetType("System.Double"));
                    dtOilHI.Columns.Add("RCSLHI", System.Type.GetType("System.Double"));
                    dtOilHI.Columns.Add("SLPFHI", System.Type.GetType("System.Double"));
                }


                ConnDatabaseUtil cdu = new ConnDatabaseUtil();

                string strSQL = string.Empty;

                double dblTotalRcylHI = 0, dblTotalRcslHI = 0, dblTotalSlpfHI = 0;
                double dblSlpfMax = 0, dblSlpfMin = 0;
                strSQL = "select jh, ny, rcsl, rcyl from dba04 where ny between '" + MainForm.strStartDate + "' and '" + MainForm.strEndDate + "' order by jh asc, ny asc"; // 在油井库里将油井按井号 年月排序
                DataTable dtDBA04 = cdu.SelectVFP(strSQL);
                //DataTable dtDBA04 = cdu.SelectDatabase(strSQL);
                DataTable dtDBA04JH = dtDBA04.DefaultView.ToTable(true, "JH");
                for (int i = 0; i < dtDBA04JH.Rows.Count; i++)
                {
                    DataRow[] rows = dtDBA04.Select("JH = '" + dtDBA04JH.Rows[i]["JH"] + "'");
                    if (rows.Count() > 0)
                    {
                        double dblRcylMax = GroupFunction(dtDBA04, "MAX", "RCYL", "JH = '" + dtDBA04JH.Rows[i]["JH"] + "'");
                        double dblRcylMin = GroupFunction(dtDBA04, "MIN", "RCYL", "JH = '" + dtDBA04JH.Rows[i]["JH"] + "'");
                        double dblRcslMax = GroupFunction(dtDBA04, "MAX", "RCSL", "JH = '" + dtDBA04JH.Rows[i]["JH"] + "'");
                        double dblRcslMin = GroupFunction(dtDBA04, "MIN", "RCSL", "JH = '" + dtDBA04JH.Rows[i]["JH"] + "'");
                        if (SlpfDAL.dtSLPF.Rows.Count > 0)
                        {
                            DataRow[] dr = SlpfDAL.dtSLPF.Select("JH = '" + dtDBA04JH.Rows[i]["JH"] + "' AND NY >= '" + MainForm.strStartDate + "' AND NY <= '" + MainForm.strEndDate + "'");
                            if (dr.Count() > 0)
                            {
                                dblSlpfMax = GroupFunction(SlpfDAL.dtSLPF, "MAX", "SLPF", "JH = '" + dtDBA04JH.Rows[i]["JH"] + "' AND NY >= '" + MainForm.strStartDate + "' AND NY <= '" + MainForm.strEndDate + "'");
                                dblSlpfMin = GroupFunction(SlpfDAL.dtSLPF, "MIN", "SLPF", "JH = '" + dtDBA04JH.Rows[i]["JH"] + "' AND NY >= '" + MainForm.strStartDate + "' AND NY <= '" + MainForm.strEndDate + "'");
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            SlpfDAL slpf = new SlpfDAL();
                            ///前台创建提示 先计算水量劈分
                            slpf.GetSlpfData();
                        }



                        if (((dblRcylMax - dblRcylMin) != 0) && ((dblRcslMax - dblRcslMin) != 0) && ((dblSlpfMax - dblSlpfMin) != 0))
                        {
                            foreach (DataRow dr in rows)
                            {
                                DataRow[] drSlpf = SlpfDAL.dtSLPF.Select("JH = '" + dr["JH"] + "' AND NY = '" + dr["NY"] + "'");
                                if (drSlpf.Count() > 0)
                                {
                                    double dblRcylAvg = GroupFunction(dtDBA04, "AVG", "RCYL", "NY = '" + dr["NY"] + "'");
                                    double dblRcslAvg = GroupFunction(dtDBA04, "AVG", "RCSL", "NY = '" + dr["NY"] + "'");
                                    double dblSlpfAvg = GroupFunction(SlpfDAL.dtSLPF, "AVG", "SLPF", "NY = '" + dr["NY"] + "'");

                                    dblTotalRcylHI += (Convert.ToDouble(dr["RCYL"]) - dblRcylAvg) / (dblRcylMax - dblRcylMin);
                                    dblTotalRcslHI += (Convert.ToDouble(dr["RCSL"]) - dblRcslAvg) / (dblRcslMax - dblRcslMin);
                                    dblTotalSlpfHI += (Convert.ToDouble(drSlpf[0]["SLPF"]) - dblSlpfAvg) / (dblSlpfMax - dblSlpfMin);
                                }

                            }
                            DataRow drHI = dtOilHI.NewRow();
                            drHI["JH"] = dtDBA04JH.Rows[i]["JH"];
                            drHI["RCYLHI"] = dblTotalRcylHI;
                            drHI["RCSLHI"] = dblTotalRcslHI;
                            drHI["SLPFHI"] = dblTotalSlpfHI;
                            dtOilHI.Rows.Add(drHI);
                            
                            dblTotalRcylHI = 0;
                            dblTotalRcslHI = 0;
                            dblTotalSlpfHI = 0;
                        }
                    }
                }

            }
            DataRow[] drTemp = dtOilHI.Select("RCSLHI > 0 AND RCYLHI >0 AND SLPFHI > 0");
            if (drTemp.Count() > 0)
            {
                dtOilFirstFilterHI1 = drTemp.CopyToDataTable();
            }
            drTemp = dtOilHI.Select("RCSLHI > 0 AND RCYLHI >0 AND SLPFHI < 0");
            if (drTemp.Count() > 0)
            {
                dtOilFirstFilterHI2 = drTemp.CopyToDataTable();
            }
            drTemp = dtOilHI.Select("RCSLHI < 0 AND RCYLHI >0 AND SLPFHI > 0");
            if (drTemp.Count() > 0)
            {
                dtOilFirstFilterHI3 = drTemp.CopyToDataTable();
            }
            drTemp = dtOilHI.Select("RCSLHI < 0 AND RCYLHI >0 AND SLPFHI < 0");
            if (drTemp.Count() > 0)
            {
                dtOilFirstFilterHI4 = drTemp.CopyToDataTable();
            }
            drTemp = dtOilHI.Select("RCSLHI < 0 AND RCYLHI < 0 AND SLPFHI > 0");
            if (drTemp.Count() > 0)
            {
                dtOilFirstFilterHI5 = drTemp.CopyToDataTable();
            }
            drTemp = dtOilHI.Select("RCSLHI < 0 AND RCYLHI <0 AND SLPFHI < 0");
            if (drTemp.Count() > 0)
            {
                dtOilFirstFilterHI6 = drTemp.CopyToDataTable();
            }
            drTemp = dtOilHI.Select("RCSLHI > 0 AND RCYLHI < 0 AND SLPFHI > 0");
            if (drTemp.Count() > 0)
            {
                dtOilFirstFilterHI7 = drTemp.CopyToDataTable();
                //foreach (DataRow dr in dtOilFirstFilterHI7.Rows)
                //{
                //    sw.WriteLine(dr["JH"] + " " + dr["RCSLHI"] + " " + dr["RCYLHI"] + " " + dr["SLPFHI"] + " ");
                //}
                
            }
            drTemp = dtOilHI.Select("RCSLHI > 0 AND RCYLHI <0 AND SLPFHI < 0");
            if (drTemp.Count() > 0)
            {
                dtOilFirstFilterHI8 = drTemp.CopyToDataTable();
            }
            //sw.Close();
            //fs.Close();

        }

        public void OilSecondFilterHIMethod()
        {
            //FileStream fs = new FileStream(@"F:\研究生\油工数据\五厂项目\20161107\LYHS2.txt", FileMode.Append);
            //StreamWriter sw = new StreamWriter(fs);

            DataTable dtOilSecondFilterHI = dtOilFirstFilterHI1.Copy();
            dtOilSecondFilterHI.Merge(dtOilFirstFilterHI2);
            dtOilSecondFilterHI.Merge(dtOilFirstFilterHI5);
            dtOilSecondFilterHI.Merge(dtOilFirstFilterHI6);

            dtOilSecondFilterHI.Columns.Add("LYHI", System.Type.GetType("System.Double"));
            dtOilSecondFilterHI.Columns.Add("HSHI", System.Type.GetType("System.Double"));
            if (dtOilSecondFilterHI.Rows.Count > 0)
            {
                ConnDatabaseUtil cdu = new ConnDatabaseUtil();

                string strSQL = string.Empty;

                double dblTotalLyHI = 0, dblTotalHsHI = 0;
                strSQL = "select jh, ny, ly, hs from dba04 where ny between '" + MainForm.strStartDate + "' and '" + MainForm.strEndDate + "' order by jh asc, ny asc"; // 在油井库里将油井按井号 年月排序
                DataTable dtDBA04 = cdu.SelectVFP(strSQL);
                //DataTable dtDBA04 = cdu.SelectDatabase(strSQL);

                foreach (DataRow dr in dtOilSecondFilterHI.Rows)
                {

                    DataRow[] rows = dtDBA04.Select("JH = '" + dr["JH"] + "'");
                    if (rows.Count() > 0)
                    {
                        double dblLyMax = GroupFunction(dtDBA04, "MAX", "LY", "JH = '" + dr["JH"] + "'");
                        double dblLyMin = GroupFunction(dtDBA04, "MIN", "LY", "JH = '" + dr["JH"] + "'");
                        double dblHsMax = GroupFunction(dtDBA04, "MAX", "HS", "JH = '" + dr["JH"] + "'");
                        double dblHsMin = GroupFunction(dtDBA04, "MIN", "HS", "JH = '" + dr["JH"] + "'");



                        if (((dblLyMax - dblLyMin) != 0) && ((dblHsMax - dblHsMin) != 0))
                        {
                            foreach (DataRow drLyHs in rows)
                            {

                                double dblLyAvg = GroupFunction(dtDBA04, "AVG", "LY", "NY = '" + drLyHs["NY"] + "'");
                                double dblHsAvg = GroupFunction(dtDBA04, "AVG", "HS", "NY = '" + drLyHs["NY"] + "'");

                                dblTotalLyHI += (Convert.ToDouble(drLyHs["LY"]) - dblLyAvg) / (dblLyMax - dblLyMin);
                                dblTotalHsHI += (Convert.ToDouble(drLyHs["HS"]) - dblHsAvg) / (dblHsMax - dblHsMin);


                            }
                            
                            dr["LYHI"] = dblTotalLyHI;
                            dr["HSHI"] = dblTotalHsHI;
                            
                            dblTotalLyHI = 0;
                            dblTotalHsHI = 0;
                            
                        }
                    }
                }
            }

            DataRow[] drTemp = dtOilSecondFilterHI.Select("HSHI > 0 AND LYHI >0");
            if (drTemp.Count() > 0)
            {
                dtOilSecondFilterHI1 = drTemp.CopyToDataTable();
                //for (int i = 0; i < drTemp.Count(); i++)
                //{
                //    sw.WriteLine(drTemp[i]["JH"] + " " + drTemp[i]["HSHI"] + " " + drTemp[i]["LYHI"]);
                //}
                
            }
            drTemp = dtOilSecondFilterHI.Select("HSHI < 0 AND LYHI >0");
            if (drTemp.Count() > 0)
            {
                dtOilSecondFilterHI2 = drTemp.CopyToDataTable();
            }
            drTemp = dtOilSecondFilterHI.Select("HSHI < 0 AND LYHI <0");
            if (drTemp.Count() > 0)
            {
                dtOilSecondFilterHI3 = drTemp.CopyToDataTable();
            }
            drTemp = dtOilSecondFilterHI.Select("HSHI > 0 AND LYHI <0");
            if (drTemp.Count() > 0)
            {
                dtOilSecondFilterHI4 = drTemp.CopyToDataTable();
            }
            //sw.Close();
            //fs.Close();

        }
        public static DataTable DXOilMethod()
        {
            DataTable dtDXOil = dtOilSecondFilterHI1.Copy();
            dtDXOil.Merge(dtOilFirstFilterHI7);
            return dtDXOil.DefaultView.ToTable(true, "JH");

        }
        public void ComputeWaterHIMethod()
        {
            if (dtWaterHI.Rows.Count == 0)
            {

                if (dtWaterHI.Columns.Count == 0)
                {
                    dtWaterHI.Columns.Add("JH", System.Type.GetType("System.String"));
                    dtWaterHI.Columns.Add("SXSZSHI", System.Type.GetType("System.Double"));
                    dtWaterHI.Columns.Add("ZRQDHI", System.Type.GetType("System.Double"));
                }
                ConnDatabaseUtil cdu = new ConnDatabaseUtil();

                string strSQL = string.Empty;

                double dblTotalSxszsHI = 0, dblTotalZrqdHI = 0;
                strSQL = "select d.jh, d.ny, round(d.rzsl / d.yy, 4) as sxszs, round(d.rzsl / h.厚度, 4) as zrqd from DBA05 as d, (select 井号, sum(砂岩厚度) as 厚度 from T_DAA074 group by 井号) as h where d.jh = h.井号 and d.yy <> 0 and d.ny between '" + MainForm.strStartDate + "' and '" + MainForm.strEndDate + "' order by d.jh asc, d.ny asc"; // 在水井库里将水井按井号 年月排序
                DataTable dtDBA05 = cdu.SelectVFP(strSQL);
                //DataTable dtDBA05 = cdu.SelectDatabase(strSQL);
                DataTable dtDBA05JH = dtDBA05.DefaultView.ToTable(true, "JH");

                for (int i = 0; i < dtDBA05JH.Rows.Count; i++)
                {
                    DataRow[] rows = dtDBA05.Select("JH = '" + dtDBA05JH.Rows[i]["JH"] + "'");
                    if (rows.Count() > 0)
                    {
                        double dblSxszsMax = GroupFunction(dtDBA05, "MAX", "SXSZS", "JH = '" + dtDBA05JH.Rows[i]["JH"] + "'");
                        double dblSxszsMin = GroupFunction(dtDBA05, "MIN", "SXSZS", "JH = '" + dtDBA05JH.Rows[i]["JH"] + "'");
                        double dblZrqdMax = GroupFunction(dtDBA05, "MAX", "ZRQD", "JH = '" + dtDBA05JH.Rows[i]["JH"] + "'");
                        double dblZrqdMin = GroupFunction(dtDBA05, "MIN", "ZRQD", "JH = '" + dtDBA05JH.Rows[i]["JH"] + "'");



                        if (((dblSxszsMax - dblSxszsMin) != 0) && ((dblZrqdMax - dblZrqdMin) != 0))
                        {
                            foreach (DataRow dr in rows)
                            {

                                double dblSxszsAvg = GroupFunction(dtDBA05, "AVG", "SXSZS", "NY = '" + dr["NY"] + "'");
                                double dblZrqdAvg = GroupFunction(dtDBA05, "AVG", "ZRQD", "NY = '" + dr["NY"] + "'");

                                dblTotalSxszsHI += (Convert.ToDouble(dr["SXSZS"]) - dblSxszsAvg) / (dblSxszsMax - dblSxszsMin);
                                dblTotalZrqdHI += (Convert.ToDouble(dr["ZRQD"]) - dblZrqdAvg) / (dblZrqdMax - dblZrqdMin);

                            }
                            DataRow drHI = dtWaterHI.NewRow();
                            drHI["JH"] = dtDBA05JH.Rows[i]["JH"];
                            drHI["SXSZSHI"] = dblTotalSxszsHI;
                            drHI["ZRQDHI"] = dblTotalZrqdHI;
                            dtWaterHI.Rows.Add(drHI);

                            dblTotalSxszsHI = 0;
                            dblTotalZrqdHI = 0;
                        }
                    }
                }
            }



            DataRow[] drTemp = dtWaterHI.Select("ZRQDHI > 0 AND SXSZSHI >0");
            //DataRow[] drTemp = dtWaterHI.Select("ZRQDHI > -0.5 AND SXSZSHI >-0.5");
            if (drTemp.Count() > 0)
            {
                dtWaterFirstFilterHI1 = drTemp.CopyToDataTable();
            }
            drTemp = dtWaterHI.Select("ZRQDHI < 0 AND SXSZSHI >0");
            if (drTemp.Count() > 0)
            {
                dtWaterFirstFilterHI2 = drTemp.CopyToDataTable();
            }
            drTemp = dtWaterHI.Select("ZRQDHI < 0 AND SXSZSHI <0");
            if (drTemp.Count() > 0)
            {
                dtWaterFirstFilterHI3 = drTemp.CopyToDataTable();
            }
            drTemp = dtWaterHI.Select("ZRQDHI > 0 AND SXSZSHI <0");
            if (drTemp.Count() > 0)
            {
                dtWaterFirstFilterHI4 = drTemp.CopyToDataTable();
            }
            
        }

        public static DataTable DXWaterMethod()
        {
            DataTable dtDXWater = dtWaterFirstFilterHI1.Copy();
            return dtDXWater.DefaultView.ToTable(true, "JH");
        }
        private double GroupFunction(DataTable dt, string type, string strColumnName, string strFilter)
        {
            if (type.ToUpper().Equals("MAX"))
                return Convert.ToDouble(dt.Compute("MAX(" + strColumnName + ")", strFilter));
            else if (type.ToUpper().Equals("MIN"))
                return Convert.ToDouble(dt.Compute("MIN(" + strColumnName + ")", strFilter));
            else if (type.ToUpper().Equals("AVG"))
                return Convert.ToDouble(dt.Compute("AVG(" + strColumnName + ")", strFilter));
            else
                return 0;
        }


    }
}

