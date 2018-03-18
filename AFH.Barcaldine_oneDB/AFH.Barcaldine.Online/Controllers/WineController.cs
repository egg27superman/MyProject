using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Threading;
using System.Text;
using System.IO;

using AFH.Barcaldine.Models;
using AFH.Barcaldine.Core;
using AFH.Barcaldine.Common;
using AFH.Barcaldine.Log;

using PayPal.Payments.Common;
using PayPal.Payments.DataObjects;

using AFH.Common.Email;

namespace AFH.Barcaldine.Online.Controllers
{
    public class WineController : Controller
    {

        private int tryCount = 0;

        //
        // GET: /Wine/
        #region WineList

        public ActionResult WineList()
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Online, this.ControllerContext.RouteData.Values["action"].ToString());

            List<ProductView> model = new List<ProductView>();
            try
            {
                //Default data: Select all data
                ProductCore productCore = new ProductCore();
                model = productCore.GetProducts(string.Empty);
                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "WineList (httpget) exception. ex:" + ex.Message);
                _logHelper.WriteLog("WineList (httpget) exception.", ex.Message, string.Empty);
            }
            return View(model);

            
        }


        #endregion

        #region WineDetail

        public ActionResult WineDetail(string id)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Online, this.ControllerContext.RouteData.Values["action"].ToString());

            WineDetailModel model = new WineDetailModel();
            try
            {
                ProductCore productCore = new ProductCore();
                model = productCore.GetWineDetailByID(id);

                model.Wines = productCore.GetWineInfo();

                WineCategoryCore wineCategoryCore = new WineCategoryCore();
                model.Categorys = wineCategoryCore.GetCategorys();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "WineDetail (httpget) exception. ex:" + ex.Message);
                _logHelper.WriteLog("WineDetail (httpget) exception.", ex.Message, string.Empty);

            }
            return View(model);
        }


        #endregion

        #region WineOrder

        public ActionResult WineOrder()
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Online, this.ControllerContext.RouteData.Values["action"].ToString());
            WineOrderModel wineOrders = new WineOrderModel();
            try
            {
                ProductCore productCore = new ProductCore();
                wineOrders.Wines = productCore.GetWineInfoForOrder();

                wineOrders.Delivery = new DeliveryModel();

                wineOrders.Payment = new PaymentModel();

                ShippingCore shippingCore = new ShippingCore();
                wineOrders.ShippingInfo = shippingCore.GetShipping();

            }

            catch (Exception ex)
            {
                ModelState.AddModelError("", "WineOrder (httpget) exception. ex:" + ex.Message);
                _logHelper.WriteLog("WineOrder (httpget) exception.", ex.Message, string.Empty);
            }
            return View(wineOrders);
        }

        [HttpPost]
        public ActionResult WineOrder(WineOrderModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Online, this.ControllerContext.RouteData.Values["action"].ToString());
            try
            {
                ProductCore productCore = new ProductCore();

                if (ModelState.IsValid)
                {

                    if (!this.CheckData(ref model))
                    {
                        return View(model);
                    }

                    this.WineOrderProcess(model, _logHelper);

                    if (model.ProcessType == (int)GlobalVariable.ProcessType.Online)
                    {
                        this.SaveProcessResult(model, _logHelper);
                    }

                    ThreadPool.QueueUserWorkItem(this.AfterPaymentProcess, model);

                    if (model.ResultStatus == (int)GlobalVariable.OrderStatus.Success)
                    {
                        OrderSuccessModel orderSuccess = new OrderSuccessModel();
                        orderSuccess.OrderNo = model.OrderNo;
                        orderSuccess.ProcessType = model.ProcessType;
                        return RedirectToAction("Success", "OrderResult", orderSuccess);
                    }
                }


            }
            catch (Exception ex)
            {
                model.ResultStatus = (int)GlobalVariable.OrderStatus.Exception;
                model.ResultMsg = "Exception occured, please retry again.";
                ModelState.AddModelError("", "WineOrder (httpget) exception. ex:" + ex.Message);
                _logHelper.WriteLog("WineOrder (httpget) exception.", ex.Message, string.Empty);
            }
            return View(model);
        }

        #endregion

        #region Wine Order Process

        private bool CheckData(ref WineOrderModel order)
        {
            //check bottles > 0
            bool isExistBottles = false;
            for (int i = 0; i < order.Wines.Count; i++)
            {
                if (order.Wines[i].Bottle > 0)
                {
                    isExistBottles = true;
                    break;
                }
            }
            if (!isExistBottles)
            {
                order.ResultStatus = (int)GlobalVariable.OrderStatus.Fail;
                order.ResultMsg = "Order bottles are required.";
                return false;
            }
            
            //age check
            DateTime dt = Convert.ToDateTime(order.Delivery.Birthday);
            if (dt.AddYears(18) > DateTime.Now)
            {
                order.ResultStatus = (int)GlobalVariable.OrderStatus.Fail;
                order.ResultMsg = "You must older than 18 years old.";
                return false;
            }

            //payment
            if (order.ProcessType == (int)GlobalVariable.ProcessType.Online)
            {
                if (string.IsNullOrEmpty(order.Payment.creditCard1)
                    || string.IsNullOrEmpty(order.Payment.creditCard2)
                    || string.IsNullOrEmpty(order.Payment.creditCard3)
                    || string.IsNullOrEmpty(order.Payment.creditCard4))
                {
                    order.ResultStatus = (int)GlobalVariable.OrderStatus.Fail;
                    order.ResultMsg = "Credit card is required";
                    return false;
                }

                if (order.Payment.ExpiryMonth == "M")
                {
                    order.ResultStatus = (int)GlobalVariable.OrderStatus.Fail;
                    order.ResultMsg = "Expiry Month is required";
                    return false;
                }

                if (order.Payment.ExpiryYear == "Y")
                {
                    order.ResultStatus = (int)GlobalVariable.OrderStatus.Fail;
                    order.ResultMsg = "Expiry Year is required";
                    return false;
                }

                if (string.IsNullOrEmpty(order.Payment.CVV))
                {
                    order.ResultStatus = (int)GlobalVariable.OrderStatus.Fail;
                    order.ResultMsg = "Card security code is required.This is the last three digit number found on the back of your credit/debit card. Please try again.";
                    return false;
                }

            }

            return true;
        }

        private WineOrderModel WineOrderProcess(WineOrderModel order, DBLogHelper _helper)
        {

            #region save order

            try
            {
                tryCount = 0;

                WineOrderCore wineOrder = new WineOrderCore();
                order = wineOrder.SaveOrder(order);
                _helper.WriteLog("save wine order success", order.OrderID.ToString());
            }
            catch (Exception ex)
            {
                _helper.WriteLog("save wine order exception", ex.Message, order.OrderID.ToString());

                this.RetrySave(order, _helper);
                if (order.ResultStatus == (int)GlobalVariable.OrderStatus.Exception)
                {
                    return order;
                }

            }

            #endregion

            if (order.ProcessType == (int)GlobalVariable.ProcessType.Online)
            {

                #region send paypal request
                Response res;
                try
                {
                    _helper.WriteLog("Start to send wine payment request", order.OrderID.ToString());

                    tryCount = 0;
                    PaypalTransactionCore paypal = new PaypalTransactionCore();

                    Invoice inv = new Invoice();
                    Currency Amt = new Currency(new decimal(order.TotalOrderPrice), "AUD");
                    inv.Amt = Amt;
                    inv.PoNum = order.OrderID.ToString();
                    inv.InvNum = order.OrderNo;

                    string creditCard = order.Payment.creditCard1 + order.Payment.creditCard2 + order.Payment.creditCard3 + order.Payment.creditCard4;
                    string expireDate = order.Payment.ExpiryMonth + order.Payment.ExpiryYear;
                    CreditCard cc = new CreditCard(creditCard, expireDate);
                    cc.Cvv2 = order.Payment.CVV;

                    res = paypal.DoSale(inv, cc, order.RequestID);

                    _helper.WriteLog("Send wine payment request success", order.OrderID.ToString());

                }
                catch (Exception ex)
                {
                    _helper.WriteLog("Send wine payment request exception", ex.Message, order.OrderID.ToString());
                    return order;
                }

                #endregion

                #region process response

                try
                {
                    _helper.WriteLog("Start to process payment response", order.OrderID.ToString());

                    if (res != null)
                    {

                        _helper.WriteLog("Wine payment request string: " + res.RequestString, order.OrderID.ToString());
                        _helper.WriteLog("Wine payment response string: " + res.ResponseString, order.OrderID.ToString());

                        TransactionResponse trxnResponse = res.TransactionResponse;
                        if (trxnResponse != null)
                        {
                            order.Response = new PaymentResponseModel();
                            order.Response.ResponseID = trxnResponse.Pnref;
                            order.Response.ResultCode = trxnResponse.Result;
                            order.Response.ResultMessage = trxnResponse.RespMsg;

                            if (trxnResponse.Result == 0)
                            {
                                order.ResultStatus = (int)GlobalVariable.OrderStatus.Success;
                                order.ResultMsg = "Payment success";
                            }
                            else
                            {
                                order.ResultStatus = (int)GlobalVariable.OrderStatus.Fail;
                                order.ResultMsg = trxnResponse.RespMsg;
                            }

                            _helper.WriteLog("Process wine payment response success", order.OrderID.ToString());
                        }
                        else
                        {
                            throw new Exception("Wine payment transactionResponse is null");
                        }
                    }
                    else
                    {
                        throw new Exception("Wine payment response is null");
                    }
                }
                catch (Exception ex)
                {
                    _helper.WriteLog("Process wine payment response exception", ex.Message, order.OrderID.ToString());
                    return order;

                }
                #endregion

            }
            else
            {
                order.ResultStatus = (int)GlobalVariable.OrderStatus.Success;
                order.ResultMsg = "Pe-order success.";
            }

            return order;
        }

        private WineOrderModel SaveProcessResult(WineOrderModel order, DBLogHelper _helper)
        {
            try
            {
                this.tryCount = 0;

                WineOrderCore wineOrder = new WineOrderCore();
                order = wineOrder.SaveOrderResult(order);
                _helper.WriteLog("save wine order process result success", order.OrderID.ToString());

            }
            catch (Exception ex)
            {
                _helper.WriteLog("save wine order process result exception", ex.Message, order.OrderID.ToString());

                this.RetrySaveProcessResult(order, _helper);
                if (order.ResultStatus == (int)GlobalVariable.OrderStatus.Exception)
                {
                    return order;
                }
            }

            return order;
        }

        private WineOrderModel RetrySaveProcessResult(WineOrderModel order, DBLogHelper _helper)
        {
            tryCount++;
            if (tryCount > 3)
            {
                order.ResultStatus = (int)GlobalVariable.OrderStatus.Exception;
                return order;
            }

            try
            {
                WineOrderCore wineOrder = new WineOrderCore();
                order = wineOrder.SaveOrderResult(order);
            }
            catch (Exception ex)
            {
                _helper.WriteLog("save wine order process result exception. Retry round: " + tryCount.ToString(), ex.Message, order.OrderID.ToString());
                order.ResultStatus = (int)GlobalVariable.OrderStatus.Exception;
                this.RetrySaveProcessResult(order, _helper);
            }
            return order;
        }

        private WineOrderModel RetrySave(WineOrderModel order, DBLogHelper _helper)
        {
            tryCount++;
            if (tryCount > 3)
            {
                order.ResultStatus = (int)GlobalVariable.OrderStatus.Exception;
                return order;
            }

            try
            {
                WineOrderCore wineOrder = new WineOrderCore();
                order = wineOrder.SaveOrder(order);
            }
            catch (Exception ex)
            {
                _helper.WriteLog("save wine order exception. Retry round: " + tryCount.ToString(), ex.Message, order.OrderID.ToString());
                order.ResultStatus = (int)GlobalVariable.OrderStatus.Exception;
                this.RetrySave(order, _helper);
            }
            return order;
        }

        private void AfterPaymentProcess(object obj)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Online, this.ControllerContext.RouteData.Values["action"].ToString());
            WineOrderModel order = (WineOrderModel)obj;

            if (order.ResultStatus != (int)GlobalVariable.OrderStatus.Fail
                && order.ResultStatus != (int)GlobalVariable.OrderStatus.Exception)
            {
                if (order.ProcessType == (int)GlobalVariable.ProcessType.Online)
                {
                    try
                    {
                        this.CreateInvoice(order, _logHelper);
                    }
                    catch (Exception ex)
                    {
                        order.ResultStatus = (int)GlobalVariable.OrderStatus.Exception;
                        order.ResultMsg = "create invoice exception.";
                        _logHelper.WriteLog("create invoice exception.", ex.Message, order.OrderID.ToString());
                    }
                }

                try
                {
                    this.SendEmail(order, _logHelper);
                }
                catch (Exception ex)
                {
                    order.ResultStatus = (int)GlobalVariable.OrderStatus.Exception;
                    order.ResultMsg = "send email exception.";
                    _logHelper.WriteLog("send email exception.", ex.Message, order.OrderID.ToString());
                }
            }

            try
            {
                //final status == exception or fail ==> fault orders table
                if (order.ResultStatus == (int)GlobalVariable.OrderStatus.Fail
                    || order.ResultStatus == (int)GlobalVariable.OrderStatus.Exception)
                {
                    FaultOrdersCore faultOrdersCore = new FaultOrdersCore();
                    faultOrdersCore.InsertFaultOrder(order.OrderID, order.ResultMsg);
                }
            }
            catch (Exception ex)
            {
                _logHelper.WriteLog("insert fault orders exception.", ex.Message, order.OrderID.ToString());
            }
        }

        private void CreateInvoice(WineOrderModel order, DBLogHelper _helper)
        {
            _helper.WriteLog("start to create invoice", order.OrderID.ToString());


            //set invoice data
            InvoiceModel invoiceData = new InvoiceModel();

            //order
            invoiceData.OrderNo = order.OrderNo;
            invoiceData.OrderDateTime = order.OrderDate;

            //billto
            List<string> lstBillTo = new List<string>();
            lstBillTo.Add(order.Delivery.Name);
            lstBillTo.Add(order.Delivery.Address);
            lstBillTo.Add(order.Delivery.State + "," + order.Delivery.PostCode);
            invoiceData.BillTo = lstBillTo;

            //item 
            List<InvoiceItemModel> lstItem = new List<InvoiceItemModel>();
            foreach (WineListModel wine in order.Wines)
            {
                if (wine.Bottle > 0)
                {
                    InvoiceItemModel item = new InvoiceItemModel();
                    item.ItemsName = wine.ProductYear.ToString() + "-" + wine.WineName;
                    item.Price = wine.Price;
                    item.Discount = 0;
                    item.Qty = wine.Bottle;
                    item.TotalPrice = wine.Price * wine.Bottle;

                    lstItem.Add(item);
                }
            }
            invoiceData.InvoiceItems = lstItem;

            //total 
            invoiceData.SubTotal = order.OriginalPrice;
            invoiceData.Shipping = order.Shipping;
            double tax = order.TotalOrderPrice * Convert.ToDouble(ConfigurationManager.AppSettings["TaxRate"].ToString());
            invoiceData.Tax = tax;
            invoiceData.Total = order.TotalOrderPrice;

            _helper.WriteLog("set invoice data success", order.OrderID.ToString());


            CreateInvoiceCore invoiceCore = new CreateInvoiceCore();
            invoiceCore.CreateInvoice(invoiceData);

            _helper.WriteLog("create invoice success", order.OrderID.ToString());
        }

        private void SendEmail(WineOrderModel order, DBLogHelper _helper)
        {
            _helper.WriteLog("start to send email", order.OrderID.ToString());

            EmailEntity myEmailEntity = new EmailEntity();
            myEmailEntity.ToAddress = new List<string>();
            myEmailEntity.ToAddress.Add(order.Delivery.Email);

            if (order.ProcessType == (int)GlobalVariable.ProcessType.Online)
            {
                //set invoice attachment
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = Path.Combine(path, "PDF");
                string fileName = order.OrderNo + ".pdf";

                System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(Path.Combine(path, order.OrderNo + ".pdf"));
                attach.Name = fileName;
                myEmailEntity.Attactments = new List<System.Net.Mail.Attachment>();
                myEmailEntity.Attactments.Add(attach);

                _helper.WriteLog("set email attachments success", order.OrderID.ToString());

                myEmailEntity.Subject = "Wine order success";
            }
            else
            {
                myEmailEntity.Subject = "Wine pre-order success";
            }
            myEmailEntity.Body = this.GetEmailBody(order);


            _helper.WriteLog("set email data success", order.OrderID.ToString());

            EmailHelper myEmailHelper = new EmailHelper();
            myEmailHelper.SendEmail(myEmailEntity);
            

            _helper.WriteLog("send email success", order.OrderID.ToString());

        }

        private string GetEmailBody(WineOrderModel order)
        {
            string orderTypeDesc = string.Empty;
            if (order.ProcessType == (int)GlobalVariable.ProcessType.Online)
            {
                orderTypeDesc = "ordering";
            }
            else
            {
                orderTypeDesc = "pre-order";
            }


            StringBuilder str = new StringBuilder();
            str.AppendLine("<html>");
            str.AppendLine("<head>");
            str.AppendLine("<style type='text/css'>");

            str.AppendLine("table {border-collapse: collapse; width: 100%; }");
            str.AppendLine("table td {border: 1px solid #d3d3d3; height: 30px; padding:5px 5px 5px 5px;}");
            str.AppendLine(".divWrap {float:left; width: 550px}");

            str.AppendLine("</style>");
            str.AppendLine("</head>");


            str.AppendLine("<body>");

            str.AppendLine("<h2>Hello " + order.Delivery.Name + ",</h2>");
            str.AppendLine("<br/>");
            str.AppendLine("<br/>");
            str.AppendLine("<br/>");
            str.AppendLine(string.Format("<h4>Thank you for {0} our wines. Your order No is {1} </h4>", orderTypeDesc, order.OrderNo));
            str.AppendLine("<br/>");
            str.AppendLine("<br/>");
            str.AppendLine("<h4>GEST NAME:  " + order.Delivery.Name + "</h4>");
            str.AppendLine("<h4>GEST ADDRESS:   " + order.Delivery.Address + "</h4>");
            str.AppendLine("<h4>GEST PHONE: " + order.Delivery.Phone + "</h4>");
            str.AppendLine("<br/>");

            str.AppendLine("<div class='divWrap'>");
            str.AppendLine("<h4>Order Detail:</h4>");

            str.AppendLine("<table>");
            str.AppendLine("<tr style='text-align: center; background: #e8eaeb;'>");
            str.AppendLine("<td width='40%'>Items</td><td>Price</td><td>Discount</td><td>Qty</td><td>Total Price</td></tr>");
            foreach (WineListModel wineDetail in order.Wines)
            {
                if (wineDetail.Bottle > 0)
                {
                    str.AppendLine("<tr>");
                    str.AppendLine(string.Format("<td>{0}</td>", wineDetail.ProductYear + "-" + wineDetail.WineName));
                    str.AppendLine(string.Format("<td style='text-align: right;'>${0}</td>", wineDetail.Price.ToString("#,###,##0.00")));
                    str.AppendLine(string.Format("<td style='text-align: right;'>${0}</td>", "0.00"));
                    str.AppendLine(string.Format("<td style='text-align: right;'>{0}</td>", wineDetail.Bottle.ToString()));
                    str.AppendLine(string.Format("<td style='text-align: right;'>${0}</td>", (wineDetail.Price * wineDetail.Bottle).ToString("#,###,##0.00")));
                    str.AppendLine("</tr>");
                }
            }
            //Total
            str.AppendLine("<tr><td colspan='4' style='text-align: right;'>Subtotal:</td>");
            str.AppendLine(string.Format("<td  style='text-align: right;'>${0}</td></tr>", order.OriginalPrice.ToString("#,###,##0.00")));

            str.AppendLine("<tr><td colspan='4' style='text-align: right;'>Shipping:</td>");
            str.AppendLine(string.Format("<td  style='text-align: right;'>${0}</td></tr>", order.Shipping.ToString("#,###,##0.00")));

            str.AppendLine("<tr><td colspan='4' style='text-align: right;'>Tax:</td>");
            str.AppendLine(string.Format("<td  style='text-align: right;'>${0}</td></tr>", (order.TotalOrderPrice * Convert.ToDouble(ConfigurationManager.AppSettings["TaxRate"].ToString())).ToString("#,###,##0.00")));

            str.AppendLine("<tr><td colspan='4' style='text-align: right;'>Total:</td>");
            str.AppendLine(string.Format("<td  style='text-align: right;'>${0}</td></tr>", order.TotalOrderPrice.ToString("#,###,##0.00")));
            str.AppendLine("</table>");
            str.AppendLine("</div>");
            str.AppendLine("<br/>");
            str.AppendLine("<br/>");

            str.AppendLine("<div class='divWrap'>");
            str.AppendLine(string.Format("<h4>If you have any questions about your {0}, please contact Miss Lingbo Support either by phone at 61 (03) 54382741 or by e-mail at manager@barcaldinehouse.com.au</h4>", orderTypeDesc));
            str.AppendLine("</div>");
            str.AppendLine("<br/>");
            str.AppendLine("<br/>");
            str.AppendLine("<div class='divWrap'>");
            str.AppendLine("<h4>Yours Sincerely,</h4>");
            str.AppendLine("<br/>");
            str.AppendLine("<h4>Barcaldine Country House</h4>");
            str.AppendLine("</div>");

            str.AppendLine("</body></html>");

            return str.ToString();
        }
        #endregion


    }
}
