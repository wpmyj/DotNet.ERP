// --------------------------------------------------
// Copyright (C) 2015 版权所有
// 创 建 人：蔡少发
// 创建时间：2015-05-22
// 描述信息：用于管理本系统的所有会员信息
// --------------------------------------------------

using Pharos.Logic.BLL.DataSynchronism;
using Pharos.Logic.BLL.LocalServices.DataSync;
using Pharos.Utility;
using System;

namespace Pharos.Logic.LocalEntity
{
    [Excel("会员信息")]

    /// <summary>
    /// 会员信息
    /// </summary>
    public class Members : BaseEntity, ICanUploadEntity
    {
        public Int64 Id { get; set; }
        public string StoreId { get; set; }

        /// <summary>
        /// 会员卡号（全局唯一）
        /// [长度：100]
        /// [不允许为空]
        /// </summary>
        [Excel("会员卡号", 2)]
        public string MemberCardNum { get; set; }


        /// <summary>
        /// 会员姓名
        /// [长度：20]
        /// </summary>
        [Excel("会员姓名", 3)]
        public string RealName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [Excel("性别", 4)]
        public short Sex { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>
        [Excel("微信号", 5)]
        public string Weixin { get; set; }


        /// <summary>
        /// 手机号（全局唯一）
        /// [长度：11]
        /// </summary>
        [Excel("手机号", 6)]
        public string MobilePhone { get; set; }


        /// <summary>
        /// Email（全局唯一）
        /// [长度：100]
        /// </summary>
        [Excel("Email", 7)]
        public string Email { get; set; }

        /// <summary>
        /// QQ
        /// [长度：100]
        /// </summary>
        [Excel("QQ", 8)]
        public string QQ { get; set; }
        /// <summary>
        /// 生日（yyyy-MM-dd）
        /// [长度：10]
        /// </summary>
        [Excel("生日", 9)]
        public string Birthday { get; set; }


        /// <summary>
        /// 可用积分
        /// [长度：10]
        /// [默认值：((0))]
        /// </summary>
        [Excel("可用积分", 10)]
        public int UsableIntegral { get; set; }

        /// <summary>
        /// 已用积分
        /// [长度：10]
        /// [默认值：((0))]
        /// </summary>
        [Excel("已用积分", 11)]
        public int UsedIntegral { get; set; }


        /// <summary>
        /// 所在城市ID
        /// [长度：10]
        /// </summary>
        [Excel("所在城市ID", 12)]
        public int CurrentCityId { get; set; }
        /// <summary>
        /// 联系地址
        /// </summary>
        [Excel("联系地址", 13)]
        public string Address { get; set; }
        /// <summary>
        /// 消费额度
        /// </summary>
        [Excel("消费额度", 14)]
        public decimal? ConsumerCredit { get; set; }

        /// <summary>
        /// 内部人员
        /// [长度：40]
        /// </summary>
        [Excel("内部人员", 15)]
        public bool Insider { get; set; }

        /// <summary>
        /// 创建时间
        /// [长度：23，小数位数：3]
        /// [不允许为空]
        /// </summary>
        [Excel("创建时间", 16)]
        public DateTime CreateDT { get; set; }

        /// <summary>
        /// 创建人UID
        /// [长度：40]
        /// [不允许为空]
        /// </summary>
        [Excel("创建人UID", 17)]
        public string CreateUID { get; set; }
        [LocalKey]
        public string MemberId { get; set; }
        /// <summary>
        /// 状态  1－可用 0－禁用
        /// </summary>
        public short Status { get; set; }

        public bool IsUpload { get; set; }
    }
}