using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFH.Barcaldine.Log
{
    public class LogModule
    {
        public int LogID { get; set; }

        public string LogContent { get; set; }

        public string ErrorMessage { get; set; }

        public string LogTime { get; set; }
        public string System { get; set; }
        public string SystemModel { get; set; }

        public string ReferenceID { get; set; }
    }
}
