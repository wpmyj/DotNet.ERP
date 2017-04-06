﻿using Pharos.Logic;
using Pharos.Logic.BLL;
using Pharos.Logic.Entity;
using Pharos.Utility;
using Pharos.Utility.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Pharos.MSPP.Retailing.Controllers
{
    public class MsppController : BaseController
    {
        //
        // GET: /OrderDelivery/

        public ActionResult Index()
        {
            ViewBag.WelcomeText = "欢迎光临";
            ViewBag.CurUserName = Sys.SupplierUser.UserName;
            ViewBag.SupplierName = Sys.SupplierUser.SupplierName;

            var memu = new Pharos.Sys.Models.MenuModel() { Id = "70", Value = "70", Name = "供应平台", Level = 0 };
            memu.Children = new List<Pharos.Sys.Models.MenuModel>()
            {
                new Pharos.Sys.Models.MenuModel(){Id="71",Value="71",Name="订单配送",Url=Url.Action("OrderDelivery", "Mspp"),Level=1},
                new Pharos.Sys.Models.MenuModel(){Id="72",Value="72",Name="退换管理",Url=Url.Action("ReturnManagement","Mspp"),Level=1},
                //new Pharos.Sys.Models.MenuModel(){Id="73",Value="73",Name="发票单据",Url=Url.Action("Invoice","Mspp"),Level=1},
                new Pharos.Sys.Models.MenuModel(){Id="74",Value="74",Name="商家信息",Url=Url.Action("MerchantInformation","Mspp"),Level=1}
            };
            var list = new List<Pharos.Sys.Models.MenuModel>();
            list.Add(memu);

            ViewBag.Menus = list;

            return View();
        }

        #region 订单配送

        #region 订单列表
        //订单列表
        public ActionResult OrderDelivery()
        {
            ViewBag.states = EnumToSelect(typeof(OrderDistributionState), emptyTitle: "全部");
            return View();
        }

        [HttpPost]
        public ActionResult OrderDeliveryList()
        {
            int count = 0;
            var list = MsppBLL.FindPageList(Request.Params, out count);
            return ToDataGrid(list, count);
        }
        #endregion

        #region 订单详情、配送
        //订单详情
        public ActionResult OrderDetail(int? Id)
        {
            ViewBag.users = new List<SelectListItem>() { new SelectListItem() { Value = Sys.CurrentUser.UID, Text = Sys.CurrentUser.UserName } };
            var obj = new VwOrder();
            if (!Id.IsNullOrEmpty())
            {
                obj = BaseService<VwOrder>.FindById(Id);
                if (!string.IsNullOrEmpty(obj.IndentOrderId))
                {
                    var orderDis = BaseService<OrderDistribution>.FindList(o => o.IndentOrderId == obj.IndentOrderId).OrderByDescending(o=>o.DistributionBatch).FirstOrDefault();
                    if (orderDis != null)
                    {
                        obj.DistributionBatch = orderDis.DistributionBatch;
                        obj.Memo = orderDis.Memo;
                    }
                }
            }
            var details = MsppBLL.LoadDetailList(obj.IndentOrderId);
            ViewData["Updated"] = details.ToJson();
            Session["orderdetails"] = details;
            return View(obj.IsNullThrow());
        }
        //配送
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult OrderDetail(string indentOrderId, string updated, string updated2, string disBatch, string disMemo)
        {
            var op = MsppBLL.Save(indentOrderId, updated, updated2, disBatch, disMemo);
            return Content(op.ToJson());
        }
        //加载明细
        [HttpPost]
        public ActionResult LoadDetailList(string orderId)
        {
            int count = 0;
            //var list = MsppBLL.LoadDetailList(orderId);
            var list = Session["orderdetails"];
            return ToDataGrid(list, count);
        }
        #endregion

        #region 配送记录
        //配送记录
        public ActionResult DeliveryRecord()
        {
            return View();
        }

        //加载明细
        [HttpPost]
        public ActionResult LoadDeliveryRecordList(string orderId, string barcode)
        {
            int count = 0;
            var list = MsppBLL.LoadDeliveryRecordList(orderId, barcode, out count);
            return ToDataGrid(list, count);
        }

        #endregion

        #endregion

        #region 退换管理
        //退换管理列表
        public ActionResult ReturnManagement()
        {
            ViewBag.returntypes = EnumToSelect(typeof(OrderReturnType), emptyTitle: "全部");
            ViewBag.returnstates = EnumToSelect(typeof(OrderReturnState), emptyTitle: "全部");
            return View();
        }

        public ActionResult OrderReturnList()
        {
            int count = 0;
            var list = OrderReturnBLL.OrderReturnList(Request.Params, out count);
            return ToDataGrid(list, count);
        }

        /// <summary>
        /// 修改订单退换状态
        /// </summary>
        /// <param name="Ids">一组Id</param>
        /// <param name="state">状态</param>
        /// <returns>修改后的列表</returns>
        [HttpPost]
        public ActionResult SetState(string Ids, short state)
        {
            var ids = Ids.Split(',').Select(o => int.Parse(o)).ToList();
            var list = OrderReturnBLL.FindList(o => ids.Contains(o.Id));
            var distIds = list.Select(o => o.DistributionId).Distinct().ToList();
            var distribs = OrderDistributionService.FindList(o => distIds.Contains(o.DistributionId));
            if(state==1)
            {
                if (list.Any(o => o.State == 1))
                    return new JsonNetResult(OpResult.Fail("请选择未处理的记录"));
                distribs.Each(o =>{
                    var rtn = list.FirstOrDefault(i => i.DistributionId == o.DistributionId);
                    if (rtn != null)
                    {
                        o.OrderReturnId = rtn.Id;
                        o.DeliveryNum = rtn.ReturnNum;
                        o.DeliveryDT = DateTime.Now;
                        o.State = 2;
                        o.ReceivedDT = null;
                        o.ReceivedNum = null;
                    }
                    o.DistributionId = CommonRules.GUID;
                });
                OrderDistributionService.AddRange(distribs,false);
            }
            list.ForEach(o => { o.State = state; });
            var re = OrderReturnBLL.Update(list);
            return new JsonNetResult(re);
        }

        //退换详情
        public ActionResult ReturnDetail(int Id)
        {
            var obj = OrderReturnBLL.FindByReturnId(Id);
            return View(obj.IsNullThrow());
        }

        #endregion

        #region 商家信息
        //根据ID获取商家信息
        public ActionResult MerchantInformation()
        {
            //获取当前用户ID
            string userID = Pharos.Sys.SupplierUser.SupplierId;
            var obj = SupplierService.Find(o => o.Id == userID);
            if (obj == null) throw new ArgumentException("传入参数不正确!");

            return View(obj);
        }
        //修改商家信息
        [HttpPost]
        public ActionResult MerchantInformation(Logic.Entity.Supplier obj)
        {
            var re = new OpResult();
            
            var supp = SupplierService.FindById(obj.Id);
            var exc = new List<string>();
            if (string.IsNullOrEmpty(obj.MasterPwd))
            {
                exc.Add("MasterPwd");
            }
            exc.Add("CreateDT");
            exc.Add("MasterState");
              
            obj.ToCopyProperty(supp, exc);
            re = SupplierService.Update(supp);
            
            return Content(re.ToJson());
        }
        #endregion

        #region 发票单据
        //单据列表
        public ActionResult Invoice()
        {
            ViewBag.types = ListToSelect(SysDataDictService.GetReceiptsCategories().Select(o => new SelectListItem() { Value = o.DicSN.ToString(), Text = o.Title }), emptyTitle: "全部");
            //ViewBag.users = ListToSelect(UserInfoService.GetList().Select(o => new SelectListItem() { Value = o.Id, Text = o.FullName, Selected = o.Id == Logic.CurrentUser.UID }), emptyTitle: "请选择");
            ViewBag.users = ListToSelect(UserInfoService.GetList().Select(o => new SelectListItem() { Value = o.UID, Text = o.FullName, Selected = o.UID == Sys.CurrentUser.UID }), emptyTitle: "全部");
            ViewBag.states = EnumToSelect(typeof(ReceipState), emptyTitle: "全部");
            return View();
        }

        [HttpPost]
        public ActionResult FindPageList(int page = 1, int rows = 30)
        {
            int count = 0;
            var list = InvoiceBLL.FindPageList(Request.Params, out count);
            return ToDataGrid(list, count);
        }

        //新增单据/修改单据（单据状态为待审核状态）
        public ActionResult InvoiceSave(string id)
        {
            ViewBag.types = ListToSelect(SysDataDictService.GetReceiptsCategories().Select(o => new SelectListItem() { Value = o.DicSN.ToString(), Text = o.Title }), emptyTitle: "请选择");
            var supplierForAdd = SupplierService.Find(o => o.Id == Pharos.Sys.SupplierUser.SupplierId);
            var obj = new Receipts() { CreateTitle = supplierForAdd.Title, CreateDT = DateTime.Now };//CreateTitle = CurrentUser.FullName
            if (!id.IsNullOrEmpty())
            {
                obj = InvoiceBLL.FindById(id);
                obj.IsNullThrow();
                var supplier = SupplierService.Find(o => o.Id == obj.CreateUID);
                if (supplier != null)
                    obj.CreateTitle = supplier.Title;
            }
            return View(obj);
        }
        [HttpPost]
        public ActionResult InvoiceSave(Receipts obj)
        {
            var re = InvoiceBLL.SaveOrUpdate(obj, Request.Files);
            return Content(re.ToJson());
        }

        //查看单据（单据状态为已审核状态）
        public ActionResult InvoiceDetail(string id)
        {
            ViewBag.types = ListToSelect(SysDataDictService.GetReceiptsCategories().Select(o => new SelectListItem() { Value = o.DicSN.ToString(), Text = o.Title }), emptyTitle: "全部");
            var obj = InvoiceBLL.FindById(id);
            obj.IsNullThrow();


            var user = UserInfoService.Find(o => o.UID == obj.CreateUID);
            if (user != null)
                obj.CreateTitle = user.FullName;
            var type = SysDataDictService.Find(o => o.DicSN == obj.CategoryId);
            if (type != null) ViewData["Category"] = type.Title;
            return View(obj);
        }

        //提交单据
        //public ActionResult AddInvoice(string id)
        //{
        //    ViewBag.types = ListToSelect(SysDataDictService.GetReceiptsCategories().Select(o => new SelectListItem() { Value = o.DicSN.ToString(), Text = o.Title }), emptyTitle: "请选择");
        //    var obj = new Receipts() { CreateTitle = CurrentUser.UserName, CreateDT = DateTime.Now };
        //    if (!id.IsNullOrEmpty())
        //        obj = ReceiptsBLL.FindById(id);
        //    return View(obj.IsNullThrow());
        //}
        //[HttpPost]
        //public ActionResult AddInvoice(Receipts obj)
        //{
        //    var re = ReceiptsBLL.SaveOrUpdate(obj, Request.Files);
        //    return Content(re.ToJson());
        //}

        //删除单据
        [HttpPost]
        public ActionResult Delete(string[] ids)
        {
            var re = InvoiceBLL.Delete(ids);
            return new JsonNetResult(re);
        }

        //删除附件
        [HttpPost]
        public ActionResult DeleteFile(string id, int fileId)
        {
            var files = AttachService.Find(o => o.Id == fileId && o.SourceClassify == 2);
            var re = AttachService.Delete(files);
            return new JsonNetResult(re);
        }

        #endregion

        #region 登陆
        public ActionResult Login()
        {
            var user = new UserLogin();

            if (Cookies.IsExist("remuc"))
            {
                user.UserName = Cookies.Get("remuc", "_uname");
                user.UserPwd = Cookies.Get("remuc", "_pwd");
                user.RememberMe = true;
            }

            return View(user);
        }

        [HttpPost]
        public ActionResult Login(UserLogin user)
        {
            if (!ModelState.IsValid) return View(user);
            var obj = SupplierService.Find(o => o.MasterAccount == user.UserName && o.MasterPwd == user.UserPwd && o.MasterState == 1 && o.BusinessType == 1 && o.CompanyId==CommonService.CompanyId);
            if (obj == null)
            {
                ViewBag.msg = "帐户或密码输入不正确!";
                return View(user);
            }
            var auth = new Sys.SysAuthorize().GetSerialNO;
            if (auth != null && auth.SupplierProper == "N")
            {
                ViewBag.msg = "非供应商专属后台不允许登陆!";
                return View(user);
            }
            //var userInfo = new Pharos.Sys.Entity.SysUserInfo()
            //{
            //    UID = obj.Id,
            //    LoginName = obj.MasterAccount,
            //    FullName = obj.Jianpin,
            //    LoginPwd = obj.MasterPwd,
            //    StoreId = "sup"
            //};

            new Sys.SupplierUser().Login(obj.Id, obj.Title, obj.MasterAccount, obj.MasterPwd, user.RememberMe);

            return RedirectToAction("Index");
        }
        public ActionResult Logout()
        {
            Sys.SupplierUser.Exit();
            return RedirectToAction("Login", "Mspp");
        }
        #endregion
    }
}