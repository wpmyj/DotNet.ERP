﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Pharos.Utility.Helpers;
using Pharos.Logic.ApiData.Mobile.Exceptions;
using Pharos.Sys.BLL;
namespace Pharos.Api.Retailing.Controllers
{
    public class GetImgsController : ApiController
    {
        static string _Theme = null;
        static string Theme
        {
            get {
                if (_Theme==null)
                {
                    _Theme=System.Configuration.ConfigurationManager.AppSettings["theme"]??"1"; 
                }
                return _Theme;
            }
        }
        /// <summary>
        /// android640,ios640,ios960
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetImagePath(string type)
        {
            return AppDomain.CurrentDomain.BaseDirectory + "/Images/app/" + type + "/" + Theme;
        }
        /// <summary>
        /// 获取配置图片
        /// </summary>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        public object POST([FromBody]JObject requestParams)
        {
            var source = requestParams.Property("source", true);
            var type = requestParams.Property("type", true);
            var ratio = requestParams.Property("ratio", true);
            var position = requestParams.Property("position", true);
            //var screenWidth = Request.Headers.GetCookies();
            if (source.IsNullOrEmpty()) throw new MessageException("来源不能为空!");
            if (type.IsNullOrEmpty()) throw new MessageException("类型不能为空!");
            if (position.IsNullOrEmpty()) throw new MessageException("位置不能为空!");

            var list = new List<object>();
            var url = Request.GetUrlHelper().Content("~/Images/app/");
           
            var version = Theme;
            if (string.Equals(source, "APP", StringComparison.CurrentCultureIgnoreCase))
            {
                string width = "", height = "";
                if (string.Equals(type, "Android", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (ratio.IsNullOrEmpty()) ratio = "640x960";
                    if (ratio.StartsWith("640x", StringComparison.CurrentCultureIgnoreCase))
                    {
                        type = "android640";
                    }
                }
                if (string.Equals(type, "IOS", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (ratio.IsNullOrEmpty()) ratio = "750x1334";
                    if (ratio.StartsWith("750x", StringComparison.CurrentCultureIgnoreCase))
                    {
                        type = "ios640";
                    }
                    else if (ratio.StartsWith("1080x", StringComparison.CurrentCultureIgnoreCase))
                        type = "ios960";
                }
                var path = GetImagePath(type);
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
                var bllset = new SysWebSettingBLL();
                var set= bllset.GetWebSetting();
                var sip = Pharos.Utility.Config.GetAppSettings("ServerIP");
                var prefix = type + "/" + version+"/";
                var filename = GetImageBySet(set,position,type);
                if (!filename.IsNullOrEmpty() && !sip.IsNullOrEmpty())
                {
                    url = sip;
                    prefix = "SysImg/"+Sys.SysCommonRules.CompanyId+"/";
                }
                if (string.Equals(position, "sysLogo", StringComparison.CurrentCultureIgnoreCase))
                {
                    url += prefix + (filename.IsNullOrEmpty() ? "login_sysLogo.png" : filename) + "?t=" + DateTime.Now.ToLongTimeString();
                    list.Add(new { Url = url, Width = width, Height = height });
                }
                else if (string.Equals(position, "sysLogo2", StringComparison.CurrentCultureIgnoreCase))
                {
                    url += prefix + (filename.IsNullOrEmpty() ?"index_sysLogo.png":filename);
                    list.Add(new { Url = url, Width = width, Height = height });
                }
                else if (string.Equals(position, "sysLoginBj", StringComparison.CurrentCultureIgnoreCase))
                {
                    var files = dir.GetFiles("login_bg_*");
                    Random random = new Random();
                    var idx = random.Next(0, files.Count());
                    url += prefix + (files.Any() ? files[idx].Name : "");
                    list.Add(new { Url = url, Width = width, Height = height });
                }
                else if (string.Equals(position, "indexTop", StringComparison.CurrentCultureIgnoreCase))
                {
                    url += prefix + (filename.IsNullOrEmpty() ?"index_bg.png":filename)+"?t=" + DateTime.Now.ToLongTimeString();
                    list.Add(new { Url = url, Width = width, Height = height });
                }
                else if (string.Equals(position, "consumerLogo", StringComparison.CurrentCultureIgnoreCase))
                {
                    url += prefix +(filename.IsNullOrEmpty() ? "db_logo.png":filename);
                    list.Add(new { Url = url, Width = width, Height = height });
                }
                else if (string.Equals(position, "indexAdv", StringComparison.CurrentCultureIgnoreCase))
                {
                    var files = dir.GetFiles("index_slides_*");
                    foreach (var fs in files)
                        list.Add(new { Url = url + prefix + fs.Name, Width = width, Height = height });
                }
            }
            
            return list;
        }
        string GetImageBySet(Pharos.Sys.Entity.SysWebSetting set,string position, string type)
        {

            if(set!=null)
            {
                if (string.Equals(position, "sysLogo", StringComparison.CurrentCultureIgnoreCase))
                {
                    return type.EndsWith("960")? set.AppIcon960:set.AppIcon640;
                }
                if (string.Equals(position, "sysLogo2", StringComparison.CurrentCultureIgnoreCase))
                {
                    return type.EndsWith("960") ? set.AppIndexIcon960 : set.AppIndexIcon640;
                }
                if (string.Equals(position, "indexTop", StringComparison.CurrentCultureIgnoreCase))
                {
                    return type.EndsWith("960") ? set.AppIndexbg960 : set.AppIndexbg640;
                }
                if (string.Equals(position, "consumerLogo", StringComparison.CurrentCultureIgnoreCase))
                {
                    return type.EndsWith("960") ? set.AppCustomer960 : set.AppCustomer640;
                }
            }
            return null;
        }
    }
}