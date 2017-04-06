﻿using Pharos.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pharos.Utility.Helpers;
using Pharos.Sys.DAL;
using Pharos.Sys.Entity;
using Pharos.Sys.EntityExtend;

namespace Pharos.Sys.BLL
{
    /// <summary>
    /// 权限逻辑层
    /// 管理全局权限code信息
    /// </summary>
    public class SysLimitsBLL
    {
        private SysLimitisDAL _dal = new SysLimitisDAL();
        /// <summary>
        /// 获得权限列表
        /// </summary>
        /// <returns></returns>
        public List<SysLimitsExt> GetList()
        {
            return _dal.GetList(SysCommonRules.CompanyId);
        }
        /// <summary>
        /// 根据ID查找权限实体信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SysLimitsExt GetModel(int id, int pobjid)
        {
            SysLimitsExt obj = _dal.GetExtModel(id);
            if (obj == null)
            {
                obj = new SysLimitsExt() { Status = 1 };
            }
            if (pobjid != 0)
            {
                var pobj = _dal.GetByColumn(pobjid, "LimitId");
                if (pobj != null)
                {
                    obj.PLimitId = pobj.LimitId;
                    obj.PDepth = pobj.Depth;
                    obj.PTitle = pobj.Title;
                }
            }
            return obj;
        }
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            return _dal.Delete(id);
        }
        /// <summary>
        /// 添加或者修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OpResult SaveLimit(SysLimits model)
        {
            var result = OpResult.Fail("数据保存失败!");
            try
            {//todo: Set Depth
                model.CompanyId = Sys.SysCommonRules.CompanyId;
                if (_dal.ExistsTitle(model))
                {
                    result = OpResult.Fail("同一级别下权限名称不能重复！");
                }
                else
                {
                    if (_dal.ExistsById(model.Id))
                    {
                        var re = _dal.Update(model);
                        if (re) { result = OpResult.Success("数据保存成功"); }
                    }
                    else
                    {
                        var maxDepId = _dal.MaxVal("LimitId",SysCommonRules.CompanyId);
                        model.LimitId = maxDepId + 1;
                        var re = _dal.Insert(model);
                        if (re > 0) { result = OpResult.Success("数据保存成功"); }
                    }
                }
            }
            catch (Exception ex)
            {
                result = OpResult.Fail("数据保存失败!" + ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 修改权限状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OpResult ChangeStatus(int id)
        {
            var result = OpResult.Fail("状态变更失败");
            try
            {
                var model = _dal.GetById(id);
                model.Status = 0;
                _dal.UpdateStatus(model);
                result = OpResult.Success("数据保存成功");
            }
            catch (Exception e)
            {
                result = OpResult.Fail("状态变更失败" + e.Message);
            }
            return result;
        }
    }
}