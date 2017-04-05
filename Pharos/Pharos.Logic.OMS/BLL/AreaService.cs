﻿using Pharos.Logic.OMS.Entity;
using Pharos.Logic.OMS.IDAL;
using Pharos.Utility;
using Pharos.Utility.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pharos.Logic.OMS.BLL
{
    public class AreaService
    {
        [Ninject.Inject]
        public IBaseRepository<Area> AreaRepository { get; set; }
        [Ninject.Inject]
        public IBaseRepository<Traders> TradersRepository { get; set; }
        public Pharos.Utility.OpResult SaveOrUpdate(Area model)
        {
            if (model.AreaPID == 0) model.AreaPID = 1;
            if (AreaRepository.GetQuery(o => o.AreaPID == model.AreaPID && o.Title == model.Title).Any())
                return OpResult.Fail("该地区已存在！");
            else if(model.AreaID==0)
            {
                model.Type = GetType;
                model.OrderNum= AreaRepository.GetQuery(o => o.AreaPID == model.AreaPID).Max(o => (int?)o.OrderNum).GetValueOrDefault() + 1;
                AreaRepository.Add(model);
            }
            else
            {
                var source = AreaRepository.Get(model.AreaID);
                model.ToCopyProperty(source);
                source.Type = GetType;
                AreaRepository.SaveChanges();
            }
            return OpResult.Success();
        }
        List<Area> GetWhereList(int pid,List<Area> alls)
        {
            if (pid == 0) return alls;
            var list=new List<Area>();
            SetParent(alls, pid, ref list);
            list.AddRange(alls.Where(o => o.AreaPID == pid));
            return list;
        }
        public IEnumerable<dynamic> GetDynamicPageList(System.Collections.Specialized.NameValueCollection nvl)
        {
            var id = nvl["pid"].ToType<int>();
            if (!nvl["id"].IsNullOrEmpty())
                id = nvl["id"].ToType<int>();
            if (id == 0) id = 1;
            var query = AreaRepository.GetQuery(o => o.AreaPID == id);
            var queryCount = AreaRepository.GetQuery();
            var qd = TradersRepository.GetQuery();
            var qys = from x in query
                      select new
                      {
                          Id=x.AreaID,
                          x.Title,
                          x.AreaPID,
                          x.AreaSN,
                          x.JianPin,
                          x.OrderNum,
                          x.PostCode,
                          x.QuanPin,
                          x.Type,
                          count = qd.Count(o => o.CurrentProvinceId == x.AreaID || o.CurrentCityId == x.AreaID || o.CurrentCounty == x.AreaID),
                          ChildCount = queryCount.Count(o=>o.AreaPID==x.AreaID)
                      };
            var recordCount = qys.Count();
            var list= qys.ToPageList();
            return list.Select(x => new {
                x.Id,
                x.Title,
                x.AreaPID,
                x.AreaSN,
                x.JianPin,
                x.OrderNum,
                x.PostCode,
                x.QuanPin,
                x.Type,
                total=recordCount,
                state =x.ChildCount>0? "closed":"",//图标
                parentId=x.AreaPID
            });
        }
        public IEnumerable<dynamic> GetPageList(System.Collections.Specialized.NameValueCollection nvl, out int recordCount)
        {
            var qd = TradersRepository.GetQuery();
            var query = AreaRepository.GetQuery();
            var qys = from x in query
                      select new
                      {
                          x.AreaID,
                          x.Title,
                          x.AreaPID,
                          x.AreaSN,
                          x.JianPin,
                          x.OrderNum,
                          x.PostCode,
                          x.QuanPin,
                          x.Type,
                          count = qd.Count(o => o.CurrentProvinceId == x.AreaID || o.CurrentCityId == x.AreaID || o.CurrentCounty == x.AreaID)
                      };

            var alls = qys.OrderBy(o => o.OrderNum).AsEnumerable().Select(x => new Area()
            {
                AreaID = x.AreaID,
                Title = x.Title,
                AreaPID = x.AreaPID,
                AreaSN = x.AreaSN,
                JianPin = x.JianPin,
                OrderNum = x.OrderNum,
                PostCode = x.PostCode,
                QuanPin = x.QuanPin,
                Type = x.Type,
                TraderNum = x.count
            }).ToList();
            var pid = nvl["AreaPID"].IsNullOrEmpty() ? 0 : int.Parse(nvl["AreaPID"]);
            var searchText = nvl["searchText"];
            alls= GetWhereList(pid, alls);
            var rs = new List<Area>();
            foreach (var item in alls.Where(o => o.AreaPID == 0))
            {
                SetChild(alls, item,item,pid);
                rs.Add(item);
            }
            recordCount = rs.Count();
            return rs.AsQueryable().ToPageList();
        }
        void SetChild(List<Area> alls, Area area,Area parea,int pid)
        {
            var childs = alls.Where(o => o.AreaPID == area.AreaID);
            area.PTitle = parea.Title;
            if (childs.Any())
            {
                if (area.Childrens == null) 
                    area.Childrens = new List<Area>();
                area.Childrens.AddRange(childs);
                parea = area;
            }
            foreach (var item in childs)
                SetChild(alls, item,parea,pid);
        }
        void SetParent(List<Area> alls,int pid, ref List<Area> list)
        {
            var obj = alls.FirstOrDefault(o => o.AreaID == pid);
            if (obj == null) return;
            list.Add(obj);
            SetParent(alls, obj.AreaPID, ref list);
        }
        public OpResult Deletes(int[] ids)
        {
            var list = AreaRepository.GetQuery(o => ids.Contains(o.AreaID)).ToList();
            AreaRepository.RemoveRange(list);
            return Utility.OpResult.Success();
        }

        public Area GetOne(object id)
        {
            var obj= AreaRepository.Get(id);
            if (obj.AreaPID > 0)
            {
                obj.PTitle = LoopArea(obj.AreaPID).TrimStart('/');
            }
            return obj;
        }
        string LoopArea(int sn)
        {
            var obj = AreaRepository.Find(o => o.AreaID == sn);
            if (obj != null)
                return LoopArea(obj.AreaPID) + "/" + obj.Title;
            return "";
        }
        public List<Area> GetProvinces()
        {
            return AreaRepository.GetQuery(o => o.Type == 2).OrderBy(o => o.OrderNum).ToList();
        }
        public List<Area> GetCitys(int pid)
        {
            return AreaRepository.GetQuery(o => o.AreaPID == pid).OrderBy(o => o.OrderNum).ToList();
        }
        public List<Area> GetList()
        {
            var list= DataCache.Get<List<Area>>("areas");
            if (list == null)
            {
                list= AreaRepository.GetQuery().OrderBy(o => o.OrderNum).ToList();
                DataCache.Set("areas", list,20);
            }
            return list;
        }
        new byte GetType
        {
            get
            {
                var val = System.Web.HttpContext.Current.Request.Form["AreaPID_0"];
                if (val.IsNullOrEmpty()) val = System.Web.HttpContext.Current.Request["AreaPID_0"];
                if (val.IsNullOrEmpty()) return 2;
                var vals = val.Split(',');
                byte grade = 0;
                for (int i = vals.Length; i > 0; i--)
                {
                    if (!vals[i - 1].IsNullOrEmpty())
                    {
                        grade = (byte)(i + 1); break;
                    }
                }
                return ++grade;
            }
        }

    }
}