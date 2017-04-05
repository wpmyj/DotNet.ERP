﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pharos.Logic.OMS.Entity;
using Pharos.Logic.OMS.Entity.View;

namespace Pharos.Logic.OMS.IDAL
{
    /// <summary>
    /// 商家支付许可档案
    /// </summary>
    public interface IPayLicenseRepository : IBaseRepository<PayLicense>
    {
       List<Traders> getListCID(string keyword);

       List<ViewPayLicense> getPageList(int CurrentPage, int PageSize, string strw, out int Count);

       /// <summary>
       /// 获取一条记录
       /// </summary>
       /// <param name="Id"></param>
       /// <returns></returns>
       PayLicense GetEntityByWhere(int Id);
    }
}