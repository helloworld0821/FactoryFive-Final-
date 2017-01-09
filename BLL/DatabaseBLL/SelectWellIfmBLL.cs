using DXApplication1.DAL.DatabaseDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.BLL.DatabaseBLL
{
    class SelectWellIfmBLL
    {
        SelectWellIfmDAL swi = new SelectWellIfmDAL();
        public DataTable GetOilWellIfm(DataTable dtOil, int chooseItem)
        {
            return swi.GetOilWellIfm(dtOil, chooseItem);
        }
        public DataTable GetWaterWellIfm(DataTable dtWater)
        {
            return swi.GetWaterWellIfm(dtWater);
        }
        }
}
