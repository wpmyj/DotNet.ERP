﻿// --------------------------------------------------
// Copyright (C) 2015 版权所有
// 创 建 人：蔡少发
// 创建时间：2015-07-24
// 描述信息：用于管理本系统的所有货品入库基本信息
// --------------------------------------------------

using System;

namespace Pharos.Logic.Entity
{
	/// <summary>
	/// 入库单信息
	/// </summary>
	[Serializable]
    public partial class InboundGoods:BaseEntity
	{
		/// <summary>
		/// 记录ID
		/// [主键：√]
		/// [长度：10]
		/// [不允许为空]
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 入库单Id
		/// [长度：40]
		/// [不允许为空]
		/// </summary>
		public string InboundGoodsId { get; set; }

		/// <summary>
		/// 入库门店ID
		/// [长度：3]
		/// [不允许为空]
		/// </summary>
		public string StoreId { get; set; }

		/// <summary>
		/// 采购单号（订货单号）
		/// [长度：40]
		/// </summary>
		public string IndentOrderId { get; set; }

		/// <summary>
		/// 供货单位ID
		/// [长度：40]
		/// </summary>
		public string SupplierID { get; set; }

		/// <summary>
		/// 到货日期
		/// [长度：10]
		/// </summary>
		public string ReceivedDT { get; set; }

		/// <summary>
		/// 采购员 ID（下单人）
		/// [长度：40]
		/// </summary>
		public string BuyerUID { get; set; }

		/// <summary>
		/// 配送批次
		/// [长度：30]
		/// </summary>
		public string DistributionBatch { get; set; }

		/// <summary>
		/// 入库员 UID
		/// [长度：40]
		/// [不允许为空]
		/// </summary>
		public string CreateUID { get; set; }

		/// <summary>
		/// 入仓日期
		/// [长度：23，小数位数：3]
		/// [不允许为空]
		/// [默认值：(getdate())]
		/// </summary>
		public DateTime CreateDT { get; set; }
        /// <summary>
        /// 已验时间
        /// </summary>
        public DateTime? VerifyTime { get; set; }
		/// <summary>
		/// 状态（ 0:待验、 1:已验）
		/// [长度：5]
		/// [不允许为空]
		/// [默认值：((0))]
		/// </summary>
		public short State { get; set; }

        /// <summary>
        /// 入库方式(1:正常,2:其它)
        /// [长度：5]
        /// [不允许为空]
        /// [默认值：((1))]
        /// </summary>
        public short InboundType { get; set; }
        /// <summary>
        /// 采购来源（1-本系统，2-外部）
        /// </summary>
        public short Source { get; set; }
	}
}