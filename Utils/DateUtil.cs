using DXApplication1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.Utils
{
    class DateUtil
    {
        //Dictionary<string, string> dic = new Dictionary<string, string>();

        /// <summary>
        /// 数模日期转换
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public DateTime TextDateStringCovertToDateTime(string str)
        {
            DateModel dmDate = new DateModel();
            string strDate = string.Empty;
            dmDate.month = str.Substring(3, 3);
            dmDate.year = str.Substring(7, 4);
            Dictionary<string, string> dic = DicSetDate();
            
            foreach (KeyValuePair<string, string> a in dic)
            {
                if (dmDate.month == a.Key)
                {
                    dmDate.month = a.Value;
                    break;
                }
            }
            strDate = dmDate.year + dmDate.month;
            return DateTime.ParseExact(strDate, "yyyyMM", null);
        }

        public DateTime DatabaseDateStringCovertToDateTime(string str)
        {
            DateTime dt = DateTime.ParseExact(str, "MM/DD/YY", null);
            string strDate = dt.ToString("yyyyMM");
            return DateTime.ParseExact(strDate, "yyyyMM", null);
        }

        public string DateTimeCovertToString(DateTime date)
        {
            DateModel dmDate = new DateModel();

            string strMonth = string.Empty;
            string strDate = date.ToString("yyyyMM");
            dmDate.year = strDate.Substring(0, 4);
            dmDate.month = strDate.Substring(4, 2);
        
            Dictionary<string, string> dic = DicSetDate();

            foreach (KeyValuePair<string, string> a in dic)
            {
                if (dmDate.month == a.Value)
                {
                    strMonth = a.Key;
                    break;
                }
            }
            strDate = "(1-" + strMonth + "-" + dmDate.year + ")";
            return strDate;
        }

        private Dictionary<string, string> DicSetDate()
        {
            Dictionary<string, string> d = new Dictionary<string, string>();//设置月份

            d.Add("JAN", "01");
            d.Add("FEB", "02");
            d.Add("MAR", "03");
            d.Add("APR", "04");
            d.Add("MAY", "05");
            d.Add("JUN", "06");
            d.Add("JLY", "07");
            d.Add("AUG", "08");
            d.Add("SEP", "09");
            d.Add("OCT", "10");
            d.Add("NOV", "11");
            d.Add("DEC", "12");

            return d;
        }
    }
}
