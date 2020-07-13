using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.BLL
{
    public class ExecuteResultManager
    {
        private readonly ExecuteResultServer dal = new ExecuteResultServer();

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(ExecuteResult model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 获取结果数据
        /// </summary>
        /// <param name="taskOrderId"></param>
        /// <returns></returns>
        public List<ExecuteResult> GetDatas(long caseId)
        {
            return dal.GetDatas(caseId);
        }

        /// <summary>
        /// 获取结果数据
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public DataTable GetResults(string strWhere, ref int caseCount, ref int vCount)
        {
            return dal.GetResults(strWhere, ref caseCount, ref vCount);
        }
    }
}
