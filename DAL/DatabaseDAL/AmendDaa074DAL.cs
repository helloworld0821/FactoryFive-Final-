using DXApplication1.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.DAL.DatabaseDAL
{
    class AmendDaa074DAL
    {
        ///未测试
        /// <summary>
        /// 修正DAA074数据库中的数据
        /// 将有效厚度为0的列修正为1/4砂岩厚度
        /// 将渗透率为0的列按传入的储层参数模板进行修正
        /// 将微相为5的数修正为相应数据
        /// </summary>
        /// <param name="dtCM">传入的储层参数模板</param>
        /// 
        public void AmendDaa074Method(DataTable dtCM)
        {
            string strTableName = "T_DAA074";
            string strTablePath = strTableName + ".dbf";
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            string strSQL;
            //Oledb
            if (File.Exists("./Data/" + strTablePath))
            {
                File.Delete("./Data/" + strTablePath);
            }
            //Oracle
            //string strSQL = "select count(*) as count from user_tables where table_name = '" + strTableName.ToUpper() + "'";
            //DataTable dtExist = cdu.SelectDatabase(strSQL);
            //if (Convert.ToInt32(dtExist.Rows[0]["Count"]) > 0)
            //{
            //    strSQL = "drop table " + strTableName;
            //    cdu.CreateOrDeleteDatabase(strSQL);
            //}
            //Oledb
            strSQL = "select distinct b.jh as 井号, b.cw as 层位, b.wx as 微相, b.stl as 渗透率, c.syhd as 砂岩厚度, c.yxhd as 有效厚度 from (select a.jh, a.cw, a.stl, a.wx, a.kxd from (select a.jh, a.cw, max(a.syhd) as syhd from (select t.*, (yczmc + dymc) as cw from DAA074 as t where t.skqk = '1') as a group by a.jh, a.cw) as d, (select t.*, (yczmc+dymc) as cw from DAA074 as t where t.skqk = '1') as a where a.jh = d.jh and a.cw = d.cw and a.syhd = d.syhd) as b, (select a.jh, a.cw, sum(a.syhd) as syhd, sum(a.yxhd) as yxhd from (select t.*, (yczmc+dymc) as cw from DAA074 as t where t.skqk = '1') as a group by a.jh, a.cw) as c where b.jh = c.jh and b.cw = c.cw order by b.cw";
            DataTable dtSKYT = cdu.SelectVFP(strSQL);
            //Oracle
            //strSQL = "select distinct b.jh 井号, b.cw 层位, b.wx 微相, b.stl 渗透率, c.syhd 砂岩厚度, c.yxhd 有效厚度 from (select a.jh, a.cw, a.stl, a.wx, a.kxd from (select a.jh, a.cw, max(a.syhd) as syhd from (select t.*, (yczmc || dymc) as cw from DAA074 t where t.skqk = 1) a group by a.jh, a.cw) d, (select t.*, (yczmc || dymc) as cw from DAA074 t where t.skqk = 1) a where a.jh = d.jh and a.cw = d.cw and a.syhd = d.syhd) b, (select a.jh, a.cw, sum(a.syhd) as syhd, sum(a.yxhd) as yxhd from (select t.*, (yczmc || dymc) as cw from DAA074 t where t.skqk = 1) a group by a.jh, a.cw) c where b.jh = c.jh and b.cw = c.cw order by b.cw";
            //DataTable dtSKYT = cdu.SelectDatabase(strSQL);

            string[] arr = { "P112b", "P121a", "P121b", "P122", "P131", "P132", "P133a", "P133b" };

            DataRow[] drYXHD = dtSKYT.Select("有效厚度 = 0");
            foreach (DataRow dr in drYXHD)
            {
                dr["有效厚度"] = Convert.ToDouble(dr["砂岩厚度"]) / 4;
            }
            DataRow[] drSTL = dtSKYT.Select("渗透率 = 0");
            foreach (DataRow dr in drSTL)
            {
                DataRow[] stlRows = dtCM.Select("YCZMC = '" + dr["层位"] + "' AND STLX = '" + dr["微相"] + "'");
                if (stlRows.Count() == 0)
                {
                    string cw = dr["层位"] + "a";
                    stlRows = dtCM.Select("YCZMC = '" + cw + "' AND STLX = '" + dr["微相"] + "'");
                    if (stlRows.Count() == 0)
                    {
                        cw = dr["层位"].ToString().Substring(0, dr["层位"].ToString().Length - 1);
                        stlRows = dtCM.Select("YCZMC = '" + cw + "' AND STLX = '" + dr["微相"] + "'");
                    }
                }
                dr["渗透率"] = Convert.ToDouble(stlRows[0]["STL"]) * Convert.ToDouble(dr["有效厚度"]);
            }
            DataRow[] drWX = dtSKYT.Select("微相 = 5");
            foreach (DataRow dr in drWX)
            {
                if (arr.Contains(dr["层位"].ToString()))
                {
                    if (Convert.ToDouble(dr["有效厚度"]) > 0.5)
                    {
                        dr["微相"] = "1";
                    }
                    else
                    {
                        dr["微相"] = "8";
                    }
                }
                else
                {
                    if (Convert.ToDouble(dr["有效厚度"]) > 0.5)
                    {
                        dr["微相"] = "2";
                    }
                    else
                    {
                        dr["微相"] = "3";
                    }
                }
            }
            //Oledb
            strSQL = "create table " + strTableName.ToUpper() + " (井号 varchar(16), 层位 varchar(16), 微相 varchar(1), 渗透率 Decimal(16, 5), 砂岩厚度 Decimal(17, 1), 有效厚度 Decimal(17, 1))";
            cdu.CreateOrDeleteVFP(strSQL);
            cdu.InsertVFP(dtSKYT, strTableName.ToUpper());
            //Oracle
            //strSQL = "create table " + strTableName.ToUpper() + " (井号 varchar(16), 层位 varchar(16), 微相 varchar(1), 渗透率 number(16, 5), 砂岩厚度 number(17, 1), 有效厚度 number(17, 1))";
            //cdu.CreateOrDeleteDatabase(strSQL);
            //cdu.InsertDatabase(dtSKYT, strTableName.ToUpper());
        }

        public void AmendDaa074Method()
        {
            string strTableName = "T_DAA074";
            string strTablePath = strTableName + ".dbf";
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            string strSQL;
            //Oledb
            if (File.Exists("./Data/" + strTablePath))
            {
                File.Delete("./Data/" + strTablePath);
            }
            //Oracle
            //string strSQL = "select count(*) as count from user_tables where table_name = '" + strTableName.ToUpper() + "'";
            //DataTable dtExist = cdu.SelectDatabase(strSQL);
            //if (Convert.ToInt32(dtExist.Rows[0]["Count"]) > 0)
            //{
            //    strSQL = "drop table " + strTableName.ToUpper();
            //    cdu.CreateOrDeleteDatabase(strSQL);
            //}
            //Oledb
            strSQL = "select distinct b.jh as 井号, b.cw as 层位, b.wx as 微相, b.stl as 渗透率, c.syhd as 砂岩厚度, c.yxhd as 有效厚度 from (select a.jh, a.cw, a.stl, a.wx, a.kxd from (select a.jh, a.cw, max(a.syhd) as syhd from (select t.*, (yczmc + dymc) as cw from DAA074 as t where t.skqk = '1') as a group by a.jh, a.cw) as d, (select t.*, (yczmc+dymc) as cw from DAA074 as t where t.skqk = '1') as a where a.jh = d.jh and a.cw = d.cw and a.syhd = d.syhd) as b, (select a.jh, a.cw, sum(a.syhd) as syhd, sum(a.yxhd) as yxhd from (select t.*, (yczmc+dymc) as cw from DAA074 as t where t.skqk = '1') as a group by a.jh, a.cw) as c where b.jh = c.jh and b.cw = c.cw order by b.cw";
            DataTable dtSKYT = cdu.SelectVFP(strSQL);
            //Oracle
            //strSQL = "select distinct b.jh 井号, b.cw 层位, b.wx 微相, b.stl 渗透率, c.syhd 砂岩厚度, c.yxhd 有效厚度 from (select a.jh, a.cw, a.stl, a.wx, a.kxd from (select a.jh, a.cw, max(a.syhd) as syhd from (select t.*, (yczmc || dymc) as cw from DAA074 t where t.skqk = 1) a group by a.jh, a.cw) d, (select t.*, (yczmc || dymc) as cw from DAA074 t where t.skqk = 1) a where a.jh = d.jh and a.cw = d.cw and a.syhd = d.syhd) b, (select a.jh, a.cw, sum(a.syhd) as syhd, sum(a.yxhd) as yxhd from (select t.*, (yczmc || dymc) as cw from DAA074 t where t.skqk = 1) a group by a.jh, a.cw) c where b.jh = c.jh and b.cw = c.cw order by b.cw";
            //DataTable dtSKYT = cdu.SelectDatabase(strSQL);

            //Oledb
            strSQL = "create table " + strTableName.ToUpper() + " (井号 varchar(16), 层位 varchar(16), 微相 varchar(1), 渗透率 Decimal(16, 5), 砂岩厚度 Decimal(17, 1), 有效厚度 Decimal(17, 1))";
            cdu.CreateOrDeleteVFP(strSQL);
            cdu.InsertVFP(dtSKYT, strTableName.ToUpper());
            //Oracle
            //strSQL = "create table " + strTableName.ToUpper() + " (井号 varchar(16), 层位 varchar(16), 微相 varchar(1), 渗透率 number(16, 5), 砂岩厚度 number(17, 1), 有效厚度 number(17, 1))";
            //cdu.CreateOrDeleteDatabase(strSQL);
            //cdu.InsertDatabase(dtSKYT, strTableName.ToUpper());
        }
    }
}
