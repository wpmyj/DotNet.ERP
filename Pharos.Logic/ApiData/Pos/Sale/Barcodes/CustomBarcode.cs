﻿using Newtonsoft.Json;
using Pharos.Logic.ApiData.Pos.DataAdapter;
using Pharos.Logic.ApiData.Pos.Exceptions;
using Pharos.Logic.ApiData.Pos.Sale;
using Pharos.Logic.ApiData.Pos.ValueObject;
using Pharos.ObjectModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pharos.Logic.ApiData.Pos.Sale.Barcodes
{
    /// <summary>
    /// 自定义条码
    /// </summary>
    public class CustomBarcode : IBarcode
    {
        #region 初始化器
        public CustomBarcode() { }
        public CustomBarcode(string storeId, string machineSn, int companyId, string barcodeString, SaleStatus saleStatus = SaleStatus.Normal, string marketingRuleId = "")
        {
            BarcodeType = Barcodes.BarcodeType.CustomBarcode;

            RecordId = Guid.NewGuid().ToString();
            //处理条码字符串的空格及移除所有前导空白字符和尾部空白字符
            if (string.IsNullOrWhiteSpace(barcodeString))
            {
                throw new BarcodeException("条码不能为空！");
            }
            var dataAdapter = DataAdapterFactory.Factory(MachinesSettings.Mode, storeId, machineSn, companyId, DataAdapterFactory.DEFUALT);
            CurrentString = barcodeString;
            Count = CurrentString.Length;
            if (dataAdapter == null)
            {
                throw new Exception("数据适配器不能为null,请实现IDataAdapter，并实例化！");
            }
            var productInfo = dataAdapter.GetProductInfoByBarcode(CurrentString);
            if (productInfo == null)
            {
                throw new SaleException("602", "未找到商品信息，请确认商品是否已经入库销售！");
            }
            if (productInfo.ProductType == ProductType.Weigh)
            {
                throw new PosException("604", "散称商品请过称！", productInfo);
            }
            MultiCode = productInfo.MultiCode;
            MainBarcode = productInfo.MainBarcode;
            ProductType = productInfo.ProductType;
            SaleNumber = 1;
            ProductCode = productInfo.ProductCode;
            SalePrice = productInfo.SystemPrice;
            Details = new ProductDetails()
            {
                Brand = productInfo.Brand,
                Category = productInfo.Category,
                EnableEditNum = productInfo.EnableEditNum,
                EnableEditPrice = productInfo.EnableEditPrice,
              //  GiftId = giftId,
                MarketingRuleId = marketingRuleId,
                SaleStatus = saleStatus,
                Size = productInfo.Size,
                SystemPrice = productInfo.SystemPrice,
                Title = productInfo.Title,
                Unit = productInfo.Unit,
                Total = SalePrice * SaleNumber,
                BuyPrice = productInfo.BuyPrice
            };
            IsMultiCode = MultiCode.Contains(CurrentString);
        }
        public CustomBarcode(IBarcode barcode)
        {
            Count = barcode.Count;
            CurrentString = barcode.CurrentString;
            ProductType = barcode.ProductType;
            SalePrice = barcode.SalePrice;
            SaleNumber = barcode.SaleNumber;
            ProductCode = barcode.ProductCode;
            Details = barcode.Details;
            IsMultiCode = barcode.IsMultiCode;
            MultiCode = barcode.MultiCode;
            AveragePrice = barcode.AveragePrice;
            MainBarcode = barcode.MainBarcode;
        }
        #endregion 初始化器

        #region 属性
        /// <summary>
        /// 主条码
        /// </summary>
        public string MainBarcode { get; set; }
        public BarcodeType BarcodeType { get; set; }
        public int Count { get; set; }
        public IEnumerable<string> MultiCode { get; set; }
        public bool IsMultiCode { get; set; }
        public bool HasEditPrice { get; set; }

        public decimal MarketingPrice { get; set; }

        public bool IsMarketingSplit { get; set; }

        public Dictionary<decimal, decimal> MarketingSplitRule { get; set; }

        public ProductStatus ProductStatus
        {
            get
            {
                if (MarketingMarks == null)
                {
                    return ValueObject.ProductStatus.Normal;
                }
                var enableIcon = MarketingMarks.Where(o => o.Type != Pharos.ObjectModels.DTOs.MarketingType.Manyuan && o.Type != Pharos.ObjectModels.DTOs.MarketingType.Zuhe && o.Type != Pharos.ObjectModels.DTOs.MarketingType.Maizeng).ToList();
                if (enableIcon.Count == 1)
                {
                    switch (enableIcon.FirstOrDefault().Type)
                    {
                        case Pharos.ObjectModels.DTOs.MarketingType.Danpinzhekou:
                            return ValueObject.ProductStatus.Discount;
                    }
                }
                else if (enableIcon.Count > 1)
                {
                    return ValueObject.ProductStatus.MarkingGroup;
                }
                if (Details.SaleStatus == SaleStatus.POSGift)
                    return ValueObject.ProductStatus.POSGift;
                if (Details.SaleStatus == SaleStatus.ActivityGifts)
                    return ValueObject.ProductStatus.ActivityGifts;
                if (MarketingPrice < Details.SystemPrice)
                {
                    return ValueObject.ProductStatus.Discount;
                }
                return ValueObject.ProductStatus.Normal;
            }
        }

        public string RecordId { get; set; }
        public string CurrentString { get; set; }

        public ProductType ProductType { get; set; }

        public decimal SalePrice { get; set; }
        public decimal AveragePrice { get; set; }

        public decimal SaleNumber { get; set; }

        public string ProductCode { get; set; }

        public bool EnableMarketing { get; set; }
        public ProductDetails Details { get; set; }
        public List<Marketings.ActiveMarketingRule> MarketingMarks { get; set; }
        #endregion 属性

        #region 方法
        public override bool Equals(object obj)
        {
            var barcode = obj as CustomBarcode;
            if (barcode == null || this == null)
            {
                return false;
            }
            if (MainBarcode != barcode.MainBarcode || string.IsNullOrEmpty(CurrentString) || string.IsNullOrEmpty(barcode.CurrentString))
            {
                return false;
            }
            if (barcode.Details == null || Details == null || Details.SaleStatus != barcode.Details.SaleStatus  || Details.MarketingRuleId != barcode.Details.MarketingRuleId)
            {
                return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            StringBuilder str = new StringBuilder(CurrentString);
            if (Details != null)
                str.Append((int)Details.SaleStatus);
            return str.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return CurrentString;
        }

        public bool VerfyEnableCombine(string barcode, SaleStatus status, bool hasEditPrice = false, string recordId = "")
        {
            if (this == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(CurrentString) || string.IsNullOrEmpty(barcode) || Details == null)
            {
                return false;
            }
            if (SameProduct(barcode) && Details.SaleStatus == status && (recordId == RecordId || (string.IsNullOrEmpty(recordId) && !HasEditPrice)))
            {
                if (MultiCode.Contains(barcode))
                {
                    IsMultiCode = true;
                }
                return true;
            }
            return false;
        }

        public bool SameProduct(string barcode)
        {
            return (CurrentString == barcode || MainBarcode == barcode || MultiCode.Contains(barcode));
        }
        public bool VerfyEnableCombine(IBarcode barcode)
        {
            return this.Equals(barcode);
        }

        #endregion 方法


    }
}