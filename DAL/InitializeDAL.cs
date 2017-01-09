using DXApplication1.DAL.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DXApplication1.Utils;
using DXApplication1.DAL.DatabaseDAL;
using System.IO;

namespace DXApplication1.DAL
{
    class InitializeDAL : IInitializeDAL
    {

        public DataTable LoadDate()
        {
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            string strSQL = "select distinct ny from DBA04 order by ny";
            return cdu.SelectVFP(strSQL);
            //return cdu.SelectDatabase(strSQL);
        }
        public void AmendDaa074()
        {
            AmendDaa074DAL add = new AmendDaa074DAL();
            add.AmendDaa074Method();
        }

        public void AmendDCB01()
        {
            string strTableName = "T_DCB01";
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            string strSQL;
            //Oledb
            string strTablePath = strTableName + ".dbf";
            if (File.Exists("./Data/" + strTablePath))
            {
                File.Delete("./Data/" + strTablePath);
            }
            strSQL = "select f.jh as 井号, f.cw as 层位, f.csrq as 测试日期, f.sumsyhd as 砂岩厚度, f.hs as 含水, round((f.sumsyhd * f.rcyl1/ g.syhd), 4) as 日产液量 from (select jh, csrq, hs, rcyl1, jdds1, jdds2, sum(sumsyhd) as syhd from (select d.*,  e.csrq, e.hs, e.rcyl1, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc+dymc as cw from DAA074 as t where t.syds <> 0) group by jh, cw) as b, (select t.*, yczmc+dymc as cw from DAA074 as t where skqk = '1') as c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds) as d, (select jh, csrq, hs, rcyl1, jdds1, jdds2 from DCB01 where hs <> 0 and rcyl1 <> 0) as e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2)) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2)))) as f group by jh, csrq, hs, rcyl1, jdds1, jdds2) as g, (select d.*, e.csrq, e.hs, e.rcyl1, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc+dymc as cw from DAA074 as t where t.syds <> 0 and t.skqk = '1') group by jh, cw) as b, (select t.*, yczmc+dymc as cw from DAA074 t) as c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds) as d, (select jh, csrq, hs, rcyl1, jdds1, jdds2 from DCB01 where hs <> 0 and rcyl1 <> 0) as e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2))) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2))) as f where g.jh = f.jh and g.hs = f.hs and g.csrq = f.csrq and g.rcyl1 = f.rcyl1 and g.jdds1 = f.jdds1 and g.jdds2 = f.jdds2";
            DataTable dtSKYT = cdu.SelectVFP(strSQL);

            strSQL = "create table " + strTableName.ToUpper() + " (井号 varchar(16), 层位 varchar(16), 测试日期 varchar(8), 砂岩厚度 Decimal(16, 5), 含水 Decimal(17,2), 日产液量 Decimal(17, 1))";
            cdu.CreateOrDeleteVFP(strSQL);
            cdu.InsertVFP(dtSKYT, strTableName.ToUpper());
        }

        public void AmendDCB02()
        {
            string strTableName = "T_DCB02";
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            string strSQL;
            //Oledb
            string strTablePath = strTableName + ".dbf";
            if (File.Exists("./Data/" + strTablePath))
            {
                File.Delete("./Data/" + strTablePath);
            }

            strSQL = "select f.jh as 井号, f.cw as 层位, f.csrq as 测试日期, f.sumsyhd as 砂岩厚度, round((f.sumsyhd * f.zrl/ g.syhd), 4) as 注入量, round((f.sumsyhd * f.zrbfs/ g.syhd), 4) as 注水百分数 from (select jh, csrq, zrl, zrbfs, jdds1, jdds2, sum(sumsyhd) as syhd from (select d.*,  e.csrq, e.zrl, e.zrbfs, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc+dymc as cw from DAA074 as t where t.syds <> 0) group by jh, cw) as b, (select t.*, yczmc+dymc as cw from DAA074 as t where skqk = '1') as c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds) as d, (select jh, csrq, zrl, zrbfs, jdds1, jdds2 from DCB02 where zrl <> 0 and zrbfs <> 0) as e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2)) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2)))) as f group by jh, csrq, zrl, zrbfs, jdds1, jdds2) as g, (select d.*, e.csrq, e.zrl, e.zrbfs, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc+dymc as cw from DAA074 as t where t.syds <> 0 and t.skqk = '1') group by jh, cw) as b, (select t.*, yczmc+dymc as cw from DAA074 as t) as c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds) as d, (select jh, csrq, zrl, zrbfs, jdds1, jdds2 from DCB02 where zrl <> 0 and zrbfs <> 0) as e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2))) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2))) as f where g.jh = f.jh and g.zrl = f.zrl and g.csrq = f.csrq and g.zrbfs = f.zrbfs and g.jdds1 = f.jdds1 and g.jdds2 = f.jdds2";
            DataTable dtSKYT = cdu.SelectVFP(strSQL);

            strSQL = "create table " + strTableName.ToUpper() + " (井号 varchar(16), 层位 varchar(16), 测试日期 date, 砂岩厚度 Decimal(16, 5), 注入量 Decimal(17,2), 注水百分数 Decimal(17, 2))";
            cdu.CreateOrDeleteVFP(strSQL);
            cdu.InsertVFP(dtSKYT, strTableName.ToUpper());
        }

    }
}
