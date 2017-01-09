using DXApplication1.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DXApplication1.DAL.TextDAL
{
    class GetTreeList
    {
        public static DataTable dtTreeList = new DataTable();
        public static void GetTreeListMethod()
        {
            List<TreeListModel> lstTLM = new List<TreeListModel>();
            dtTreeList = ListToDataTableUtil.ListToDataTable(lstTLM);
            string s = DXApplication1.Properties.Resources.treelist;
            string[] arr = Regex.Split(s, @"\s+");
            for (int i = 0; i < arr.Count() - 1; i = i + 3)
            {
                DataRow drTreeList = dtTreeList.NewRow();
                drTreeList["parentid"] = Convert.ToInt32(arr[i]);
                drTreeList["id"] = Convert.ToInt32(arr[i + 1]);
                drTreeList["name"] = arr[i + 2];
               
                dtTreeList.Rows.Add(drTreeList);
            }
            
        }
    }
    class TreeListModel
    {
        public int parentid { set; get; }
        public int id { set; get; }
        public string name { set; get; }
    }
}
