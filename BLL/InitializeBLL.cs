using DXApplication1.DAL;
using DXApplication1.DAL.IDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.BLL
{
    class InitializeBLL
    {
        IInitializeDAL ist = new InitializeDAL();

        public DataTable LoadDate()
        {
            return ist.LoadDate();
        }

        public void AmendDaa074()
        {
            ist.AmendDaa074();
        }
        public void AmendDCB01()
        {
            ist.AmendDCB01();
        }
        public void AmendDCB02()
        {
            ist.AmendDCB02();
        }

    }
}
