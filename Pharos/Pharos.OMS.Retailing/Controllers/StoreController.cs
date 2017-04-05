﻿using Pharos.Logic.OMS;
using Pharos.Logic.OMS.Entity;
using Pharos.Logic.OMS.EntityExtend;
using Pharos.Logic.OMS.Entity.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pharos.Utility.Helpers;
using Pharos.Utility;
using Pharos.Logic.OMS.BLL;

namespace Pharos.OMS.Retailing.Controllers
{
    public class StoreController : BaseController
    {
        //结算账户信息BLL
        [Ninject.Inject]
        BankCardInfoService bankCardInfoService { get; set; }

        [Ninject.Inject]
        TradersService tradersService { get; set; }

        [Ninject.Inject]
        TradersStoreService tradersStoreService { get; set; }

        [Ninject.Inject]
        TradersPaySecretKeyService tradersPaySecretKeyService { get; set; }

        //BLL商家登录账号
        [Ninject.Inject]
        TradersUserService tradersUserService { get; set; }

        [SysPermissionValidate]
        public ActionResult Index()
        {
            //指派人
            ViewBag.user = ListToSelect(tradersService.getUserList().Select(o => new SelectListItem() { Value = o.UserId, Text = o.FullName }), emptyTitle: "全部");
            return View();
        }

        [SysPermissionValidate(129)]
        public ActionResult Save(int? id)
        {
            List<TradersStore> listStore = new List<TradersStore>();
            var obj = new TradersStore
            {

            };
            if (id.HasValue)
            {
                obj = tradersStoreService.GetEntityById(id.Value);
                listStore = tradersStoreService.GetListByWhere(o=>o.CID==obj.CID&&o.State>0&&o.Id!=obj.Id);
            }
            //指派人
            ViewBag.user = ListToSelect(tradersService.getUserList().Select(o => new SelectListItem() { Value = o.UserId, Text = o.FullName }), emptyTitle: "请选择");
            ViewBag.listStore = listStore;
            return View(obj.IsNullThrow());
        }

        [HttpPost]
        public ActionResult Save(int Id)
        {
            TradersStore tradersStore = new TradersStore();
            if (Id == 0)
            {
                tradersStore.StoreNum = tradersStoreService.getMaxStoreNum();
                tradersStore.TStoreInfoId = CommonService.GUID.ToUpper();
                tradersStore.CreateUID = CurrentUser.UID;
                tradersStore.CreateDT = DateTime.Now;
            }
            else
            {
                tradersStore = tradersStoreService.GetEntityById(Id);
            }
            tradersStore.ModifyUID = CurrentUser.UID;
            tradersStore.ModifyDT = DateTime.Now;
            string[] s = new string[] { "CID", "MainAccount", "AssignUID", "StoreName", "StoreNum3" };
            TryUpdateModel<TradersStore>(tradersStore,s);
            var op = tradersStoreService.Save(tradersStore, Id, Request.Params);
            return new OpActionResult(op);
        }

        public ActionResult FindPageList()
        {
            var count = 0;
            var list = tradersStoreService.GetPageList(Request.Params, out count);
            return ToDataGrid(list, count);
        }

        public ActionResult QRCode(int mch_id, string store_id,int id=0)
        {
            //if (store_id.Length < 5) {
            //    store_id = store_id.PadLeft(5, '0');
            //}
            TradersStore s = tradersStoreService.GetEntityById(id);
            string tit = "";
            if (s != null)
            {
                tit = s.StoreName;
            }
            ViewBag.mch_id = mch_id;
            ViewBag.store_id = store_id;
            ViewBag.tit = tit;
            return View();
        }

        /// <summary>
        /// 获取CID
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public ActionResult GetCidWhere(string keyword)
        {
            var list = tradersStoreService.GetCIDWhere(Request.Params);
            return ToDataGrid(list, 0);
        }

        /// <summary>
        /// 获取商家登录账号
        /// </summary>
        /// <param name="CID"></param>
        /// <returns></returns>
        public ActionResult GetTradersUser(int CID)
        {
            var list = tradersUserService.GetListByWhere(o=>o.CID==CID&&o.State==2);
            list.Insert(0, new TradersUser() { TUserId="", LoginName="请选择" });
            return new JsonNetResult(list);
        }

        public string getListStore(int CID,int Id)
        {
            List<TradersStore> listStore = tradersStoreService.GetListByWhere(o => o.CID == CID && o.State > 0 && o.Id != Id);
            return listStore.ToJson();
        }

        [HttpPost]
        public ActionResult UpState(string ids, short state)
        {
            return new JsonNetResult(tradersStoreService.UpState(ids, state));
        }

        [HttpPost]
        [SysPermissionValidate(134)]
        public ActionResult Delete(string ids)
        {
            return new JsonNetResult(tradersStoreService.Delete(ids));
        }
    }
}