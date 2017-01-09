using DXApplication1.DAL.IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using DXApplication1.Utils;

namespace DXApplication1.DAL
{
    class TreeNodeMouseClickDAL : ITreeNodeMouseClickDAL
    {

        public DataTable GetDataTable(string strNodeText)
        {
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            string strSQL = string.Empty;
            if (strNodeText.Equals("井号库"))
            {
                strSQL = "select jh as 井号 from DAA01";
            }
            else if (strNodeText.Equals("井位坐标库"))
            {
                strSQL = "select jh as 井号, zzbx as 纵坐标X, hzby as 横坐标Y from DAA02";
            }
            else if (strNodeText.Equals("连通库"))
            {
                strSQL = "select jhy as 油井号, jhs as 水井号 from " + MainForm.strLTKName;
            }
            else if (strNodeText.Equals("标准小层库"))
            {
                strSQL = "select 井号, 层位 from T_DAA074";
            }
            else if (strNodeText.Equals("射孔库"))
            {
                strSQL = "select 井号, 层位, 微相, 砂岩厚度, 有效厚度, 渗透率 from T_DAA074";
            }
            else if (strNodeText.Equals("油井井史库"))
            {
                strSQL = "select jh as 井号, ny as 年月, scts as 生产天数, ly as 流压, hs as 含水, rcyl as 日产油量, rcsl as 日产水量, ycyl as 月产油量, ycsl as 月产水量, ljcyl as 累积产油量, ljcsl as 累积产水量, bz as 备注 from DBA04";
            }
            else if (strNodeText.Equals("水井井史库"))
            {
                strSQL = "select jh as 井号, ny as 年月, scts as 生产天数, yy as 油压, rzsl as 日注水量,  yzsl as 月注水量, ljzsl as 累积注水量, bz as 备注 from DBA05";
            }
            else if (strNodeText.Equals("吸水剖面"))
            {
                strSQL = "select * from (select f.jh as 井号, f.cw as 层位, f.csrq as 测试日期, f.sumsyhd as 砂岩厚度, round((f.sumsyhd * f.zrl/ g.syhd), 4) as 注入量, round((f.sumsyhd * f.zrbfs/ g.syhd), 4) as 注水百分数 from (select jh, csrq, zrl, zrbfs, jdds1, jdds2, sum(sumsyhd) as syhd from (select d.*,  e.csrq, e.zrl, e.zrbfs, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc+dymc as cw from DAA074 as t where t.syds <> 0) group by jh, cw) as b, (select t.*, yczmc+dymc as cw from DAA074 as t where skqk = '1') as c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds) as d, (select jh, csrq, zrl, zrbfs, jdds1, jdds2 from DCB02 where zrl <> 0 and zrbfs <> 0) as e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2)) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2)))) as f group by jh, csrq, zrl, zrbfs, jdds1, jdds2) as g, (select d.*, e.csrq, e.zrl, e.zrbfs, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc+dymc as cw from DAA074 as t where t.syds <> 0 and t.skqk = '1') group by jh, cw) as b, (select t.*, yczmc+dymc as cw from DAA074 as t) as c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds) as d, (select jh, csrq, zrl, zrbfs, jdds1, jdds2 from DCB02 where zrl <> 0 and zrbfs <> 0) as e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2))) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2))) as f where g.jh = f.jh and g.zrl = f.zrl and g.csrq = f.csrq and g.zrbfs = f.zrbfs and g.jdds1 = f.jdds1 and g.jdds2 = f.jdds2)";
            }
            else if (strNodeText.Equals("产液剖面"))
            {
                strSQL = "select * from (select f.jh as 井号, f.cw as 层位, f.csrq as 测试日期, f.sumsyhd as 砂岩厚度, f.hs as 含水, round((f.sumsyhd * f.rcyl1/ g.syhd), 4) as 日产液量 from (select jh, csrq, hs, rcyl1, jdds1, jdds2, sum(sumsyhd) as syhd from (select d.*,  e.csrq, e.hs, e.rcyl1, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc+dymc as cw from DAA074 as t where t.syds <> 0) group by jh, cw) as b, (select t.*, yczmc+dymc as cw from DAA074 as t where skqk = '1') as c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds) as d, (select jh, csrq, hs, rcyl1, jdds1, jdds2 from DCB01 where hs <> 0 and rcyl1 <> 0) as e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2)) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2)))) as f group by jh, csrq, hs, rcyl1, jdds1, jdds2) as g, (select d.*, e.csrq, e.hs, e.rcyl1, e.jdds1, e.jdds2 from (select b.jh, b.cw, b.minsyds, (b.syds + c.syhd) as maxsyds, b.sumsyhd from (select jh, cw, min(syds) as minsyds, max(syds) as syds, sum(syhd) as sumsyhd from (select t.*, yczmc+dymc as cw from DAA074 as t where t.syds <> 0 and t.skqk = '1') group by jh, cw) as b, (select t.*, yczmc+dymc as cw from DAA074 t) as c where b.jh = c.jh and b.cw = c.cw and b.syds = c.syds) as d, (select jh, csrq, hs, rcyl1, jdds1, jdds2 from DCB01 where hs <> 0 and rcyl1 <> 0) as e where d.jh = e.jh and ((d.minsyds between e.jdds1 and e.jdds2) or (d.maxsyds between e.jdds1 and e.jdds2) or ((d.minsyds > e.jdds1) and (d.maxsyds < e.jdds2))) or ((d.minsyds < e.jdds1) and (d.maxsyds > e.jdds2))) as f where g.jh = f.jh and g.hs = f.hs and g.csrq = f.csrq and g.rcyl1 = f.rcyl1 and g.jdds1 = f.jdds1 and g.jdds2 = f.jdds2)";
            }
            return cdu.SelectVFP(strSQL);
            //return cdu.SelectDatabase(strSQL);
        }

        public DataTable GetCompute(string strNodeText)
        {
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            string strSQL = string.Empty;
            DataTable dt = new DataTable();
            if (strNodeText.Equals("吨油耗水"))
            {
                DyhsDAL dyhs = new DyhsDAL();
                dyhs.GetDyhsData();
                dt = SlpfDAL.dtSLPF;
            }
            return dt;
        }
        public void ShowHIImg(string strParentNodeText, string strNodeText)
        {
            ComputeHIDAL chid = new ComputeHIDAL();

            if (strParentNodeText.Equals("低效循环油井识别"))
            {
                if (strNodeText.Equals("一次筛选"))
                {
                    SlpfDAL slpf = new SlpfDAL();
                    slpf.GetSlpfData();
                    ComputeHIDAL.dtOilFirstFilterHI1.Clear();
                    ComputeHIDAL.dtOilFirstFilterHI2.Clear();
                    ComputeHIDAL.dtOilFirstFilterHI3.Clear();
                    ComputeHIDAL.dtOilFirstFilterHI4.Clear();
                    ComputeHIDAL.dtOilFirstFilterHI5.Clear();
                    ComputeHIDAL.dtOilFirstFilterHI6.Clear();
                    ComputeHIDAL.dtOilFirstFilterHI7.Clear();
                    ComputeHIDAL.dtOilFirstFilterHI8.Clear();
                    ComputeHIDAL.dtOilHI.Clear();
                    chid.ComputeOilHIMethod();

                }
                else if (strNodeText.Equals("二次筛选"))
                {
                    ComputeHIDAL.dtOilSecondFilterHI1.Clear();
                    ComputeHIDAL.dtOilSecondFilterHI2.Clear();
                    ComputeHIDAL.dtOilSecondFilterHI3.Clear();
                    ComputeHIDAL.dtOilSecondFilterHI4.Clear();
                    chid.OilSecondFilterHIMethod();
                }
            }
            else if (strParentNodeText.Equals("低效循环水井识别"))
            {

                if (strNodeText.Equals("一次筛选"))
                {
                    ComputeHIDAL.dtWaterFirstFilterHI1.Clear();
                    ComputeHIDAL.dtWaterFirstFilterHI2.Clear();
                    ComputeHIDAL.dtWaterFirstFilterHI3.Clear();
                    ComputeHIDAL.dtWaterFirstFilterHI4.Clear();
                    ComputeHIDAL.dtWaterHI.Clear();
                    chid.ComputeWaterHIMethod();

                }

            }

        }

        public DataTable IdeDxLayer(string strNodeText)
        {
            DataTable dt = new DataTable();
            if (strNodeText.Equals("层位识别"))
            {
                //GldDAL gld = new GldDAL();
                //gld.GetGldData();

                DxxhcwsbDAL dd = new DxxhcwsbDAL();
                dt = dd.DxxhcesbMethod();
            }


            return dt;
        }
    }
}
