﻿using Pharos.Logic.InstalmentDomain.Interfaces;
using System;

namespace Pharos.Logic.InstalmentDomain.QuanChengTaoInstalment.InstalmentItems
{
    public class QuanChengTaoInstalmentItem : IInstalmentItem
    {
        public string InstalmentRuleId { get; set; }
        public decimal InstalmentNumber { get; set; }
        public string IntegralRecordId { get; set; }

        public DateTime InstalmentDT { get; set; }
    }
}