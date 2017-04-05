﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pharos.Logic.ApiData.Pos.Entity.LocalCeEntity
{
    public class MemberIntegral : BaseEntity
    {
        public string PaySN { get; set; }
        public string MemberId { get; set; }
        public decimal ActualPrice { get; set; }
        public int Integral { get; set; }
        public DateTime CreateDT { get; set; }
        
    }
}