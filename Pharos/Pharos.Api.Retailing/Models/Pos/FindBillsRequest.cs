﻿using Pharos.Logic.ApiData.Pos.Sale;
using System;

namespace Pharos.Api.Retailing.Models.Pos
{
    public class FindBillsRequest : BaseApiParams
    {
        public DateTime Date { get; set; }

        public Range Range { get; set; }
        public string PaySn { get; set; }
        public string Cashier { get; set; }       
        public string QueryMachineSn { get; set; }
    }
}