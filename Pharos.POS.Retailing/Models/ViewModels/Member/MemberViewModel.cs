﻿using Pharos.Wpf.ViewModelHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pharos.POS.Retailing.Models.ViewModels.Member
{
    public class MemberViewModel : BaseViewModel
    {
        public AddMember AddMember { get; set; }

        public FindMember FindMember { get; set; }

        public FindCardDetails Details { get; set; }

        public StoredValueCardRecharge Recharge { get; set; }
    }
}