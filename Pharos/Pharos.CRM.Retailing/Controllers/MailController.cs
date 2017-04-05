﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pharos.Logic.Entity;
using Pharos.Logic.BLL;
using Pharos.Utility.Helpers;
using Pharos.Utility;
namespace Pharos.CRM.Retailing.Controllers
{
    public class MailController : BaseController
    {
        //
        // GET: /Mail/

        public ActionResult Index()
        {
            return View();
        }

        //写信
        public ActionResult Send(string id)
        {
            var obj = new SysMailSender();
            if (!id.IsNullOrEmpty())
            {
                obj = SysMailService.GetObj(id, Request["read"] != null);
                obj = SysMailService.GetObjFormat(obj, Request["reback"]);
            }
            return View(obj);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Send(SysMailSender obj)
        {
            var re = SysMailService.SendMail(obj, Request.Files, Request["fileId"]);
            return Content(re.ToJson());
        }
        //收件箱
        public ActionResult Inbox()
        {
            ViewBag.states = ListToSelect(new List<SelectListItem>() { new SelectListItem() { Text = "未读", Value = "0" }, new SelectListItem() { Text = "已读", Value = "1" } }, emptyTitle: "全部");
            return View();
        }
        [HttpPost]
        public ActionResult InboxList()
        {
            int count = 0;
            var list = SysMailService.InboxList(Request.Params, out count);
            return ToDataGrid(list, count);
        }
        //发件箱
        public ActionResult Outbox()
        {
            ViewBag.states = ListToSelect(new List<SelectListItem>() { new SelectListItem() { Text = "草稿", Value = "0" }, new SelectListItem() { Text = "已发送", Value = "1" } }, emptyTitle: "全部");
            return View();
        }
        [HttpPost]
        public ActionResult OutboxList()
        {
            int count = 0;
            var list = SysMailService.OutboxList(Request.Params, out count);
            return ToDataGrid(list, count);
        }
        public ActionResult Delete(string[] ids, int? type)
        {
            var op = SysMailService.Delete(ids, type.GetValueOrDefault());
            return new JsonNetResult(op);
        }
        [HttpPost]
        public ActionResult DeleteFile(int fileId, string isBack)
        {
            var op = new OpResult();
            if (isBack == "1")//在转发或恢复中删除
                op.Successed = true;
            else
            {
                var file = AttachService.Find(o => o.Id == fileId && o.SourceClassify == 3);
                var re = AttachService.Delete(file);
                if (re.Successed) System.IO.File.Delete(System.IO.Path.Combine(Pharos.Sys.SysConstPool.GetRoot, file.SaveUrl));
            }
            return new JsonNetResult(op);
        }
        public ActionResult SetState(string[] ids)
        {
            var receives = BaseService<SysMailReceive>.FindList(o => ids.Contains(o.Id) && o.ReceiveCode == Sys.CurrentUser.UserName && o.State == 0);
            foreach (var rece in receives)
            {
                rece.State = 1;
                rece.ReadDate = DateTime.Now;
            }
            var op = new OpResult();
            if (receives.Any())
            {
                op = BaseService<SysMailReceive>.Update(receives);
                #region 写入日志
                Sys.LogEngine log = new Sys.LogEngine();
                string msg = "成功修改邮件状态！";
                var module = Pharos.Sys.LogModule.邮件管理;
                if (op.Successed)
                {
                    for (int i = 0; i < receives.Count(); i++)
                    {
                        msg += "<br />Id=" + receives[i].Id + "，<br />状态=已读" + "。";
                        log.WriteUpdate(msg, module);
                        msg = "成功修改邮件状态！";
                    }
                }
                else
                {
                    msg = "修改邮件状态失败！";
                    log.WriteUpdate(msg, module);
                }
                #endregion
            }
            else
                op.Message = "无需要更新的记录";
            return new JsonNetResult(op);
        }
    }
}