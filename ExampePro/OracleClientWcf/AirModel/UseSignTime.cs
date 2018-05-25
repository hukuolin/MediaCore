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
        public DateTime LeavingTime { get; set; }
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
            LoggerWriter.CreateLogFile(sql, new AppDirHelper().GetAppDir(AppCategory.WebApp), ELogType.DebugData,DateTime.Now.ToString("yyyyMMdd")+".log",true);
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
    
    
}