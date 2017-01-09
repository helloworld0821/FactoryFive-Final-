using DXApplication1.DAL;
using DXApplication1.DAL.IDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.BLL.DatabaseBLL
{
    class OprDatabsBLL
    {
        public void UpDatabs(DataTable dtCM)
        {
            IOprDatabsDAL idl = new OprDatabsDAL();
            idl.UpDatabs(dtCM);
        }

        public void SelDatabs(string str)
        {
            
        }
        public DataTable EndDateList()
        {
            OprDatabsDAL odl = new OprDatabsDAL();
            return odl.EndDateList();
        }

        public DataTable StartDateList()
        {
            OprDatabsDAL odl = new OprDatabsDAL();
            return odl.StartDateList();
        }
    }
}
