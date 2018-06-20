using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Services;

namespace OracleClientWcf
{
    /// <summary>
    /// FileUploadService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class FileUploadService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public void UploadFile(byte[] myStream, string pathKey, string dir, string fileName)
        {
            if (myStream == null || myStream.Length == 0)
            {
                return;
            }
            string _DeirctPath = AppDomain.CurrentDomain.BaseDirectory;
            string path = _DeirctPath + "UploadFile";
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }

                catch (Exception ex)
                {
                    LogExtend.InsertLog(ex.Message);
                    return;
                }
            }
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new MemoryStream(myStream))
            {
                try
                {
                    using (Image _image = formatter.Deserialize(stream) as Image)
                    {
                        if (_image != null)
                        {
                            _image.Save(string.Format("{0}/{1}", path, fileName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogExtend.InsertLog( ex.Message);
                }

                stream.Close();
            }
        }
    }
}
