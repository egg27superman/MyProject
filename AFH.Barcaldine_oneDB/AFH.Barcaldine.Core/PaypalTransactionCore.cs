using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using PayPal.Payments.Common;
using PayPal.Payments.Common.Utility;
using PayPal.Payments.DataObjects;
using PayPal.Payments.Transactions;

using AFH.Common.Security;

namespace AFH.Barcaldine.Core
{
    public class PaypalTransactionCore
    {
        UserInfo _user;

        public PaypalTransactionCore()
        {
            string user = ConfigurationManager.AppSettings["PAYFLOW_User"].ToString();
            string vendor = ConfigurationManager.AppSettings["PAYFLOW_Vendor"].ToString();
            string partner = ConfigurationManager.AppSettings["PAYFLOW_Partner"].ToString();
            string password = SecurityHelper.Decode(ConfigurationManager.AppSettings["PAYFLOW_Password"].ToString());

            this._user = new UserInfo(user, vendor, partner, password);
        }

        public Response DoSale(Invoice Inv, CreditCard CC, string requestID)
        {
            PayflowConnectionData Connection = new PayflowConnectionData();

            CardTender Card = new CardTender(CC);

            SaleTransaction Trans = new SaleTransaction(
                this._user, Connection, Inv, Card, requestID);

            Response Resp = Trans.SubmitTransaction();

            return Resp;

        }
    }
}
