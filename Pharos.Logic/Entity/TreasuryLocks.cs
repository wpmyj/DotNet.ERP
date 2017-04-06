// --------------------------------------------------
// Copyright (C) 2015 版权所有
// 创 建 人：蔡少发
// 创建时间：2015-07-24
// 描述信息：用于管理本系统的所有库存货品盘点信息
// --------------------------------------------------

using System;

namespace Pharos.Logic.Entity
{
	/// <summary>
	/// 库存锁定
	/// </summary>
	[Serializable]
    public partial class TreasuryLocks:BaseEntity
	{
		/// <summary>
		/// 记录ID
		/// [主键：√]
		/// [长度：10]
		/// [不允许为空]
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 锁定仓库GUID
		/// [长度：3]
		/// [不允许为空]
		/// </summary>
		public string LockStoreID { get; set; }

		/// <summary>
		/// 锁定品类ID（多个ID以,号间隔）
		/// [长度：500]
		/// </summary>
		public string LockCategorySN { get; set; }

		/// <summary>
		/// 盘点批次（全局唯一）
		/// [长度：30]
		/// [不允许为空]
		/// </summary>
		public string CheckBatch { get; set; }

		/// <summary>
		/// 锁定日期
		/// [长度：23，小数位数：3]
		/// [不允许为空]
		/// [默认值：(getdate())]
		/// </summary>
		public DateTime LockDate { get; set; }
        /// <summary>
        /// 审批状态（0:未审、1:已审）
        /// </summary>
        public short State { get; set; }
		/// <summary>
		/// 锁定人UID
		/// [长度：40]
		/// [不允许为空]
		/// </summary>
		public string LockUID { get; set; }
	}
}