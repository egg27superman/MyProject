using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

using AFH.Barcaldine.Models;
using AFH.Barcaldine.Common;
using AFH.Common.DataBaseAccess;
using AFH.Common.Serializer;

namespace AFH.Barcaldine.Core
{
    public class MenuCore
    {
        public MenuModule GetMenu(int userID)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" m.MenuID, m.Menuname, m.Action as ActionName, m.Control as ControllerName, m.ParentMenuID");
            sql.AppendLine(" FROM user_menu us with(Nolock)");
            sql.AppendLine(" left join menu m with(Nolock) on us.menuid = m.menuID");
            sql.AppendLine(" WHERE us.userID = @UserID");
            sql.AppendLine(" and IsSelected = 1");
            sql.AppendLine(" and ParentMenuID <> 0");
            sql.AppendLine(" order by m.menuID asc");


            SqlParameter[] paras = new SqlParameter[1];
            paras[0] = new SqlParameter("@UserID", Convert.ToInt32(userID));

            SqlAccess mySqlAccess = new SqlAccess();
            DataSet ds = mySqlAccess.ExecuteAdapter(sql.ToString(), paras);

            MenuModule menu = new MenuModule();
            menu.Menus = new List<MenuDetailModule>();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                menu.Menus = DataMappingHelper<MenuDetailModule>.DataTableToList(ds.Tables[0]);
            }


            sql = new StringBuilder();
            sql.AppendLine("SELECT ");
            sql.AppendLine(" m.MenuID, m.Menuname, m.Action as ActionName, m.Control as ControllerName, m.ParentMenuID");
            sql.AppendLine(" FROM menu m with(Nolock)");
            sql.AppendLine(" WHERE ParentMenuID = 0");
            sql.AppendLine(" order by m.menuID asc");

            menu.ParentMenus = new List<MenuDetailModule>();
            DataSet dsParent = mySqlAccess.ExecuteAdapter(sql.ToString());
            if (dsParent != null && dsParent.Tables.Count > 0 && dsParent.Tables[0].Rows.Count > 0)
            {
                menu.ParentMenus = DataMappingHelper<MenuDetailModule>.DataTableToList(dsParent.Tables[0]);
            }
            return menu;
        }
    }
}
