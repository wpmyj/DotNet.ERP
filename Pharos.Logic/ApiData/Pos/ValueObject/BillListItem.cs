﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pharos.Logic.ApiData.Pos.ValueObject
{
    public class BillListItem
    {
        /// <summary>
        /// 订单流水号
        /// </summary>
        public string PaySn { get; set; }
        /// <summary>
        /// 订单商品件数
        /// </summary>
        public decimal Number { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 订单时间
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderStatus { get; set; }

        public string Cashier { get; set; }

        public string SaleMan { get; set; }

        public short OrderType { get; set; }
    }
}