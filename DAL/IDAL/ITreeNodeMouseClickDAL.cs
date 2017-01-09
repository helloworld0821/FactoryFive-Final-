using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.DAL.IDAL
{
    interface ITreeNodeMouseClickDAL
    {
        DataTable GetDataTable(string strNodeText);
        DataTable IdeDxLayer(string strNodeText);
        void ShowHIImg(string strParentNodeText, string strNodeText);
    }
}
