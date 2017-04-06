// --------------------------------------------------
// Copyright (C) 2015 版权所有
// 创 建 人：蔡少发
// 创建时间：2015-05-22
// 描述信息：用于管理本系统的所有订货明细配送信息
// --------------------------------------------------

using System;

namespace Pharos.Logic.Entity
{
	/// <summary>
	/// 订货配送信息
	/// </summary>
	[Serializable]
    public partial class OrderDistribution:BaseEntity
	{
        /// <summary>
        /// 记录ID
        /// [主键：√]
        /// </summary>
        public int Id { get; set; }

		/// <summary>
		/// 配送 ID	
		/// [长度：40]
		/// [不允许为空]
		/// </summary>
        public string DistributionId { get; set; }

		/// <summary>
		/// 采购单ID(订货单ID)
		/// [长度：40]
		/// [不允许为空]
		/// </summary>
		public string IndentOrderId { get; set; }

		/// <summary>
		/// 商品条码
		/// [长度：30]
		/// [不允许为空]
		/// </summary>
		public string Barcode { get; set; }
        
        /// <summary>
        /// 生产日期
        /// [长度：10]
        /// </summary>
        public string ProducedDate { get; set; }
        //[Newtonsoft.Json.JsonConverter(typeof(Pharos.Utility.JsonShortDate))]
        //public DateTime? ProducedDate { get; set; }

		/// <summary>
		/// 有效期（天数）
		/// [长度：5]
		/// [默认值：((-1))]
		/// </summary>
		public short ExpiryDate { get; set; }
        /// <summary>
        /// 有效期单位（ 1:天、 2:月、 3:年）
        /// </summary>
        public short? ExpiryDateUnit { get; set; }
		/// <summary>
		/// 截止保质日期
		/// [长度：10]
		/// </summary>
		public string ExpirationDate { get; set; }

		/// <summary>
		/// 生产批次
		/// [长度：30]
		/// </summary>
		public string ProductionBatch { get; set; }
        /// <summary>
        /// 配送批次
        /// [长度：30]
        /// </summary>
        public string DistributionBatch { get; set; }
		/// <summary>
		/// 配送数量
		/// [默认值：((0))]
		/// </summary>
        public decimal? DeliveryNum { get; set; }

		/// <summary>
		/// 配送时间
		/// [长度：23，小数位数：3]
		/// [默认值：(getdate())]
		/// </summary>
        [Pharos.Utility.Exclude]
		public DateTime DeliveryDT { get; set; }
        /// <summary>
        /// 收货数量
        /// [默认值：0]
        /// </summary>
        public decimal? ReceivedNum { get; set; }
        /// <summary>
        /// 收货时间
        /// [默认值：(getdate())]
        /// </summary>
        public DateTime? ReceivedDT { get; set; }

		/// <summary>
        /// 状态（1: 未配送、 2:配送中、 3:已中止、4:已配送、 5:已收货、6:已预约退换）
		/// [长度：5]
		/// [不允许为空]
		/// [默认值：((5))]
		/// </summary>
        [Pharos.Utility.Exclude]
		public short State { get; set; }
        /// <summary>
        /// 备注
        /// [长度：200]
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        ///退换ID
        /// </summary>
        public int? OrderReturnId { get; set; }
	}
}