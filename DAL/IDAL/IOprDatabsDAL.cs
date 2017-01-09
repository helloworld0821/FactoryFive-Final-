using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.DAL.IDAL
{
    interface IOprDatabsDAL
    {
        void UpDatabs(DataTable dtCM);

        DataTable SelDatabs();
    }
}
