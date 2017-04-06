﻿
using Pharos.Logic.ApiData.Pos.ValueObject;
namespace Pharos.Api.Retailing.Models
{
    public class LoginRequest : BaseApiParams
    {
        /// <summary>
        /// 帐号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 入口点
        /// </summary>
        public EntryPoint EntryPoint { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType DeviceType { get; set; }
        /// <summary>
        /// 进入练习模式
        /// </summary>
        public bool InTestMode { get; set; }
        int _CID = 1;
        public int CID { get { return _CID; } set { _CID = value; } }
        /// <summary>
        /// Pos是否锁屏登录
        /// </summary>
        public bool IsLock { get; set; }
    }
}