using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using DataHelp;
using System.Data.OracleClient;
using System.Collections;
using System.Collections.Generic;
using CommonHelperEntity;
using Domain.GlobalModel;
using System.Reflection;
using System.Data.Linq.Mapping;
using System.Text.RegularExpressions;
using Domain.CommonData;
using Infrastructure.ExtService;
namespace OracleClientWcf
{
    [TableField(TableName="")]
    public class UseSignTime
    {
        /*
         
         */
        public string FlightId { get; set; }
        public DateTime FlightDate { get; set; }
        public string PCode { get; set; }
        public DateTime SignInTime { get; set; }
        public char ModuleFlag { get; set; }
        public string Oper { get; set; }
        public DateTime OpTime { get; set; }
        public string Remarks { get; set; }
        public string OperIp { get; set; }
        public string OperHost { get; set; }
        public DateTime PlanSignInTime { get; set; }
        public DateTime PlanLeavingTime { get; set; }
        public DateTime LeaveTime { get; set; }
        string[] db = new string[] { "flight_id", "flight_date", "p_code", "sign_in_time", "module_flag", "oper", "op_time", "remarks", "oper_ip", "oper_host", "plan_sign_in_time", "plan_leaving_time", "leaving_time" };
       
        public void GenerateMuchDay(int day, DateTime startDayTime, string connString)
        {
            

            Guid hash = Guid.NewGuid();
            int seed = hash.GetHashCode();
            int minSignTimeMinute = 30;
            int maxSignTimeMinute = 120;
            for (int i = 0; i < day; i++)
            {//根据开始的时间计算
                Random ran = new Random(seed);
                int signTime = ran.Next(minSignTimeMinute, maxSignTimeMinute);
                DateTime time = startDayTime.AddDays(i);

            }
            UseSignTime use = new UseSignTime();
            OracleSqlHelp oracle = new OracleSqlHelp();
            MapClassVsTable map = new MapClassVsTable();
            Dictionary<string, string> dict = map.MapPropertyVsDBColumn<UseSignTime>(db);
            string sql = oracle.PrepareInsertSql<UseSignTime>(dict);
            LoggerWriter.CreateLogFile(sql, new AppDirHelper().GetAppDir(AppCategory.WebApp), ELogType.DebugData);
            OracleParameter[] ps = oracle.PrepareParamBySql(use, sql);

        }
    }
    public class MapClassVsTable
    { 
        /// <summary>
        /// 列-属性 建立匹配关系
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Dictionary<string, string> MapColumnVsProperty<T>(string[] columns) where T:class 
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            T entity = System.Activator.CreateInstance<T>();
            string[] pis= entity.GetAllProperties();
            foreach (var item in columns)
            {
                string deal = item.Replace("_", "");
                foreach (var p in pis)
                {
                    if (deal.ToLower() == p.ToLower())
                    {
                        dict.Add(item, p);
                        break;
                    }
                }
            }
            return dict;
        }
        /// <summary>
        /// 属性-列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public Dictionary<string, string> MapPropertyVsDBColumn<T>(string[] columns) where T : class
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            T entity = System.Activator.CreateInstance<T>();
            string[] pis = entity.GetAllProperties();
            foreach (var item in columns)
            {
                string deal = item.Replace("_", "");
                foreach (var p in pis)
                {
                    if (deal.ToLower() == p.ToLower())
                    {
                        dict.Add(p,item);
                        break;
                    }
                }
            }
            return dict;
        }
    }
    public class SqlHelp 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyMapColumnDict">属性匹配列的字典</param>
        /// <returns></returns>
        public string PrepareInsertSQL<T>(Dictionary<string,string> propertyMapColumnDict) where T:class
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
                columns.Add( pn );
                dbColumn.Add(propertyMapColumnDict[pn]);
            }
            if (columns.Count == 0)
            {//没有匹配的数据库列 
                return string.Empty;
            }
            return "insert into {table} ([columns]) values({columnsParam})"
                .Replace("{columnsParam}", string.Join(",", columns))
                .Replace("{table}", table);
        }
    }
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
        public  OracleParameter[] PrepareParamBySql<T>( T data,string sql)  where T:class
        {//根据SQL准备参数
            List<OracleParameter> ps = new List<OracleParameter>();
            string[] pis= data.GetAllProperties();
            string paramMapRule = "{(.*?)}";
            Regex reg = new Regex(paramMapRule);
            MatchCollection mc= reg.Matches(sql);
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
                        string name =  objName + "_" + property.Key;//oracle 参数规则
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
            int result= comm.ExecuteNonQuery();
            conn.Close();
            return result > 0;
        }
    }
}