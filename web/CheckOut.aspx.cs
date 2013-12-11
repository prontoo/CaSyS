using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Web.UI;
using CaSys;

namespace web
{
    public partial class CheckOut : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Pay_Click(object sender, EventArgs e)
        {

            var records = (InvoiceDetails[])Session["records"];
            var recordsForSend = new Collection<InvoiceDetails>();
            foreach (var item in records)
            {
                if (item.chkValue && item.Invoice_Unbilled_Amount > 0)
                {
                    var idet = new InvoiceDetails();
                    idet.Invoice_Unbilled_Amount = item.Invoice_Unbilled_Amount;
                    idet.Invoice_Ref_Number = item.Invoice_Ref_Number;
                    recordsForSend.Add(idet);
                }
            }

            try
            {
                if (recordsForSend.Count > 0)
                {
                    var p = new Pay();
                    LoginHelper loginHelper = new LoginHelper();
                    info = loginHelper.GetLoginInfoPage(Page);
                    var customer = loginHelper.GetProfile(info);
                    customer.eMail = txtEmail.Text;
                    if (customer.eMail != string.Empty)
                    {
                        //ToDo Da se sredi
                        //Provider.ProfileUpdate(info, customer);
                    }

                    p.PayInvoicesCaSyS(recordsForSend, info, "MK", txtEmail.Text);
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                //Utils.LogError(ex);
            }
        }
    }
}