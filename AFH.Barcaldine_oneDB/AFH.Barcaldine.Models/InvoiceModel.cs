using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFH.Barcaldine.Models
{
    public class InvoiceModel
    {
        public string OrderNo { get; set; }
        public DateTime OrderDateTime { get; set; }

        public List<string> BillTo { get; set; }

        public List<InvoiceItemModel> InvoiceItems { get; set; }

        public double SubTotal { get; set; }
        public double Shipping { get; set; }
        public double Tax { get; set; }
        public double Total { get; set; }

    }

    public class InvoiceItemModel
    {
        public string ItemsName { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public int Qty { get; set; }
        public double TotalPrice { get; set; }
    }
}
