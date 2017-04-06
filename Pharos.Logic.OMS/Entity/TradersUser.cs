// --------------------------------------------------
// Copyright (C) 2017 版权所有
// 创 建 人：
// 创建时间：
// 描述信息：
// --------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharos.Logic.OMS.Entity
{
	/// <summary>
	/// 用于管理本系统商家登录支付后台账号信息
	/// </summary>
	[Serializable]
	public class TradersUser
	{
		/// <summary>
		/// 记录ID
		/// [主键：√]
		/// [长度：10]
		/// [不允许为空]
		/// </summary>
		public int Id
		{
			get { return _Id; }
			set { _Id = value; }
		}
		private int _Id;

		/// <summary>
		/// 账号ID
		/// [长度：40]
		/// [不允许为空]
		/// </summary>
		public string TUserId
		{
			get { return _TUserId; }
			set { _TUserId = value; }
		}
		private string _TUserId;

        /// <summary>
        /// TradersStore表TStoreInfoId
        /// </summary>
        public string TStoreInfoId { get; set; }

		/// <summary>
		/// 商户号
		/// [长度：10]
		/// [不允许为空]
		/// </summary>
		public int CID
		{
			get { return _CID; }
			set { _CID = value; }
		}
		private int _CID;

		/// <summary>
		/// 登录账号
		/// [长度：100]
		/// [不允许为空]
		/// </summary>
		public string LoginName
		{
			get { return _LoginName; }
			set { _LoginName = value; }
		}
		private string _LoginName;

		/// <summary>
		/// 登录密码
		/// [长度：100]
		/// </summary>
		public string LoginPwd
		{
			get { return _LoginPwd; }
			set { _LoginPwd = value; }
		}
		private string _LoginPwd;

		/// <summary>
		/// 员工姓名
		/// [长度：50]
		/// [不允许为空]
		/// </summary>
		public string FullName
		{
			get { return _FullName; }
			set { _FullName = value; }
		}
		private string _FullName;

		/// <summary>
		/// 联系电话
		/// [长度：50]
		/// </summary>
		public string Phone
		{
			get { return _Phone; }
			set { _Phone = value; }
		}
		private string _Phone;

		/// <summary>
		/// 账号类型（1:管理员， 2:普通账号）
		/// [长度：10]
		/// [不允许为空]
		/// </summary>
		public int AccountType
		{
			get { return _AccountType; }
			set { _AccountType = value; }
		}
		private int _AccountType;

		/// <summary>
		/// 状态（1:未审核，2:可用，3:停用）
		/// [长度：10]
		/// [不允许为空]
		/// </summary>
		public int State
		{
			get { return _State; }
			set { _State = value; }
		}
		private int _State;

        /// <summary>
        /// 是否隐藏（1是，0否）
        /// </summary>
        public int IsHide { get; set; }

		/// <summary>
		/// 备注
		/// [长度：50]
		/// </summary>
		public string Memo
		{
			get { return _Memo; }
			set { _Memo = value; }
		}
		private string _Memo;

		/// <summary>
		/// 平台创建人（SysUser表UserId）
		/// [长度：40]
		/// </summary>
		public string SysCreateUID
		{
			get { return _SysCreateUID; }
			set { _SysCreateUID = value; }
		}
		private string _SysCreateUID;

		/// <summary>
		/// 门店创建人（TradersUser表TuserId）
		/// [长度：40]
		/// </summary>
		public string StoCreateUID
		{
			get { return _StoCreateUID; }
			set { _StoCreateUID = value; }
		}
		private string _StoCreateUID;

		/// <summary>
		/// 创建时间
		/// [长度：23，小数位数：3]
		/// [不允许为空]
		/// </summary>
		public DateTime CreateDT
		{
			get { return _CreateDT; }
			set { _CreateDT = value; }
		}
		private DateTime _CreateDT;

        /// <summary>
        /// 商户全称
        /// </summary>
        [NotMapped]
        public string TradersFullTitle { get; set; }

        /// <summary>
        /// 门店全称
        /// </summary>
        [NotMapped]
        public string StoreFullTitle { get; set; }

	}
}