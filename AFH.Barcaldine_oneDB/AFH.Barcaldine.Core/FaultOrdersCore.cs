using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

using AFH.Barcaldine.Common;
using AFH.Common.DataBaseAccess;
using AFH.Common.Serializer;
using AFH.Barcaldine.Models;

namespace AFH.Barcaldine.Core
{
    public class FaultOrdersCore
    {
        public void InsertFaultOrder(int orderID, string faultMessage)
        {
            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            StringBuilder str = new StringBuilder();
            str.AppendLine("Insert into FaultOrders");
            str.AppendLine(" (OrderID, faultMessage, IsDelete, CreateUser, CreateTime, UpdateUser, UpdateTime) ");
            str.AppendLine(" Values (@OrderID, @FaultMessage, 0, 'system', getdate(), 'system', getdate());");

            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("@OrderID", orderID);
            paras[1] = new SqlParameter("@FaultMessage", Common.VariableConvert.ConvertStringToDBValue(faultMessage));

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);

        }



        public void DeleteFault(string orderID, string updateUser)
        {
            List<string> sqls = new List<string>();
            List<SqlParameter[]> cmdParms = new List<SqlParameter[]>();

            StringBuilder str = new StringBuilder();
            str.AppendLine("update FaultOrders set isdelete =1, UpdateUser = @UpdateUser, UpdateTime=getdate()");
            str.AppendLine(" where orderid = @OrderID ");
            
            SqlParameter[] paras = new SqlParameter[2];
            paras[0] = new SqlParameter("@OrderID", orderID);
            paras[1] = new SqlParameter("@UpdateUser", Common.VariableConvert.ConvertStringToDBValue(updateUser));

            sqls.Add(str.ToString());
            cmdParms.Add(paras);

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuerys(sqls, cmdParms);


        }



    }
}
