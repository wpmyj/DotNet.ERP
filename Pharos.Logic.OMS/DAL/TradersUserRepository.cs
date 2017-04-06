﻿using Pharos.Logic.OMS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pharos.Logic.OMS.Entity.View;
using System.Data;
using System.Data.SqlClient;
using Pharos.Logic.OMS.IDAL;

namespace Pharos.Logic.OMS.DAL
{
    /// <summary>
    /// DAL商家登录账号
    /// </summary>
    public class TradersUserRepository : BaseRepository<TradersUser>, ITradersUserRepository
    {
        /// <summary>
        /// 获取CID
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<TradersStore> getCIDStore(string strW = "")
        {
            using (EFDbContext db = new EFDbContext())
            {
                var sql = "select CID from TradersStore where State=1 " + strW + " group by CID order by CID";
                var list = db.Database.SqlQuery<int>(sql).ToList();
                List<TradersStore> list2 = new List<TradersStore>();
                foreach(var v in list)
                {
                    TradersStore tradersStore = new TradersStore();
                    tradersStore.CID = v;
                    list2.Add(tradersStore);
                }
                return list2;
            }
        }

        /// <summary>
        /// 分页获取
        /// </summary>
        /// <param name="CurrentPage">当前页,从1开始,不是0</param>
        /// <param name="PageSize">每页显示多少条数据</param>
        /// <param name="strw">where条件</param>
        /// <param name="Count">总数</param>
        /// <returns></returns>
        public List<ViewTradersUser> getPageList(int CurrentPage, int PageSize, string strw, out int Count)
        {
            string Table = "";
            Table = Table + "TradersUser u ";
            Table = Table + "left join TradersStore s on s.TStoreInfoId=u.TStoreInfoId ";
            Table = Table + "left join SysUser i on i.UserId=u.SysCreateUID ";

            string Fields = "";
            Fields = Fields + "u.Id,u.State,u.CID,u.LoginName,u.AccountType,s.StoreNum,s.StoreName,u.FullName,u.Phone,u.Memo,u.CreateDT,i.FullName as SFullName ";

            string Where = "u.IsHide=0 ";
            if (strw != "")
            {
                Where = Where + strw;
            }

            string OrderBy = "u.CreateDT desc ";

            return CommonDal.getPageList<ViewTradersUser>(Table, Fields, Where, OrderBy, CurrentPage, PageSize, 0, out Count);
        }
    }
}