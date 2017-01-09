using DXApplication1.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.DAL.DatabaseDAL
{
    class SelectWellIfmDAL
    {
        public DataTable GetOilWellIfm(DataTable dtOil, int chooseItem)
        {
            DataTable dt = new DataTable();
            DataTable dtTemp = new DataTable();
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            string strSQL = string.Empty;
            for (int i = 0; i < dtOil.Rows.Count; i++)
            {
                if (chooseItem == 1)
                {
                    //OLEDB
                    strSQL = "select d.jh as 井号, d.ny as 年月, d.ycsl as 月产水量, round(a.avgsl, 4) as 全区月产水量平均值, d.ycyl as 月产液量, round(a.avgyl,4) as 全区月产液量平均值, d.bz as 备注 from dba04 as d, (select ny, avg(ycsl) as avgsl, avg(ycyl) as avgyl from dba04 group by ny having ny between '" + MainForm.strStartDate + "' and '" + MainForm.strEndDate + "') as a where a.ny = d.ny and d.jh = '" + dtOil.Rows[i]["JH"] + "' order by d.jh asc, d.ny asc";
                    dtTemp = cdu.SelectVFP(strSQL);
                    //Oracle
                    //strSQL = "select d.jh 井号, d.ny 年月, d.ycsl 月产水量, round(a.avgsl, 4) 全区月产水量平均值, d.ycyl 月产液量, round(a.avgyl,4) 全区月产液量平均值, d.bz 备注 from dba04 d, (select ny, avg(ycsl) as avgsl, avg(ycyl) as avgyl from dba04 group by ny having ny between '" + MainForm.strStartDate + "' and '" + MainForm.strEndDate + "') a where a.ny = d.ny and d.jh = '" + dtOil.Rows[i]["JH"] + "' order by d.jh asc, d.ny asc";
                    //dtTemp = cdu.SelectDatabase(strSQL);
                    dt.Merge(dtTemp);
                }
                else if (chooseItem == 2)
                {
                    //OLEDB
                    strSQL = "select d.jh as 井号, d.ny as 年月, d.ycsl as 月产水量, round(a.avgsl, 4) as 全区月产水量平均值, d.ycyl as 月产液量, round(a.avgyl,4) as 全区月产液量平均值, d.ly as 流压, round(a.aly,4) as 全区月流压平均值, d.hs as 含水, round(a.ahs,4) as 全区月含水平均值, d.bz as 备注 from dba04 as d, (select ny, avg(ycsl) as avgsl, avg(ycyl) as avgyl, avg(ly) as aly, avg(hs) as ahs from dba04 group by ny having ny between '" + MainForm.strStartDate + "' and '" + MainForm.strEndDate + "') as a where a.ny = d.ny and d.jh = '" + dtOil.Rows[i]["JH"] + "' order by d.jh asc, d.ny asc";
                    dtTemp = cdu.SelectVFP(strSQL);
                    //Oracle
                    //strSQL = "select d.jh 井号, d.ny 年月, d.ycsl 月产水量, round(a.avgsl, 4) 全区月产水量平均值, d.ycyl 月产液量, round(a.avgyl,4) 全区月产液量平均值, d.ly 流压, round(a.aly,4) 全区月流压平均值, d.hs 含水, round(a.ahs,4) 全区月含水平均值, d.bz 备注 from dba04 d, (select ny, avg(ycsl) as avgsl, avg(ycyl) as avgyl, avg(ly) as aly, avg(hs) ahs from dba04 group by ny having ny between '" + MainForm.strStartDate + "' and '" + MainForm.strEndDate + "') a where a.ny = d.ny and d.jh = '" + dtOil.Rows[i]["JH"] + "' order by d.jh asc, d.ny asc";
                    //dtTemp = cdu.SelectDatabase(strSQL);
                    dt.Merge(dtTemp);
                }
            }
            return dt;
        }
        public DataTable GetWaterWellIfm(DataTable dtWater)
        {
            DataTable dt = new DataTable();
            DataTable dtTemp = new DataTable();
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            string strSQL = string.Empty;
            for (int i = 0; i < dtWater.Rows.Count; i++)
            {
                //OLEDB
                strSQL = "select b.jh as 井号, b.ny as 年月, b.sxszs as 视吸水指数, round(a.asx, 4) as 全区平均视吸水指数, b.zrqd as 注入强度, round(a.azr, 4) as 全区平均注入强度, b.bz as 备注 from (select d.bz, d.jh, d.ny, round(d.rzsl / d.yy, 4) as sxszs, round(d.rzsl / h.厚度, 4) as zrqd from DBA05 as d, (select 井号, sum(砂岩厚度) as 厚度 from T_DAA074 group by 井号) as h where d.jh = h.井号 and d.yy <> 0) as b,(select ny, avg(sxszs) as asx, avg(zrqd) as azr from (select d.jh, d.ny, round(d.rzsl / d.yy, 4) as sxszs, round(d.rzsl / h.厚度, 4) as zrqd from DBA05 as d, (select 井号, sum(砂岩厚度) as 厚度 from T_DAA074 group by 井号) as h where d.jh = h.井号 and d.yy <> 0 and d.ny between '" + MainForm.strStartDate + "' and '" + MainForm.strEndDate + "') group by ny) as a where a.ny = b.ny and b.jh = '" + dtWater.Rows[i]["JH"] + "' order by a.ny asc";
                dtTemp = cdu.SelectVFP(strSQL);
                //Oracle
                //strSQL = "select b.jh 井号, b.ny 年月, b.sxszs 视吸水指数, round(a.asx, 4) 全区平均视吸水指数, b.zrqd 注入强度, round(a.azr, 4) 全区平均注入强度, b.bz 备注 from (select d.bz, d.jh, d.ny, round(d.rzsl / d.yy, 4) as sxszs, round(d.rzsl / h.厚度, 4) as zrqd from DBA05 d, (select 井号, sum(砂岩厚度) as 厚度 from T_WELL_DAA074 group by 井号) h where d.jh = h.井号 and d.yy <> 0) b,(select ny, avg(sxszs) asx, avg(zrqd) azr from (select d.jh, d.ny, round(d.rzsl / d.yy, 4) as sxszs, round(d.rzsl / h.厚度, 4) as zrqd from DBA05 d, (select 井号, sum(砂岩厚度) as 厚度 from T_WELL_DAA074 group by 井号) h where d.jh = h.井号 and d.yy <> 0 and d.ny between '" + MainForm.strStartDate + "' and '" + MainForm.strEndDate + "') group by ny) a where a.ny = b.ny and b.jh = '" + dtWater.Rows[i]["JH"] + "' order by a.ny asc";
                //dtTemp = cdu.SelectDatabase(strSQL);
                dt.Merge(dtTemp);
            }
            return dt;

        }
    }
}
