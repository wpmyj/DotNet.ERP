﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pharos.Logic.OMS.Entity.View
{
    public class ViewPayLicense
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 指派人
        /// </summary>
        public string DesigneeFullName { get; set; }

        /// <summary>
        /// 许可状态
        /// </summary>
        public short State { get; set; }

        /// <summary>
        /// 结算账户状态
        /// </summary>
        public int? BankState { get; set; }

        /// <summary>
        /// 支付账号状态
        /// </summary>
        public int? PayState { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        public int? AgentsId { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public int CID { get; set; }

        /// <summary>
        /// 商家公司名称
        /// </summary>
        public string FullTitle { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContractNo { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 所属体系
        /// </summary>
        public short SourceType { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegisterAddress { get; set; }

        /// <summary>
        /// 营业执照注册号
        /// </summary>
        public string RegisterNumber { get; set; }

        /// <summary>
        /// 经营范围
        /// </summary>
        public string BusinessScope { get; set; }

        /// <summary>
        /// 商家业务联系人
        /// </summary>
        public string Linkman { get; set; }

        /// <summary>
        /// 商家业务联系人电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 商家业务联系人邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CreateDT { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string CreateFullName { get; set; }

    }
}