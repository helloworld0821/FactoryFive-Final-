using DXApplication1.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.DAL
{
    class GldDAL
    {

        ///未测试
        /// <summary>
        /// 关联度 全区
        /// </summary>
        
        public static DataTable dtGLD = new DataTable("Table_GLD");
        public void GetGldData()
        {
            if (dtGLD.Rows.Count == 0)
            {
                if (dtGLD.Columns.Count == 0)
                {
                    dtGLD.Columns.Add("JHS", System.Type.GetType("System.String"));
                    dtGLD.Columns.Add("JHY", System.Type.GetType("System.String"));
                    dtGLD.Columns.Add("GLD", System.Type.GetType("System.Double"));
                }
                ConnDatabaseUtil cdu = new ConnDatabaseUtil();
                string jhy = string.Empty, jhs = string.Empty;

                string strSQL = string.Empty;

                double fz = 0;//分子
                double yfm = 0, sfm = 0;//油分母，水分母

                strSQL = "select jh, ny, round((ycyl+ycsl) * 30 / scts, 4) as ycyl1 from DBA04 where scts <> 0 and ny between '" + MainForm.strStartDate + "' and '" + MainForm.strEndDate + "' order by jh asc, ny asc";
                DataTable dtYCYL = cdu.SelectDatabase(strSQL);
                DataTable dtYCYLJH = dtYCYL.DefaultView.ToTable(true, "JH");
                for (int i = 0; i < dtYCYLJH.Rows.Count; i++)
                {
                    double ya = Convert.ToDouble(dtYCYL.Compute("AVG(YCYL1)", "JH = '" + dtYCYLJH.Rows[i]["JH"] + "'"));//油井月产液量平均值
                    DataTable dtDjYCYL = dtYCYL.Select("JH = '" + dtYCYLJH.Rows[i]["JH"] + "'", "NY ASC").CopyToDataTable();//当前单井月产液量
                    int cntYNY = Convert.ToInt32(dtDjYCYL.Compute("COUNT(NY)", ""));//当前油井月份数
                    strSQL = "select distinct * from " + MainForm.strLTKName + " where jhy = '" + dtYCYLJH.Rows[i]["JH"] + "'";
                    DataTable dtLTWater = cdu.SelectDatabase(strSQL);
                    for (int j = 0; j < dtLTWater.Rows.Count; j++)
                    {
                        strSQL = "select d.jh, d.ny, round(d.yzsl * 30 / d.scts, 4) as yzsl1 from DBA05 d where d.scts <> 0 and jh = '" + dtLTWater.Rows[j]["JHS"] + "' and ny between '" + MainForm.strStartDate + "' and '" + MainForm.strEndDate + "' order by jh asc, ny asc";
                        DataTable dtYZSL = cdu.SelectDatabase(strSQL);
                        if (dtYZSL.Rows.Count > 0)
                        {
                            double sa = Convert.ToDouble(dtYZSL.Compute("AVG(YZSL1)", ""));//水井月注水量平均值

                            for (int k = 0; k < cntYNY; k++)
                            {
                                DataRow[] sny = dtYZSL.Select("NY = '" + dtDjYCYL.Rows[k]["NY"] + "'");
                                if (sny.Count() > 0)
                                {
                                    fz += (Convert.ToDouble(sny[0]["YZSL1"]) - sa) * (Convert.ToDouble(dtDjYCYL.Rows[k]["YCYL1"]) - ya);
                                    sfm += Math.Pow((Convert.ToDouble(sny[0]["YZSL1"]) - sa), 2);
                                    yfm += Math.Pow((Convert.ToDouble(dtDjYCYL.Rows[k]["YCYL1"]) - ya), 2);
                                }
                            }
                            sfm = Math.Pow(sfm, 0.5);
                            yfm = Math.Pow(yfm, 0.5);
                            double r = fz / (sfm * yfm);
                            fz = 0;
                            sfm = 0;
                            yfm = 0;
                            DataRow dr = dtGLD.NewRow();
                            dr["JHS"] = dtLTWater.Rows[j]["JHS"];
                            dr["JHY"] = dtYCYLJH.Rows[i]["JH"];
                            dr["GLD"] = r;
                            dtGLD.Rows.Add(dr);
                        }
                    }
                }

            }
        }
    }
}
