using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AFH.Barcaldine.Models;

using AFH.Barcaldine.Core;//需要加的
//using AFH.Barcaldine.Log;//需要加的
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Web;
using AFH.Common.DataBaseAccess;

namespace AFH.Barcaldine.Core
{
    public class CheckLoginInfoCore
    {
        public int CheckLogin(LoginModule LoginInfo)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("select UserID, Password from LoginUser where IsDelete=0 and Username=@UserName");

            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@UserName", Common.VariableConvert.ConvertStringToDBValue(LoginInfo.UserName));

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet myds = mySqlAccess.ExecuteAdapter(str.ToString(), paras);

            if (myds.Tables[0] != null && myds.Tables[0].Rows.Count > 0)
            {
                if (LoginInfo.PassWord == Common.SecurityHelper.Decode(myds.Tables[0].Rows[0]["Password"].ToString().Trim()))
                {
                    return Convert.ToInt32(myds.Tables[0].Rows[0]["UserID"]);
                }
            }
            return -1;
        }

        public int UserType(LoginModule LoginInfo)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("select Role from LoginUser where IsDelete=0 and Username=@UserName");

            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@UserName", Common.VariableConvert.ConvertStringToDBValue(LoginInfo.UserName));

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet myds = mySqlAccess.ExecuteAdapter(str.ToString(), paras);

            if (myds.Tables[0] != null && myds.Tables[0].Rows.Count > 0)
            {
                if (myds.Tables[0].Rows[0].ItemArray[0].ToString().Trim() == "admin")
                {
                    return 1;
                }
                else if (myds.Tables[0].Rows[0].ItemArray[0].ToString().Trim() == "superuser")
                {
                    return 2;
                }
            }
            return 3;
        }
    }
}
