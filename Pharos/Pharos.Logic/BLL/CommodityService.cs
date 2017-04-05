﻿using System.Linq;
using System.Collections.Generic;
using Pharos.Logic.Entity;
using Pharos.Utility.Helpers;
using System.Collections.Specialized;
using Pharos.Utility;
using System;
using System.Data;
using System.Reflection.Emit;
using Pharos.Sys;

namespace Pharos.Logic.BLL
{
    public class CommodityService : BaseService<Commodity>
    {
        static DAL.StoreDAL dal = new DAL.StoreDAL();
        public static IEnumerable<KeyValuePair<string, string>> GetStockout()
        {
            var store = CurrentUser.StoreId;
            var commodity = (from a in CurrentRepository.Entities
                             group a by new { a.Barcode, a.StoreId } into g
                             select new { StoreId = g.Key.StoreId, Barcode = g.Key.Barcode, StockNumber = g.Sum(o => o.StockNumber) }
                         );

            var query = (
                         from a in commodity
                         from b in WarehouseService.CurrentRepository.Entities
                         from c in ProductService.CurrentRepository.Entities
                         where a.StoreId == b.StoreId && c.Barcode == a.Barcode
                         && a.StockNumber <= (c.InventoryWarning ?? 0) && a.StoreId == store
                         select new
                         {
                             Store = b.Title,
                             Product = c.Title
                         }
                       ).ToList();
            return query.Select(o => new KeyValuePair<string, string>(o.Store, o.Product));
        }
        /// <summary>
        /// 库存查询列表
        /// </summary>
        /// <param name="nvl"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public static DataTable QueryInventoryPageList(NameValueCollection nvl, out int recordCount,ref object footer)
        {
            var nl = new NameValueCollection() { nvl };
            DataTable dt = null;
            if (Sys.CurrentUser.IsStore)
            {
                nl["store"] = Sys.CurrentUser.StoreId;
                var store = WarehouseService.Find(o => o.CompanyId == CommonService.CompanyId && o.StoreId == Sys.CurrentUser.StoreId);
                var childs = ProductCategoryService.GetChildSNs(store.CategorySN.Split(',').Select(o => int.Parse(o)).ToList(), true);
                if (!nl["parentType"].IsNullOrEmpty())
                {
                    var childs2 = ProductCategoryService.GetChildSNs(new List<int>() { int.Parse(nl["parentType"]) }, true);
                    childs = childs.Where(o => childs2.Contains(o)).ToList();
                }
                nl["parentType"] = childs.Any() ? string.Join(",", childs) : "";
                //dt = dal.QueryStoreInventorys(nl, out recordCount);
                dt = dal.QueryInventorys(nl);
            }
            else
            {
                if (!nl["store"].IsNullOrEmpty())
                {
                    var storeId = nl["store"];
                    var store = WarehouseService.Find(o =>o.CompanyId==CommonService.CompanyId && o.StoreId == storeId);
                    var childs = ProductCategoryService.GetChildSNs(store.CategorySN.Split(',').Select(o => int.Parse(o)).ToList(), true);
                    if (!nl["parentType"].IsNullOrEmpty())
                    {
                        var childs2 = ProductCategoryService.GetChildSNs(new List<int>() { int.Parse(nl["parentType"]) }, true);
                        childs = childs.Where(o => childs2.Contains(o)).ToList();
                    }
                    nl["parentType"] = childs.Any() ? string.Join(",", childs) : "0";
                }
                else if (!nl["parentType"].IsNullOrEmpty())
                {
                    var childs = ProductCategoryService.GetChildSNs(new List<int>() { int.Parse(nl["parentType"]) }, true);
                    nl["parentType"] = string.Join(",", childs);
                }
                dt = dal.QueryInventorys(nl);
            }
            recordCount = 0;
            if (nl["ispage"] != "0")//分页
            {
                if (dt.Rows.Count > 0)
                {
                    recordCount = Convert.ToInt32(dt.Rows[0]["RecordTotal"]);
                }
            }
            decimal stockNumbers = 0, saleAmounts = 0, stockAmounts = 0;
            if (dt.Rows.Count > 0)
            {
                var dr = dt.Rows[0];
                stockNumbers += Convert.ToDecimal(dr["StockNumbers"]);
                saleAmounts += Convert.ToDecimal(dr["SaleAmounts"]);
                stockAmounts += Convert.ToDecimal(dr["StockAmounts"]);
            }
            footer = new List<object>() { 
                new {StockNumber=stockNumbers,SaleAmount=saleAmounts,StockAmount=stockAmounts,Title="合计:"}
            };
            ProductService.SetSysPrice(nl["store"], dt);
            return dt;
        }
        /// <summary>
        /// 获取各门店库存数
        /// </summary>
        /// <param name="barcode">产品编码</param>
        /// <returns>门店名－库存量</returns>
        public static Dictionary<string, decimal> GetStockNums(string barcode)
        {
            var queryComm = CurrentRepository.QueryEntity;
            var queryStore = WarehouseService.CurrentRepository.QueryEntity;
            var query = from x in queryStore
                        let o = from y in queryComm where x.StoreId == y.StoreId && y.Barcode == barcode select y
                        select new
                        {
                            x.Title,
                            StockNums = o.Any() ? o.Sum(i => i.StockNumber) : 0
                        };
            return query.ToDictionary(o => o.Title, o => o.StockNums);

        }
        /// <summary>
        /// 更新库存
        /// </summary>
        /// <param name="datas">条码和量</param>
        /// <param name="add">0-减少，1-增加</param>
        /// <returns></returns>
        public static OpResult UpdateStock(Dictionary<string, int> datas, int add)
        {
            return null;
        }
        /// <summary>
        /// 获取某一门店中的商品库存数
        /// </summary>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public static Dictionary<string, decimal> GetStockNumsByStoreId(string storeId)
        {
            //var queryComm = CurrentRepository.QueryEntity;
            //var query = from x in queryComm
            //            where x.StoreId == storeId
            //            group x by x.Barcode into g
            //            select new
            //            {
            //                g.Key,
            //                StockNum = g.Sum(o => o.StockNumber)
            //            };
            //return query.ToDictionary(o => o.Key, o => o.StockNum);
            return BaseService<Inventory>.FindList(o => o.StoreId == storeId).ToDictionary(o => o.Barcode, o => o.StockNumber);
        }
        /// <summary>
        /// 出库扣除相应门店的库存量
        /// </summary>
        /// <param name="datas">条码和量(必须已按条码进行分组)</param>
        /// <param name="stroreId">门店Id</param>
        /// <returns></returns>
        public static OpResult OutBoundReduceStock(Dictionary<string, decimal> datas, string stroreId)
        {
            var re = new OpResult();
            try
            {
                List<Commodity> updateCommodityList = new List<Commodity>();
                foreach (var d in datas)
                {
                    if (!string.IsNullOrEmpty(d.Key))
                    {
                        var outNumber = d.Value;
                        int i = 0;
                        var stocks = FindList(o => o.Barcode == d.Key && o.StoreId == stroreId).OrderBy(o => o.ExpirationDate).ToList();
                        //if (stocks.Count == 0)
                        //{
                        //    re.Message = string.Format("条码：{0} 不存在库存记录，无法出库", d.Key);
                        //    return re;
                        //}
                        while (outNumber > 0)
                        {
                            if (i >= stocks.Count)
                            {//库存不足，从最快过期的开始扣（允许负库存）
                                stocks[0].StockNumber = stocks[0].StockNumber - outNumber;
                                outNumber = 0;
                            }
                            else
                            {
                                if (stocks[i].StockNumber >= outNumber)
                                {//如果这条库存数量足够
                                    stocks[i].StockNumber = stocks[i].StockNumber - outNumber;
                                    outNumber = 0;
                                }
                                else
                                {//如果这条库存数量不足
                                    outNumber = outNumber - stocks[i].StockNumber;
                                    stocks[i].StockNumber = 0;
                                }
                            }

                            i++;
                        }
                        updateCommodityList.AddRange(stocks);
                    }

                }
                re = Update(updateCommodityList);
            }
            catch (Exception ex)
            {
                re.Message = ex.Message;

            }
            return re;


        }

        public static IEnumerable<KeyValuePair<string, Commodity>> GetExpiresProduct()
        {
            var storeId = CurrentUser.StoreId;

            var collections = CurrentRepository.Entities.Where(o => o.StoreId == storeId).GroupBy(o => o.Barcode).ToList();
            var result = new List<KeyValuePair<string, Commodity>>();
            var storeDict = WarehouseService.CurrentRepository.Entities.ToDictionary(o => o.StoreId, o => o.Title);
            foreach (var item in collections)
            {
                try
                {
                    var product = ProductService.Find(o => o.Barcode == item.Key);
                    if (product != null)
                    {
                        var date = DateTime.Now.Date.AddDays(product.ValidityWarning ?? 5);
                        foreach (var info in item)
                        {
                            if (!string.IsNullOrEmpty(info.ExpirationDate))
                            {
                                var expirationDate = Convert.ToDateTime(info.ExpirationDate);
                                if (expirationDate <= date)
                                {
                                    result.Add(new KeyValuePair<string, Commodity>(storeDict[info.StoreId] + "有部分" + product.Title, info));
                                }
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                }
            }
            return result;
        }
        public static IEnumerable<dynamic> DetailPageList(NameValueCollection dicts, out int recordCount)
        {
            var categorys = new List<int>();
            var suppliers = new List<string>();
            if (!dicts["parentType"].IsNullOrEmpty())
            {
                var categorysn = dicts["parentType"].Split(',').Select(o => int.Parse(o)).ToList();
                categorys = ProductCategoryService.GetChildSNs(categorysn, true);
            }
            if (!dicts["supplierId"].IsNullOrEmpty())
            {
                suppliers = dicts["supplierId"].Split(',').ToList();
            }
            var start=dicts["StartDate"].ToType<DateTime?>();
            var end=dicts["EndDate"].ToType<DateTime?>();
            var bar = dicts["Barcode"];
            var store = dicts["store"];
            var balanceWhere = DynamicallyLinqHelper.Empty<InventoryBalance>().And(o => o.StoreId == store, store.IsNullOrEmpty()).And(o => o.CompanyId == CommonService.CompanyId)
                .And(o => o.BalanceDate >= start, !start.HasValue).And(o => o.BalanceDate <= end, !end.HasValue);
            var productWhere = DynamicallyLinqHelper.Empty<VwProduct>().And(o => categorys.Contains(o.CategorySN), !categorys.Any()).And(o => suppliers.Contains(o.SupplierId),!suppliers.Any())
                .And(o=>o.CompanyId==CommonService.CompanyId);
            var query1 = from x in BaseService<InventoryBalance>.CurrentRepository.QueryEntity.Where(balanceWhere)
                        from y in BaseService<VwProduct>.CurrentRepository.QueryEntity.Where(productWhere)
                        where (y.Barcode == x.Barcode || ("," + y.Barcodes + ",").Contains("," + x.Barcode + ","))
                        && x.Barcode == bar
                        select new { 
                            x.Id,
                            StoreTitle=WarehouseService.CurrentRepository.QueryEntity.Where(o=>o.StoreId==x.StoreId && o.CompanyId==x.CompanyId).Select(o=>o.Title).FirstOrDefault(),
                            x.Barcode,
                            y.Title,
                            y.CategoryTitle,
                            y.SubUnit,
                            x.BalanceDate,
                            x.StockAmount,
                            x.Number,
                            x.SaleAmount
                        };
            var query2 = from x in BaseService<InventoryBalance>.CurrentRepository.QueryEntity.Where(balanceWhere)
                         join y in BaseService<Bundling>.CurrentRepository.QueryEntity on new { x.Barcode,x.CompanyId } equals new { Barcode=y.NewBarcode,y.CompanyId}
                         where x.Barcode == bar
                         select new
                         {
                             x.Id,
                             StoreTitle = WarehouseService.CurrentRepository.QueryEntity.Where(o => o.StoreId == x.StoreId && o.CompanyId == x.CompanyId).Select(o => o.Title).FirstOrDefault(),
                             x.Barcode,
                             y.Title,
                             CategoryTitle="",
                             SubUnit="捆",
                             x.BalanceDate,
                             x.StockAmount,
                             x.Number,
                             x.SaleAmount
                         };
            var query = query1.Union(query2);
            recordCount= query.Count();
            return query.ToPageList();
        }
    }
}