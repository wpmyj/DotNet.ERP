﻿using Pharos.Logic.OMS.DAL;
using Pharos.Logic.OMS.Entity;
using Pharos.Logic.OMS.IDAL;
using Pharos.Logic.OMS.Models;
using QCT.Pay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pharos.Logic.OMS.BLL
{
    /// <summary>
    /// 支付公共Service
    /// </summary>
    public class CommonPayService
    {
        /// <summary>
        /// 支付渠道仓储
        /// </summary>
        IBaseRepository<PayChannelManage> PayChannelMgRepost { get; set; }
        /// <summary>
        /// 支付渠道细目仓储
        /// </summary>
        IBaseRepository<PayChannelDetail> PayChannelDetailRepost { get; set; }
        /// <summary>
        /// 交易订单仓储
        /// </summary>
        IBaseRepository<TradeOrder> TradeOrderRepost { get; set; }
        /// <summary>
        /// 根据表明及字段名获取对应字段Max值
        /// </summary>
        /// <param name="tbName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public int GetMaxNo(string tbName,string fieldName)
        {
            string sql = "SELECT ISNULL(MAX({0}),0) FROM {1}";
            var db = ContextFactory.GetCurrentContext<EFDbContext>().Database;
            var result = db.SqlQuery<int>(string.Format(sql, fieldName, tbName)).FirstOrDefault();
            return result + 1;
        }
        /// <summary>
        /// 根据ApiNo获取收单渠道细目信息
        /// </summary>
        /// <param name="merchObj"></param>
        /// <returns></returns>
        public PayChannelModel GetPayChannelDetail(MerchantChannelModel merchObj)
        {
            PayChannelMgRepost = new BaseRepository<PayChannelManage>();
            PayChannelDetailRepost = new BaseRepository<PayChannelDetail>();
            var query = from pcd in PayChannelDetailRepost.GetQuery()
                        join jpcm in PayChannelMgRepost.GetQuery() on pcd.ChannelNo equals jpcm.ChannelNo into ipcm
                        from pcm in ipcm.DefaultIfEmpty()
                        where pcd.ChannelNo == merchObj.ChannelNo && pcm.State == (short)PayChannelState.Enabled
                        && pcd.ChannelPayMode == merchObj.ChannelPayMode && (pcd.OptType.Contains(merchObj.OptType.ToString()) || pcd.OptType.Contains("0"))
                        select new PayChannelModel()
                        {
                            ApiNo = merchObj.ApiNo,
                            OptType = merchObj.OptType,
                            ChannelPayMode = merchObj.ChannelPayMode,
                            MonthFreeTradeAmount = pcd.MonthFreeTradeAmount,
                            OverServiceRate = pcd.OverServiceRate,
                            SingleServFeeLowLimit = pcd.SingleServFeeLowLimit,
                            SingleServFeeUpLimit = pcd.SingleServFeeUpLimit
                        };
            var rst = query.FirstOrDefault();
            return rst;
        }
        /// <summary>
        /// 获取当月交易金额总和 fishtodo：待确认哪些操作类型需要计算手续费
        /// </summary>
        /// <param name="date"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public decimal GetMonthTotalTradeAmt(DateTime date, int cid)
        {
            TradeOrderRepost = new BaseRepository<TradeOrder>();
            var monthFirstDay = FirstDayOfPreviousMonth(date).ToString("yyyyMMdd").ToString();
            var monthLastDay = LastDayOfMonth(date).ToString("yyyyMMdd").ToString();
            var monthTotalAmt = TradeOrderRepost.GetQuery(o => o.State == (short)PayState.PaySuccess
                                 && o.TradeDate.CompareTo(monthFirstDay) >= 0 && o.TradeDate.CompareTo(monthLastDay) <= 0
                                 && o.TradeType != (short)QctTradeType.Expense).Sum(o => (decimal?)o.ReceiptAmount).GetValueOrDefault();
            return monthTotalAmt;
        }
        /// <summary>  
        /// 取得某月的最后一天  
        /// </summary>  
        /// <param name="datetime">要取得月份最后一天的时间</param>  
        /// <returns></returns>  
        private DateTime LastDayOfMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
        }
        /// <summary>  
        /// 取得上个月第一天  
        /// </summary>  
        /// <param name="datetime">要取得上个月第一天的当前时间</param>  
        /// <returns></returns>  
        private DateTime FirstDayOfPreviousMonth(DateTime datetime)
        {
            return datetime.AddDays(1 - datetime.Day).AddMonths(-1);
        }
    }
}