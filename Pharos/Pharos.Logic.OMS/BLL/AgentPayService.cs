﻿using Pharos.Logic.OMS.DAL;
using Pharos.Logic.OMS.Entity;
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
    public class AgentPayService : BaseService<AgentPay>
    {

        [Ninject.Inject]
        // 代理商档案
        IBaseRepository<AgentPay> agentPayRepository { get; set; }

        //支付接口
        [Ninject.Inject]
        IBaseRepository<PayApi> payApiRepository { get; set; }

        /// <summary>
        /// 增加或者修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OpResult SaveOrUpdate(AgentPay model)
        {
            if (model.Id == 0)
            {
                model.CreateUid = CurrentUser.UID;
                model.CreateTime = DateTime.Now;
                model.UpdateTime = DateTime.Now;
                model.AgentPayId = CommonService.GUID.ToUpper();
                agentPayRepository.Add(model);
            }
            else
            {
                model.UpdateTime = DateTime.Now;
                var source = agentPayRepository.Get(model.Id);
                model.ToCopyProperty(source, new List<string>() { "AgentPayId", "CreateUid", "CreateTime" });
            }
            agentPayRepository.SaveChanges();
            return OpResult.Success();
        }


        public AgentPay GetOne(int AgentsId)
        {
           AgentPay agentPay = agentPayRepository.GetQuery(o=>o.AgentsId==AgentsId).FirstOrDefault();
           if (agentPay != null)
           {
               agentPay.Status = payApiRepository.GetQuery(o => o.ApiNo == agentPay.ApiNo).Select(o => o.State).FirstOrDefault();
           }
           return agentPay==null?new AgentPay():agentPay;
        }
    }
}