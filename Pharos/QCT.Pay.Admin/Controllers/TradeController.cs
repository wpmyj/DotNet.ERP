﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QCT.Pay.Admin.Controllers
{
    /// <summary>
    /// 交易管理
    /// </summary>
    public class TradeController : Controller
    {
        //
        // GET: /Trade/
        #region 交易流水数据页面
        /// <summary>
        /// 交易流水数据页面-页面加载
        /// </summary>
        /// <returns></returns>
        public ActionResult TradeDataIndex()
        {
            return View();
        }
        #endregion
    }
}