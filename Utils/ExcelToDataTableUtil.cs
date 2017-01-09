using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace DXApplication1.Utils
{
    class ExcelToDataTableUtil
    {
        public static System.Data.DataTable ExcelToDataTable(string strExcelFileName)
        {
            //源的定义
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + strExcelFileName + ";" + "Extended Properties='Excel 8.0;HDR=YES;IMEX=1';";

            //Sql语句
            //string strExcel = string.Format("select * from [{0}$]", strSheetName); 
            //string strExcel = "select * from [sheet1$]";这是一种方法

            //定义存放的数据表
            DataSet ds = new DataSet();

            //连接数据源
            OleDbConnection conn = new OleDbConnection(strConn);

            conn.Open();

            System.Data.DataTable schemaTable = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
            string strSheetName = schemaTable.Rows[0][2].ToString().Trim();

            string strExcel = string.Format("select * from [{0}]", strSheetName);

            OleDbDataAdapter adapter = new OleDbDataAdapter(strExcel, strConn);
            adapter.Fill(ds, strSheetName);

            conn.Close();

            System.Data.DataTable dt = ds.Tables[strSheetName];

            return dt;
        }

    }
}
