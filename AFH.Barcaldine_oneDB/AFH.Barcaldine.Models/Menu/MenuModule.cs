using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFH.Barcaldine.Models
{
    public class MenuModule
    {
        public List<MenuDetailModule> ParentMenus { get; set; }

        public List<MenuDetailModule> Menus { get; set; }

    }

    public class MenuDetailModule
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public int ParentMenuID { get; set; }
    }


}
