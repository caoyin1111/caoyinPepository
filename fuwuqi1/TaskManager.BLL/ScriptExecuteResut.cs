using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.BLL
{
   public partial class ScriptExecuteResut
    {
        private readonly TaskManager.DLL.ScriptExecuteResutServer dal = new DLL.ScriptExecuteResutServer();
        public ScriptExecuteResutManager()
        { }
        #region  Method

        private static ScriptExecuteResutManager scriptExecuteResutManager = null;
        public static ScriptExecuteResutManager GetScriptExecuteResutManager()
        {
            if (scriptExecuteResutManager == null)
                scriptExecuteResutManager = new ScriptExecuteResutManager();
            return scriptExecuteResutManager;
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(ScriptExecuteResut model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(ScriptExecuteResut model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int Id)
        {

            return dal.Delete(Id);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(string Idlist)
        {
            return dal.DeleteList(Idlist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public ScriptExecuteResut GetModel(int Id)
        {

            return dal.GetModel(Id);
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
        public List<ScriptExecuteResut> GetModelList(string strWhere)
        {
            DataTable ds = dal.GetList(strWhere);
            return DataTableToList(ds);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<ScriptExecuteResut> DataTableToList(DataTable dt)
        {
            List<ScriptExecuteResut> modelList = new List<ScriptExecuteResut>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                ScriptExecuteResut model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = new ScriptExecuteResut();
                    if (dt.Rows[n]["Id"] != null && dt.Rows[n]["Id"].ToString() != "")
                    {
                        model.Id = int.Parse(dt.Rows[n]["Id"].ToString());
                    }
                    if (dt.Rows[n]["AccomPlishNum"] != null && dt.Rows[n]["AccomPlishNum"].ToString() != "")
                    {
                        model.AccomPlishNum = int.Parse(dt.Rows[n]["AccomPlishNum"].ToString());
                    }
                    if (dt.Rows[n]["succeed"] != null && dt.Rows[n]["succeed"].ToString() != "")
                    {
                        model.succeed = int.Parse(dt.Rows[n]["succeed"].ToString());
                    }
                    if (dt.Rows[n]["fail"] != null && dt.Rows[n]["fail"].ToString() != "")
                    {
                        model.fail = int.Parse(dt.Rows[n]["fail"].ToString());
                    }
                    if (dt.Rows[n]["SatrtDate"] != null && dt.Rows[n]["SatrtDate"].ToString() != "")
                    {
                        model.SatrtDate = dt.Rows[n]["SatrtDate"].ToString();
                    }
                    if (dt.Rows[n]["EndDate"] != null && dt.Rows[n]["EndDate"].ToString() != "")
                    {
                        model.EndDate = dt.Rows[n]["EndDate"].ToString();
                    }
                    if (dt.Rows[n]["ter_id"] != null && dt.Rows[n]["ter_id"].ToString() != "")
                    {
                        model.ter_id = int.Parse(dt.Rows[n]["ter_id"].ToString());
                    }
                    if (dt.Rows[n]["ExecuteTrimLenght"] != null && dt.Rows[n]["ExecuteTrimLenght"].ToString() != "")
                    {
                        model.ter_id = int.Parse(dt.Rows[n]["ExecuteTrimLenght"].ToString());
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
