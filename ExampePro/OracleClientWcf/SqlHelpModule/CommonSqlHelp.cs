using Domain.GlobalModel;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Web;

namespace OracleClientWcf
{
    public class SqlHelp
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyMapColumnDict">属性匹配列的字典</param>
        /// <returns></returns>
        public string PrepareInsertSQL<T>(Dictionary<string, string> propertyMapColumnDict) where T : class
        {
            Type ty = typeof(T);
            string table = ty.Name;
            object[] obj = ty.GetCustomAttributes(typeof(TableFieldAttribute), false);
            List<string> ignoreField = new List<string>();//忽略字段
            if (obj != null && obj.Length > 0)
            {
                TableFieldAttribute map = obj[0] as TableFieldAttribute;
                table = string.IsNullOrEmpty(map.TableName) ? table : map.TableName;
                if (map.IgnoreProperty != null)
                {
                    ignoreField.AddRange(map.IgnoreProperty);
                }
            }

            List<string> columns = new List<string>();
            List<string> dbColumn = new List<string>();
            foreach (PropertyInfo item in ty.GetProperties())
            {
                string pn = item.Name;
                //是否是被忽略的属性
                object[] ignore = item.GetCustomAttributes(typeof(PropertyIgnoreFieldAttribute), false);
                if (ignore != null && ignore.Length > 0)
                {
                    ignoreField.Add(pn);
                    continue;
                }
                object[] propertyMapColumn = item.GetCustomAttributes(typeof(ColumnAttribute), false);
                if (propertyMapColumn != null && propertyMapColumn.Length > 1)
                {
                    ColumnAttribute col = propertyMapColumn[0] as ColumnAttribute;
                    pn = string.IsNullOrEmpty(col.Name) ? pn : col.Name;
                }
                columns.Add("{" + pn + "}");
                dbColumn.Add(propertyMapColumnDict[pn]);
            }
            if (columns.Count == 0)
            {//没有匹配的数据库列 
                return string.Empty;
            }
            return "insert into {table} ([columns]) values({columnsParam})"
                .Replace("[columns]", string.Join(",", dbColumn))
                .Replace("{columnsParam}", string.Join(",", columns))
                .Replace("{table}", table);
        }
    }
}