using Domain.CommonData;
using Infrastructure.ExtService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AirBatchDataTool
{
    public partial class BatchSceduleFrm : Form
    {
        BackgroundWorker bc = new BackgroundWorker();

        public BatchSceduleFrm()
        {
            InitializeComponent();
            InitUi();
        }
        void InitUi() 
        {
            DateTime now = DateTime.Now;
            dtpTmplBeginTime.Value = AppConfig.ScheduleBeingDateTime;
            dtpTmplEndTime.Value = AppConfig.ScheduleEndDateTime;
            dtpTargetBeginTime.Value =now;
            dtpTargetEndTime.Value = now;
            btnGenerate.Click += new EventHandler(Button_Click);
            bc.DoWork += new DoWorkEventHandler(DoBackEvent);

        }
        void Button_Click(object sender,EventArgs e)
        {
            if (!bc.IsBusy) 
            {
                bc.RunWorkerAsync();
            }

        }
        void DoBackEvent(object sender,DoWorkEventArgs e) 
        {
            if (this.InvokeRequired) 
            {
                this.Invoke(new DoWorkEventHandler(DoBackEvent), new object[] { sender, e });
                return;
            }
            try
            {
                AirDataWcf.LinkOracleClient air = new AirDataWcf.LinkOracleClient();
                DateTime now = DateTime.Now;
                DateTime begin = dtpTmplBeginTime.Value;
                DateTime end = dtpTmplEndTime.Value;
                string format = Common.Data.CommonFormat.DateFormat;
                int day = (dtpTargetEndTime.Value - dtpTargetBeginTime.Value).Days+1;//创建多少天数据
                int tmplNumber = 1;
                if (int.TryParse(txtTmplQueryNummber.Text, out tmplNumber))
                { 
                
                }
                if (tmplNumber < 1) { tmplNumber = 1; }
                for (int i = 1; i <= day; i++)
                {
                    try
                    {

                       string msg=  air.TemplateInsertFltSchedule(new AirDataWcf.PageParam()
                        {
                            BeginRow = 1,
                            EndRow = tmplNumber,
                            BeginTime = begin,
                            EndTime = end
                        }, i);
                        LogHelper.InsertLog(string.Format("Create  time=[ {0} ~ {1}] after day [{2}] end.result= {3}", begin.ToString(format), end.ToString(format), i,msg));
                        rtbTip.Text += msg;

                    }
                    catch (Exception ex)
                    {
                        rtbTip.Text += ex.Message;
                        LogHelper.InsertLog(string.Format("Create  time=[ {0} ~ {1}] after day [{2}] happend exception,\r\n{3}", begin.ToString(format), end.ToString(format), i, ex.ToString()));
                    }

                }
            }
            catch (Exception ex)
            {
                rtbTip.Text += ex.Message;
            }

        }
    }
    public class AppConfig
    {
        static string ScheduleBeginDate
        {
            get { return GetConfigAppSetting("ScheduleBeginDate"); }
        }
        static string GetConfigAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
        static string ScheduleEndDate
        {
            get { return GetConfigAppSetting("ScheduleEndDate"); }
        }
        public static DateTime ScheduleBeingDateTime
        {
            get
            {
                string cfg = ScheduleBeginDate;
                DateTime begin;
                if (!DateTime.TryParse(cfg, out begin))
                {
                    return DateTime.Now.AddDays(-1);
                }
                return begin;
            }
        }
        public static DateTime ScheduleEndDateTime
        {
            get
            {
                string cfg = ScheduleEndDate;
                DateTime begin;
                if (!DateTime.TryParse(cfg, out begin))
                {
                    return ScheduleBeingDateTime.AddDays(1);
                }
                return begin;
            }
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
    public class LogHelper
    {
        public static void InsertLog(string text)
        {
            string format = "yyyyMMdd";
            string time = DateTime.Now.ToString(format) + ".log";
            text = DateTime.Now.ToString(Common.Data.CommonFormat.DateTimeFormat) + "\t" + text;
            LoggerWriter.CreateLogFile(text, new AppDirHelper().GetAppDir(AppCategory.WebApp), ELogType.DebugData, time, true);

        }
    }
}
