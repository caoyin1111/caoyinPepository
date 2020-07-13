using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.BLL
{
    public partial class LogInFo
    {
        private readonly LogInFoDataServer dal = new LogInFoDataServer();
        public LogInFoManager()
        { }
        #region  Method

        private static LogInFoManager logInFoManager = null;

        public static LogInFoManager GetLogInFoManager()
        {
            if (logInFoManager == null)
                logInFoManager = new LogInFoManager();
            return logInFoManager;
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int ID)
        {
            return dal.Exists(ID);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(LogInFo model)
        {
            dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(LogInFo model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int ID)
        {
            return dal.Delete(ID);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(string IDlist)
        {
            return dal.DeleteList(IDlist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public LogInFo GetModel(int ID)
        {

            return dal.GetModel(ID);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataTable GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<LogInFo> GetModelList(string strWhere)
        {
            DataTable ds = dal.GetList(strWhere);
            return DataTableToList(ds);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<LogInFo> DataTableToList(DataTable dt)
        {
            List<LogInFo> modelList = new List<LogInFo>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                LogInFo model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = new LogInFo();
                    if (dt.Rows[n]["ID"] != null && dt.Rows[n]["ID"].ToString() != "")
                    {
                        model.ID = int.Parse(dt.Rows[n]["ID"].ToString());
                    }
                    if (dt.Rows[n]["LogLevel"] != null && dt.Rows[n]["LogLevel"].ToString() != "")
                    {
                        model.LogLevel = int.Parse(dt.Rows[n]["LogLevel"].ToString());
                    }
                    if (dt.Rows[n]["LogDate"] != null && dt.Rows[n]["LogDate"].ToString() != "")
                    {
                        model.LogDate = DateTime.Parse(dt.Rows[n]["LogDate"].ToString());
                    }
                    if (dt.Rows[n]["LogData"] != null && dt.Rows[n]["LogData"].ToString() != "")
                    {
                        model.LogData = dt.Rows[n]["LogData"].ToString();
                    }
                    if (dt.Rows[n]["Source"] != null && dt.Rows[n]["Source"].ToString() != "")
                    {
                        model.Source = dt.Rows[n]["Source"].ToString();
                    }
                    modelList.Add(model);
                }
            }
            return modelList;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataTable GetAllList()
        {
            return GetList("");
        }

        #endregion  Method
    }
}
}
