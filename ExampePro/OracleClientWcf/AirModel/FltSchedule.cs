using System;
using Domain.GlobalModel;
using System.ComponentModel;
namespace OracleClientWcf  
{
    [TableField(TableName = "T_FLT_SCHEDULE")]
    public class FltScheduleFullField
    {   
        public DateTime op_time { get; set; }
        public string remarks { get; set; }
        #region  从数据库中查询到数据更改为当年数据时需要修改的字段列表
        public decimal flight_id { get; set; }
        public string flight_date { get; set; }
        public string carrier { get; set; }
        public string flight_no { get; set; }
        public string plan_departure { get; set; }
        public string departure_airport { get; set; }
        public string plan_arrival { get; set; }
        public string arrival_airport { get; set; }
        public DateTime std { get; set; }
        public DateTime etd { get; set; }
        
        public DateTime sta { get; set; }
        public DateTime eta { get; set; }


        /*
         public decimal? crew_link_line { get; set; }
        public decimal? stew_link_line { get; set; }
        public decimal? plce_link_line { get; set; }
        public DateTime atd { get; set; }
        public DateTime ata { get; set; }
         * * 
         可移除的列
        public string oper_ip { get; set; }
        public string oper_host { get; set; } 
        public DateTime? off_wheel { get; set; }
        public DateTime? on_wheel { get; set; }
        public int flight_fen { get; set; }
        public string oper { get; set; } 
        public string ac_owner { get; set; }
        public string onward_flight { get; set; }
        public int ac_layover { get; set; }
        public decimal fly_hours { get; set; }  
        public char flight_type { get; set; }
        public string ac_type { get; set; }
        public string ac_reg { get; set; }
        public string flg_patch { get; set; }
         public string crew_owner { get; set; }
        public string stew_owner { get; set; }
        public string plce_owner { get; set; }
        public string mant_owner { get; set; }
        public string backup_type { get; set; }
           public string flg_vip { get; set; } 
           public string taxi_out { get; set; }
        public string ods_id { get; set; }
        public string sch_pub_user { get; set; }
        public string train_pub_user { get; set; }
        public string dep_bay { get; set; }
        public string arr_bay { get; set; }
        
        public string flight_id_ref { get; set; }
        public string flt_season { get; set; }
    
        public string ac_link_line { get; set; }
        public string ftb_no { get; set; }
     
        public string jx_foc_flight_id { get; set; }
        public string cobt { get; set; }
         */
        #endregion
        public void SetAsNowYearData(int copyDay) 
        {
            DateTime time = std;
            DateTime now=DateTime.Now;
            int year = now.Year - time.Year;
            std.AddYears(year).AddDays(copyDay);
            sta.AddYears(year).AddDays(copyDay);
            etd.AddYears(year).AddDays(copyDay);
            eta.AddYears(year).AddDays(copyDay);
            DateTime fd = DateTime.Parse(flight_date);
            flight_date = fd.AddYears(year).AddDays(copyDay).ToString("yyyy-MM-dd");
            
        }
    }
    [Description("分页参数")]
    public class PageParam
    {
        public int BeginRow { get; set; }
        public int EndRow { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
