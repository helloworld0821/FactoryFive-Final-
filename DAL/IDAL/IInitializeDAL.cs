﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.DAL.IDAL
{
    interface IInitializeDAL
    {
        DataTable LoadDate();
        void AmendDaa074();
        void AmendDCB01();
        void AmendDCB02();
    }
}
