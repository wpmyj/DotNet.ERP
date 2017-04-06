﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Pharos.Logic.ApiData.Mobile.Services;
using Pharos.Api.Retailing.Models.Mobile;

namespace Pharos.Api.Retailing.Controllers.Mobile
{
    /// <summary>
    /// 报表
    /// </summary>
    [RoutePrefix("api/mobile")]
    public class ReportController : ApiController
    {
        /// <summary>
        /// 销售类型日结
        /// </summary>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaleTypeDayReport")]
        public object SaleTypeDayReport([FromBody]JObject requestParams)
        {
            string storeId = requestParams.Property("storeId", true);
            string date = requestParams.Property("date", true);
            return ReportService.SaleTypeReport(storeId, date, 1);
        }
        /// <summary>
        /// 销售类型月结
        /// </summary>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaleTypeMonthReport")]
        public object SaleTypeMonthReport([FromBody]JObject requestParams)
        {
            string storeId = requestParams.Property("storeId", true);
            string date = requestParams.Property("date", true);
            return ReportService.SaleTypeReport(storeId, date, 2);
        }
        /// <summary>
        /// 销售员销售统计
        /// </summary>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaleCashierDateReport")]
        public object SaleCashierDateReport([FromBody]SaleReportRequest requestParams)
        {
            return ReportService.SaleCashierDateReport(requestParams.StoreId, requestParams.Date,requestParams.Date2, requestParams.Type);
        }
        /// <summary>
        /// 帐单
        /// </summary>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaleDateBills")]
        public object SaleDateBills([FromBody]SaleBillRequest requestParams)
        {
            return ReportService.SaleDateBills(requestParams.StoreId, requestParams.Type, requestParams.Time, requestParams.Cashier,requestParams.Saler,requestParams.PageIndex,requestParams.PageSize);
        }
        /// <summary>
        /// 帐单汇总前6个月
        /// </summary>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaleMonthBillSummarys")]
        public object SaleMonthBillSummarys([FromBody]BaseParams requestParams)
        {
            return ReportService.SaleMonthBillSummarys();
        }
        /// <summary>
        /// 帐单汇总
        /// </summary>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaleDateBillSummarys")]
        public object SaleDateBillSummarys([FromBody]SaleSummaryRequest requestParams)
        {
            return ReportService.SaleDateBillSummarys(requestParams.Type,requestParams.StartDate,requestParams.EndDate);
        }
        /// <summary>
        /// 门店销售统计
        /// </summary>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaleStoreDateReport")]
        public object SaleStoreDateReport([FromBody]SaleReportRequest requestParams)
        {
            return ReportService.SaleStoreDateReport(requestParams.Date, requestParams.Date2, requestParams.Type);
        }
        
        /// <summary>
        /// 销售商品日统计
        /// </summary>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaleDetailDayReport")]
        public object SaleDetailDayReport([FromBody]JObject requestParams)
        {
            string storeId = requestParams.Property("storeId", true);
            string date = requestParams.Property("date", true);
            return ReportService.SaleDetailDayReport(storeId, date);
        }
        /// <summary>
        /// 销售品类日统计
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SaleCategoryDayReport")]
        public object SaleCategoryDayReport([FromBody]JObject requestParams)
        {
            string storeId = requestParams.Property("storeId", true);
            string date = requestParams.Property("date", true);
            return ReportService.SaleDayReport(storeId, date,"1");
        }
        /// <summary>
        /// 销售品牌日统计
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SaleBrandDayReport")]
        public object SaleBrandDayReport([FromBody]JObject requestParams)
        {
            string storeId = requestParams.Property("storeId", true);
            string date = requestParams.Property("date", true);
            return ReportService.SaleDayReport(storeId, date, "2");
        }
        /// <summary>
        /// 销售供应商日统计
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SaleSupplierDayReport")]
        public object SaleSupplierDayReport([FromBody]JObject requestParams)
        {
            string storeId = requestParams.Property("storeId", true);
            string date = requestParams.Property("date", true);
            return ReportService.SaleDayReport(storeId, date, "3");
        }
        /// <summary>
        /// 销售赠送月统计
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SaleGiftReport")]
        public object SaleGiftReport([FromBody]JObject requestParams)
        {
            string date = requestParams.Property("date", true);
            return ReportService.SaleGiftReport(date);
        }
        /// <summary>
        /// 缺货统计
        /// </summary>
        /// <returns></returns>
        [Route("StockOutReport")]
        public object StockOutReport()
        {
            return ReportService.StockOutReport();
        }
        /// <summary>
        /// 过剩统计
        /// </summary>
        /// <returns></returns>
        [Route("StockMoreReport")]
        public object StockMoreReport()
        {
            return ReportService.StockMoreReport();
        }
        /// <summary>
        /// 今日会员量
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("MemberDayReport")]
        public object MemberDayReport([FromBody]JObject requestParams)
        {
            string date = requestParams.Property("date", true);
            return ReportService.MemberReport(date,1);
        }
        /// <summary>
        /// 本月会员量
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("MemberMonthReport")]
        public object MemberMonthReport([FromBody]JObject requestParams)
        {
            string date = requestParams.Property("date", true);
            return ReportService.MemberReport(date, 2);
        }
        /// <summary>
        /// 全部会员量
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("MemberAllReport")]
        public object MemberAllReport([FromBody]JObject json2)
        {
            string date = json2.Property("date", true);
            return ReportService.MemberReport(date, 3);
        }
    }
}