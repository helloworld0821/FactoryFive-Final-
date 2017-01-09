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
    class GetTracerDAL
    {
        public static DataTable dtTracer = new DataTable();
        public static DataTable dtOil = new DataTable();
        public static DataTable dtWater = new DataTable();

        public void GetTracer(string strPath, string strTrace)
        {

            GetTraceJH gtj = new GetTraceJH();

            DataTable dtTracerJH = gtj.GetTraceJHMethod(strTrace);

            DateUtil du = new DateUtil();
            string strStartDate = du.DateTimeCovertToString(DateTime.ParseExact((MainForm.strStartDate), "yyyyMM", null));
            string strEndDate = du.DateTimeCovertToString(DateTime.ParseExact((MainForm.strEndDate), "yyyyMM", null).AddMonths(1));

            string strReadLine;
            string strJH = string.Empty;
            string strDate = string.Empty;
            string strTracer = string.Empty;
            bool isWell = false;
            bool isRead = false;
            bool isWater = false;
            bool isDate = false;
            FileStream fs = new FileStream(strPath, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            WellModel wm;
            List<WellModel> lstWM = new List<WellModel>();
            List<WellModel> lstOilM = new List<WellModel>();
            List<WellModel> lstWaterM = new List<WellModel>();
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
                        isDate = true;
                    }
                    if (strDate.Equals(strEndDate))
                    {
                        isDate = false;
                        break;

                    }
                }
                if (isDate)
                {
                    if (strArray[0].Equals("TRACER") && strArray.Count() == 6 && strArray[5].Equals("PASSIVE") && strArray[3].Equals("WATER") && strArray[4].Equals("PHASE"))
                    {
                        DataRow[] drTracer = dtTracerJH.Select("trace = '" + strArray[2] + "'");
                        {
                            if (drTracer.Count() > 0)
                            {
                                strTracer = drTracer[0]["smjh"].ToString();
                            }
                        }

                    }


                    if (((strArray.Count() == 11) && strArray[3].Equals("PROD")) || ((strArray.Count() == 11) && strArray[3].Equals("WINJ")) || ((strArray.Count() == 10) && strArray[0].Equals("BLOCK")))
                    {
                        if ((strArray.Count() == 11) && strArray[3].Equals("PROD"))
                        {
                            isWater = false;
                        }
                        else if ((strArray.Count() == 11) && strArray[3].Equals("WINJ"))
                        {
                            isWater = true;
                        }
                        if (!strArray[0].Equals("BLOCK"))
                        {
                            strJH = strArray[0];
                        }
                        isWell = true;
                        while (isWell)
                        {
                            strArray = Regex.Split(trim, @"\s+");
                            if (strArray[0].Equals("BLOCK"))
                            {

                                wm = new WellModel();

                                if (isWater)
                                {

                                    if (!strArray[4].Equals("SHUT"))
                                    {
                                        if (Convert.ToDouble(strArray[4]) > 0)
                                        {
                                            wm.jhy = "";
                                            wm.jhs = strJH;
                                            wm.mncs = Convert.ToDouble(strArray[4]);
                                            wm.szj = strTracer;
                                            wm.szjnd = Convert.ToDouble(strArray[7]);
                                            wm.date = strDate;
                                            wm.dtDate = du.TextDateStringCovertToDateTime(strDate);
                                            wm.x = Convert.ToInt32(strArray[1]);
                                            wm.y = Convert.ToInt32(strArray[2]);
                                            wm.k = Convert.ToInt32(strArray[3]);
                                            lstWM.Add(wm);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!strArray[4].Equals("SHUT"))
                                    {
                                        if (Convert.ToDouble(strArray[5]) > 0)
                                        {
                                            wm.jhy = strJH;
                                            wm.jhs = "";
                                            wm.mncs = Convert.ToDouble(strArray[5]);
                                            wm.szj = strTracer;
                                            wm.szjnd = Convert.ToDouble(strArray[7]);
                                            wm.date = strDate;
                                            wm.dtDate = du.TextDateStringCovertToDateTime(strDate);
                                            wm.x = Convert.ToInt32(strArray[1]);
                                            wm.y = Convert.ToInt32(strArray[2]);
                                            wm.k = Convert.ToInt32(strArray[3]);
                                            lstWM.Add(wm);
                                        }
                                    }

                                }

                                strReadLine = sr.ReadLine();
                                if (strReadLine != null)
                                {
                                    strReadLine = strReadLine.Replace(":", " ").Replace(",", " ");
                                    trim = strReadLine.Trim();
                                }
                            }

                            else
                            {
                                isWell = false;
                            }

                        }


                    }
                    else
                    {
                        if (trim.Equals("INJECTION  REPORT"))
                        {
                            isRead = true;
                            isWater = true;
                        }
                        else if (trim.Equals("PRODUCTION REPORT"))
                        {
                            isRead = true;
                            isWater = false;
                        }
                        else if (trim.Equals("CUMULATIVE PRODUCTION/INJECTION TOTALS"))
                        {
                            isRead = false;
                        }

                        if (isRead)
                        {

                            trim = strReadLine.Trim();

                            strArray = Regex.Split(trim, @"\s+");
                            if (isWater)
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
                                                        wm = new WellModel();
                                                        wm.date = strDate;
                                                        wm.dtDate = du.TextDateStringCovertToDateTime(strDate);
                                                        wm.jhs = strJH;
                                                        wm.x = Convert.ToInt32(strArray[1]);
                                                        wm.y = Convert.ToInt32(strArray[2]);
                                                        wm.k = Convert.ToInt32(strArray[3]);
                                                        if (Convert.ToDouble(strArray[5]) > 0)
                                                        {
                                                            wm.mncs = Convert.ToDouble(strArray[5]);
                                                            lstWaterM.Add(wm);
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

                                                        wm = new WellModel();
                                                        wm.date = strDate;
                                                        wm.dtDate = du.TextDateStringCovertToDateTime(strDate);
                                                        wm.jhy = strJH;
                                                        wm.x = Convert.ToInt32(strArray[1]);
                                                        wm.y = Convert.ToInt32(strArray[2]);
                                                        wm.k = Convert.ToInt32(strArray[3]);
                                                        if (Convert.ToDouble(strArray[5]) < 0)
                                                        {
                                                            wm.mncs = 0;
                                                        }
                                                        else
                                                        {
                                                            wm.mncs = Convert.ToDouble(strArray[5]);
                                                        }

                                                        if (Convert.ToDouble(strArray[4]) > 0)
                                                        {
                                                            wm.mncy = Convert.ToDouble(strArray[4]);
                                                            wm.mncye = wm.mncy + wm.mncs;
                                                            lstOilM.Add(wm);
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
                }
                strReadLine = sr.ReadLine();
                if (strReadLine != null)
                {
                    strReadLine = strReadLine.Replace(":", " ").Replace(",", " ");
                }
                //break;
            }
            
            dtTracer.Merge(ListToDataTableUtil.ListToDataTable(lstWM));
            dtOil = ListToDataTableUtil.ListToDataTable(lstOilM);
            dtWater = ListToDataTableUtil.ListToDataTable(lstWaterM);

            sr.Close();
            fs.Close(); ;
        }
    }
}
