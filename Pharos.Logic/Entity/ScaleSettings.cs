﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
namespace Pharos.Logic.Entity
{
    //ScaleSettings
    public class ScaleSettings
    {

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// SN
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// CompanyId
        /// </summary>
        public int CompanyId { get; set; }
        public string Store { get; set; }
        /// <summary>
        /// 秤名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 设备类型（来自字典）
        /// </summary>
        public string TypeSN { get; set; }
        /// <summary>
        /// 型号（来自字典）
        /// </summary>
        public string ModelSN { get; set; }
        /// <summary>
        /// 按键个数
        /// </summary>
        public int KeyCount { get; set; }
        /// <summary>
        /// 分页模式（单，双页）
        /// </summary>
        public short PageModel { get; set; }
        /// <summary>
        /// 局域网ip地址
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDt { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUID { get; set; }

    }
}