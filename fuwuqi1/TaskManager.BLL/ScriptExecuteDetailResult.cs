using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TaskManager.BLL
{
    public partial class ScriptExecuteDetailResult
    {
        private readonly ScriptExecuteDetailResultServer dal = new ScriptExecuteDetailResultServer();
        public ScriptExecuteDetailResultManager()
        { }
        #region  Method

        private static ScriptExecuteDetailResultManager scriptExecuteDetailResultManager = null;

        public static ScriptExecuteDetailResultManager GetScriptExecuteDetailResultManager()
        {
            if (scriptExecuteDetailResultManager == null)
                scriptExecuteDetailResultManager = new ScriptExecuteDetailResultManager();
            return scriptExecuteDetailResultManager;
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int Id)
        {
            return dal.Exists(Id);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(ScriptExecuteDetailResult model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(ScriptExecuteDetailResult model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public ScriptExecuteDetailResult GetModel(int Id)
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
        public List<ScriptExecuteDetailResult> GetModelList(string strWhere)
        {
            DataTable ds = dal.GetList(strWhere);
            return DataTableToList(ds);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<ScriptExecuteDetailResult> DataTableToList(DataTable dt)
        {
            List<ScriptExecuteDetailResult> modelList = new List<ScriptExecuteDetailResult>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                ScriptExecuteDetailResult model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = new ScriptExecuteDetailResult();
                    if (dt.Rows[n]["Id"] != null && dt.Rows[n]["Id"].ToString() != "")
                    {
                        model.Id = int.Parse(dt.Rows[n]["Id"].ToString());
                    }
                    if (dt.Rows[n]["ScriptName"] != null && dt.Rows[n]["ScriptName"].ToString() != "")
                    {
                        model.ScriptName = dt.Rows[n]["ScriptName"].ToString();
                    }
                    if (dt.Rows[n]["ser_id"] != null && dt.Rows[n]["ser_id"].ToString() != "")
                    {
                        model.ser_id = int.Parse(dt.Rows[n]["ser_id"].ToString());
                    }
                    if (dt.Rows[n]["IsSucceed"] != null && dt.Rows[n]["IsSucceed"].ToString() != "")
                    {
                        model.IsSucceed = dt.Rows[n]["IsSucceed"].ToString();
                    }
                    if (dt.Rows[n]["StartDate"] != null && dt.Rows[n]["StartDate"].ToString() != "")
                    {
                        model.StartDate = dt.Rows[n]["StartDate"].ToString();
                    }
                    if (dt.Rows[n]["EndDate"] != null && dt.Rows[n]["EndDate"].ToString() != "")
                    {
                        model.EndDate = dt.Rows[n]["EndDate"].ToString();
                    }
                    if (dt.Rows[n]["LogFilePath"] != null && dt.Rows[n]["LogFilePath"].ToString() != "")
                    {
                        model.LogFilePath = dt.Rows[n]["LogFilePath"].ToString();
                    }
                    if (dt.Rows[n]["terminal"] != null && dt.Rows[n]["terminal"].ToString() != "")
                    {
                        model.Terminal = dt.Rows[n]["terminal"].ToString();
                    }
                    if (dt.Rows[n]["remark"] != null && dt.Rows[n]["remark"].ToString() != "")
                    {
                        model.Remark = dt.Rows[n]["remark"].ToString();
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

        /// <summary>
        /// 脚本统计数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public DataTable GetScriptStatisticsData(string where, int currentPage, out int pageTotle, int pageNum, out int totleNum)
        {
            return dal.GetScriptStatisticsData(where, currentPage, out pageTotle, pageNum, out totleNum);
        }

        /// <summary>
        /// 脚本统计数据
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public DataTable GetScriptStatisticsData(string where)
        {
            return dal.GetScriptStatisticsData(where);
        }

        /// <summary>
        /// 获取导出数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetExportToXLSData(DataTable table, string where)
        {
            string colStr = string.Empty;
            string leftJoinStr = string.Empty;
            foreach (DataRow row in table.Rows)
            {
                colStr += string.Format(",{0}.value as {0}", row["name"].ToString().Replace("#", "").Replace("&", "").Replace("$", "").Replace("*", "").Replace("@", "").Replace("-", ""));
                leftJoinStr += string.Format(" left join ExecuteResult {0} on a.Id={0}.caseid and {0}.name='{1}'", row["name"].ToString().Replace("#", "").Replace("&", "").Replace("$", "").Replace("*", "").Replace("@", "").Replace("-", ""), row["name"]);
            }
            string sql = string.Format(@"select ScriptName,tn,taskNum,DeviceName,terminal,StartDate,EndDate,ExecuteTrimLenght,IsSucceed,remark {0}
                                        from (select  eor.id as eorId,eor.TaskMain_ID,ter.id as terID,dr.Id,eor.ScriptName as tn,ter.TaskName,dr.ScriptName,dr.IsSucceed,dr.StartDate,dr.EndDate,dr.LogFilePath,dr.terminal,dr.DeviceName,dr.ExecuteTrimLenght,dr.remark,dr.remark,ter.tasknum || '-' || dr.taskNum as taskNum,dr.remark,case dr.IsUnLoad when 0 then '未上传' when 1 then '已上传' end IsUnLoad
                                        from ScriptExecuteDetailResult as dr left join  ScriptExecuteResut as er on (dr.ser_id = er.id) left join
                                        TaskExecuteResult as ter on (er.ter_id = ter.id) left join
                                        TaskOrders as eor on (ter.taskOrders_id = eor.id) where dr.isDel <> 1  and dr.IsSucceed <> 'ON' {2}) as a {1}", colStr, leftJoinStr, where);
            return dal.GetData(sql);
        }

        public DataTable GetExecuteResultName(string where)
        {
            return dal.GetExecuteResultName(where);
        }

        public DataTable GetExecuteResultTable(string where)
        {
            return dal.GetExecuteResultTable(where);
        }

        public DataTable GetScriptStatisticsDataImage(string where)
        {
            return dal.GetScriptStatisticsDataImage(where);
        }

        /// <summary>
        /// 修改上传状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool UpdateIsUnLoadState(string id)
        {
            return dal.UpdateIsUnLoadState(id);
        }

        /// <summary>
        /// 脚本执行成功率
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public DataTable GetScriptExecuteSuccess(string where)
        {
            return dal.GetScriptExecuteSuccess(where);
        }

        public int SetDeleteState(string idList)
        {
            return dal.SetDeleteState(idList);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public int DeleteList(string idList)
        {
            return dal.DeleteList(idList);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public int DeleteList(int sId)
        {
            return dal.DeleteList(sId);
        }

        public List<string> CaseNames(string strWhere)
        {
            return dal.CaseNames(strWhere);
        }
        #endregion  Method
    }
}
}
