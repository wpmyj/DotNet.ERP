﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pharos.Logic.ApiData.Pos.Entity.LocalCeEntity
{
    public class PosIncomePayout : BaseEntity
    {
        public string StoreId { get; set; }
        public string MachineSN { get; set; }
        public string CreateUID { get; set; }
        public short Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateDT { get; set; }
        
        public bool IsTest { get; set; }
    }
}