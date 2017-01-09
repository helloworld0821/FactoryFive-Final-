using DXApplication1.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DXApplication1.Utils
{
    class CCCSMBUtil
    {
        public static bool isCCCSMB = false;
        public DataTable GetCCCSMB(string strFilePath)
        {
            List<CCCSMBModel> lstCM = new List<CCCSMBModel>();
            DataTable dtCM = ListToDataTableUtil.ListToDataTable(lstCM);

            FileStream fs = new FileStream(strFilePath, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            string strReadLine = sr.ReadLine();
            for (int i = 0; i < 3; i++)
            {
                strReadLine = sr.ReadLine();
            }
            try
            {
                while (strReadLine != null)
                {

                    string[] strArray = Regex.Split(strReadLine, @"\s+");
                    DataRow drCM = dtCM.NewRow();
                    drCM["yczmc"] = strArray[0];
                    drCM["stlx"] = strArray[1];
                    drCM["kxd"] = Convert.ToDouble(strArray[2]);
                    drCM["bhd"] = Convert.ToDouble(strArray[3]);
                    drCM["stl"] = Convert.ToDouble(strArray[4]);
                    dtCM.Rows.Add(drCM);
                    strReadLine = sr.ReadLine();
                    isCCCSMB = true;
                }
            }
            catch (Exception)
            {
                isCCCSMB = false;

            }
            finally
            {
                sr.Close();
                fs.Close();
            }
            return dtCM;
        }
    }
}
