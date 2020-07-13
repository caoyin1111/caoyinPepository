using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.BLL
{
   public partial class LogInFoImage
    {
        private readonly LogInFoImageServer dal = new LogInFoImageServer();
        public LogInFoImageManager()
        { }
        #region  Method

        private static LogInFoImageManager logInFoImageManager = null;

        public static LogInFoImageManager GetLogInFoImageManager()
        {
            if (logInFoImageManager == null)
                logInFoImageManager = new LogInFoImageManager();
            return logInFoImageManager;
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add(LogInFoImage model)
        {
            dal.Add(model);
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
        public List<LogInFoImage> GetModelList(string strWhere)
        {
            DataTable ds = dal.GetList(strWhere);
            return DataTableToList(ds);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<LogInFoImage> DataTableToList(DataTable dt)
        {
            List<LogInFoImage> modelList = new List<LogInFoImage>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                LogInFoImage model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = new LogInFoImage();
                    if (dt.Rows[n]["Id"] != null && dt.Rows[n]["Id"].ToString() != "")
                    {
                        model.Id = int.Parse(dt.Rows[n]["Id"].ToString());
                    }
                    if (dt.Rows[n]["ser_script_id"] != null && dt.Rows[n]["ser_script_id"].ToString() != "")
                    {
                        model.ser_script_id = int.Parse(dt.Rows[n]["ser_script_id"].ToString());
                    }
                    if (dt.Rows[n]["path"] != null && dt.Rows[n]["path"].ToString() != "")
                    {
                        model.path = dt.Rows[n]["path"].ToString();
                    }
                    if (dt.Rows[n]["createDate"] != null && dt.Rows[n]["createDate"].ToString() != "")
                    {
                        model.createDate = dt.Rows[n]["createDate"].ToString();
                    }
                    modelList.Add(model);
                }
            }
            return modelList;
        }
        #endregion  Method
    }

}
