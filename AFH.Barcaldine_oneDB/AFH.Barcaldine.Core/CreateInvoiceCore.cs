using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using iTextSharp.text.pdf;
using iTextSharp.text; 
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.html;  


using AFH.Barcaldine.Models;
using AFH.Barcaldine.Common;
using AFH.Common.DataBaseAccess;
using AFH.Common.Serializer;

namespace AFH.Barcaldine.Core
{
    public class CreateInvoiceCore
    {
        public CreateInvoiceCore()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        public void CreateInvoice(InvoiceModel invoice)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = Path.Combine(path, "PDF");
            string fileName = invoice.OrderNo + ".pdf";

            using (Document document = new Document())
            {
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(Path.Combine(path, fileName), FileMode.Create));

                using (HTMLWorker worker = new HTMLWorker(document))
                {
                    document.Open();

                    string css = this.CreateCss();
                    string content = this.CreateContent(invoice);

                    using (var msCss = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(css)))
                    {
                        using (var msHtml = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
                        {
                            //Parse the HTML
                            iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, msHtml, msCss);
                        }
                    }

                    //worker.EndDocument();
                    worker.Close();
                }
                document.Close();
            }
        }

        private string CreateCss()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine(".body {margin: 0 auto;}");
            str.AppendLine(".clear{clear:both;}");

            str.AppendLine("h1 {text-align: center;font-size:40px;}");

            str.AppendLine(".divImage { float:left; width:240px;}");
            str.AppendLine(".divCompany { float:left;}");
            str.AppendLine(".imglogo { width: 230px;height: 80px;}");

            str.AppendLine("table {border-collapse: collapse; width: 100%; }");
            str.AppendLine("table td {border: 1px solid #d3d3d3; height: 30px; padding:5px 5px 5px 5px;}");

            str.AppendLine(".tblInvoice {float: right;width: 280px; padding-top: 30px;}");
            str.AppendLine(".tblBillTo {float: left;width: 280px; padding-top: 30px;}");
            str.AppendLine(".tblItem {float: left;width: 100%;padding-top:20px;}");


            return str.ToString();
        }


        private string CreateContent(InvoiceModel invoice)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("<html><head></head><body>");


            //logo
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,  "PDF");
            string logofileName = Path.Combine(path, "logo.png");
            str.AppendLine("<div>");
            str.AppendLine("<div class= 'divImage'><img src='" + logofileName + "' class='imglogo' /></div>");

            //companyinfo
            str.AppendLine("<div class ='divCompany'><p>Address: 238 Dairy Flat Road, Musk, Victoria, 3461</p><p>Phone: (03) 5348 2741</p><p>Email:manager@barcaldinehouse.com.au</p></div>");
            str.AppendLine("<div class='clear'></div>");
            str.AppendLine("</div>");

            str.AppendLine("<br/>");

            //title
            str.AppendLine("<h1>Invoice</h1>");

            //invoice info
            str.AppendLine("<div class='tblInvoice'>");
            str.AppendLine("<table >");
            str.AppendLine("<tr>");
            str.AppendLine("<td>Date:</td>");
            str.AppendLine(string.Format("<td>{0}</td>", invoice.OrderDateTime.ToString("dd/MM/yyyy")));
            str.AppendLine("</tr><tr>");
            str.AppendLine("<td>Invoice No:</td>");
            str.AppendLine(string.Format("<td>{0}</td></tr>", invoice.OrderNo));
            str.AppendLine("</table></div>");

            //billto
            str.AppendLine("<div class='tblBillTo'>");
            str.AppendLine("<table>");
            str.AppendLine("<tr style='background: #e8eaeb;'><td>Bill To</td></tr>");
            str.AppendLine("<tr><td>");
            foreach(string billInfo in invoice.BillTo)
            {
                str.AppendLine(string.Format("<p>{0}</p>", billInfo));
            }
            str.AppendLine("</td></tr>");
            str.AppendLine("</table></div>");

            str.AppendLine("<div class='clear'></div>");

            //item
            str.AppendLine("<div class='tblItem'>");
            str.AppendLine("<table>");
            str.AppendLine("<tr style='text-align: center; background: #e8eaeb;'>");
            str.AppendLine("<td width='40%'>Items</td><td>Price</td><td>Discount</td><td>Qty</td><td>Total Price</td></tr>");

            foreach (InvoiceItemModel items in invoice.InvoiceItems)
            {
                str.AppendLine("<tr>");
                str.AppendLine(string.Format("<td>{0}</td>", items.ItemsName));
                str.AppendLine(string.Format("<td style='text-align: right;'>${0}</td>", items.Price.ToString("#,###,##0.00")));
                str.AppendLine(string.Format("<td style='text-align: right;'>${0}</td>", items.Discount.ToString("#,###,##0.00")));
                str.AppendLine(string.Format("<td style='text-align: right;'>{0}</td>", items.Qty.ToString()));
                str.AppendLine(string.Format("<td style='text-align: right;'>${0}</td>", items.TotalPrice.ToString("#,###,##0.00")));
                str.AppendLine("</tr>");
            }

            //Total
            str.AppendLine("<tr><td colspan='4' style='text-align: right;'>Subtotal:</td>");
            str.AppendLine(string.Format("<td  style='text-align: right;'>${0}</td></tr>", invoice.SubTotal.ToString("#,###,##0.00")));

            str.AppendLine("<tr><td colspan='4' style='text-align: right;'>Shipping:</td>");
            str.AppendLine(string.Format("<td style='text-align: right;'>${0}</td></tr>", invoice.Shipping.ToString("#,###,##0.00")));

            str.AppendLine("<tr><td colspan='4' style='text-align: right;'>Tax:</td>");
            str.AppendLine(string.Format("<td style='text-align: right;'>${0}</td></tr>", invoice.Tax.ToString("#,###,##0.00")));

            str.AppendLine("<tr><td colspan='4' style='text-align: right;'>Total:</td>");
            str.AppendLine(string.Format("<td style='text-align: right;'>${0}</td></tr>", invoice.Total.ToString("#,###,##0.00")));

            str.AppendLine("</table></div>");

            str.AppendLine("</body></html>");

            return str.ToString();
        }

    }
}



