// --------------------------------------------------
// Copyright (C) 2015 版权所有
// 创 建 人：蔡少发
// 创建时间：2015-07-24
// 描述信息：用于管理本系统的所有货品出库基本信息
// --------------------------------------------------

using System;

namespace Pharos.Logic.Entity
{
	/// <summary>
	/// 出库单信息
	/// </summary>
	[Serializable]
    public partial class OutboundGoods:BaseEntity
	{
		/// <summary>
		/// 记录ID
		/// [主键：√]
		/// [长度：10]
		/// [不允许为空]
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 出库单ID
		/// [长度：40]
		/// [不允许为空]
		/// </summary>
		public string OutboundId { get; set; }

		/// <summary>
		/// 出货仓库ID
		/// [长度：3]
		/// [不允许为空]
		/// </summary>
		public string StoreId { get; set; }

		/// <summary>
		/// 提货单位ID
		/// [长度：40]
		/// </summary>
		public string ApplyOrgId { get; set; }

		/// <summary>
		/// 提货员UID
		/// [长度：40]
		/// </summary>
		public string ApplyUID { get; set; }

		/// <summary>
		/// 经办人UID 
		/// [长度：40]
		/// [不允许为空]
		/// </summary>
		public string OperatorUID { get; set; }

		/// <summary>
		/// 出仓日期
		/// [长度：23，小数位数：3]
		/// [不允许为空]
		/// </summary>
		public DateTime CreateDT { get; set; }
        /// <summary>
        /// 已审时间
        /// </summary>
        public DateTime? VerifyTime { get; set; }
		/// <summary>
		/// 状态（ 0:待审、 1:已审）
		/// [长度：5]
		/// [不允许为空]
		/// [默认值：((0))]
		/// </summary>
		public short State { get; set; }

        /// <summary>
        /// 出货渠道（0：门店、1：批发）
        /// [长度：5]
        /// [不允许为空]
        /// [默认值：((0))]
        /// </summary>
        public short? Channel { get; set; }

        /// <summary>
        /// 出库方式(1:正常,2:其它)
        /// [长度：5]
        /// [不允许为空]
        /// [默认值：((1))]
        /// </summary>
        public short OutboundType { get; set; }
	}
}