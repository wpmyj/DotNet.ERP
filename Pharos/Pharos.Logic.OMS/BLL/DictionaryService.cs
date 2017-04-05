﻿using Pharos.Logic.OMS.Entity;
using Pharos.Logic.OMS.IDAL;
using Pharos.Utility;
using Pharos.Utility.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pharos.Logic.OMS.BLL
{
    public class DictionaryService
    {
        [Ninject.Inject]
        IDictRepository DictionaryRepository { get; set; }
        public OpResult SaveOrUpdate(SysDataDictionary model)
        {
            var obj= DictionaryRepository.GetQuery(o =>o.DicPSN==model.DicPSN && o.Title == model.Title && o.Id != model.Id).FirstOrDefault();
            if (obj != null)
            {
                model.DicSN = obj.DicSN;
                return OpResult.Fail("已存在该名称!");
            }
            if (model.Id == 0)
            {
                model.DicSN = MaxSN() + 1;
                model.Status = true;
                model.CreateDT = DateTime.Now;
                model.CreateUID = CurrentUser.UID;
                DictionaryRepository.Add(model);
            }
            else
            {
                var source = DictionaryRepository.Get(model.Id);
                model.ToCopyProperty(source, new List<string>() { "CreateDT", "CreateUID","" });
                DictionaryRepository.SaveChanges();
            }
            return OpResult.Success("数据保存成功");
        }
        public List<SysDataDictionary> GetSubUnitCategories()
        {
            return DictionaryRepository.GetQuery(o => o.DicPSN == 4 && o.Status).OrderBy(o => o.SortOrder).ToList();
        }
        public int MaxSN()
        {
            return DictionaryRepository.GetQuery().Max(o => (int?)o.DicSN).GetValueOrDefault();
        }

        public List<SysDataDictionaryExt> GetPageList(int pageIndex, int pageSize, string key)
        {
            return DictionaryRepository.GetPageList(pageIndex, pageSize, key);
        }
        public List<SysDataDictionary> GetChildList(int psn,bool all=true)
        {
            var query = DictionaryRepository.GetQuery(o => o.DicPSN == psn);
            if (!all) query = query.Where(o => o.Status);
            return query.OrderBy(o => o.SortOrder).ToList();
        }
        public List<SysDataDictionary> GetChildList(IEnumerable<int> psns, bool all = true)
        {
            var query = DictionaryRepository.GetQuery(o =>psns.Contains( o.DicPSN));
            if (!all) query = query.Where(o => o.Status);
            return query.OrderBy(o => o.SortOrder).ToList();
        }
        public SysDataDictionaryExt GetExtModel(int id,int psn)
        {
            var query = DictionaryRepository.GetQuery();
            var q = from x in query
                    where x.Id==id
                    select new
                    { 
                        x.Id,
                        x.DicSN,
                        x.DicPSN,
                        x.Title,
                        x.Depth,
                        x.Status,
                        x.CreateDT,
                        x.CreateUID,
                        PTitle = query.Where(o => o.DicSN == x.DicPSN).Select(o => o.Title).FirstOrDefault()
                    };
            var source = q.FirstOrDefault();
            var obj = new SysDataDictionaryExt();
            if (source == null)
            {
                var pobj = query.FirstOrDefault(o => o.DicSN == psn);
                obj.DicPSN = psn;
                obj.PTitle = pobj.Title;
            }
            else
            {
                source.ToCopyProperty(obj);
            }
            return obj;
        }
        public OpResult ChangeStatus(int sn)
        {
            var dict= DictionaryRepository.Find(o => o.DicSN == sn);
            dict.Status = !dict.Status;
            DictionaryRepository.SaveChanges();
            return OpResult.Success("数据保存成功");
        }
        public OpResult MoveItem(int mode,int sn)
        {
            var obj = DictionaryRepository.Find(o => o.DicSN == sn);
            var list = DictionaryRepository.GetQuery(o => o.DicPSN == obj.DicPSN).OrderBy(o => o.SortOrder).ToList();
            switch (mode)
            {
                case 2://下移
                    var obj1 = list.LastOrDefault();
                    if (obj.Id != obj1.Id)
                    {
                        SysDataDictionary next = null;
                        for (var i = 0; i < list.Count; i++)
                        {
                            if (obj.Id == list[i].Id)
                            {
                                next = list[i + 1]; break;
                            }
                        }
                        if (next != null)
                        {
                            var sort = obj.SortOrder;
                            obj.SortOrder = next.SortOrder;
                            next.SortOrder = sort;
                            DictionaryRepository.SaveChanges();
                        }
                    }
                    break;
                default:
                    var obj2 = list.FirstOrDefault();
                    if (obj.Id != obj2.Id)
                    {
                        SysDataDictionary prev = null;
                        for (var i = 0; i < list.Count; i++)
                        {
                            if (obj.Id == list[i].Id)
                            {
                                prev = list[i - 1]; break;
                            }
                        }
                        if (prev != null)
                        {
                            var sort = obj.SortOrder;
                            obj.SortOrder = prev.SortOrder;
                            prev.SortOrder = sort;
                            DictionaryRepository.SaveChanges();
                        }
                    }
                    break;
            }
            return OpResult.Success("顺序移动成功");
        }
    }
}