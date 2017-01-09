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
    class TreeNodeMouseClickBLL
    {
        /// <summary>
        /// 显示当前鼠标选中节点的数据表
        /// </summary>
        /// <param name="strNodeText">鼠标选中当前节点名称</param>
        /// <returns>当前节点显示的数据表</returns>
        public DataTable ShowDataTable(string strNodeText)
        {
            ITreeNodeMouseClickDAL inc = new TreeNodeMouseClickDAL();
            return inc.GetDataTable(strNodeText);
        }
        
        public void ShowHIImg(string strParentNodeText, string strNodeText)
        {
            ITreeNodeMouseClickDAL inc = new TreeNodeMouseClickDAL();
            inc.ShowHIImg(strParentNodeText, strNodeText);
        }
        public DataTable IdeDxLayer(string strNodeText)
        {
            ITreeNodeMouseClickDAL inc = new TreeNodeMouseClickDAL();
            return inc.IdeDxLayer(strNodeText);
        }
    }
}
