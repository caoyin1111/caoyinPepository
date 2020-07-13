using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace TaskManager.DLL
{
    public abstract class SqliteHelpDao
    {
        
        //public static string ConnectionString = "Data Source=" + System.AppDomain.CurrentDomain.BaseDirectory + @"DataBase\Task.db;New=False;Compress=True;Version=3;";
        public static string ConnectionString = "Data Source=" + "C:\\Mobot Files\\" + @"tasks\task.db;New=False;Compress=True;Version=3;";
        //SqlHelper.SqliteHelpDao.ConnectionString = "Data Source=" + Application.StartupPath + @"\DataBase\test.db3;New=False;Compress=True;Version=3;";
        //public static string ConnectionString = string.Empty;
        // 哈希表用来存储缓存的参数信息，哈希表可以存储任意类型的参数。
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        public SqliteHelpDao() { }

        /// <summary>
        /// 执行多条带事务的SQL语句
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="IsTrans"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="pramsList"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, bool IsTrans, CommandType cmdType, string[] cmdText, ArrayList pramsList)
        {
            int pramsListIndex = 0;
            SQLiteParameter[] parms = null;
            SQLiteTransaction objTrans = null;
            //用于返回数据受影响的行数
            int val = 0;
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    foreach (string str in cmdText)
                    {
                        parms = (SQLiteParameter[])pramsList[pramsListIndex];
                        //判断数据库连接状态
                        if (conn.State != ConnectionState.Open)
                            conn.Open();
                        SQLiteCommand cmd = new SQLiteCommand();
                        //判断
                        if (pramsListIndex == 0)
                        {
                            objTrans = conn.BeginTransaction();
                        }

                        //通过PrePareCommand方法将参数逐个加入到SqlCommand的参数集合中
                        PrepareCommand(cmd, conn, objTrans, cmdType, str, parms);

                        val = cmd.ExecuteNonQuery();

                        //清空SqlCommand中的参数列表
                        cmd.Parameters.Clear();
                        cmd = null;
                        pramsListIndex++;
                    }
                    //事务的提交
                    objTrans.Commit();
                    return val;
                }
            }
            catch (Exception err)
            {
                val = -1;
                //事务的回滚
                objTrans.Rollback();
                Console.WriteLine(err);
            }

            return val;
        }

        /// <summary>
        /// 执行一条不需要返回值的SqlCommand命令
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SQLiteParameter[] commandParameters)
        {

            SQLiteCommand cmd = new SQLiteCommand();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                conn.Close();
                return val;
            }
        }
        /// <summary>
        /// 执行一个自定义链接的SQL数据
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SQLiteConnection connection, CommandType cmdType, string cmdText, params SQLiteParameter[] commandParameters)
        {

            SQLiteCommand cmd = new SQLiteCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            connection.Close();
            return val;
        }
        /// <summary>
        /// 执行一个带事务的SQL
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SQLiteTransaction trans, CommandType cmdType, string cmdText, params SQLiteParameter[] commandParameters)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            trans.Connection.Close();
            return val;
        }
        /// <summary>
        ///  执行一条返回结果集的SqlCommand命令，通过专用的连接字符串。返回一个包含结果的SqlDataReader
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static SQLiteDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SQLiteParameter[] commandParameters)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteConnection conn = new SQLiteConnection(connectionString);

            // 在这里使用try/catch处理是因为如果方法出现异常，则SqlDataReader就不存在，
            //CommandBehavior.CloseConnection的语句就不会执行，触发的异常由catch捕获。
            //关闭数据库连接，并通过throw再次引发捕捉到的异常。
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                SQLiteDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        /// <summary>
        /// 执行一条不带参数的select语句
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public static SQLiteDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteConnection conn = new SQLiteConnection(connectionString);

            // 在这里使用try/catch处理是因为如果方法出现异常，则SqlDataReader就不存在，
            //CommandBehavior.CloseConnection的语句就不会执行，触发的异常由catch捕获。
            //关闭数据库连接，并通过throw再次引发捕捉到的异常。
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText);
                SQLiteDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }
        /// <summary>
        /// 执行一条返回第一条记录第一列的SqlCommand命令，通过专用的连接字符串。 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SQLiteParameter[] commandParameters)
        {
            SQLiteCommand cmd = new SQLiteCommand();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                connection.Close();
                if (val == DBNull.Value)
                    return 0;
                return val;
            }
        }
        /// <summary>
        /// 执行一条返回第一条记录第一列的SqlCommand命令，通过已经存在的数据库连接。
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static object ExecuteScalar(SQLiteConnection connection, CommandType cmdType, string cmdText, params SQLiteParameter[] commandParameters)
        {

            SQLiteCommand cmd = new SQLiteCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            connection.Close();
            return val;
        }


        /// <summary>
        /// 查询结构返回一个数据集
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="tableName"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string connectionString, CommandType cmdType, string cmdText, string tableName, params SQLiteParameter[] commandParameters)
        {
            DataSet ds = new DataSet();

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                using (SQLiteDataAdapter da = new SQLiteDataAdapter())
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    da.SelectCommand = cmd;
                    da.Fill(ds, tableName);
                    cmd.Parameters.Clear();
                    conn.Close();
                }
            }
            return ds;
        }

        /// <summary>
        /// 查询结构返回一个数据集
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="tableName"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string connectionString, CommandType cmdType, string cmdText1, string tableName1, string cmdText, string tableName)
        {
            DataSet ds = new DataSet();

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                using (SQLiteDataAdapter da = new SQLiteDataAdapter())
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText1, null);
                    da.SelectCommand = cmd;
                    da.Fill(ds, tableName1);

                    PrepareCommand(cmd, conn, null, cmdType, cmdText, null);
                    da.SelectCommand = cmd;
                    da.Fill(ds, tableName);

                    cmd.Parameters.Clear();
                    conn.Close();
                }
            }
            return ds;
        }


        /// <summary>
        /// 查询结构返回一个数据集
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdText"></param>
        /// <param name="tableName"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string connectionString, string cmdText, string tableName, params SQLiteParameter[] commandParameters)
        {
            DataSet ds = new DataSet();

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand();
                using (SQLiteDataAdapter da = new SQLiteDataAdapter())
                {
                    PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, commandParameters);
                    da.SelectCommand = cmd;
                    da.Fill(ds, tableName);
                    cmd.Parameters.Clear();
                    conn.Close();
                }
            }
            return ds;
        }


        /// <summary>
        /// 缓存参数数组
        /// </summary>
        /// <param name="cacheKey">参数缓存的键值</param>
        /// <param name="cmdParms">被缓存的参数列表</param>
        public static void CacheParameters(string cacheKey, params SQLiteParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// 获取被缓存的参数
        /// </summary>
        /// <param name="cacheKey">用于查找参数的KEY值</param>
        /// <returns>返回缓存的参数数组</returns>
        public static SQLiteParameter[] GetCachedParameters(string cacheKey)
        {
            SQLiteParameter[] cachedParms = (SQLiteParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            //新建一个参数的克隆列表
            SQLiteParameter[] clonedParms = new SQLiteParameter[cachedParms.Length];

            //通过循环为克隆参数列表赋值
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                //使用clone方法复制参数列表中的参数
                clonedParms[i] = (SQLiteParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }
        #region 为执行参数做准备
        private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, SQLiteTransaction trans, CommandType cmdType, string cmdText, SQLiteParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            //判断是否需要事物处理
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (SQLiteParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, SQLiteTransaction trans, CommandType cmdType, string cmdText)
        {
            //判断数据库连接状态
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            //判断是否需要事物处理
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
        }
        #endregion
    }
}
