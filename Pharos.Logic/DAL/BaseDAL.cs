﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using Pharos.DBFramework;
using Pharos.Utility;
using Pharos.Utility.Helpers;
namespace Pharos.Logic.DAL
{
    public class BaseDAL
    {
        internal DBHelper _db = new DBHelper();
        /// <summary>
        /// 自动分页方法
        /// </summary>
        /// <param name="strSql">完整sql语句</param>
        /// <param name="recordCount">记录总数</param>
        /// <param name="nvl">参数值</param>
        /// <returns></returns>
        protected System.Data.DataTable ExceuteSqlForPage(string strSql,out int recordCount,System.Collections.Specialized.NameValueCollection nvl=null)
        {
            nvl = nvl ?? HttpContext.Current.Request.Params;
            var pageIndex = 1;
            var pageSize = 30;
            var sort = "Id";
            var order = "asc";
            if (!nvl["page"].IsNullOrEmpty())
                pageIndex = int.Parse(nvl["page"]);
            if (!nvl["rows"].IsNullOrEmpty())
                pageSize = int.Parse(nvl["rows"]);
            if (!nvl["sort"].IsNullOrEmpty())
                sort = nvl["sort"];
            if (!nvl["order"].IsNullOrEmpty())
                order = nvl["order"];
            order = order.ToLower();
            if (!(order == "asc" || order == "desc"))
                throw new ArgumentException("排序类型错误!");

            string orderSql = string.Format("(ROW_NUMBER() OVER ( ORDER BY {0} {1})) AS RSNO", sort, order);
            strSql = string.Format("select * from(select {0},* from ({1}) tb) t", orderSql, strSql);
            var page = new Utility.Paging();
            var parms = new SqlParameter[] { 
                new SqlParameter("@SqlStr",strSql),
                new SqlParameter("@CurrentPage",pageIndex),
                new SqlParameter("@PageSize",pageSize)
            };
            var dt = _db.DataTable("Comm_PageList", parms, ref page);
            recordCount = page.RecordTotal;
            return dt;
        }
        /// <summary>
        /// 自动按分组分页方法
        /// </summary>
        /// <param name="strSql">完整sql语句</param>
        /// <param name="recordCount">记录总数</param>
        /// <param name="nvl">参数值</param>
        /// <returns></returns>
        protected System.Data.DataTable ExceuteSqlForGroupPage(string strSql, out int recordCount, System.Collections.Specialized.NameValueCollection nvl = null)
        {
            nvl = nvl ?? HttpContext.Current.Request.Params;
            var pageIndex = 1;
            var pageSize = 30;
            var sort = "Id";
            var order = "asc";
            if (!nvl["page"].IsNullOrEmpty())
                pageIndex = int.Parse(nvl["page"]);
            if (!nvl["rows"].IsNullOrEmpty())
                pageSize = int.Parse(nvl["rows"]);
            if (!nvl["sort"].IsNullOrEmpty())
                sort = nvl["sort"];
            if (!nvl["order"].IsNullOrEmpty())
                order = nvl["order"];
            order = order.ToLower();
            if (!(order == "asc" || order == "desc"))
                throw new ArgumentException("排序类型错误!");

            string orderSql = string.Format("(DENSE_RANK() OVER ( ORDER BY {0} {1})) AS RSNO", sort, order);
            strSql = string.Format("select * from(select {0},* from ({1}) tb) t", orderSql, strSql);
            var page = new Utility.Paging();
            var parms = new SqlParameter[] { 
                new SqlParameter("@SqlStr",strSql),
                new SqlParameter("@CurrentPage",pageIndex),
                new SqlParameter("@PageSize",pageSize)
            };
            var dt = _db.DataTable("Comm_PageList", parms, ref page);
            recordCount = page.RecordTotal;
            return dt;
        }
        /// <summary>
        /// 验验证界面输入信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        internal OpResult ValidatorInput<T>(T entity) where T:class,new ()
        {
            var op = new OpResult() { Successed=true};
            var type = typeof(T);
            var tableName = type.Name;
            var dt= DataCache.Get<System.Data.DataTable>("validators");
            if (dt == null)
            {
                string sql = @"SELECT a.name filedName,a.length filedLen,b.name tableName,a.isnullable FROM syscolumns a INNER JOIN sysobjects b ON a.id=b.id AND b.xtype='u'
                    WHERE a.xtype in(167,231) AND a.length>0";
                dt= _db.DataTableText(sql, null);
                DataCache.Set("validators", dt,20);
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                var props = type.GetProperties().Where(o => o.PropertyType == typeof(string));
                foreach (var o in props)
                {
                    var drs = dt.Select("tableName='" + tableName + "' and filedName='" + o.Name + "'");
                    if (drs.Length > 0)
                    {
                        var maxlen = Convert.ToInt16(drs[0]["filedLen"]);
                        var isNull = Convert.ToInt16(drs[0]["isnullable"]);
                        var val = Convert.ToString(o.GetValue(entity, null));
                        if (!val.IsNullOrEmpty())
                        {
                            var len = System.Text.Encoding.Default.GetByteCount(val);
                            if (len > maxlen)
                            {
                                op.Message = "[" + o.Name + "]字符串内容过长";
                                op.Successed = false;
                                return op;
                            }
                        }
                    }
                }
            }
            return op;
        }
    }
}