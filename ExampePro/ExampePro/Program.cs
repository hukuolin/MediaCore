using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Domain.CommonData;
namespace ExampePro
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateClassFile();
            return;
            GenerateAirData();
            return;
            LinkRemoteDB();
            Console.ReadLine();
            Console.WriteLine("Link wcf");
            LinkWcf();
            Console.ReadLine();
            LinkCompanyWcf();
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
            ConsoleDataSetInfo(ds);
        }
        static void ConsoleDataSetInfo(DataSet ds)
        {
            if (ds == null)
            {
                Console.WriteLine("QuerResult:null");
                return;
            }
            int table = ds.Tables.Count;
            Console.WriteLine("QueryResult table:" + table);
            if (table > 0)
            {
                Console.WriteLine("Row:" + ds.Tables[0].Rows.Count);
            }
        }
        static void LinkWcf() 
        {
            OracleClientExample.LinkOracleClient link = new OracleClientExample.LinkOracleClient();
            DataSet ds= link.QueryAllCREW();
            ConsoleDataSetInfo(ds);
        }
        static void LinkCompanyWcf() 
        {
            XizangCrewWcf.SysManagerContractClient sys = new XizangCrewWcf.SysManagerContractClient();
            DataSet ds = sys.GetUserDSByCode("fadmin");
            ConsoleDataSetInfo(ds);
        }
        static void GenerateAirData() 
        {
            Console.WriteLine("Call Query Oracle data ......");
            AirOracleWcf.LinkOracleClient air = new AirOracleWcf.LinkOracleClient();
            bool succ= air.GenerateSign(100, DateTime.Now.AddDays(-50));
            Console.WriteLine("Generate result=" + succ);
        }
        static void GenerateClassFile() 
        {
            string dir = new Infrastructure.ExtService.AppDirHelper().GetAppDir(Infrastructure.ExtService.AppCategory.WinApp);
            string fromFile = dir+@"\GenerateClass\ClassProperty.txt";
            //读取文件中的属性
            string property = CommonHelperEntity.FileFormatExt.ReadFileUtf8Text(fromFile);
            string file = "FltSchedule";
            GenerateMoldeFile.GenerateClassFile(dir,file, "Model", "Generate", property, "\r\n");
        }
    }
    #region 授权许可证
    public class GenerateMoldeFile
    {
        static string GenerateClass(string programName, string className, string propertyList, string spiltSign)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("namespace {namespace}  ".Replace("{namespace}", programName));
            sb.AppendLine("{");
            sb.AppendLine("public class {class} {".Replace("{class}", className));
            //写入到类中
            string property = "\t public string {P} { get;set; }";
            List<string> ps = new List<string>();
            foreach (var item in propertyList.Split(new string[] { spiltSign }, StringSplitOptions.None))
            {
                if (!string.IsNullOrEmpty(item))
                    sb.AppendLine(property.Replace("{P}", item));
            }
            sb.AppendLine("}\r\n}");
            return sb.ToString();
        }
        public static string GenerateClassFile(string dir, string fileName, string programName, string className, string propertyList, string spiltSign)
        {
            string text = GenerateClass(programName, className, propertyList, spiltSign);
            string file = fileName + ".cs";
            LoggerWriter.CreateLogFile(text, dir, ELogType.DebugData, file, true);
            return dir + "/" + file;
        }
    }
    public class GenerateClassSetting
    {
        public string NameSpace { get; set; }
        public string ClassName { get; set; }
        public string PropertyListStr { get; set; }
    }
    #endregion
}
