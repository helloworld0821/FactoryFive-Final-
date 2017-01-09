using DXApplication1.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.DAL
{
    class DyhsDAL
    {
        ///未测试
        /// <summary>
        /// 吨油耗水-全区
        /// </summary>
        public void GetDyhsData()
        {
            ///先计算水量劈分
            SlpfDAL slpf = new SlpfDAL();
            slpf.GetSlpfData();
            if (!SlpfDAL.dtSLPF.Columns.Contains("DYHS"))
            {
                SlpfDAL.dtSLPF.Columns.Add("DYHS", System.Type.GetType("System.Double"));
            }
            ConnDatabaseUtil cdu = new ConnDatabaseUtil();
            string jhy = string.Empty, jhs = string.Empty;

            string strSQL = string.Empty;
            
            strSQL = "select jh, ny, round((ycyl+ycsl) * 30 / scts, 4) as ycyl1 from DBA04 where scts <> 0 and ny between '" + MainForm.strStartDate + "' and '" + MainForm.strEndDate + "' order by jh asc, ny asc"; // 在油井库里将油井按井号 年月排序            
            DataTable dtYCYL = cdu.SelectDatabase(strSQL);
            foreach (DataRow dr in dtYCYL.Rows)
            {
                if (Convert.ToDouble(dr["YCYL1"]) > 0)
                {
                    DataRow[] drSLPF = SlpfDAL.dtSLPF.Select("JH = '" + dr["JH"] + "' AND NY = '" + dr["NY"] + "'");
                    if (drSLPF.Count() > 0)
                    {
                        drSLPF[0]["DYHS"] = Convert.ToDouble(drSLPF[0]["SLPF"]) / Convert.ToDouble(dr["YCYL1"]);
                    }
                   
                }
            }
            
        }
    }
}
