﻿
namespace Pharos.POS.Retailing.Models.ApiParams
{
    public class ThirdPartyPaymentStatusParams : BaseApiParams
    {
        /// <summary>
        /// 交易流水号
        /// </summary>
        public string PaySn { get; set; }
        /// <summary>
        /// 对应枚举 3=支付宝扫码 4=微信扫码 银联支付=2 8=支付宝扫码枪扫描 9=微信扫码枪扫描
        /// </summary>
        public int Mode { get; set; }
    }
}