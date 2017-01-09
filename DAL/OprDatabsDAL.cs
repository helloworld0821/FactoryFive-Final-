using DXApplication1.DAL.DatabaseDAL;
using DXApplication1.DAL.IDAL;
using DXApplication1.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.DAL
{
    class OprDatabsDAL : IOprDatabsDAL
    {
        ConnDatabaseUtil cdu = new ConnDatabaseUtil();
        public DataTable SelDatabs()
        {
            throw new NotImplementedException();
        }

        public void UpDatabs(DataTable dtCM)
        {
            AmendDaa074DAL add = new AmendDaa074DAL();
            add.AmendDaa074Method(dtCM);
        }
        public DataTable EndDateList()
        {
            string strSQL = "select distinct ny from DBA04 where ny > '" + MainForm.strStartDate + "' order by ny";
            return cdu.SelectVFP(strSQL);
            //return cdu.SelectDatabase(strSQL);
        }

        public DataTable StartDateList()
        {
            string strSQL = "select distinct ny from DBA04 where ny < '" + MainForm.strEndDate + "' order by ny";
            return cdu.SelectVFP(strSQL);
            //return cdu.SelectDatabase(strSQL);
        }

    }
}
