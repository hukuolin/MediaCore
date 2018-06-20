using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using System.Collections;
using System.Configuration;
using Domain.CommonData;
using Infrastructure.ExtService;

namespace OracleClientWcf
{
    public enum ConnType { FOC, SMPS, SMS, SMSSQL }
    public class DBAccess
    {
        public static string GetConfiguration(string appsetting) 
        {
           return ConfigurationManager.AppSettings[appsetting];
        }

        public static string DBConnectionString = GetConfiguration("SMPSDBConnection");

        public static string DBConnectionStringFOC = GetConfiguration("FOCDBConnection");

        public static string DBConnectionStringSMS =GetConfiguration("SMSDBConnection");

        public static string SMSSqlString =GetConfiguration("SMSSqlString");

        public static string AirDBR5 = GetConfiguration("AirDBR5");

        public bool CancelFlag = false;

        private OracleConnection CreateDBConnection()
        {
            OracleConnection myConn = new OracleConnection(DBAccess.DBConnectionString);
            return myConn;
        }

        private OracleConnection CreateFOCDBConnection()
        {
            OracleConnection myConn = new OracleConnection(DBAccess.DBConnectionStringFOC);
            return myConn;
        }

        private OracleConnection CreateSMSSQLConnection()
        {
            OracleConnection myConn = new OracleConnection(DBAccess.SMSSqlString);
            return myConn;
        }
        private OracleConnection CreateSMSDBConnection()
        {
            OracleConnection myConn = new OracleConnection(DBAccess.DBConnectionStringSMS);
            return myConn;
        }

        #region ����sql����,���ַ���Sql��ʽ�������ݿ⡣
        /// <summary>
        /// �򿪵������ݱ��ȡ���ݼ�,������DataSet��
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public DataSet OpenTable(string TableName)
        {
            OracleConnection myConn = this.CreateDBConnection();
            DataSet myDataSet = new DataSet();
            OracleDataAdapter myDataAdapter;

            try
            {
                myConn.Open();
                myDataAdapter = new OracleDataAdapter("select * from " + TableName, myConn);
                myDataAdapter.Fill(myDataSet);
            }
            catch(Exception ex)
            {
               
            }
            finally
            {
                myConn.Close();
            }

            return myDataSet;
        }

        /// <summary>
        /// ��ִ�и��ӵ�sql����ȡ������ֶε����ݼ�,������DataSet��
        /// </summary>
        /// <param name="StrSql"></param>
        /// <returns></returns>
        public DataSet ExecuteSql(string StrSql)
        {
            OracleConnection myConn = this.CreateDBConnection();
            DataSet myDataSet = new DataSet();
            OracleDataAdapter myDataAdapter;

            try
            {
                myConn.Open();
                myDataAdapter = new OracleDataAdapter(StrSql, myConn);
                myDataAdapter.Fill(myDataSet);
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                myConn.Close();
            }

            return myDataSet;
        }

        /// <summary>
        /// ִ�е���Sql�����ݵ���ӡ��޸ġ�ɾ��������Ȳ�����
        /// </summary>
        /// <param name="StrSql"></param>
        /// <returns></returns>
        public bool OperateDataACD(string StrSql)
        {
            bool bolResult = false;

            OracleConnection myConn = this.CreateDBConnection();
            OracleTransaction myTrans = null;
            OracleCommand myCommand = null;

            try
            {
                myConn.Open();
                
                myTrans = myConn.BeginTransaction();
                myCommand = myConn.CreateCommand();

                myCommand.Transaction = myTrans;

                myCommand.CommandText = StrSql;
                myCommand.ExecuteNonQuery();
                myTrans.Commit();

                bolResult = true;
            }
            catch (Exception ex)
            {
                if (myTrans != null)
                {
                    myTrans.Rollback();
                }

                bolResult = false;

               
            }
            finally
            {
                myConn.Close();
            }

            return bolResult;
        }

        /// <summary>
        /// ͬʱִ�ж���Sql�����ݵ���ӡ��޸ġ�ɾ��������Ȳ�����
        /// </summary>
        /// <param name="TranString"></param>
        /// <returns></returns>
        public bool ExecuteTran(ArrayList TranString)
        {
            if (TranString == null || TranString.Count == 0)
            {
                return false;
            }

            bool bolResult = false;
            OracleConnection myConn = this.CreateDBConnection();

            OracleCommand myCommand = null;
            OracleTransaction myTrans = null;

            try
            {
                myConn.Open();
                myTrans = myConn.BeginTransaction();
                myCommand = myConn.CreateCommand();
                myCommand.Transaction = myTrans;

                foreach (string strSql in TranString)
                {
                    myCommand.CommandText = strSql;
                    myCommand.ExecuteNonQuery();
                }

                myTrans.Commit();
                bolResult = true;
            }
            catch (Exception ex)
            {                
                bolResult = false;

                

                if (myCommand != null)
                {
                     
                }

                if (myTrans != null)
                {
                    myTrans.Rollback();
                }
            }
            finally
            {
                myConn.Close();
            }

            return bolResult;
        }

        /// <summary>
        /// ִ�в�ѯ,�����ز�ѯ�����صĽ�����е�һ�еĵ�һ�С�
        /// </summary>
        /// <param name="StrSql"></param>
        /// <returns></returns>
        public object ExecuteScalar(string StrSql)
        {
            // need code here
            return null;
        }
        #endregion
        public DataSet ExecuteSqlItem(string cmd,OracleParameter[] param)
        {
            DataSet ds = new DataSet();
            OracleConnection myConn = this.CreateDBConnection();
            OracleCommand myCommand = null;
            OracleDataAdapter myAdapter = null;
            try
            {
                myCommand = myConn.CreateCommand();
                myAdapter = new OracleDataAdapter();
                myAdapter.SelectCommand = myCommand;

                myCommand.CommandText = cmd;
                if (param != null)
                {
                    myCommand.Parameters.AddRange(param);
                }
                myAdapter.Fill(ds);        
            }
            catch (Exception ex)
            {
                if (myCommand != null)
                {
                    
                }
            }
            finally
            {
                myConn.Close();
            }
            return ds;
        }
    }
    public class AppConfig
    {
        static string GetConfigAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
        
        
        public static int GenerateHowDaySchedule
        {
            get
            {
                string cfg = GetConfigAppSetting("GenerateDayNumber");
                int day = 1;
                if (!int.TryParse(cfg, out day))
                {
                    return 1;
                }
                return day;
            }
        }
    }
    public class LogExtend
    {
        public static  void InsertLog(string text)
        {
            string format = "yyyyMMdd";
            string time = DateTime.Now.ToString(format) + ".log";
            text = DateTime.Now.ToString(Common.Data.CommonFormat.DateTimeFormat) + "\t" + text;
            LoggerWriter.CreateLogFile(text, new AppDirHelper().GetAppDir(AppCategory.WebApp), ELogType.DebugData, time, true);

        }
    }
}
