﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Pharos.Logic.OMS.Models
{
    public class ProductMenuModel
    {
        [JsonProperty("children")]
        public virtual List<ProductMenuModel> Childrens { get; set; }
        public int ParentId { get; set; }
        public int Id { get; set; }
        /// <summary>
        /// 树形涨开或收缩(open|closed)
        /// </summary>
        [JsonProperty("state")]
        public string TreeState { get; set; }
        public int Index { get; set; }
        public bool HasLimit { get; set; }

        public int MenuId { get; set; }
        public int PMenuId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool Status { get; set; }
        public bool Expand { get; set; }
    }
    public class ProductRoleMenuModel
    {
        [JsonProperty("children")]
        public virtual List<ProductRoleMenuModel> Childrens { get; set; }
        public int ParentId { get; set; }
        public int Id { get; set; }
        public int Index { get; set; }
        /// <summary>
        /// 树形涨开或收缩(open|closed)
        /// </summary>
        [JsonProperty("state")]
        public string TreeState { get; set; }
        public int MenuId { get; set; }
        public int PMenuId { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// 所有
        /// </summary>
        public string Limitids { get; set; }
        /// <summary>
        /// 选中
        /// </summary>
        public string MenuIdSelects { get; set; }
        /// <summary>
        /// 选中
        /// </summary>
        public string LimitSelects { get; set; }
    }
    [NotMapped]
    public class DictionaryModel:Entity.ProductDictionaryData
    {
        [JsonProperty("children")]
        public virtual List<DictionaryModel> Childrens { get; set; }
        public int ParentId { get; set; }
        public int id { get { return Id; } }
        public int Index { get; set; }
        public int Count { get; set; }
        /// <summary>
        /// 树形涨开或收缩(open|closed)
        /// </summary>
        [JsonProperty("state")]
        public string TreeState { get; set; }
        public string ChildTitle { get; set; }
    }
    [NotMapped]
    public class ProductDataSqlModel : Entity.ProductDataSql
    {
        public int Index { get; set; }
        public int Count { get; set; }
        public string Title { get; set; }
        public string SqlMore { get; set; }
    }
}