using System.Collections.ObjectModel;
using System.Configuration;

namespace CaSys
{
    using System;
    using System.Threading;


    public class Pay
    {
        public void PayInvoicesCaSyS(Collection<InvoiceDetails> invoices, LoginInfo info, string lang, string email)
        {
            var _invoicesNumbers = string.Empty;
            var amount = 0m;
            foreach (var item in invoices)
            {
                amount += item.Invoice_Unbilled_Amount;
                _invoicesNumbers += item.Invoice_Ref_Number + "; ";
            }

            _invoicesNumbers = _invoicesNumbers.Trim().TrimEnd(';');
            try
            {
                var invoiceCount = invoices.Count;
                var tmName1 = string.Empty;
                var tmName2 = string.Empty;


                //ToDo Da se sredi
                var myOracle = new MyOracle(ConfigurationManager.ConnectionStrings["OracleConnString"].ToString());
                var ePaymentSessionId = myOracle.GenerateOrderId();

                var casys = new Casys
                {
                    PaymentOKURL = EnvironmentHelper.PaymentOkurlInvoices + "?orderId=" + ePaymentSessionId + "&lang=" + lang,
                    PaymentFailURL = EnvironmentHelper.PaymentFailurlInvoices + "?orderId=" + ePaymentSessionId + "&lang=" + lang,
                    PayToMerchant = Settings.Default.MerchantIdInvoices,

                    AmountToPay = InvoicesAmountForCaSyS(amount),
                    AmountCurrency = "MKD",
                    Details1 = info.MSISDN,
                    Details2 = ePaymentSessionId,
                    Details3 = lang,
                    Telephone = info.MSISDN,
                    MerchantName = Settings.Default.MerchantName,
                    Email = email,
                    CasysPassword = EnvironmentHelper.CasysPasswordInvoices,
                    CasysUrl = EnvironmentHelper.ReturnCasysURL(lang)
                };
              
                if (SaveToDB(ePaymentSessionId, info, invoices, email))
                {
                    var rp = new RemotePost();
                    rp.Post(casys); // -------->>>> Kon CaSyS
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception ex)
            {
                //Utils.LogError(ex);
                throw;
            }
        }

        private bool SaveToDB(string ePaymentSessionId, LoginInfo info, Collection<InvoiceDetails> invoicesCurent, string email)
        {
            try
            {
                var myOracle = new MyOracle(ConfigurationManager.ConnectionStrings["OracleConnString"].ToString());
                var isSuccess = myOracle.CreateEPayment(ePaymentSessionId, email, info.MSISDN);
                if (!isSuccess)
                {
                    return false;
                }

                foreach (var inv in invoicesCurent)
                {
                    myOracle = new MyOracle(ConfigurationManager.ConnectionStrings["OracleConnString"].ToString());
                    var orderId = myOracle.GenerateOrderId();
                    myOracle = new MyOracle(ConfigurationManager.ConnectionStrings["OracleConnString"].ToString());
                    isSuccess = myOracle.CreateEPaymentDetails(
                                    orderId,
                                    ePaymentSessionId,
                                    inv.Invoice_Ref_Number,
                                    Convert.ToDecimal(inv.Invoice_Unbilled_Amount));
                    if (!isSuccess)
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                //Utils.LogError(ex);
                return false;
            }

            return true;
        }

        private string InvoicesAmountForCaSyS(decimal amount)
        {
            try
            {
                amount = amount * 100;
                var forreturnallInvoicesAmount = Convert.ToInt32(amount).ToString();
                return forreturnallInvoicesAmount;
            }
            catch (Exception ex)
            {
                //Utils.LogError(ex);
                throw;
            }
        }

        private static string InvoicesAmount(decimal allInvoicesAmount, string lang)
        {
            if (lang == "en")
            {
                return allInvoicesAmount.ToString("N2") + @" " + MyTranslation.GetResourceString("valuta");
            }

            return allInvoicesAmount.ToString("N2") + @" " + MyTranslation.GetResourceString("valuta") + ".";
        }
    }
}
