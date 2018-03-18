using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFH.Barcaldine.Models
{
    public class PaymentResponseModel
    {
        public string ResponseID { get; set; }
        public int ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public DateTime ResponseDatetime { get; set; }
    }
}
