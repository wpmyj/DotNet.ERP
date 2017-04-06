﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Pharos.Logic.Entity.Views;

namespace Pharos.Logic.BLL
{
    /// <summary>
    /// 积分记录
    /// </summary>
    public class IntegralRecordsBLL
    {
        private IntegralRecordsService _service = new IntegralRecordsService();
        /// <summary>
        /// 积分兑换单
        /// </summary>
        /// <returns></returns>
        public List<IntegralRecordViewModel> GetIntegralRecordPageList(NameValueCollection nvc, out int count)
        {
            return _service.GetIntegralRecordPageList(nvc, out count);
        }

        public object GetIntegralRecordDetailPageList(string id, out int count)
        {
            return _service.GetIntegralRecordDetailPageList(id, out count);
        }
    }
}