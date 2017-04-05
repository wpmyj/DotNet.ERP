﻿using Pharos.Utility.Helpers;
using Pharos.Logic.OMS.BLL;
using Pharos.Logic.OMS.BLL.Pay;
using QCT.Pay.Common.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace QCT.Api.Pay.Utils
{
    /// <summary>
    /// 验证签名
    /// </summary>
    public class SignVerifyAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 签名字段(SignField 属性值为空，支持Qct、Sxf)
        /// </summary>
        public string SignField { get; set; }
        /// <summary>
        /// 获取记录Action的动作的日志标题
        /// </summary>
        /// <returns></returns>
        private string LogTitle
        {
            get
            {
                if (SignField == "signature")
                    return "接收通知请求";
                else
                    return "接收交易请求";
            }
        }
        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var dic = GetForm(System.Web.HttpContext.Current.Request.Form);
            try
            {
                //请求日志记录
                PayLogServer.WriteInfo(string.Format("{0}：{1}", LogTitle, dic.ToJson()));

                var signSvc = new PaySignService();
                if (!signSvc.VerifySign(dic, SignField))
                {
                    PayLogServer.WriteInfo(string.Format("{0}时签名失败：，{1}", LogTitle, dic.ToJson()));
                    actionContext.Response = GetResponseMsg(QctPayReturn.Fail(msg: "签名失败").ToJson());
                }
            }
            catch (Exception ex)
            {
                PayLogServer.WriteError(string.Format("{0}异常：{1}", LogTitle, dic.ToJson()), ex);
                actionContext.Response = GetResponseMsg(QctPayReturn.Fail(msg: "参数格式错误").ToJson());
            }

            base.OnActionExecuting(actionContext);
        }
        /// <summary>
        /// 获取参数内容
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        async private Task<string> PostRaw(HttpContent content)
        {
            return await content.ReadAsStringAsync();
        }
        /// <summary>
        /// 返回对象
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private HttpResponseMessage GetResponseMsg(string msg)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(msg)
            };
            return response;
        }
        /// <summary>
        /// 获取Form Keys转换为数据字典对象
        /// </summary>
        /// <param name="collect"></param>
        /// <returns></returns>
        private Dictionary<string, object> GetForm(NameValueCollection collect)
        {
            var dic = new Dictionary<string, object>();
            foreach (String s in collect.AllKeys)
            {
                dic.Add(s, collect[s]);
            }
            return dic;
        }
    }

}