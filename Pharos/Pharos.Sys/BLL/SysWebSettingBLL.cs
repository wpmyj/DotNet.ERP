﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pharos.Sys.DAL;
using Pharos.Sys.Entity;
using Pharos.Utility;
using System.Data;

namespace Pharos.Sys.BLL
{
    public class SysWebSettingBLL
    {
        private SysWebSettingDAL _dal = new SysWebSettingDAL();

        /// <summary>
        /// 获取基本配置信息列表
        /// </summary>
        /// <returns></returns>
        public SysWebSetting GetWebSetting()
        {
            return _dal.GetWebSetting();
        }


        /// <summary>
        /// 更新基本配置信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OpResult SaveWebSetting(SysWebSetting model)
        {
            var result = OpResult.Fail("数据保存失败!");
            try
            {
                model.CompanyId = Sys.SysCommonRules.CompanyId;
                if(_dal.SaveOrUpdate(model))
                    result = OpResult.Success("数据保存成功");
            }
            catch (Exception ex)
            {
                result = OpResult.Fail("数据保存失败!" + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取logo
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        public string getLogo(int CompanyId)
        {
            SysWebSetting s = _dal.GetWebSetting(CompanyId);
            if (s != null)
            {
                if (string.IsNullOrEmpty(s.LoginLogo))
                {
                    return "";
                }
                return "/SysImg/" +CompanyId+"/"+ s.LoginLogo;
            }
            else
            {
                return "";
            }
        }

    }
}