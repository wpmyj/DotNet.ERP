﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Pharos.Logic.DAL
{
    internal class OrderDAL:BaseDAL
    {
        public DataTable LoadDetailList(string orderId,int companyId)
        {
            string sql = @"SELECT a.*,a.IndentNum InboundNumber,b.SubUnit,b.Title,b.ProductCode,b.CategorySN,b.Expiry,
                STUFF((SELECT '<br/>'+Barcode+' '+dbo.F_ProductNameBybarcode(Barcode,CompanyId)+' '+ dbo.F_NumberAutoStr(IndentNum)+'件' FROM IndentOrderList WHERE Nature=1 AND resbarcode=a.Barcode AND IndentOrderId=a.IndentOrderId FOR XML PATH('')),1,11,'') Detail,
                STUFF((SELECT ','+Barcode+'~'+CAST(IndentNum AS VARCHAR(20)) from IndentOrderList WHERE Nature=1 AND resbarcode=a.Barcode AND IndentOrderId=a.IndentOrderId FOR XML PATH('')),1,1,'') Gift
                FROM dbo.IndentOrderList a 
                INNER JOIN dbo.Vw_Product b ON b.Barcode=a.Barcode
                WHERE a.Nature=0 AND a.IndentOrderId='" + orderId + "' and CompanyId="+companyId;
            //sql += "AND b.StartVal<=" + ordernum + " and b.EndVal>=" + ordernum;
            DataTable dt = new DataTable();
            using (EFDbContext db = new EFDbContext())
            {
                //var dt= db.Database.SqlQuery<DataTable>(sql, new SqlParameter[] { });
                var conn = db.Database.Connection;
                dt= db.Database.SqlQueryForDataTatable(sql,CommandType.Text);
            }
            return dt;
        }
        public DataTable LoadReportDetailList(string orderId, int companyId)
        {
            string sql = @"SELECT *,price*t.IndentNum Subtotal FROM(
	SELECT t.ProductCode,t.Barcode,t.Title,t.SubUnit,t.Nature,t.IndentNum,t.SysPrice,CASE WHEN t.Nature=1 THEN 0 ELSE t.Price END Price FROM dbo.Vw_OrderList t WHERE t.CompanyId=@CompanyId and t.IndentOrderId=@IndentOrderId) t";
            return _db.DataTableText(sql, new System.Data.SqlClient.SqlParameter[] { 
                new System.Data.SqlClient.SqlParameter("@CompanyId",companyId),
                new System.Data.SqlClient.SqlParameter("@IndentOrderId",orderId)
            });
        }

    }
}