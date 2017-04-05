﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Pharos.Logic.DAL
{
    internal class STHouseMoveDAL : BaseDAL
    {
        public DataTable LoadDetailList(string moveId)
        {
            string sql = @"SELECT a.MoveId,a.OutStoreId,a.InStoreId, b.OrderQuantity,b.DeliveryQuantity,b.ActualQuantity,b.State,b.Memo,b.Barcode, c.ProductCode, c.Title,c.SubUnit,c.SysPrice,
                IsNull(d.StockNumber,0) AS StockNumber
                FROM dbo.HouseMove a 
                JOIN dbo.HouseMoveList b ON b.MoveId=a.MoveId
                LEFT JOIN dbo.Vw_Product c ON c.Barcode=b.Barcode or ','+c.barcodes+',' LIKE '%,'+ b.Barcode+',%'
                LEFT JOIN dbo.Inventory d ON (d.StoreId=a.OutStoreId OR d.StoreId IS NULL) AND (d.Barcode=b.Barcode OR d.Barcode IS NULL)
                WHERE (d.StoreId=a.OutStoreId OR d.StoreId IS NULL) AND a.MoveId=" + "'" + moveId + "' AND c.CompanyId=" + Sys.SysCommonRules.CompanyId + "";

            DataTable dt = new DataTable();
            using (EFDbContext db = new EFDbContext())
            {
                var conn = db.Database.Connection;
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                var dr = cmd.ExecuteReader();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    dt.Columns.Add(dr.GetName(i), dr.GetFieldType(i));
                }
                int k = 0;
                while (dr.Read())
                {
                    var row = dt.NewRow();
                    foreach (DataColumn col in dt.Columns)
                    {
                        var obj = dr[col.ColumnName];
                        row[col.ColumnName] = obj;
                    }
                    dt.Rows.Add(row);
                    k++;
                }
                dr.Close();
                conn.Close();
            }
            return dt;
        }

    }
}