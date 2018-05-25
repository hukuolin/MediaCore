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
        public DataSet GenerateSign(int daySize,DateTime beginDay) 
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
                Dictionary<Type,string> entityAndSelectSql=new Dictionary<Type,string>();
                DateTime now=DateTime.Now;
                entityAndSelectSql.Add(typeof(FltScheulde), flt.GetBetweenDaySql(now.AddDays(-10), now));
                DataSet ds= oracle.QueryData(entityAndSelectSql, DBAccess.AirDBR5);
                InsertLog(string.Format("query table=【{0}】",ds.Tables.Count));
                List<Type> models=new List<Type>();
                models.Add(typeof(FltScheulde));
                Dictionary<string,List<Type> > data= oracle.DataSetConvertEntity(ds, models);
                return ds;
            }
            catch (Exception ex)
            {
                InsertLog(ex.Message);
                return null;
            }
        }
    }
}
