using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DataHelp;
using CommonHelperEntity;
using Infrastructure.ExtService;
using System.Data;
using Domain.GlobalModel;
using System.Reflection;
namespace OracleClientWcf
{
    public class OracleSqlHelp
    {
        /// <summary>
        /// 准备insert 语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyMapColumnDict">属性匹配列的字典</param>
        /// <returns></returns>
        public string PrepareInsertSql<T>(Dictionary<string, string> propertyMapColumnDict) where T : class
        {
            SqlHelp help = new SqlHelp();
            return help.PrepareInsertSQL<T>(propertyMapColumnDict);
        }
        public OracleParameter[] PrepareParamBySql<T>(T data, string sql) where T : class
        {//根据SQL准备参数
            List<OracleParameter> ps = new List<OracleParameter>();
            string[] pis = data.GetAllProperties();
            string paramMapRule = "{(.*?)}";
            Regex reg = new Regex(paramMapRule);
            MatchCollection mc = reg.Matches(sql);
            string objName = typeof(T).Name;
            Dictionary<string, object> propertyValue = data.GetAllPorpertiesNameAndValues();
            Dictionary<string, string> paramMapPropertyRule = new Dictionary<string, string>();
            foreach (Match item in mc)
            {
                string g = item.Groups[0].Value;//参数串
                string pn = item.Groups[1].Value;//参数名剔除特殊限定
                string lowerName = pn.ToLower();
                bool map = false;
                foreach (var property in propertyValue)
                {
                    if (property.Key.ToLower() == lowerName)
                    {
                        map = true;
                        paramMapPropertyRule.Add(g, property.Key);
                        string name = objName + "_" + property.Key;//oracle 参数规则
                        sql = sql.Replace(g, ":" + name);
                        ps.Add(new OracleParameter() { ParameterName = name, Value = property.Value });
                        break;
                    }
                }
                if (!map)
                {//属性没有匹配 

                }
            }
            return ps.ToArray();
        }
        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="sqlConnString"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static bool ExecuteNoQuery(string sql, string sqlConnString, OracleParameter[] ps)
        {
            OracleConnection conn = new OracleConnection(sqlConnString);
            OracleCommand comm = new OracleCommand(sql);
            if (ps != null)
                comm.Parameters.AddRange(ps);
            conn.Open();
            int result = comm.ExecuteNonQuery();
            conn.Close();
            return result > 0;
        }
        static DataTable ReadDataFromSqlArrary(string sql, string tableName,OracleParameter[] param,OracleConnection conn) 
        {
            OracleCommand comm = new OracleCommand(sql, conn);
            if (param != null)
            {
                comm.Parameters.AddRange(param);
            }
            OracleDataAdapter dap = new OracleDataAdapter(comm);
            DataTable table = new DataTable();
            dap.Fill(table);
            table.TableName = tableName;
            return table;
        }
        /// <summary>
        /// 执行批量的查询SQL
        /// </summary>
        /// <param name="sqlParam"></param>
        /// <param name="sqlConnString">数据库连接串</param>
        /// <returns></returns>
        public static DataSet ReadDataSet(List<SqlParamDataSet> sqlParam,string sqlConnString) 
        {
            OracleConnection conn = new OracleConnection(sqlConnString);
            DataSet ds = new DataSet();
            conn.Open();
            foreach (var item in sqlParam)
            {
                DataTable table= ReadDataFromSqlArrary(item.ExceuteSql, item.ClassName, item.SqlParamArrary, conn);
                ds.Tables.Add(table);
            }
            conn.Close();
            return ds;
        }
        public DataSet QueryData(Dictionary<Type, string> entityWithSelectSql,string sqlConnString)
        {
            List<SqlParamDataSet> paramList = new List<SqlParamDataSet>();
            foreach (var item in entityWithSelectSql)
            {
                SqlParamDataSet ps = new SqlParamDataSet()
                {
                    ExceuteSql = item.Value,
                    ClassName = item.Key.Name
                };
                paramList.Add(ps);
            }
            return ReadDataSet(paramList, sqlConnString);
        }
        public Dictionary<string, List<Type>> DataSetConvertEntity(DataSet ds, List<Type> entityObj) 
        {
            Dictionary<string, List<Type>> entityRow = new Dictionary<string, List<Type>>();
            foreach (var item in entityObj)
            {
                string name = item.Name;
                DataTable table= ds.Tables[name];
                if (table == null || table.Rows.Count == 0)
                {
                    entityRow.Add(name, new List<Type>());
                    continue;
                }
                List<Type> data=DataTableConvertEntity(table,item ,null);
                entityRow.Add(name, data);
            }
            return entityRow;
        }
        public List<Type> DataTableConvertEntity(DataTable table, Type t, Dictionary<string, string> columnMapPropery)
        {
            if (columnMapPropery == null)
            {
                return FillDataByModel(table, t);
            }
            else
            {
                return FillDataByRuleDict(table, t, columnMapPropery);
            }
        }
        List<Type> FillDataByModel(DataTable table, Type t) 
        {
            Dictionary<string, string> columnMapProperty = new Dictionary<string, string>();
            //提取数据
            object[] rely = t.GetCustomAttributes(typeof(TableFieldAttribute), false);
            if (rely == null || rely.Length == 0)
            {  //没有匹配关系
                //建立匹配关系
            }
            return FillDataByRuleDict(table, t, columnMapProperty);
        }
        List<Type> FillDataByRuleDict(DataTable table, Type t, Dictionary<string, string> columnMapPropery)
        { //使用匹配字典进行数据填充
            List<Type> data = new List<Type>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                Type rec= DataRowFillModel(row, t, columnMapPropery);
                data.Add(rec);
            }
            return data;
        }
        Type DataRowFillModel(DataRow row, Type t, Dictionary<string, string> columnMapProperty)
        {
            Type mt = t.GetType();
            foreach (var item in columnMapProperty)
            {
                object obj = row[item.Key];
                if (obj == null)
                {
                    continue;
                }
                PropertyInfo pi = mt.GetProperty(item.Value);
                if (pi == null)
                {
                    continue;
                }
                if (pi.GetSetMethod() == null)
                {//只读属性 
                    continue;
                }

                pi.SetValue(mt, Convert.ChangeType(obj,pi.PropertyType), null);
            }
            return mt;
        }
        public class SqlParamDataSet
        {
            public string ClassName { get; set; }
            public string ExceuteSql { get; set; }
            public OracleParameter[] SqlParamArrary { get; set; }
        }
        public class Data
        {
            public Type TargetClass { get; set; }
            public Dictionary<string, string> TableColumnMapProperty { get; set; }
        }
    }
}