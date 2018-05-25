using System;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using DataHelp;
using CommonHelperEntity;
using Infrastructure.ExtService;
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
    }
}