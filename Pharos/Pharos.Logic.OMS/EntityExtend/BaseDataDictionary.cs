// --------------------------------------------------
// Copyright (C) 2016 版权所有
// 创 建 人：蔡少发
// 创建时间：2016-09-03
// 描述信息：
// --------------------------------------------------

using System;

namespace Pharos.Logic.OMS.Entity
{
	/// <summary>
	/// 用于管理本系统的所有业务数据字典信息
	/// </summary>
	[Serializable]
    public abstract class BaseDataDictionary
	{
		/// <summary>
		/// 记录ID
		/// [主键：√]
		/// [长度：10]
		/// [不允许为空]
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 编号（该编号全局唯一）
		/// [长度：10]
		/// [不允许为空]
		/// </summary>
		public int DicSN { get; set; }

		/// <summary>
		/// 父编号ID（0：顶级）
		/// [长度：10]
		/// [不允许为空]
		/// </summary>
		public int DicPSN { get; set; }

		/// <summary>
		/// 排序（0:无）
		/// [长度：10]
		/// [不允许为空]
		/// [默认值：((0))]
		/// </summary>
		public int SortOrder { get; set; }

		/// <summary>
		/// 类别名称
		/// [长度：50]
		/// [不允许为空]
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// 深度(1:一级、2:二级、3:三级、4:四级、9:具体字典)
		/// [长度：5]
		/// [默认值：((1))]
		/// </summary>
		public short Depth { get; set; }
        /// <summary>
        /// 是否有子节点
        /// </summary>
        public bool HasChild { get; set; }
		/// <summary>
		/// 状态（0:关闭、1:可用）
		/// [长度：1]
		/// [不允许为空]
		/// [默认值：((1))]
		/// </summary>
		public bool Status { get; set; }

		/// <summary>
		/// 
		/// [长度：23，小数位数：3]
		/// [默认值：(getdate())]
		/// </summary>
		public DateTime CreateDT { get; set; }

		/// <summary>
		/// 
		/// [长度：40]
		/// </summary>
		public string CreateUID { get; set; }
	}
}