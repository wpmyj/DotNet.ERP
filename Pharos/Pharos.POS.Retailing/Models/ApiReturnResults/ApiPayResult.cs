﻿
using System;
namespace Pharos.POS.Retailing.Models.ApiReturnResults
{
    public class ApiPayResult
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 支付流水号
        /// </summary>
        public string PaySN { get; set; }

        public DateTime CreatDt { get; set; }
    }
}