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
    public class UseSignTime : FlightBaseField
    {
        /*
         
         */
        public string PCode { get; set; }
        public DateTime SignInTime { get; set; }
        public string ModuleFlag { get; set; }
        public string Oper { get; set; }
        public DateTime OpTime { get; set; }
        public string Remarks { get; set; }
        public string OperIp { get; set; }
        public string OperHost { get; set; }
        public DateTime PlanSignInTime { get; set; }
        public DateTime PlanLeavingTime { get; set; }
        public DateTime LeavingTime { get; set; }
        static Dictionary<string, string> ColumnMapProperty() 
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("flight_id", "FlightId");
            dict.Add("flight_date", "FlightDate");
            dict.Add("p_code", "PCode");
            dict.Add("sign_in_time", "SignInTime");
            dict.Add("module_flag", "ModuleFlag");
            dict.Add("op_time", "OpTime");
            dict.Add("remarks", "Remarks");
            dict.Add("plan_sign_in_time", "PlanSignInTime");
            dict.Add("plan_leaving_time", "PlanLeavingTime");
            dict.Add("leaving_time", "LeavingTime");
            return dict;
        }
        /// <summary>
        /// 生成随机的签到时间【含计划】
        /// </summary>
        /// <param name="use"></param>
        /// <param name="startDayTime">对应FlightDate</param>
        public static void GeneratePlanSignTime(UseSignTime use, DateTime startDayTime)
        {
            Guid hash = Guid.NewGuid();
            int seed = hash.GetHashCode();
            int minSignTimeMinute = 30;
            int maxSignTimeMinute = 120;
            Random ran = new Random(seed);
            int signTime = ran.Next(minSignTimeMinute, maxSignTimeMinute);
            DateTime time = startDayTime.AddMinutes(signTime);
            use.PlanLeavingTime = time;
            int sign = ran.Next(10, 30);
            use.PlanSignInTime = time.AddMinutes(-sign);
            use.OpTime = DateTime.Now;
            use.SignInTime = time.AddMinutes(-ran.Next(10, 30));
            use.LeavingTime = startDayTime.AddMinutes(ran.Next(240, 480));
            //功能调整
            /*
            OracleSqlHelp oracle = new OracleSqlHelp();
            MapClassVsTable map = new MapClassVsTable();
            Dictionary<string, string> dict = map.MapPropertyVsDBColumn<UseSignTime>(db);
            string sql = oracle.PrepareInsertSql<UseSignTime>(dict);
            LoggerWriter.CreateLogFile(sql, new AppDirHelper().GetAppDir(AppCategory.WebApp), ELogType.DebugData,DateTime.Now.ToString("yyyyMMdd")+".log",true);
            OracleParameter[] ps = oracle.PrepareParamBySql(use, sql);
            */
        }
        public static string PrepareInsertSql() 
        {
            Dictionary<string,string> dict=ColumnMapProperty();
            return "insert into    t_Sch_Sign_In_Time (" + string.Join(",", dict.Keys) + ") values({"+string.Join("},{" ,dict.Values)+"})";
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
    #region 航班计划
    public class FltScheulde : FlightBaseField
    {
        public string FlightNo { get; set; }
        public string PlanDeparture { get; set; }
        public string DepartureAirport { get; set; }
        public string PlanArrival { get; set; }
        public string ArrivalAirport { get; set; }
        public DateTime Std { get; set; } 
        string sql = @"flight_id,flight_date,carrier,flight_no,plan_departure,departure_airport,plan_arrival,
arrival_airport,std,etd,atd,sta,eta,ata,d_or_i,flight_type,ac_type,ac_reg,flg_delay,flg_vr,flg_cs,flg_patch,off_wheel,
on_wheel,ac_owner,crew_owner,stew_owner,plce_owner,mant_owner,is_manual,onward_flight,ac_layover,
flight_flag,sch_com_flag,sch_ver_flag,sch_pub_flag,tele_flag,ftb_prn_flag,ftb_rec_flag,cust_app_flag,
fly_hours,crew_link_line,stew_link_line,plce_link_line,flight_fen,oper,op_time,remarks,oper_ip,oper_host,
p_or_c,fxw_id,flg_vip,duty_flag,from_sm,co_old,so_old,po_old,mo_old,dep_bay,arr_bay,fltdate,ao_old,
flight_id_ref,flt_season,page_flag,ac_link_line,ftb_no,stew_com_flag,stew_ver_flag,stew_pub_flag,
plce_com_flag,plce_ver_flag,plce_pub_flag,jx_foc_flight_id,spe_service,close_door_time,
open_door_time,cobt,backup_id,eng_time,taxi_out,ods_id,sch_pub_user,sch_pub_time,backup_type,
ref_atatime_flag,train_pub_flag,train_pub_user,train_pub_time,flightdatelocal,flightdateutc,
duty_date,delay_after_takeoff,delay_type,mm_leg_id";
        public string GetSelectSql()
        {
            return @"select 
flight_id,flight_date,carrier,flight_no,plan_departure,departure_airport,plan_arrival,arrival_airport,std,
etd,atd,sta,eta,ata,d_or_i,flight_type,ac_type,ac_reg,flg_delay,flg_vr,flg_cs,flg_patch,off_wheel,on_wheel,
ac_owner,crew_owner,stew_owner,plce_owner,mant_owner,is_manual,onward_flight,ac_layover,flight_flag,
sch_com_flag,sch_ver_flag,sch_pub_flag,tele_flag,ftb_prn_flag,ftb_rec_flag,cust_app_flag,fly_hours,
crew_link_line,stew_link_line,plce_link_line,flight_fen,oper,op_time,remarks,oper_ip,oper_host,p_or_c,
fxw_id,flg_vip,duty_flag,from_sm,co_old,so_old,po_old,mo_old,dep_bay,arr_bay,fltdate,ao_old,flight_id_ref,
flt_season,page_flag,ac_link_line,ftb_no,stew_com_flag,stew_ver_flag,stew_pub_flag,plce_com_flag,
plce_ver_flag,plce_pub_flag,jx_foc_flight_id,spe_service,close_door_time,open_door_time,cobt,backup_id,
eng_time,taxi_out,ods_id,sch_pub_user,sch_pub_time,backup_type,ref_atatime_flag,train_pub_flag,
train_pub_user,train_pub_time,flightdatelocal,flightdateutc,duty_date,delay_after_takeoff,delay_type,mm_leg_id
from t_Flt_Schedule";
        }
        public Dictionary<string, string> ColumnMapProperty()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("flight_id", "FlightId");
            dict.Add("flight_date", "FlightDate");
            dict.Add("flight_no", "FlightNo");
            dict.Add("departure_airport", "DepartureAirport");
            dict.Add("arrival_airport", "ArrivalAirport");
            dict.Add("plan_arrival", "PlanArrival");
            dict.Add("plan_departure", "PlanDeparture");
            dict.Add("Std", "Std");
            return dict;
        }
        public string GetBetweenDaySql(DateTime beginTime,DateTime endTime) 
        {
            string format = "yyyy-MM-dd";
            return GetSelectSql() +
                string.Format(" where flight_date between to_date('{0}','yyyy-MM-dd') and to_date('{1}','yyyy-MM-dd')",
                beginTime.ToString(format),endTime.ToString(format));
        }
    }
    #endregion
    #region 机组排班计划
    public class SchRoster : FlightBaseField
    {
        public Dictionary<string, string> ColumnMapProperty() 
        {
        //flight_id,flight_date,p_code
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("flight_id", "FlightId");
            dict.Add("flight_date", "FlightDate");
            dict.Add("p_code", "PCode");
            dict.Add("MODULE_FLAG", "ModuleFlag");
            return dict;
        }
        
        public string PCode { get; set; }
        public string ModuleFlag { get; set; }
        public string QueryListSqlByFlightIds(decimal[] ids) 
        {
            return "select " + string.Join(",", ColumnMapProperty().Keys) + " from t_Sch_Roster where flight_id in ('" + string.Join("','", ids) + "')";
        }
    }
    #region 航班公用字段
    public class FlightBaseField
    {
        public decimal FlightId { get; set; }
        public DateTime FlightDate { get; set; }
    }
    #endregion
    #endregion
    
}