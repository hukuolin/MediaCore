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
        #region 数据查询与填充
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
        public DataSet QueryData(List<EntityDataMapTable>  entityWithSelectSql,string sqlConnString)
        {
            List<SqlParamDataSet> paramList = new List<SqlParamDataSet>();
            foreach (EntityDataMapTable item in entityWithSelectSql)
            {
                Type t = item.TargetClass.GetType();
                SqlParamDataSet ps = new SqlParamDataSet()
                {
                    ExceuteSql = item.ExecuteSql,
                    ClassName =t.Name
                };
                paramList.Add(ps);
            }
            return ReadDataSet(paramList, sqlConnString);
        }
        public Dictionary<string, List<object>> DataSetConvertEntity(DataSet ds, List<EntityDataMapTable> entityObj) 
        {
            Dictionary<string, List<object>> entityRow = new Dictionary<string, List<object>>();
            foreach (var item in entityObj)
            {
                string name = item.TargetClass.GetType().Name;
                DataTable table= ds.Tables[name];
                if (table == null || table.Rows.Count == 0)
                {
                    entityRow.Add(name, new List<object>());
                    continue;
                }
                List<object> data=DataTableConvertEntity(table,item.TargetClass ,item.TableColumnMapProperty);
                entityRow.Add(name, data);
            }
            return entityRow;
        }
        public List<object> DataTableConvertEntity(DataTable table, object t, Dictionary<string, string> columnMapPropery)
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
        /// <summary>
        /// 使用实体之间进行建立匹配关系规则，并将数据填充到数据中
        /// </summary>
        /// <param name="table"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        List<object> FillDataByModel(DataTable table, object t) 
        {
            Dictionary<string, string> columnMapProperty = new Dictionary<string, string>();
            //提取数据
            Type tt = t.GetType();
            object[] rely = tt.GetCustomAttributes(typeof(TableFieldAttribute), false);
            List<string> ignoreField = new List<string>();
            if (rely.Length> 0)
            {   //建立匹配关系
                TableFieldAttribute att = rely[0] as TableFieldAttribute;
                ignoreField.AddRange(att.IgnoreProperty);
            }
            foreach (var item in t.GetAllProperties())
            {
                if (ignoreField.Contains(item))
                {
                    continue;
                }
                columnMapProperty.Add(item, item);
            }
            return FillDataByRuleDict(table, t, columnMapProperty);
        }
        List<object> FillDataByRuleDict(DataTable table, object t, Dictionary<string, string> columnMapPropery)
        { //使用匹配字典进行数据填充
            List<object> data = new List<object>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                DataRow row = table.Rows[i];
                object rec= DataRowFillModel(row, t, columnMapPropery);
                data.Add(rec);
            }
            return data;
        }
        object DataRowFillModel(DataRow row, object t, Dictionary<string, string> columnMapProperty)
        {
            Type mt = t.GetType();
            object data = System.Activator.CreateInstance(mt);
            foreach (var item in columnMapProperty)
            {
                object obj = row[item.Key];//需要判断是否存在该列
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

                pi.SetValue(data, Convert.ChangeType(obj, pi.PropertyType), null);
            }
            return data;
        }
        #endregion
        #region 扩展封装执行操作的参数
        public class SqlParamDataSet
        {
            public string ClassName { get; set; }
            public string ExceuteSql { get; set; }
            public OracleParameter[] SqlParamArrary { get; set; }
        }
        public class EntityDataMapTable  
        {
            public string ExecuteSql { get; set; }
            public object TargetClass { get; set; }
            /// <summary>
            /// 列匹配属性的字典关系
            /// </summary>
            public Dictionary<string, string> TableColumnMapProperty { get; set; }
        }
        #endregion
        #region  batch 
        /// <summary>
        /// 批量执行【非查询SQL】
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="data"></param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public Dictionary<int, bool> BatchExcuteNoQuery<T>(string sql, List<T> data, string connString) where T : class 
        {
            List<SqlParamDataSet> param = new List<SqlParamDataSet>();
            foreach (var item in data)
            {
                //规则提取属性进行参数注入
                SqlParamDataSet p = GenerateParam(item, sql);
                param.Add(p);   
            }
            return ExecuteListNoQuery(connString, param.ToArray());
        }
        SqlParamDataSet GenerateParam<T>(T data, string sql) where T : class
        {
            List<OracleParameter> ps = new List<OracleParameter>();
            SqlParamDataSet param = new SqlParamDataSet() { ClassName=data.GetType().Name};
            string[] pis = data.GetAllProperties();
            string paramMapRule = "{(.*?)}";
            Regex reg = new Regex(paramMapRule);
            MatchCollection mc = reg.Matches(sql);
            string objName = param.ClassName;
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
            param.ExceuteSql = sql;
            param.SqlParamArrary = ps.ToArray();
            return param;
        }
        public static Dictionary<int, bool> ExecuteListNoQuery(string sqlConnString, SqlParamDataSet[] param)
        {
            Dictionary<int, bool> excute = new Dictionary<int, bool>();
            OracleConnection conn = new OracleConnection(sqlConnString);
           
            for (int i = 0; i < param.Length; i++)
            {
                SqlParamDataSet item = param[i];
                try
                {
                    OracleCommand comm = new OracleCommand(item.ExceuteSql,conn);
                    if (conn.State!= ConnectionState.Open) 
                    {
                        conn.Open();
                    }
                    if (item.SqlParamArrary != null)
                        comm.Parameters.AddRange(item.SqlParamArrary);
                    int result = comm.ExecuteNonQuery();
                    excute.Add(i, true);
                }
                catch (Exception ex)
                {
                    excute.Add(i, false);
                }
            }
            conn.Close();
            return excute ;
        }
        #endregion
    }
}