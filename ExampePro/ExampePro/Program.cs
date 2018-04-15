using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace ExampePro
{
    class Program
    {
        static void Main(string[] args)
        {
            LinkRemoteDB();
            Console.ReadLine();
        }
        static void LinkRemoteDB() 
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
            if (ds == null)
            {
                Console.WriteLine("QuerResult:null");
                return;
            }
            int table= ds.Tables.Count;
            Console.WriteLine("QueryResult table:" +table);
            if (table > 0) 
            {
                Console.WriteLine("Row:" + ds.Tables[0].Rows.Count);
            }
        }
    }
}
