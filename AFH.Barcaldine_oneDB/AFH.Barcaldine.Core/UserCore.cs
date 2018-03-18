using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;
using System.Data;
using System.Web.Mvc;

using AFH.Barcaldine.Models;//需要加的
using AFH.Barcaldine.Core;//需要加的
using AFH.Common.DataBaseAccess;

namespace AFH.Barcaldine.Core
{
   public class UserCore
    {
        /// <summary>
        /// offline 的 user管理的列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<UserResultListModule> GetUserList(UserSearchModule model)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine(" SELECT distinct");
            str.AppendLine(" us.UserID, u.Username ");
            str.AppendLine(" FROM User_Menu us with(Nolock)");
            str.AppendLine(" left join LoginUser u with(Nolock) on us.UserID = u.UserID");
            if (model.UserID != null)
            {
                str.AppendLine(" WHERE us.UserID = @UserID");
            }
            if (model.UserName != null)
            {
                str.AppendLine(" WHERE u.Username like '%' + @UserName + '%'");
            }


            str.AppendLine(" ORDER BY UserID ASC");

            SqlParameter[] paras = new SqlParameter[3];
            if (model.UserID != null)
            {
                paras[0] = new SqlParameter("@UserID", model.UserID);
            }
            if (model.UserName != null)
            {
                paras[1] = new SqlParameter("@UserName", model.UserName);
            }

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(str.ToString(), paras);

            List<UserResultListModule> userResultList = new List<UserResultListModule>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                userResultList = DataMappingHelper<UserResultListModule>.DataTableToList(ds.Tables[0]);
            }

            return userResultList;

        }

        /// <summary>
        /// 获取User管理详细页面数据
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UserUpdateModule GetUserByID(string userID)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine(" SELECT ");
            str.AppendLine(" UserID, Username, Password, IsDelete ");
            str.AppendLine(" FROM LoginUser with(Nolock)");
            str.AppendLine(" WHERE UserID = @UserID");

            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@UserID", userID);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(str.ToString(), paras);

            UserUpdateModule user = new UserUpdateModule();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                user = DataMappingHelper<UserUpdateModule>.DataTableToEntity(ds.Tables[0]);
            }
            return user;
        }

        /// <summary>
        /// 获取User权限
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<PrivilegeDetailModel> GetUserpribilegeByID(string userID)
        {
           //StringBuilder str = new StringBuilder();
            StringBuilder str_usermenu = new StringBuilder();
          //str.AppendLine(" SELECT ");
          //str.AppendLine(" UserID, Username, IsDelete ");
          //str.AppendLine(" FROM LoginUser with(Nolock)");
          //str.AppendLine(" WHERE UserID = @UserID");
          /*SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@UserID", userID);

            SqlAccess mySqlAccess = new SqlAccess("WebsiteDBConnection");
            DataSet ds = mySqlAccess.ExecuteAdapter(str.ToString(), paras);

            UserUpdateModule user = new UserUpdateModule();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                user.UserID = Convert.ToInt32(ds.Tables[0].Rows[0]["UserID"]);
            }
            */
            str_usermenu.AppendLine(" SELECT ");
            str_usermenu.AppendLine(" m.MenuID, m.menuname, um.UserID, um.MenuID, um.IsSelected ");
            str_usermenu.AppendLine(" FROM Menu m with(Nolock) left join User_Menu um with(Nolock) on m.MenuID = um.MenuID and um.UserID = @UserID");
            str_usermenu.AppendLine(" where m.ParentMenuID <> 0");
            str_usermenu.AppendLine(" order by m.menuID asc");
            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@UserID", userID);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet dsUser = mySqlAccess.ExecuteAdapter(str_usermenu.ToString(), paras);
         

            List<PrivilegeDetailModel> PrivilegeDetail = new List<PrivilegeDetailModel>();
            if (dsUser != null && dsUser.Tables.Count > 0)
            {

                PrivilegeDetail = DataMappingHelper<PrivilegeDetailModel>.DataTableToList(dsUser.Tables[0]);
                /*操作数据 一 一对应
               foreach (DataRow dw in dsUser.Tables[0].Rows)
               {
                   PrivilegeDetailModel privilege = new PrivilegeDetailModel();
                   
                   privilege.UserID = Convert.ToInt32(dsUser.Tables[0].Rows[0]["UserID"]);
                   privilege.MenuID = Convert.ToInt32(dsUser.Tables[0].Rows[0]["MenuID"]);
                   privilege.IsSelected = Convert.ToBoolean(dsUser.Tables[0].Rows[0]["IsSelected"]);
                   user.UserPrivilegeList.Add(privilege);
                    
               }
                 */
            }
            
            
            

            /*if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                user = DataMappingHelper<UserUpdateModule>.DataTableToEntity(ds.Tables[0]);
            }*/
            return PrivilegeDetail;
        }


        /// <summary>
        /// 创建新的User权限
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<PrivilegeDetailModel> SetUserpribilegeByID(string userID)
        {
            //StringBuilder str = new StringBuilder();
            StringBuilder str_usermenu = new StringBuilder();
            //str.AppendLine(" SELECT ");
            //str.AppendLine(" UserID, Username, IsDelete ");
            //str.AppendLine(" FROM LoginUser with(Nolock)");
            //str.AppendLine(" WHERE UserID = @UserID");
            /*SqlParameter[] paras = new SqlParameter[1];
              paras[0] = new SqlParameter("@UserID", userID);

              SqlAccess mySqlAccess = new SqlAccess("WebsiteDBConnection");
              DataSet ds = mySqlAccess.ExecuteAdapter(str.ToString(), paras);

              UserUpdateModule user = new UserUpdateModule();
              if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
              {
                  user.UserID = Convert.ToInt32(ds.Tables[0].Rows[0]["UserID"]);
              }
              */
            str_usermenu.AppendLine(" SELECT ");
            str_usermenu.AppendLine(" MenuID, menuname");
            str_usermenu.AppendLine(" FROM Menu with(Nolock)");
            str_usermenu.AppendLine(" where ParentMenuID <> 0");
      

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet dsUser = mySqlAccess.ExecuteAdapter(str_usermenu.ToString());
            List<PrivilegeDetailModel> UserPrivilegeList = new List<PrivilegeDetailModel>();
            foreach (DataRow dw in dsUser.Tables[0].Rows)
            {
                PrivilegeDetailModel privilege = new PrivilegeDetailModel();

                privilege.UserID = Convert.ToInt32(userID);
                privilege.MenuID = Convert.ToInt32(dw["MenuID"]);
                privilege.menuname = Convert.ToString(dw["Menuname"]);
                privilege.IsSelected = false;
                UserPrivilegeList.Add(privilege);

            }






            return UserPrivilegeList;
        }


        /// <summary>
        /// 下拉菜单username绑定和赋值
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetUsernameList(string select)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" UserID, Username");
            sql.AppendLine(" FROM LoginUser WITH(NOLOCK)");
            sql.AppendLine(" WHERE IsDelete = 0 and userID not in (select userid from User_Menu with(nolock))");

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString());
            List<SelectListItem> items = new List<SelectListItem>();
/*
            if (showAll)
            {
                if (string.IsNullOrEmpty(select))
                {
                    items.Add(new SelectListItem { Text = "All", Value = "-1", Selected = true });
                }
                else
                {
                    items.Add(new SelectListItem { Text = "All", Value = "-1" });
                }
            }
 */

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0];
                foreach (DataRow dw in dt.Rows)
                {


                    string name = dw["Username"].ToString();
                    string id = dw["UserID"].ToString();
                    if (id == select)
                    {
                        items.Add(new SelectListItem { Text = name, Value = id, Selected = true });
                    }
                    else
                    {
                        items.Add(new SelectListItem { Text =name, Value = id });
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// UserName 是否已经存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool ExistUser(string userName)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine(" SELECT ");
            str.AppendLine(" UserID, Username, Password, IsDelete ");
            str.AppendLine(" FROM LoginUser with(Nolock)");
            str.AppendLine(" WHERE Username = @Username");

            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@Username", userName);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(str.ToString(), paras);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取user的密码
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetUserPasswordByName(string userName)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine(" SELECT ");
            str.AppendLine(" Password ");
            str.AppendLine(" FROM LoginUser with(Nolock)");
            str.AppendLine(" WHERE Username = @Username");

            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@Username", userName);

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(str.ToString(), paras);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Rows[0]["Password"].ToString();
            }
            return string.Empty;
        }

       /// <summary>
        /// 新增userPrivilege
        /// </summary>
        /// <param name="model"></param>
        public void AddUserPrivilege(UserUpdateModule model)
        {

            List<string> sql = new List<string>();
            List<SqlParameter[]> param = new List<SqlParameter[]>();

            for (int i = 0; i < model.UserPrivilegeList.Count; i++)
            {
                StringBuilder str_insert = new StringBuilder();
                str_insert.AppendLine("INSERT INTO User_Menu");
                str_insert.AppendLine("(UserID, MenuID, IsSelected)");
                str_insert.AppendLine(" VALUES ");
                str_insert.AppendLine(" (@UserID, @MenuID, @IsSelected)");

                SqlParameter[] paras2 = new SqlParameter[3];
                paras2[0] = new SqlParameter("@UserID", model.UserNameSearch.UserID);
                paras2[1] = new SqlParameter("@MenuID", model.UserPrivilegeList[i].MenuID);
                paras2[2] = new SqlParameter("@IsSelected", model.UserPrivilegeList[i].IsSelected);


                sql.Add(str_insert.ToString());
                param.Add(paras2);

            }
           
            SqlAccess mySqlAccess_insert = new SqlAccess();
            mySqlAccess_insert.ExecuteNonQuerys(sql, param);
        }

        /// <summary>
        /// update管理中的更新
        /// </summary>
        /// <param name="model"></param>
        public void UpdateUser(UserUpdateModule model)
        {
            List<string> sql = new List<string>();
            List<SqlParameter[]> para = new List<SqlParameter[]>();

            StringBuilder str_del = new StringBuilder();
            str_del.AppendLine(" DELETE from User_Menu");
            str_del.AppendLine(" WHERE UserID = @UserID");
            SqlParameter[] paras1 = new SqlParameter[1];
            paras1[0] = new SqlParameter("@UserID", model.UserID);
            sql.Add(str_del.ToString());
            para.Add(paras1);


            for (int i = 0; i < model.UserPrivilegeList.Count; i++)
            {
                StringBuilder str_insert = new StringBuilder();
                str_insert.AppendLine("INSERT INTO User_Menu");
                str_insert.AppendLine("(UserID, MenuID, IsSelected)");
                str_insert.AppendLine(" VALUES ");
                str_insert.AppendLine(" (@UserID, @MenuID, @IsSelected)");

                SqlParameter[] paras2 = new SqlParameter[3];
                paras2[0] = new SqlParameter("@UserID", model.UserPrivilegeList[i].UserID);
                paras2[1] = new SqlParameter("@MenuID", model.UserPrivilegeList[i].MenuID);
                paras2[2] = new SqlParameter("@IsSelected", model.UserPrivilegeList[i].IsSelected);

                sql.Add(str_insert.ToString());
                para.Add(paras2);
            }


            SqlAccess mySqlAccess_insert = new SqlAccess();
            mySqlAccess_insert.ExecuteNonQuerys(sql, para);

        }

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="model"></param>
        public void UpdatePassword(ChangePasswordModule model)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine(" UPDATE LoginUser SET");
            str.AppendLine(" Password = @Password,");
            str.AppendLine(" UpdateUser = @UpdateUser,");
            str.AppendLine(" UpdateTime = getdate()");
            str.AppendLine(" WHERE UserName = @UserName");

            SqlParameter[] paras = new SqlParameter[3];
            paras[0] = new SqlParameter("@Password", model.NewPassword);
            paras[1] = new SqlParameter("@UpdateUser", model.UpdateUser);
            paras[2] = new SqlParameter("@UserName", model.UserName);

            SqlAccess mySqlAccess = new SqlAccess();
            mySqlAccess.ExecuteNonQuery(str.ToString(), paras);
        }
    }
}
