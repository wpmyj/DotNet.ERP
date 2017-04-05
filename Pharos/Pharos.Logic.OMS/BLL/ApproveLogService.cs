﻿using Pharos.Logic.OMS.DAL;
using Pharos.Logic.OMS.Entity;
using Pharos.Logic.OMS.Entity.View;
using Pharos.Logic.OMS.IDAL;
using Pharos.Utility;
using Pharos.Utility.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace Pharos.Logic.OMS.BLL
{
    /// <summary>
    /// BLL审批日志
    /// </summary>
    public class ApproveLogService : BaseService<ApproveLog>
    {
        [Ninject.Inject]
        // 审核日志
        public IBaseRepository<ApproveLog> approveLogRepository { get; set; }

        [Ninject.Inject]
        public IBaseRepository<SysUser> sysUserInfoRepository { get; set; }

        /// <summary>
        /// 获取审核日志
        /// </summary>
        /// <param name="LicenseId"></param>
        /// <returns></returns>
        public List<ViewApproveLog> getList(string LicenseId)
        {
            short moduleNum = Convert.ToInt16(ApproveLogNum.支付许可);
            var log = approveLogRepository.GetQuery(o => o.ItemId == LicenseId && o.ModuleNum == moduleNum);
            var user = sysUserInfoRepository.GetQuery();

            var q = from l in log
                    join u in user on l.OperatorUID equals u.UserId
                    into uu
                    from uuu in uu.DefaultIfEmpty()
                    orderby l.Id
                    select new ViewApproveLog
                    {
                        CreateTime=l.CreateTime,
                        Description=l.Description,
                        OperatorFullName = uuu == null ? "" : uuu.FullName
                    };
            return q.ToList();
        }
    }
}