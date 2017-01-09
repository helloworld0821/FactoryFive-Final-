using DXApplication1.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DXApplication1.DAL
{
    ///未测试
    /// <summary>
    /// 水量劈分 全区
    /// </summary>
    class SlpfDAL
    {

        public static DataTable dtSLPF = new DataTable("Table_SLPF");
        private void CreateSlpfDB()
        {
            string strTableName = "T_SLPF";
            string strTablePath = strTableName + ".dbf";
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            string strSQL;
            //Oledb
            if (File.Exists("./Data/" + strTablePath))
            {
                File.Delete("./Data/" + strTablePath);
            }

            strSQL = "select f.jh, f.cw, f.rq, round(g.渗透率*g.砂岩厚度,4) as kh from (select distinct jh, cw, rq from (select d.*, e.rq, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc+dymc as cw from DAA074 as t where t.syds <> 0) group by jh, cw) as b, (select t.*, yczmc+dymc as cw from DAA074 as t) as c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds) as d, (select jh, rq, jdds1, jdds2 from DAA091) as e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2)) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2))))) as f, T_DAA074 as g where f.jh = g.井号 and f.cw = g.层位";
            DataTable dt = cdu.SelectVFP(strSQL);

            strSQL = "create table " + strTableName.ToUpper() + " (jh varchar(16), cw varchar(16), rq date, kh Decimal(16, 5))";
            cdu.CreateOrDeleteVFP(strSQL);
            cdu.InsertVFP(dt, strTableName.ToUpper());
           
        }
        public void GetSlpfData()
        {
            //FileStream fsw = new FileStream(@"F:\研究生\油工数据\五厂项目\20161109\kh2.txt", FileMode.Append);
            //StreamWriter sw = new StreamWriter(fsw);
            if (dtSLPF.Columns.Count == 0)
            {
                dtSLPF.Columns.Add("JH", System.Type.GetType("System.String"));
                dtSLPF.Columns.Add("NY", System.Type.GetType("System.String"));
                dtSLPF.Columns.Add("SLPF", System.Type.GetType("System.Double"));
                dtSLPF.Columns.Add("DYHS", System.Type.GetType("System.Double"));
            }

            //if (dtSLPF.Rows.Count == 0)
            //{
            //    FileStream fs = new FileStream("处理连通库水量劈分.txt", FileMode.Open);
            //    StreamReader sr = new StreamReader(fs);
            //    string strReadLine = sr.ReadLine();
            //    while (strReadLine != null)
            //    {
            //        string[] strArray = Regex.Split(strReadLine, @"\s+");
            //        DataRow dr = dtSLPF.NewRow();
            //        dr["NY"] = strArray[0];
            //        dr["JH"] = strArray[1];
            //        dr["SLPF"] = Convert.ToDouble(strArray[2]);
            //        dtSLPF.Rows.Add(dr);

            //        strReadLine = sr.ReadLine();
            //    }
            //    sr.Close();
            //    fs.Close();

            //}

            //整个代码块 -   从数据库中计算
            if (dtSLPF.Rows.Count >= 0)
            {
                dtSLPF.Clear();
                CreateSlpfDB();
                
                ConnDatabaseUtil cdu = new ConnDatabaseUtil();
                DataTable dtSLPFTable = cdu.SelectVFP("select * from T_SLPF");
                string jhy = string.Empty, jhs = string.Empty;

                string strSQL = string.Empty;


                DateTime date;
                double totalInj = 0;
                double currentKH = 0;
                double totalKH = 0;
                strSQL = "select jh, ny from DBA04 where ny between '" + MainForm.strStartDate + "' and '" + MainForm.strEndDate + "' order by jh asc, ny asc"; // 在油井库里将油井按井号 年月排序
                DataTable dtOil = cdu.SelectVFP(strSQL);
                //DataTable dtOil = cdu.SelectDatabase(strSQL);

                for (int m = 0; m < dtOil.Rows.Count; m++)
                {
                    date = DateTime.ParseExact((string)dtOil.Rows[m]["NY"], "yyyyMM", null);
                    strSQL = "select distinct jhs, jhy from " + MainForm.strLTKName + " where jhy = '" + (string)dtOil.Rows[m]["JH"] + "'";//查询当前油井连通的水井
                    DataTable dtLTWater = cdu.SelectVFP(strSQL);
                    //DataTable dtLTWater = cdu.SelectDatabase(strSQL);


                    if (dtLTWater.Rows.Count == 0)
                    {
                        strSQL = "select count(jh) as cnt from DBA04 where jh = '" + (string)dtOil.Rows[m]["JH"] + "'";//查询当前油井个数

                        DataTable dtCNT = cdu.SelectVFP(strSQL);
                        //DataTable dtCNT = cdu.SelectDatabase(strSQL);
                        m += Convert.ToInt32(dtCNT.Rows[0]["CNT"]) - 1;
                        continue;
                    }
                    else
                    {
                        //strSQL = "select * from T_SLPF where jh = '" + (string)dtOil.Rows[m]["JH"] + "' and rq < to_date('" + date + "', 'yyyy/mm/dd hh24:mi:ss')";
                        DataTable dtYSK = new DataTable();//油射孔
                        DataRow[] drYSK = dtSLPFTable.Select("JH = '" + (string)dtOil.Rows[m]["JH"] + "' AND RQ < #" + date + "#");
                        if (drYSK.Count() > 0)
                        {
                            dtYSK = drYSK.CopyToDataTable();
                        }
                        for (int i = 0; i < dtLTWater.Rows.Count; i++)
                        {
                            //strSQL = "select * from T_SLPF  where jh = '" + (string)dtLTWater.Rows[i]["JHS"] + "' and rq < to_date('" + date + "', 'yyyy/mm/dd hh24:mi:ss')";
                            //DataTable dtSSK = cdu.SelectDatabase(strSQL);//水射孔
                            DataTable dtSSK = new DataTable();//油射孔
                            DataRow[] drSSK = dtSLPFTable.Select("JH = '" + (string)dtLTWater.Rows[i]["JHS"] + "' AND RQ < #" + date + "#");
                            if (drSSK.Count() > 0)
                            {
                                dtSSK = drSSK.CopyToDataTable();
                            }
                            if (dtSSK.Rows.Count > 0)
                            {
                                for (int j = 0; j < dtYSK.Rows.Count; j++)
                                {
                                    DataRow[] rows = dtSSK.Select("CW = '" + (string)dtYSK.Rows[j]["CW"] + "'");
                                    if (rows.Count() != 0)
                                    {
                                        currentKH += ((Convert.ToDouble(dtYSK.Rows[j]["KH"]) + Convert.ToDouble(rows[0]["KH"])) / 2);

                                    }
                                }
                                strSQL = "select distinct jhy, jhs from LTK where jhs = '" + (string)dtLTWater.Rows[i]["JHS"] + "'";
                                DataTable dtOthrOil = cdu.SelectVFP(strSQL);
                                //DataTable dtOthrOil = cdu.SelectDatabase(strSQL);
                                for (int j = 0; j < dtOthrOil.Rows.Count; j++) //查找其他油井于当前水井所在的连通层
                                {
                                    //strSQL = "select * from T_WELL_SLPF where jh = '" + (string)dtOthrOil.Rows[j]["JHY"] + "' and rq < to_date('" + date + "', 'yyyy/mm/dd hh24:mi:ss')";
                                    //DataTable dtOthrOilSK = cdu.SelectDatabase(strSQL);

                                    DataTable dtOthrOilSK = new DataTable();
                                    DataRow[] drOthrOilSK = dtSLPFTable.Select("JH = '" + (string)dtOthrOil.Rows[j]["JHY"] + "' AND RQ < #" + date + "#");
                                    if (drOthrOilSK.Count() > 0)
                                    {
                                        dtOthrOilSK = drOthrOilSK.CopyToDataTable();
                                    }

                                    for (int k = 0; k < dtOthrOilSK.Rows.Count; k++)
                                    {
                                        DataRow[] rows = dtSSK.Select("CW = '" + (string)dtOthrOilSK.Rows[k]["CW"] + "'");
                                        if (rows.Count() != 0)
                                        {
                                            totalKH += ((Convert.ToDouble(dtOthrOilSK.Rows[k]["KH"]) + Convert.ToDouble(rows[0]["KH"])) / 2);
                                        }
                                    }
                                }
                                if (totalKH != 0)
                                {
                                    double ratio = currentKH / totalKH;

                                    strSQL = "select jh, ny, round(yzsl * 30 / scts, 4) as yzsl1 from DBA05 where scts <> 0 and jh= '" + (string)dtLTWater.Rows[i]["JHS"] + "' and ny = '" + date.ToString("yyyyMM") + "'";
                                    DataTable dtZRL = cdu.SelectVFP(strSQL);
                                    //DataTable dtZRL = cdu.SelectDatabase(strSQL);
                                    if (dtZRL.Rows.Count > 0)
                                    {

                                        double dblQwo = Convert.ToDouble(dtZRL.Rows[0]["YZSL1"]) * ratio;
                                        totalInj += dblQwo;
                                    }

                                }

                                totalKH = 0;
                                currentKH = 0;
                            }
                        }
                        DataRow dr = dtSLPF.NewRow();
                        dr["JH"] = dtOil.Rows[m]["JH"];
                        dr["NY"] = dtOil.Rows[m]["NY"];
                        dr["SLPF"] = totalInj;
                        dtSLPF.Rows.Add(dr);

                        totalInj = 0;

                    }

                }
            }
            
        }

    }
}
