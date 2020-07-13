using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TaskManager.DLL;

namespace fuwuqi1
{
    class Program
    {
        static HttpListener listener;
        static string ConnectionString;
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                ConnectionString = "Data Source=" + args[0];
            }


            listener = new HttpListener();

            try
            {
                listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                listener.Prefixes.Add("http://+:8090/");
               

                listener.Start();
                listener.BeginGetContext(Result,null);
                Console.WriteLine($"服务端初始化完毕，正在等待客户端请求,时间：{DateTime.Now.ToString()}\r\n");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("服务器启动失败：", ex.Message);
                Console.ReadKey();

            }

        }

        private static void Result(IAsyncResult ar)
        {
            
            listener.BeginGetContext(Result,null);
            Console.WriteLine($"接到新的请求");
            var context = listener.EndGetContext(ar);
            var request = context.Request;
            var response = context.Response;
            context.Response.ContentType = "text/plain;charset=UTF-8";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
            //context.Response.AddHeader("Content-type", "text/plain");//添加响应头信息
            context.Response.ContentEncoding = Encoding.UTF8;
            string returnObj = null;//定义返回客户端的信息
            if (request.HttpMethod == "POST" && request.InputStream != null)
            {
                try
                {
  
                    Stream stream = request.InputStream;
                    StreamReader strStream = new StreamReader(stream, Encoding.UTF8);
                    string retString = strStream.ReadToEnd();

                    stream.Close();
                    strStream.Close();
                    //var ret = JObject.Parse(retString).ToObject<Dictionary<String, object>>();
                    //Dictionary<String,object> ret = JsonConvert.DeserializeObject<Dictionary<String, object>>(retString);
                    //Dictionary<String,List<Message>> me = JsonConvert.DeserializeObject<Dictionary<String, List<Message>>>(ret["Table"].ToString());
                    //JArray jarray = JArray.Parse(ret["Table"].ToString());
                    JArray jarray = JArray.Parse(retString);
                    foreach (var jsonitem in jarray)
                    {
                        JObject job = (JObject)jsonitem;
                        int ser_id = AddScriptResult(job);
                        ScriptExecuteDetailResult model =  addRe(ser_id, job);
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("insert into ScriptExecuteDetailResult(");
                        strSql.Append("ScriptName,ser_id,IsSucceed,StartDate,EndDate,LogFilePath,terminal,isDel,DeviceName,ExecuteTrimLenght,remark,TaskNum,IsUnLoad)");
                        strSql.Append(" values (");
                        strSql.Append("@ScriptName,@ser_id,@IsSucceed,@StartDate,@EndDate,@LogFilePath,@terminal,@isDel,@DeviceName,@ExecuteTrimLenght,@remark,@TaskNum,0)");
                        strSql.Append(";select LAST_INSERT_ROWID()");
                        SQLiteParameter[] parameters = {
                    new SQLiteParameter("@ScriptName", DbType.String),
                    new SQLiteParameter("@ser_id", DbType.Int32,8),
                    new SQLiteParameter("@IsSucceed", DbType.String),
                    new SQLiteParameter("@StartDate", DbType.String),
                    new SQLiteParameter("@EndDate", DbType.String),
                    new SQLiteParameter("@LogFilePath", DbType.String),
                    new SQLiteParameter("@terminal",DbType.String),
                    new SQLiteParameter("@isDel",DbType.Int16,4),
                    new SQLiteParameter("@DeviceName",DbType.String,50),
                    new SQLiteParameter("@ExecuteTrimLenght",DbType.Int16,4),
                    new SQLiteParameter("@remark",DbType.String,1000),
                    new SQLiteParameter("@TaskNum",DbType.Int32,8)};
                        parameters[0].Value = model.ScriptName;
                        parameters[1].Value = model.ser_id;
                        parameters[2].Value = model.IsSucceed;
                        parameters[3].Value = model.StartDate;
                        parameters[4].Value = model.EndDate;
                        parameters[5].Value = model.LogFilePath;
                        parameters[6].Value = model.Terminal;
                        parameters[7].Value = model.IsDel;
                        parameters[8].Value = model.DeviceName;
                        parameters[9].Value = model.ExecuteTrimLenght;
                        parameters[10].Value = model.Remark;
                        parameters[11].Value = model.TaskNum;

                        object obj = SqliteHelpDao.ExecuteScalar(ConnectionString, CommandType.Text, strSql.ToString(), parameters);
                        int a;
                        if (obj == null)
                        {
                            a = 0;
                        }
                        else
                        {
                            a = Convert.ToInt32(obj);
                        }
                    }
                    response.StatusDescription = "200";//获取或设置返回给客户端的 HTTP 状态代码的文本说明。
                    response.StatusCode = 200;// 获取或设置返回给客户端的 HTTP 状态代码。
                    returnObj = "已收到，接收成功";

                }
                catch (Exception ex)
                {
                    response.StatusDescription = "404";
                    response.StatusCode = 404;
                    returnObj = ex.Message;

                }
               
                Stream outstream = response.OutputStream;
                byte[] buffer = Encoding.UTF8.GetBytes(returnObj);
                outstream.Write(buffer, 0, buffer.Length);
                outstream.Close();



            }
       



        }

        private static int AddScriptResult(JObject job)
        {
            ScriptExecuteResut serModel = new ScriptExecuteResut();
            serModel.ter_id = (int)job["terID"];
            serModel.ScriptName = job["ScriptName"].ToString();
            serModel.SatrtDate = DateTime.Now.ToString();
            serModel.EndDate = DateTime.Now.ToString();
            serModel.AccomPlishNum = 1;
            serModel.ExecuteTrimLenght = 0;
            int currentScript_id=add(job,serModel);
            return currentScript_id;
           
        }
        private static int add(JObject job,ScriptExecuteResut model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into ScriptExecuteResut(");
            strSql.Append("AccomPlishNum,succeed,fail,SatrtDate,EndDate,ter_id,ScriptName,LogFilePath,ExecuteTrimLenght,isDel)");
            strSql.Append(" values (");
            strSql.Append("@AccomPlishNum,@succeed,@fail,@SatrtDate,@EndDate,@ter_id,@ScriptName,@LogFilePath,@ExecuteTrimLenght,@isDel)");
            strSql.Append(";select LAST_INSERT_ROWID()");
            SQLiteParameter[] parameters = {
                    new SQLiteParameter("@AccomPlishNum", DbType.Int32,8),
                    new SQLiteParameter("@succeed", DbType.Int32,8),
                    new SQLiteParameter("@fail", DbType.Int32,8),
                    new SQLiteParameter("@SatrtDate", DbType.String),
                    new SQLiteParameter("@EndDate", DbType.String),
                    new SQLiteParameter("@ter_id", DbType.Int32,8),
                    new SQLiteParameter("@ScriptName",DbType.String),
                    new SQLiteParameter("@LogFilePath",DbType.String),
                    new SQLiteParameter("@ExecuteTrimLenght",DbType.Int32,8),
                    new SQLiteParameter("@isDel",DbType.Int32,4)};
            parameters[0].Value = model.AccomPlishNum;
            parameters[1].Value = model.succeed;
            parameters[2].Value = model.fail;
            parameters[3].Value = model.SatrtDate;
            parameters[4].Value = model.EndDate;
            parameters[5].Value = model.ter_id;
            parameters[6].Value = model.ScriptName;
            parameters[7].Value = model.LogFilePath;
            parameters[8].Value = model.ExecuteTrimLenght;
            parameters[9].Value = model.IsDel;

            object obj = SqliteHelpDao.ExecuteScalar(ConnectionString, CommandType.Text, strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        private static ScriptExecuteDetailResult addRe(int currentScript_id,JObject job)
        {
            ScriptExecuteDetailResult model = new ScriptExecuteDetailResult();

            model.ser_id = currentScript_id;
            model.ScriptName = job["ScriptName"].ToString();
            model.IsSucceed = job["IsSucceed"].ToString();//ON表示其他
            model.EndDate = job["EndDate"].ToString();
            model.Terminal = job["terminal"].ToString();
            string s = job["taskNum"].ToString();
            int startIndex = s.IndexOf("-");
            int start = startIndex + 1;
            string a = s.Substring(start);
            int taskNum = Convert.ToInt32(a);

            model.TaskNum = taskNum;
            model.StartDate = job["StartDate"].ToString();
            model.DeviceName = job["DeviceName"].ToString();
            model.ExecuteTrimLenght = Convert.ToInt16(job["ExecuteTrimLenght"]);
            model.LogFilePath = job["LogFilePath"].ToString();
            model.Remark = job["remark"].ToString();
            return model;

        }
    }
    public class Message
    {
        public string ScriptName { get; set; }
        public string ser_id { get; set; }
        public string IsSucceed { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string LogFilePath { get; set; }
        public string terminal { get; set; }
        public string isDel { get; set; }
        public string DeviceName { get; set; }
        public string ExecuteTrimLenght { get; set; }
        public string remark { get; set; }
        public string TaskNum { get; set; }






    }
    public partial class ScriptExecuteResut
    {
        public ScriptExecuteResut()
        { }
        #region Model
        private int _id;
        private int? _accomplishnum;
        private int? _succeed;
        private int? _fail;
        private string _satrtdate;
        private string _enddate;
        private int? _ter_id;
        private string scriptName;
        private string _logFilePath;
        private int _executeTrimLenght;
        private int _isDel = 0;

        public int IsDel
        {
            get { return _isDel; }
            set { _isDel = value; }
        }

        public int ExecuteTrimLenght
        {
            get { return _executeTrimLenght; }
            set { _executeTrimLenght = value; }
        }

        public string LogFilePath
        {
            get { return _logFilePath; }
            set { _logFilePath = value; }
        }

        public string ScriptName
        {
            get { return scriptName; }
            set { scriptName = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? AccomPlishNum
        {
            set { _accomplishnum = value; }
            get { return _accomplishnum; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? succeed
        {
            set { _succeed = value; }
            get { return _succeed; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? fail
        {
            set { _fail = value; }
            get { return _fail; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SatrtDate
        {
            set { _satrtdate = value; }
            get { return _satrtdate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string EndDate
        {
            set { _enddate = value; }
            get { return _enddate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? ter_id
        {
            set { _ter_id = value; }
            get { return _ter_id; }
        }
        #endregion Model

    }


    public partial class ScriptExecuteDetailResult
    {
        public ScriptExecuteDetailResult()
        { }
        #region Model
        private int _id;
        private string _scriptname;
        private int? _ser_id;
        private string _issucceed;
        private string _startdate;
        private string _enddate;
        private string _logfilepath;
        private string _terminal;
        private int _isDel = 0;
        private int _ExecuteTrimLenght;
        private string _DeviceName;
        private string _Remark;
        private int _TaskNum;
        private string _DeviceExceptionMessage;
        private Bitmap _ErrorSavedImage;

        public Bitmap ErrorSavedImage
        {
            get { return _ErrorSavedImage; }
            set { _ErrorSavedImage = value; }
        }

        public string DeviceExceptionMessage
        {
            get { return _DeviceExceptionMessage; }
            set { _DeviceExceptionMessage = value; }
        }

        public int TaskNum
        {
            get { return _TaskNum; }
            set { _TaskNum = value; }
        }

        public string Remark
        {
            get { return _Remark; }
            set { _Remark = value; }
        }

        public string DeviceName
        {
            get { return _DeviceName; }
            set { _DeviceName = value; }
        }

        public int ExecuteTrimLenght
        {
            get { return _ExecuteTrimLenght; }
            set { _ExecuteTrimLenght = value; }
        }

        public int IsDel
        {
            get { return _isDel; }
            set { _isDel = value; }
        }

        public string Terminal
        {
            get { return _terminal; }
            set { _terminal = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ScriptName
        {
            set { _scriptname = value; }
            get { return _scriptname; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? ser_id
        {
            set { _ser_id = value; }
            get { return _ser_id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string IsSucceed
        {
            set { _issucceed = value; }
            get { return _issucceed; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string StartDate
        {
            set { _startdate = value; }
            get { return _startdate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string EndDate
        {
            set { _enddate = value; }
            get { return _enddate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string LogFilePath
        {
            set { _logfilepath = value; }
            get { return _logfilepath; }
        }
        #endregion Model

    }
}
