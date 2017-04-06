﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Data;
using System.Text;
using System.Data.Entity;
using Pharos.Logic.Entity;
using Pharos.Utility;
using Pharos.Utility.Helpers;
using Pharos.Logic.DAL;
using System.Web;
namespace Pharos.Logic.BLL
{
    public class MemberIntegralSetService : BaseService<MemberIntegralSet>
    {
        public MemberIntegralSetService()
        {

        }
        public static OpResult SaveOrUpdate(MemberIntegralSet obj)
        {
            if (!obj.EndDate.HasValue)
                obj.EndDate = DateTime.Now.AddYears(20);
            var Inserted = HttpContext.Current.Request["Inserted"];
            var InsertedType = HttpContext.Current.Request["Inserted2"];
            var Deleted = HttpContext.Current.Request["Deleted"];
            var DeletedType = HttpContext.Current.Request["Deleted2"];
            var Updated = HttpContext.Current.Request["Updated"];
            var UpdatedType = HttpContext.Current.Request["Updated2"];
            obj.CustomerObj = HttpContext.Current.Request["CustomerObj"];
            var insertList = new List<MemberIntegralSetList>();
            var deleteList = new List<MemberIntegralSetList>();
            var updateList = new List<MemberIntegralSetList>();
            if (!Inserted.IsNullOrEmpty())
            {
                var list = Inserted.ToObject<List<MemberIntegralSetList>>();
                list.Each(o => { o.SetType = 1; });
                insertList.AddRange(list);
            }
            if (!InsertedType.IsNullOrEmpty())
            {
                var list = InsertedType.ToObject<List<MemberIntegralSetList>>();
                list.Each(o => { o.SetType = 2; });
                insertList.AddRange(list);
            }
            if (!Updated.IsNullOrEmpty())
            {
                updateList.AddRange(Updated.ToObject<List<MemberIntegralSetList>>());
            }
            if (!UpdatedType.IsNullOrEmpty())
            {
                updateList.AddRange(UpdatedType.ToObject<List<MemberIntegralSetList>>());
            }
            if (!Deleted.IsNullOrEmpty())
            {
                deleteList.AddRange(Deleted.ToObject<List<MemberIntegralSetList>>());
            }
            if (!DeletedType.IsNullOrEmpty())
            {
                deleteList.AddRange(DeletedType.ToObject<List<MemberIntegralSetList>>());
            }
            obj.OperatorUID = Sys.CurrentUser.UID;
            obj.OperatorTime = DateTime.Now;
            obj.CompanyId = CommonService.CompanyId;
            var op = new OpResult();
            if (obj.Id == 0)
            {
                obj.ProductList = insertList;
                #region 操作日志
                var msg = Sys.LogEngine.CompareModelToLog<MemberIntegralSet>(Sys.LogModule.消费积分, obj);
                new Sys.LogEngine().WriteInsert(msg, Sys.LogModule.消费积分);
                #endregion
                op= Add(obj);
            }
            else
            {
                var res = CurrentRepository.QueryEntity.Include(o => o.ProductList).FirstOrDefault(o => o.Id == obj.Id);
                obj.ToCopyProperty(res);
                updateList.Each(o =>
                {
                    var pro = res.ProductList.FirstOrDefault(i => i.Id == o.Id);
                    if (pro != null)
                        o.ToCopyProperty(pro);
                });
                deleteList.Each(o =>
                {
                    var pro = res.ProductList.FirstOrDefault(i => i.Id == o.Id);
                    if (pro != null)
                        BaseService<MemberIntegralSetList>.CurrentRepository.Remove(pro, false);
                });
                res.ProductList.AddRange(insertList);
                op= Update(res);
            }
            if (op.Successed)
            {
                var stores = string.Join(",", WarehouseService.GetList().Select(o => o.StoreId));
                Pharos.Infrastructure.Data.Redis.RedisManager.Publish("SyncDatabase", new Pharos.ObjectModels.DTOs.DatabaseChanged() { CompanyId = Sys.SysCommonRules.CompanyId, StoreId = stores, Target = "MemberIntegralSetPackage" });
            }
            return op;
        }
        public static object FindPageList(NameValueCollection nvl, out int recordCount)
        {
            /*var query = from x in CurrentRepository.QueryEntity
                        join y in BaseService<MemberIntegralSetList>.CurrentRepository.QueryEntity on x.Id equals y.IntegralId
                        //let pro=from p in ProductService.CurrentRepository.QueryEntity where p.Barcode==y.BarcodeOrCategorySN && y.SetType==1 select p.Title
                        //let cate = from p in ProductCategoryService.CurrentRepository.QueryEntity where p.CategorySN+""==y.BarcodeOrCategorySN && y.SetType ==2 select p.Title
                        where x.Nature==2
                        select new { 
                            x.Id,
                            x.StartDate,
                            x.EndDate,
                            x.CustomerObj,
                            x.OperatorTime,
                            x.Promotion,
                            x.Scale,
                            y.IntegralId,
                            y.BarcodeOrCategorySN,
                            y.SaleMoney,
                            y.SetType,
                            y.IntegralCount
                        };
            var text= nvl["searchText"];
            if (!text.IsNullOrEmpty())
                query = query.Where(o =>o.BarcodeOrCategorySN!=null && o.BarcodeOrCategorySN.Contains(text));
            recordCount = query.Count();
            var list = query.ToPageList();
            var bars= list.Where(o => o.SetType == 1).Select(o => o.BarcodeOrCategorySN).ToList();
            var catesns = list.Where(o => o.SetType == 2).Select(o =>int.Parse(o.BarcodeOrCategorySN)).ToList();
            List<ProductCategory> categorys = null;
            List<ProductRecord> products = null;
            if (catesns.Any())
                categorys=ProductCategoryService.FindList(o => catesns.Contains(o.CategorySN));
            if (bars.Any())
                products = ProductService.FindList(o => bars.Contains(o.Barcode));
            return list.Select(o => new {
                o.Id,
                o.IntegralId,
                o.StartDate,
                o.EndDate,
                o.OperatorTime,
                o.Promotion,
                o.Scale,
                o.BarcodeOrCategorySN,
                o.SaleMoney,
                o.SetType,
                o.IntegralCount,
                DateSpacing = DateDesc(o.StartDate,o.EndDate),
                Title=TitleDesc(o.BarcodeOrCategorySN,o.SetType,products,categorys),
                CustomerObj=CustomerDesc(o.CustomerObj)
            });*/
            var dal = new MemberIntegralDAL();
            var dt = dal.GetPageList(nvl["searchText"], CommonService.CompanyId, out recordCount);
            return dt.AsEnumerable().Select(o => new
            {
                Id = o["Id"],
                IntegralId = o["IntegralId"],
                StartDate = o["StartDate"],
                EndDate = o["EndDate"],
                OperatorTime = o["OperatorTime"],
                Promotion = o["Promotion"],
                Scale = o["Scale"],
                BarcodeOrCategorySN = o["BarcodeOrCategorySN"],
                SaleMoney = o["SaleMoney"],
                SetType = o["SetType"],
                IntegralCount = o["IntegralCount"],
                DateSpacing = DateDesc(o["StartDate"] is DBNull ? new Nullable<DateTime>() : (DateTime)o["StartDate"], o["EndDate"] is DBNull ? new Nullable<DateTime>() : (DateTime)o["EndDate"]),
                Title = o["Title"],
                CustomerObj = CustomerDesc(o["CustomerObj"].ToString())
            });

        }
        public static MemberIntegralSet FindOne(int id)
        {
            return CurrentRepository.QueryEntity.Include(o => o.ProductList).FirstOrDefault(o => o.Id == id);
        }
        public static OpResult Delete(int[] ids)
        {
            var list = CurrentRepository.QueryEntity.Include(o => o.ProductList).Where(o => ids.Contains(o.Id)).ToList();
            #region 操作日志
            foreach (var item in list)
            {
                var msg = Sys.LogEngine.CompareModelToLog<MemberIntegralSet>(Sys.LogModule.消费积分, null, item);
                new Sys.LogEngine().WriteDelete(msg, Sys.LogModule.消费积分);
            }
            #endregion
            var op= Delete(list);
            if (op.Successed)
            {
                var stores = string.Join(",", WarehouseService.GetList().Select(o => o.StoreId));
                Pharos.Infrastructure.Data.Redis.RedisManager.Publish("SyncDatabase", new Pharos.ObjectModels.DTOs.DatabaseChanged() { CompanyId = Sys.SysCommonRules.CompanyId, StoreId = stores, Target = "MemberIntegralSetPackage" });
            }
            return op;
        }
        public static object LoadProductList(int? id)
        {
            if (id == 0) return null;
            var list = BaseService<MemberIntegralSetList>.FindList(o => o.IntegralId == id && o.SetType == 1);
            var bars = list.Where(o => o.SetType == 1).Select(o => o.BarcodeOrCategorySN).ToList();
            List<ProductRecord> products = null;
            if (bars.Any())
                products = ProductService.FindList(o => bars.Contains(o.Barcode) && o.CompanyId == CommonService.CompanyId);
            return list.Select(o => new
            {
                o.Id,
                o.IntegralId,
                o.BarcodeOrCategorySN,
                o.SaleMoney,
                o.SetType,
                o.IntegralCount,
                Title = TitleDesc(o.BarcodeOrCategorySN, o.SetType, products, null)
            });
        }
        public static object LoadTypeList(int? id)
        {
            if (id == 0) return null;
            var dal = new MemberIntegralSetDAL();
            return dal.LoadTypeList(id.Value);
        }
        static string DateDesc(DateTime? start, DateTime? end)
        {
            return start.GetValueOrDefault().ToString("yyyy-MM-dd") + "至" + end.GetValueOrDefault().ToString("yyyy-MM-dd");
        }
        static string TitleDesc(string barOrCate, short type, List<ProductRecord> products, List<ProductCategory> categorys)
        {
            if (type == 1 && products != null)
            {
                var pro = products.FirstOrDefault(o => o.Barcode == barOrCate);
                return pro != null ? pro.Title : "";
            }
            if (type == 2 && categorys != null)
            {
                var pro = categorys.FirstOrDefault(o => o.CategorySN == int.Parse(barOrCate));
                return pro != null ? pro.Title : "";
            }
            return "";
        }
        static string CustomerDesc(string customers)
        {
            if (customers.IsNullOrEmpty()) return "";
            string title = "";
            foreach (var c in customers.Split(','))
            {
                if (c.IsNullOrEmpty()) continue;
                title += "," + Enum.GetName(typeof(CustomerType), byte.Parse(c));
            }
            return title.Substring(1);
        }
    }
}