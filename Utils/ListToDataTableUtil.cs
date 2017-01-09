using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.Utils
{
    class ListToDataTableUtil
    {
        public static DataTable ListToDataTable(Object o)
        {
            DataTable dt = new DataTable();

            Type t = o.GetType().GetGenericArguments()[0];

            PropertyInfo[] propertys = t.GetProperties();

            foreach (PropertyInfo pro in propertys)
            {
                DataColumn dc = new DataColumn();
                dc = dt.Columns.Add(pro.Name, System.Type.GetType(pro.PropertyType.FullName));
            }
            IEnumerable list = o as IEnumerable;
            foreach (Object obj in list)
            {
                DataRow newRow = dt.NewRow();
                for (int i = 0; i < propertys.Count(); i++)
                {
                    newRow[dt.Columns[i]] = obj.GetType().GetProperty(dt.Columns[i].ToString()).GetValue(obj);
                }

                dt.Rows.Add(newRow);
            }
            return dt;
        }
    }
}
