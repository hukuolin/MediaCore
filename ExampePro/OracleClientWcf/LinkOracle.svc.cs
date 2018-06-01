using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data;
using Infrastructure.ExtService;
using Domain.CommonData;
namespace OracleClientWcf
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
  
    public class LinkOracle : ILinkOracle
    {
        public DataSet QueryAllCREW() 
        {
            string cmd = @"select t.User_Name,
                                       t.USER_CODE,
                                       t.PASSWORDS,
                                       t.DEPT_CODE,
                                       s.dept_name, s.filiale,
                                       t.SEX,
                                       t.PHONE,
                                       t.PASS_MODIFY_DATE,
                                       t.PASS_VALID_DATE,
                                       t.VALID_FLAG,
                                       t.VALID_DATE,
                                       t.E_MAIL,
                                       t.AccessType,
                                       Decode(s.valid_flag,'H','F',s.valid_flag) Module_Flag,
                                       E_NAME 
                                  from T_SYS_USER t, t_sys_dept s
                                 Where t.dept_code = s.dept_code";
            DBAccess db = new DBAccess();
            DataSet ds = db.ExecuteSqlItem(cmd, null);
            return ds;
        }
        void InsertLog(string text) 
        {
            string format = "yyyyMMdd";
            string time = DateTime.Now.ToString(format) + ".log";
            LoggerWriter.CreateLogFile(text, new AppDirHelper().GetAppDir(AppCategory.WebApp), ELogType.DebugData, time, true);
        
        }
        public bool GenerateSign(int daySize,DateTime beginDay) 
        {
           
            string format = "yyyyMMdd";
            string time = DateTime.Now.ToString(format) + ".log";
            try
            {
                LoggerWriter.CreateLogFile(string.Format(" into to wcf [GenerateSign] parama -> daySize=【{0}】，time =【{1}】", daySize, beginDay.ToString(format)),
                    new AppDirHelper().GetAppDir(AppCategory.WebApp), ELogType.DebugData, time, true);
                UseSignTime map = new UseSignTime();
                // map.GenerateMuchDay(daySize, beginDay, DBAccess.AirDBR5);
                FltScheulde flt = new FltScheulde();
                OracleSqlHelp oracle = new OracleSqlHelp();
                string sql = DBAccess.AirDBR5;
                List<OracleSqlHelp.EntityDataMapTable> entityAndSelectSql = new List<OracleSqlHelp.EntityDataMapTable>();
                DateTime now=DateTime.Now;
                string fltSql=flt.GetBetweenDaySql(now.AddDays(-daySize/2), now.AddDays(daySize-daySize/2));
                InsertLog(fltSql);
                OracleSqlHelp.EntityDataMapTable fltMap = new OracleSqlHelp.EntityDataMapTable() 
                {
                    ExecuteSql = fltSql,
                     TargetClass=flt,
                     TableColumnMapProperty=flt.ColumnMapProperty()
                };
                entityAndSelectSql.Add(fltMap);
                DataSet ds= oracle.QueryData(entityAndSelectSql, DBAccess.AirDBR5);
                InsertLog(string.Format("query table=【{0}】",ds.Tables.Count));
                Dictionary<string,List<object> > data= oracle.DataSetConvertEntity(ds, entityAndSelectSql);
                foreach (var item in data)
                {
                    InsertLog(item.Key + " rows=" + item.Value.Count);
                }
                List<object> coll = data[flt.GetType().Name];
                List<decimal> flightIds = new List<decimal>();
                foreach (var item in coll)
                {
                    FltScheulde sch = item as FltScheulde;
                    flightIds.Add(sch.FlightId);
                }
                if (flightIds.Count == 0)
                {
                    InsertLog("no t_Flt_Schedule");
                    return false;
                }
                //找出航班计划对应的排班计划
                SchRoster ros = new SchRoster();
                string rosterSql = ros.QueryListSqlByFlightIds(flightIds.ToArray());
                InsertLog(rosterSql);
                List<OracleSqlHelp.EntityDataMapTable> rosterMap = new List<OracleSqlHelp.EntityDataMapTable>();
                rosterMap.Add(new OracleSqlHelp.EntityDataMapTable() { TargetClass = ros, ExecuteSql = rosterSql, TableColumnMapProperty = ros.ColumnMapProperty() });
                DataSet dsRoster = oracle.QueryData(rosterMap, DBAccess.AirDBR5);
                //提取排班计划
                Dictionary<string, List<object>> entity = oracle.DataSetConvertEntity(dsRoster, rosterMap);
                if (entity.Count == 0)
                {
                    InsertLog("no set sch_roster data");
                    return false;
                }
                List<UseSignTime> rs = new List<UseSignTime>();
                foreach (var item in entity[ros.GetType().Name])
                {
                    SchRoster r = item as SchRoster;
                    UseSignTime sign = new UseSignTime()
                    {
                        FlightDate = r.FlightDate,
                        FlightId = r.FlightId,
                        PCode = r.PCode,
                        Remarks = "Code Generate Sign Time",
                        ModuleFlag=r.ModuleFlag
                    };
                    UseSignTime.GeneratePlanSignTime(sign, sign.FlightDate);
                    rs.Add(sign);
                }
                //存储人员签到时间数据
                Dictionary<int, bool> excute= oracle.BatchExcuteNoQuery(UseSignTime.PrepareInsertSql(), rs, DBAccess.AirDBR5);
                StringBuilder sb = new StringBuilder();
                foreach (var item in excute)
                {
                    sb.AppendLine(item.Key + "=" + item.Value);
                }
                InsertLog(sb.ToString());
                return true;
            }
            catch (Exception ex)
            {
                InsertLog(ex.Message);
                return false;
            }
        }
        public void BatchSetLicense() 
        {
            string sql = "";
        }
    }
}
