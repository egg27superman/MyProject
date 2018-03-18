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
    public class AccomodationController : Controller
    {
        private int tryCount = 0;


        //
        // GET: /Accomodation/

        public ActionResult AccomodationOrder()
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Online, this.ControllerContext.RouteData.Values["action"].ToString());

            AccomodationOrderModel model = new AccomodationOrderModel();
            try
            {
                model.Contact = new ContactModel();

                model.Payment = new PaymentModel();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "AccomodationOrder (httpget)exception. ex:" + ex.Message);
                _logHelper.WriteLog("AccomodationOrder (httpget) exception.", ex.Message, string.Empty);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AccomodationOrder(AccomodationOrderModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Online, this.ControllerContext.RouteData.Values["action"].ToString());
            try
            {

                if (ModelState.IsValid)
                {
                    if (!this.CheckOrderData(model))
                    {
                        model.Contact = new ContactModel();
                        model.Payment = new PaymentModel();

                        return View(model);
                    }


                    this.AccomodationOrderProcess(model, _logHelper);

                    this.SaveProcessResult(model, _logHelper);

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

                ModelState.AddModelError("", "AccomodationOrder (HttpPost) exception. ex:" + ex.Message);
                _logHelper.WriteLog("AccomodationOrder (HttpPost) exception.", ex.Message, string.Empty);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult GetAvailableRoom(AccomodationOrderModel model)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Online, this.ControllerContext.RouteData.Values["action"].ToString());
            try
            {
                if (model.Contact == null)
                {
                    model.Contact = new ContactModel();
                }
                if (model.Payment == null)
                {
                    model.Payment = new PaymentModel();
                }

                if (model.CheckOutDate <= model.CheckInDate)
                {
                    model.ResultStatus = (int)GlobalVariable.OrderStatus.Fail;
                    model.ResultMsg = "Check-Out must later than Check-in";
                    return View("AccomodationOrder", model); 
                }

                if (ModelState.IsValid)
                {
                    RoomStatusCore roomStatusCore = new RoomStatusCore();
                    model = roomStatusCore.GetAvailableRoom(model);

                    AccomodationOrderCore accomodationOrderCore = new AccomodationOrderCore();
                    model = accomodationOrderCore.CalcRoomRate(model);
                }


            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Get room info exception. ex:" + ex.Message);
                _logHelper.WriteLog("Get room info exception.", ex.Message, string.Empty);

            }

            return View("AccomodationOrder", model);
        }

        // Get AccomodationPrice page
        public ActionResult AccomodationPrice()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AccomodationPrice(AccomodationOrderModel model)
        {
            return View(model);
        }

       

        #region order process

        /// <summary>
        /// check order data before submit
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private bool CheckOrderData(AccomodationOrderModel order)
        {
            if (order.CheckOutDate < order.CheckInDate)
            {
                order.ResultStatus = (int)GlobalVariable.OrderStatus.Fail;
                order.ResultMsg = "Check-Out must later than Check-in";
                return false;
            }

            bool haveRoomNum = false;
            foreach (AccomodationOrderListModel orderList in order.AccomodationOrderList)
            {
                if (orderList.OrderRoomNumber > 0 && orderList.SubTotalPrice > 0)
                {
                    haveRoomNum = true;
                    break;
                }
            }
            if (!haveRoomNum)
            {
                order.ResultStatus = (int)GlobalVariable.OrderStatus.Fail;
                order.ResultMsg = "Please order a room";
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

            //Concurrency: check available room again
            AccomodationOrderModel availableOrderRoom = new AccomodationOrderModel();
            availableOrderRoom.CheckInDate = order.CheckInDate;
            availableOrderRoom.CheckOutDate = order.CheckOutDate;
            RoomStatusCore roomStatusCore = new RoomStatusCore();
            availableOrderRoom = roomStatusCore.GetAvailableRoom(availableOrderRoom);
            foreach (AccomodationOrderListModel orderList in order.AccomodationOrderList)
            {
                if (orderList.OrderRoomNumber > 0)
                {
                    AccomodationOrderListModel availableRoomList = availableOrderRoom.AccomodationOrderList.Find(m => m.RoomType == orderList.RoomType);
                    if (availableRoomList == null)
                    {
                        order.ResultStatus = (int)GlobalVariable.OrderStatus.Fail;
                        order.ResultMsg = "The room is not available right now. Please update the Check-in and Check-out Date then try again.";
                        return false;
                    }
                    if (orderList.OrderRoomNumber > availableRoomList.AvailableRoomNumber)
                    {
                        order.ResultStatus = (int)GlobalVariable.OrderStatus.Fail;
                        order.ResultMsg = "The room is not available right now. Please update the Check-in and Check-out Date then try again.";
                        return false;
                    }
                }
            }

            return true;
        }

        private AccomodationOrderModel AccomodationOrderProcess(AccomodationOrderModel order, DBLogHelper _helper)
        {

            #region save order

            try
            {
                tryCount = 0;

                AccomodationOrderCore accomodationOrderCore = new AccomodationOrderCore();
                order = accomodationOrderCore.SaveOrder(order);
                _helper.WriteLog("save accomodation order success", order.OrderID.ToString());
            }
            catch (Exception ex)
            {
                _helper.WriteLog("save accomodation order exception", ex.Message, order.OrderID.ToString());

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
                    _helper.WriteLog("Start to send accomodation payment request", order.OrderID.ToString());

                    tryCount = 0;
                    PaypalTransactionCore paypal = new PaypalTransactionCore();

                    Invoice inv = new Invoice();
                    Currency Amt = new Currency(new decimal(order.TotalPrice), "AUD");
                    inv.Amt = Amt;
                    inv.PoNum = order.OrderID.ToString();
                    inv.InvNum = order.OrderNo;

                    string creditCard = order.Payment.creditCard1 + order.Payment.creditCard2 + order.Payment.creditCard3 + order.Payment.creditCard4;
                    string expireDate = order.Payment.ExpiryMonth + order.Payment.ExpiryYear;
                    CreditCard cc = new CreditCard(creditCard, expireDate);
                    cc.Cvv2 = order.Payment.CVV;

                    res = paypal.DoSale(inv, cc, order.RequestID);

                    _helper.WriteLog("Send accomodation payment request success", order.OrderID.ToString());

                }
                catch (Exception ex)
                {
                    _helper.WriteLog("Send accomodation payment request exception", ex.Message, order.OrderID.ToString());
                    return order;
                }

                #endregion

                #region process response

                try
                {
                    _helper.WriteLog("Start to process payment response", order.OrderID.ToString());

                    if (res != null)
                    {

                        _helper.WriteLog("Accomodation payment request string: " + res.RequestString, order.OrderID.ToString());
                        _helper.WriteLog("Accomodation payment response string: " + res.ResponseString, order.OrderID.ToString());

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

                            _helper.WriteLog("Process Accomodation payment response success", order.OrderID.ToString());
                        }
                        else
                        {
                            throw new Exception("Accomodation payment transactionResponse is null");
                        }
                    }
                    else
                    {
                        throw new Exception("Accomodation payment response is null");
                    }
                }
                catch (Exception ex)
                {
                    _helper.WriteLog("Process Accomodation payment response exception", ex.Message, order.OrderID.ToString());
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

        private AccomodationOrderModel RetrySave(AccomodationOrderModel order, DBLogHelper _helper)
        {
            tryCount++;
            if (tryCount > 3)
            {
                order.ResultStatus = (int)GlobalVariable.OrderStatus.Exception;
                return order;
            }

            try
            {
                AccomodationOrderCore accomodationOrderCore = new AccomodationOrderCore();
                order = accomodationOrderCore.SaveOrder(order);
            }
            catch (Exception ex)
            {
                _helper.WriteLog("save accomodation order exception. Retry round: " + tryCount.ToString(), ex.Message, order.OrderID.ToString());
                order.ResultStatus = (int)GlobalVariable.OrderStatus.Exception;
                this.RetrySave(order, _helper);
            }
            return order;
        }

        private AccomodationOrderModel SaveProcessResult(AccomodationOrderModel order, DBLogHelper _helper)
        {
            try
            {
                this.tryCount = 0;

                AccomodationOrderCore accomodationOrderCore = new AccomodationOrderCore();
                order = accomodationOrderCore.SaveOrderResult(order);
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

        private AccomodationOrderModel RetrySaveProcessResult(AccomodationOrderModel order, DBLogHelper _helper)
        {
            tryCount++;
            if (tryCount > 3)
            {
                order.ResultStatus = (int)GlobalVariable.OrderStatus.Exception;
                return order;
            }

            try
            {
                AccomodationOrderCore accomodationOrderCore = new AccomodationOrderCore();
                order = accomodationOrderCore.SaveOrderResult(order);
            }
            catch (Exception ex)
            {
                _helper.WriteLog("save wine order process result exception. Retry round: " + tryCount.ToString(), ex.Message, order.OrderID.ToString());
                order.ResultStatus = (int)GlobalVariable.OrderStatus.Exception;
                this.RetrySaveProcessResult(order, _helper);
            }
            return order;
        }

        private void AfterPaymentProcess(object obj)
        {
            DBLogHelper _logHelper = new DBLogHelper(GlobalVariable.Log.Online, this.ControllerContext.RouteData.Values["action"].ToString());
            AccomodationOrderModel order = (AccomodationOrderModel)obj;

            if (order.ResultStatus != (int)GlobalVariable.OrderStatus.Fail
                && order.ResultStatus != (int)GlobalVariable.OrderStatus.Exception)
            {

                try
                {
                    if (order.ProcessType == (int)GlobalVariable.ProcessType.Online)
                    {
                        this.CreateInvoice(order, _logHelper);
                    }
                }
                catch (Exception ex)
                {
                    order.ResultStatus = (int)GlobalVariable.OrderStatus.Exception;
                    order.ResultMsg = "create invoice exception.";
                    _logHelper.WriteLog("create invoice exception.", ex.Message, order.OrderID.ToString());
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

                try
                {
                    if (order.ProcessType == (int)GlobalVariable.ProcessType.Online)
                    {
                        RoomStatusCore roomStatusCore = new RoomStatusCore();
                        roomStatusCore.UpdateRoomStatus(order);
                    }
                }
                catch (Exception ex)
                {
                    order.ResultStatus = (int)GlobalVariable.OrderStatus.Exception;
                    order.ResultMsg = "update room status exception.";
                    _logHelper.WriteLog("update room status exception.", ex.Message, order.OrderID.ToString());
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

        private void CreateInvoice(AccomodationOrderModel order, DBLogHelper _helper)
        {
            _helper.WriteLog("start to create invoice", order.OrderID.ToString());


            //set invoice data
            InvoiceModel invoiceData = new InvoiceModel();

            //order
            invoiceData.OrderNo = order.OrderNo;
            invoiceData.OrderDateTime = order.OrderDate;

            //billto
            List<string> lstBillTo = new List<string>();
            lstBillTo.Add(order.Contact.Name);
            lstBillTo.Add(order.Contact.Address);
            lstBillTo.Add(order.Contact.State + "," + order.Contact.PostCode);
            invoiceData.BillTo = lstBillTo;

            //item 
            List<InvoiceItemModel> lstItem = new List<InvoiceItemModel>();
            foreach (AccomodationOrderListModel accomodation in order.AccomodationOrderList)
            {
                if (accomodation.OrderRoomNumber > 0)
                {
                    InvoiceItemModel item = new InvoiceItemModel();
                    item.ItemsName = this.GetRoomTypeName(accomodation.RoomType) + "<br/>Stay Date:" 
                        + Convert.ToDateTime(order.CheckInDate).ToString("dd/MM/yyyy") + "~" 
                        + Convert.ToDateTime(order.CheckOutDate).ToString("dd/MM/yyyy");
                    item.Price = accomodation.Price;
                    item.Discount = accomodation.DiscountPrice;
                    item.Qty = accomodation.OrderRoomNumber;
                    item.TotalPrice = accomodation.SubTotalPrice;

                    lstItem.Add(item);
                }
            }
            invoiceData.InvoiceItems = lstItem;

            //total 
            invoiceData.SubTotal = order.TotalPrice;
            invoiceData.Shipping = 0;
            double tax = order.TotalPrice * Convert.ToDouble(ConfigurationManager.AppSettings["TaxRate"].ToString());
            invoiceData.Tax = tax;
            invoiceData.Total = order.TotalPrice;

            _helper.WriteLog("set invoice data success", order.OrderID.ToString());


            CreateInvoiceCore invoiceCore = new CreateInvoiceCore();
            invoiceCore.CreateInvoice(invoiceData);

            _helper.WriteLog("create invoice success", order.OrderID.ToString());
        }

        private void SendEmail(AccomodationOrderModel order, DBLogHelper _helper)
        {
            _helper.WriteLog("start to send email", order.OrderID.ToString());


            EmailEntity myEmailEntity = new EmailEntity();
            myEmailEntity.ToAddress = new List<string>();
            myEmailEntity.ToAddress.Add(order.Contact.Email);

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

                myEmailEntity.Subject = "Accomodation order success";
            }
            else
            {
                myEmailEntity.Subject = "Accomodation pre-order success";
            }
            myEmailEntity.Body = this.GetEmailBody(order);


            _helper.WriteLog("set email data success", order.OrderID.ToString());

            EmailHelper myEmailHelper = new EmailHelper();
            myEmailHelper.SendEmail(myEmailEntity);

            //EmailEntity myEmailEntity = new EmailEntity();
            //myEmailEntity.Subject = "Accomodation order success";

            //StringBuilder str = new StringBuilder();
            //str.AppendLine("Hello " + order.Contact.Name + ",");
            //str.AppendLine("");
            //str.AppendLine("");
            //str.AppendLine("");
            //str.AppendLine("Thank you for booking our accomodation service. Your order No is " + order.OrderNo);
            //str.AppendLine("");
            //str.AppendLine("");
            //str.AppendLine("GEST NAME:  " + order.Contact.Name);
            //str.AppendLine("GEST ADDRESS:   " + order.Contact.Address);
            //str.AppendLine("GEST PHONE: " + order.Contact.Phone);
            //str.AppendLine("CHECK IN:   " + Convert.ToDateTime(order.CheckInDate).ToString("dd/MM/yyyy"));
            //str.AppendLine("CHECK OUT:  " + Convert.ToDateTime(order.CheckOutDate).ToString("dd/MM/yyyy"));

            //string roomType = string.Empty;
            //for(int i=0;i<order.AccomodationOrderList.Count;i++)
            //{
            //    if (order.AccomodationOrderList[i].OrderRoomNumber > 0)
            //    {
            //        if (string.IsNullOrEmpty(roomType))
            //        {
            //            roomType = this.GetRoomTypeName(order.AccomodationOrderList[i].RoomType);
            //        }
            //        else
            //        {
            //            roomType += " / " + this.GetRoomTypeName(order.AccomodationOrderList[i].RoomType);
            //        }
            //    }
            //}
            //str.AppendLine("ROOM TYPE:  " + roomType);

            //str.AppendLine("NUMBER OF Adult:    " + order.AccomodationOrderDetail.AdultCount.ToString());
            //str.AppendLine("NUMBER OF Kid:  " + order.AccomodationOrderDetail.KidsCount.ToString());
            //str.AppendLine("TOTAL COST:     AUD" + order.TotalPrice.ToString("#,###,##0.00"));
            //str.AppendLine("ADDITIONAL DETAIL:   " + order.AccomodationOrderDetail.AdditionalDetail);
            //str.AppendLine("");
            //str.AppendLine("");
            //str.AppendLine("APPLICABLE CANCELLATION POLICY: : IF CANCELLED OR MODIFIED UP TO 7 DAYS BEFORE DATE OF ARRIVEL, NO FEE WILL BE CHARGED. IF CANCELLED OR MODIFUED LATER OR IN CASE OF NO-SHOW, 50 PERCENT OF THE FIRST NIGHT WILL BE CHARGED.");
            //str.AppendLine("");
            //str.AppendLine("");
            //str.AppendLine("");
            //str.AppendLine("Your payment has been accepted. Please print or save a copy of this mail attachment, as it is your proof of payment.");
            //str.AppendLine("");
            //str.AppendLine(@"If you have any questions about your order, please contact Miss Lingbo Support either by phone at 61 (03) 54382741 or by e-mail at manager@barcaldinehouse.com.au");
            //str.AppendLine("");
            //str.AppendLine("");
            //str.AppendLine("");
            //str.AppendLine("");
            //str.AppendLine("Yours Sincerely,");
            //str.AppendLine("");
            //str.AppendLine("Barcaldine Country House");

            //myEmailEntity.Body = str.ToString();

            //myEmailEntity.ToAddress = new List<string>();
            //myEmailEntity.ToAddress.Add(order.Contact.Email);

            //_helper.WriteLog("set email data success", order.OrderID.ToString());

            //string path = AppDomain.CurrentDomain.BaseDirectory;
            //path = Path.Combine(path, "PDF");
            //string fileName = order.OrderNo + ".pdf";

            //System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(Path.Combine(path, order.OrderNo + ".pdf"));
            //attach.Name = fileName;
            //myEmailEntity.Attactments = new List<System.Net.Mail.Attachment>();
            //myEmailEntity.Attactments.Add(attach);

            //_helper.WriteLog("set email attachments success", order.OrderID.ToString());

            //EmailHelper myEmailHelper = new EmailHelper();
            //myEmailHelper.SendEmail(myEmailEntity);


            _helper.WriteLog("send email success", order.OrderID.ToString());

        }

        private string GetEmailBody(AccomodationOrderModel order)
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

            str.AppendLine("<h2>Hello " + order.Contact.Name + ",</h2>");
            str.AppendLine("<br/>");
            str.AppendLine("<br/>");
            str.AppendLine("<br/>");
            str.AppendLine(string.Format("<h4>Thank you for {0} our wines. Your order No is {1} </h4>", orderTypeDesc, order.OrderNo));
            str.AppendLine("<br/>");
            str.AppendLine("<br/>");
            str.AppendLine("<h4>GEST NAME:  " + order.Contact.Name + "</h4>");
            str.AppendLine("<h4>GEST ADDRESS:   " + order.Contact.Address + "</h4>");
            str.AppendLine("<h4>GEST PHONE: " + order.Contact.Phone + "</h4>");
            str.AppendLine("<h4>CHECK IN: " + Convert.ToDateTime(order.CheckInDate).ToString("dd/MM/yyyy") + "</h4>");
            str.AppendLine("<h4>CHECK OUT: " + Convert.ToDateTime(order.CheckInDate).ToString("dd/MM/yyyy") + "</h4>");
            str.AppendLine("<h4>NUMBER OF ADULT: " + order.AccomodationOrderDetail.AdultCount.ToString() + "</h4>");
            str.AppendLine("<h4>NUMBER OF KID: " + order.AccomodationOrderDetail.KidsCount.ToString() + "</h4>");
            str.AppendLine("<h4>SPECIAL REQUESTS: " + order.AccomodationOrderDetail.AdditionalDetail.ToString() + "</h4>");
            str.AppendLine("<br/>");

            str.AppendLine("<div class='divWrap'>");
            str.AppendLine("<h4>Order Detail:</h4>");

            str.AppendLine("<table>");
            str.AppendLine("<tr style='text-align: center; background: #e8eaeb;'>");
            str.AppendLine("<td width='40%'>Items</td><td>Price</td><td>Discount</td><td>Qty</td><td>Total Price</td></tr>");
            foreach (AccomodationOrderListModel accomodationList in order.AccomodationOrderList)
            {
                if (accomodationList.OrderRoomNumber > 0)
                {
                    str.AppendLine("<tr>");
                    str.AppendLine(string.Format("<td>{0}</td>", this.GetRoomTypeName(accomodationList.RoomType)));
                    str.AppendLine(string.Format("<td style='text-align: right;'>${0}</td>", accomodationList.Price.ToString("#,###,##0.00")));
                    str.AppendLine(string.Format("<td style='text-align: right;'>${0}</td>", accomodationList.DiscountPrice.ToString("#,###,##0.00")));
                    str.AppendLine(string.Format("<td style='text-align: right;'>{0}</td>", accomodationList.OrderRoomNumber.ToString()));
                    str.AppendLine(string.Format("<td style='text-align: right;'>${0}</td>", accomodationList.SubTotalPrice.ToString("#,###,##0.00")));
                    str.AppendLine("</tr>");
                }
            }
            //Total
            str.AppendLine("<tr><td colspan='4' style='text-align: right;'>Subtotal:</td>");
            str.AppendLine(string.Format("<td  style='text-align: right;'>${0}</td></tr>", order.TotalPrice.ToString("#,###,##0.00")));

            str.AppendLine("<tr><td colspan='4' style='text-align: right;'>Shipping:</td>");
            str.AppendLine(string.Format("<td  style='text-align: right;'>${0}</td></tr>","0.00"));

            str.AppendLine("<tr><td colspan='4' style='text-align: right;'>Tax:</td>");
            str.AppendLine(string.Format("<td  style='text-align: right;'>${0}</td></tr>", (order.TotalPrice * Convert.ToDouble(ConfigurationManager.AppSettings["TaxRate"].ToString())).ToString("#,###,##0.00")));

            str.AppendLine("<tr><td colspan='4' style='text-align: right;'>Total:</td>");
            str.AppendLine(string.Format("<td  style='text-align: right;'>${0}</td></tr>", order.TotalPrice.ToString("#,###,##0.00")));
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

        private string GetRoomTypeName(int roomType)
        {
            string roomName = string.Empty;
            if (roomType == GlobalVariable.RoomType.King)
            {
                roomName = "King Room";
            }
            else if (roomType == GlobalVariable.RoomType.Queen)
            {
                roomName = "Queen Room";
            }
            else if (roomType == GlobalVariable.RoomType.Princess)
            {
                roomName = "Princess Room";
            }
            return roomName;
        }

        #endregion

    }
}
