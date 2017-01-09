
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
    class GetJJ
    {
        public static DataTable dtJJ = new DataTable();
        
        public static void GetJJMethod()
        {
            List<JJModel> lstJJ = new List<JJModel>();
            dtJJ = ListToDataTableUtil.ListToDataTable(lstJJ);
           
            string s = DXApplication1.Properties.Resources.jj;
            string[] arr = Regex.Split(s, @"\s+");
            for (int i = 0; i < arr.Count() - 1; i = i+3)
            {
                DataRow drJJ = dtJJ.NewRow();
                drJJ["jhy"] = arr[i];
                drJJ["jhs"] = arr[i + 1];
                drJJ["jj"] = arr[i + 2];
                dtJJ.Rows.Add(drJJ);
            }
            
        }
    }

    class JJModel
    {
        public string jhy { set; get; }
        public string jhs { set; get; }
        public int jj { set; get; }
    }
}
