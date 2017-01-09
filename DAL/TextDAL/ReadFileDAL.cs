using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DXApplication1.DAL.TextDAL
{
    class ReadFileDAL
    {
        public static List<decimal> lstPermx = new List<decimal>();
        public static void ReadPermX()
        {
            string s = DXApplication1.Properties.Resources.permx.Trim();
            string[] arr = Regex.Split(s, @"\s+");
            for (int i = 0; i < arr.Length; i++)
            {
                lstPermx.Add(Convert.ToDecimal(Convert.ToDouble(arr[i].Trim())));
            }
        }
    }
}
