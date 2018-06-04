using System;
using Domain.GlobalModel;
namespace OracleClientWcf  
{
    [TableField(TableName = "T_FLT_SCHEDULE")]
    public class FltSchedule
    {
      
     
        public string carrier { get; set; }
        public string flight_no { get; set; }
        public string plan_departure { get; set; }
        public string departure_airport { get; set; }
        public string plan_arrival { get; set; }
        public string arrival_airport { get; set; }
       
        public string d_or_i { get; set; }
        public string flight_type { get; set; }
        public string ac_type { get; set; }
        public string ac_reg { get; set; }
        public string flg_delay { get; set; }
        public string flg_vr { get; set; }
        public string flg_cs { get; set; }
        public string flg_patch { get; set; }
        public DateTime? off_wheel { get; set; }
        public DateTime? on_wheel { get; set; }
        public string ac_owner { get; set; }
        public string crew_owner { get; set; }
        public string stew_owner { get; set; }
        public string plce_owner { get; set; }
        public string mant_owner { get; set; }
        public string is_manual { get; set; }
        public string onward_flight { get; set; }
        public int ac_layover { get; set; }
        public string flight_flag { get; set; }
        public string sch_com_flag { get; set; }
        public string sch_ver_flag { get; set; }
        public string sch_pub_flag { get; set; }
        public string tele_flag { get; set; }
        public string ftb_prn_flag { get; set; }
        public string ftb_rec_flag { get; set; }
        public string cust_app_flag { get; set; }
        public int fly_hours { get; set; }
       
        public int flight_fen { get; set; }
        public string oper { get; set; }
        public string op_time { get; set; }
        public string remarks { get; set; }
        public string oper_ip { get; set; }
        public string oper_host { get; set; }
        public string p_or_c { get; set; }
        public string fxw_id { get; set; }
        public string flg_vip { get; set; }
        public string duty_flag { get; set; }
        public string from_sm { get; set; }
        public string co_old { get; set; }
        public string so_old { get; set; }
        public string po_old { get; set; }
        public string mo_old { get; set; }
        public string dep_bay { get; set; }
        public string arr_bay { get; set; }
        public string fltdate { get; set; }
        public string ao_old { get; set; }
        public string flight_id_ref { get; set; }
        public string flt_season { get; set; }
        public string page_flag { get; set; }
        public string ac_link_line { get; set; }
        public string ftb_no { get; set; }
        public string stew_com_flag { get; set; }
        public string stew_ver_flag { get; set; }
        public string stew_pub_flag { get; set; }
        public string plce_com_flag { get; set; }
        public string plce_ver_flag { get; set; }
        public string plce_pub_flag { get; set; }
        public string jx_foc_flight_id { get; set; }
        public string spe_service { get; set; }
        public string close_door_time { get; set; }
        public string open_door_time { get; set; }
        public string cobt { get; set; }
        public string backup_id { get; set; }
        public string eng_time { get; set; }
        public string taxi_out { get; set; }
        public string ods_id { get; set; }
        public string sch_pub_user { get; set; }
        public string sch_pub_time { get; set; }
        public string backup_type { get; set; }
        public string ref_atatime_flag { get; set; }
        public string train_pub_flag { get; set; }
        public string train_pub_user { get; set; }
        public string train_pub_time { get; set; }
        public string flightdatelocal { get; set; }
        public string flightdateutc { get; set; }
        public string duty_date { get; set; }
        public string delay_after_takeoff { get; set; }
        public string delay_type { get; set; }
        public string mm_leg_id { get; set; }
        #region  从数据库中查询到数据更改为当年数据时需要修改的字段列表
        public decimal flight_id { get; set; }
        public DateTime flight_date { get; set; }
        public DateTime std { get; set; }
        public DateTime etd { get; set; }
        public DateTime atd { get; set; }
        public DateTime sta { get; set; }
        public DateTime eta { get; set; }
        public DateTime ata { get; set; }
        public decimal crew_link_line { get; set; }
        public decimal stew_link_line { get; set; }
        public decimal plce_link_line { get; set; }
        #endregion
    }
}
