using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.Utils
{
    class ConnDatabaseUtil
    {
        /// <summary>
        /// 连接数据库并执行SQL语句返回DataTable
        /// </summary>
        /// <param name="str">SQL语句</param>
        /// <returns></returns>
        public DataTable SelectDatabase(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string _connStr = ConfigurationManager.ConnectionStrings["OracleConnection"].ToString();
                OracleConnection conn = new OracleConnection(_connStr);
                conn.Open();
                OracleDataAdapter da = new OracleDataAdapter(str, _connStr);
                DataSet ds = new DataSet();
                da.Fill(ds);
                DataTable dt = ds.Tables[0];
                conn.Close();
                return dt;
            }
            return new DataTable();
        }

        /// <summary>
        /// 连接数据库并执行SQL语句创建或删除数据表
        /// </summary>
        /// <param name="str">传入的SQL语句</param>
        public void CreateOrDeleteDatabase(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string _connStr = ConfigurationManager.ConnectionStrings["OracleConnection"].ToString();
                OracleConnection conn = new OracleConnection(_connStr);
                conn.Open();
                OracleCommand cmd = new OracleCommand(str, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// 将DataTable中的数据向数据库中的表里插入数据
        /// </summary>
        /// <param name="dt">需要插入数据库的DataTable</param>
        /// <param name="targetTable">数据库中已经存在的目标表</param>
        public void InsertDatabase(DataTable dt, string targetTable)
        {
            string _connStr = ConfigurationManager.ConnectionStrings["OracleConnection"].ToString();
            OracleConnection conn = new OracleConnection(_connStr);
            OracleBulkCopy oracleBulkCopy = new OracleBulkCopy(_connStr, OracleBulkCopyOptions.UseInternalTransaction);

            //oracleBulkCopy.BatchSize = dt.Rows.Count;   //每一批次中的行数             
            try
            {
                conn.Open();
                if ((dt != null) && (dt.Rows.Count != 0))
                {
                    oracleBulkCopy.DestinationTableName = targetTable;    //服务器上目标表的名称 
                    oracleBulkCopy.BatchSize = 100000; //每一批次中的行数         
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string col = dt.Columns[i].ColumnName;
                        oracleBulkCopy.ColumnMappings.Add(col, col);
                    }
                    oracleBulkCopy.WriteToServer(dt);   //将提供的数据源中的所有行复制到目标表中    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataTable SelectVFP(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string _connStr = ConfigurationManager.ConnectionStrings["VFPConnection"].ToString();
                OleDbConnection conn = new OleDbConnection(_connStr);
                conn.Open();
                OleDbDataAdapter da = new OleDbDataAdapter(str, _connStr);

                DataSet ds = new DataSet();
                da.Fill(ds);
                DataTable dt = ds.Tables[0];
                conn.Close();
                return dt;
            }
            return new DataTable();
        }

        public void CreateOrDeleteVFP(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string _connStr = ConfigurationManager.ConnectionStrings["VFPConnection"].ToString();
                OleDbConnection conn = new OleDbConnection(_connStr);
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(str, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        public void InsertVFP(DataTable dt, string targetTable)
        {
            string str = "select * from " + targetTable + " where 1 = 2";
            string _connStr = ConfigurationManager.ConnectionStrings["VFPConnection"].ToString();
            OleDbConnection conn = new OleDbConnection(_connStr);
            
            DataTable dtNew = new DataTable();
            try
            {
                conn.Open();
                OleDbDataAdapter da = new OleDbDataAdapter(str, _connStr);

                OleDbCommandBuilder myBuilder = new OleDbCommandBuilder(da);
                da.Fill(dtNew);
                foreach (DataRow dr in dt.Rows)
                {
                    dtNew.Rows.Add(dr.ItemArray);
                }
                
                da.Update(dtNew);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
