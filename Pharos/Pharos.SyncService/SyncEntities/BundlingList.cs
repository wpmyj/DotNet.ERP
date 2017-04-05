﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pharos.SyncService.SyncEntities
{
    [Serializable]
    public class BundlingList : SyncDataObject
    {
        public string CommodityId { get; set; }

        /// <summary>
        /// 商品条码
        /// [长度：30]
        /// [不允许为空]
        /// </summary>
        public string Barcode { get; set; }

        public decimal? SysPrice { get; set; }
        public decimal? BuyPrice { get; set; }

        /// <summary>
        /// 每捆数量
        /// [长度：10]
        /// [不允许为空]
        /// [默认值：((1))]
        /// </summary>
        public decimal Number { get; set; }
    }
}