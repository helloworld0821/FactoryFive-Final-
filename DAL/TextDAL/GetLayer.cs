using DXApplication1.Model;
using DXApplication1.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DXApplication1.DAL.TextDAL
{
    class GetLayer
    {
        public static DataTable dtWater = new DataTable();
        public static DataTable dtOil = new DataTable();
        public void GetMnLayer(string strPath)
        {

            dtWater.Clear();
            dtOil.Clear();

            DateUtil du = new DateUtil();
            string strStartDate = du.DateTimeCovertToString(DateTime.ParseExact((MainForm.strStartDate), "yyyyMM", null));
            string strEndDate = du.DateTimeCovertToString(DateTime.ParseExact((MainForm.strEndDate), "yyyyMM", null).AddMonths(1));

            string strReadLine;
            string strJH = string.Empty;
            string strDate = string.Empty;
            bool isWell = false;
            bool isWaterWell = false;
            int isDate = -1, isRead = -1;
            FileStream fs = new FileStream(strPath, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            
            List<WellModel> lstWaterM = new List<WellModel>();
            List<WellModel> lstOilM = new List<WellModel>();
            dtWater = ListToDataTableUtil.ListToDataTable(lstWaterM);
            dtOil = ListToDataTableUtil.ListToDataTable(lstOilM);


            strReadLine = sr.ReadLine().Replace(":", " ").Replace(",", " ");
            string trim = strReadLine.Trim();
            while (strReadLine != null)
            {
                trim = strReadLine.Trim();

                string[] strArray = Regex.Split(trim, @"\s+");

                if (((strArray.Length == 12) && (strArray[0].Equals("STEP")) && (strArray[8].Equals("REPT"))) || ((strArray.Length == 11) && (strArray[0].Equals("STEP")) && (strArray[7].Equals("REPT"))))
                {
                    
                    strDate = strArray[strArray.Length - 1];
                    if (strDate.Equals(strStartDate))
                    {
                        isDate = 0;
                    }
                    if (strDate.Equals(strEndDate))
                    {
                        isRead = -1;
                        isDate = -1;
                        break;

                    }
                }
                if (isDate == 0)
                {
                    if (trim.Equals("INJECTION  REPORT"))
                    {
                        isRead = 0;
                        isWaterWell = true;
                    }
                    else if (trim.Equals("PRODUCTION REPORT"))
                    {
                        isRead = 0;
                        isWaterWell = false;
                    }
                    else if (trim.Equals("CUMULATIVE PRODUCTION/INJECTION TOTALS"))
                    {
                        isRead = -1;
                    }

                    if (isRead >= 0)
                    {
                        if (isRead == 0)
                        {
                            isRead++;
                            for (int i = 0; i < 7; i++)
                            {
                                strReadLine = sr.ReadLine();
                                if (strReadLine != null)
                                {
                                    strReadLine = strReadLine.Replace(":", " ").Replace(",", " ");
                                }
                            }
                        }
                        trim = strReadLine.Trim();

                        strArray = Regex.Split(trim, @"\s+");
                        if (isWaterWell)
                        {
                            if ((strArray.Count() >= 12) && (strArray.Count() <= 13))
                            {
                                if (strArray.Count() == 12 || strArray[4].Equals("SHUT"))
                                {
                                    if (strArray[0].Length > 0)
                                    {
                                        if (!strArray[0].Equals("BLOCK"))
                                        {
                                            strJH = strArray[0];
                                        }
                                        isWell = true;
                                        if (strArray[1].Equals("GROUP") || strArray[0].Equals("REPORT"))
                                        {
                                            isWell = false;
                                        }
                                        while (isWell)
                                        {
                                            strArray = Regex.Split(trim, @"\s+");
                                            if (strArray[0].Equals("BLOCK"))
                                            {
                                                if (!strArray[4].Equals("SHUT"))
                                                {
                                                    
                                                    if (Convert.ToDouble(strArray[5]) > 0)
                                                    {
                                                        DataRow drWater = dtWater.NewRow();
                                                        drWater["DATE"] = strDate;
                                                        drWater["dtDate"] = du.TextDateStringCovertToDateTime(strDate);
                                                        drWater["jhs"] = strJH;
                                                        drWater["x"] = Convert.ToInt32(strArray[1]);
                                                        drWater["y"] = Convert.ToInt32(strArray[2]);
                                                        drWater["k"] = Convert.ToInt32(strArray[3]);
                                                        drWater["mncs"] = Convert.ToDouble(strArray[5]);
                                                        dtWater.Rows.Add(drWater);
                                                    }

                                                }
                                                strReadLine = sr.ReadLine().Replace(":", " ").Replace(",", " ");
                                                trim = strReadLine.Trim();
                                            }

                                            else
                                            {
                                                isWell = false;
                                            }

                                        }

                                    }
                                }
                            }
                        }
                        else
                        {
                            if (strArray.Count() >= 16)
                            {
                                if (strArray.Count() == 16 || strArray[4].Equals("SHUT"))
                                {
                                    if (strArray[0].Length > 0)
                                    {
                                        if (!strArray[0].Equals("BLOCK"))
                                        {
                                            strJH = strArray[0];
                                        }
                                        isWell = true;
                                        if (strArray[1].Equals("GROUP") || strArray[0].Equals("REPORT"))
                                        {
                                            isWell = false;
                                        }
                                        while (isWell)
                                        {
                                            strArray = Regex.Split(trim, @"\s+");
                                            if (strArray[0].Equals("BLOCK"))
                                            {
                                                if (!strArray[4].Equals("SHUT"))
                                                {
                                                    if (Convert.ToDouble(strArray[4]) > 0)
                                                    {


                                                        DataRow drOil = dtOil.NewRow();
                                                        drOil["DATE"] = strDate;
                                                        drOil["dtDate"] = du.TextDateStringCovertToDateTime(strDate);
                                                        drOil["jhy"] = strJH;
                                                        drOil["x"] = Convert.ToInt32(strArray[1]);
                                                        drOil["y"] = Convert.ToInt32(strArray[2]);
                                                        drOil["k"] = Convert.ToInt32(strArray[3]);
                                                        if (Convert.ToDouble(strArray[5]) < 0)
                                                        {
                                                            drOil["mncs"] = 0;
                                                        }
                                                        else
                                                        {
                                                            drOil["mncs"] = Convert.ToDouble(strArray[5]);
                                                        }

                                                        drOil["mncy"] = Convert.ToDouble(strArray[4]);
                                                        drOil["mncye"] = Convert.ToDouble(strArray[5]) + Convert.ToDouble(strArray[4]);
                                                        dtOil.Rows.Add(drOil);
                                                    }


                                                }
                                                strReadLine = sr.ReadLine().Replace(":", " ").Replace(",", " ");
                                                trim = strReadLine.Trim();
                                            }
                                            else
                                            {
                                                isWell = false;
                                            }

                                        }

                                    }
                                }
                            }
                        }
                    }
                }
                strReadLine = sr.ReadLine();
                if (strReadLine != null)
                {
                    strReadLine = strReadLine.Replace(":", " ").Replace(",", " ");
                }

            }

            
            sr.Close();
            fs.Close();
        }
    }
}
