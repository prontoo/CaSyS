using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using CaSys;

namespace web
{
    public partial class PaySuccess : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            var sessionId = string.Empty;
            var casysTransactionId = string.Empty;
            var requestResponse = new RequestResponse();
            var returnedParams = GetFormParams();
            var infoLog = new StringBuilder();
            var doc = new XmlDocument();
            XmlNode paramsNode = null;
            const string strFilename = "C:\\Temp\\returnParms.xml";
            var lang = "MK";
            var tmisIssuedByAcquirer = string.Empty;

            if (!IsPostBack)
            {
                Translate();
            }

            if (CheckForm() == "get")
            {
                if (!string.IsNullOrEmpty(Request.QueryString["orderId"]))
                {
                    MyOracle p = new MyOracle(ConfigurationManager.ConnectionStrings["OracleConnString"].ToString());
                    var invoiceForGet = p.GetEPaymentByIdForGet(Request.QueryString["orderId"]);
                    if (invoiceForGet != null)
                    {
                        if (invoiceForGet.Count > 0)
                        {
                            lang = Request.QueryString["lang"];
                            FillRequestInvoices(invoiceForGet, lang, true);
                            return;
                        }

                        return;
                    }
                }

                return;
            }

            if (returnedParams.Count > 0)
            {
                if (EnvironmentHelper.EnableLogging)
                {
                    #region [Kreira XML kade sto gi zapisuva return parametrite - koga ce se pusti vo live okolina EnableLogging vo web.config treba da e false ]

                    if (File.Exists(strFilename))
                    {
                        File.Delete(strFilename);
                    }

                    XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    doc.AppendChild(docNode);
                    paramsNode = doc.CreateElement("Parameters");
                    doc.AppendChild(paramsNode);
                    #endregion
                }

                if (EnvironmentHelper.EnableLogging)
                {
                    infoLog.AppendLine("Return parametars from CaSyS ... started ... Date nad Time: " + DateTime.Now);
                }

                foreach (var rtp in returnedParams)
                {
                    if (EnvironmentHelper.EnableLogging)
                    {
                        #region [Kreira XML kade sto gi zapisuva return parametrite - koga ce se pusti vo live okolina EnableLogging vo web.config treba da e false ]

                        XmlNode paramNode = doc.CreateElement("parameter");
                        var nameAttr = doc.CreateAttribute("name");
                        nameAttr.Value = rtp.ParameterName;

                        // koga ce se pusti vo live okolina EnableLogging vo web.config treba da e false
                        if (EnvironmentHelper.EnableLogging)
                        {
                            infoLog.AppendLine(rtp.ParameterName + ": " + rtp.ParameterValue);
                        }

                        if (paramNode.Attributes != null)
                        {
                            paramNode.Attributes.Append(nameAttr);
                        }

                        var valueAttr = doc.CreateAttribute("value");
                        valueAttr.Value = rtp.ParameterValue;
                        if (paramNode.Attributes != null)
                        {
                            paramNode.Attributes.Append(valueAttr);
                        }

                        if (paramsNode != null)
                        {
                            paramsNode.AppendChild(paramNode);
                        }

                        #endregion
                    }

                    for (var y = 0; y < requestResponse.RequestResponseProperties().Count; y++)
                    {
                        var propName = requestResponse.RequestResponseProperties().GetKey(y);
                        if (propName != rtp.ParameterName)
                        {
                            continue;
                        }

                        var propertyInfo = typeof(RequestResponse).GetProperty(propName);
                        if (propertyInfo.GetSetMethod() != null)
                        {
                            propertyInfo.SetValue(requestResponse, rtp.ParameterValue, null);
                        }
                    }
                }

                // koga ce se pusti vo live okolina EnableLogging vo web.config treba da e false
                if (EnvironmentHelper.EnableLogging)
                {
                    infoLog.AppendLine("Return parametars from CaSyS ... end ...");
                    Utils.LogError(infoLog.ToString());
                    doc.Save(strFilename);
                }

                sessionId = requestResponse.Details2;
                lang = requestResponse.Details3;

                requestResponse.CasysPassword = EnvironmentHelper.CasysPasswordInvoices;
                requestResponse.CasysUrl = EnvironmentHelper.ReturnCasysUrl(lang);
                casysTransactionId = requestResponse.CPayPaymentRef;
                tmisIssuedByAcquirer = requestResponse.TMisIssuedByAcquirer;
            }

            try
            {
                info.SessionId = sessionId;
                MyOracle p = new MyOracle(ConfigurationManager.ConnectionStrings["OracleConnString"].ToString());
                var o = p.GetEPaymentById(sessionId);

                string alredyPayed;
                var oFromDb = GetEPaymentFromDb(sessionId, casysTransactionId, lang, out alredyPayed);
                if (o != null)
                {
                    if (string.IsNullOrEmpty(alredyPayed))
                    {
                        if (o.Count > 0)
                        {
                            if (Utilities.IsValidMd5Invoices(oFromDb, requestResponse.ReturnCheckSum))
                            {
                                p = new MyOracle(ConfigurationManager.ConnectionStrings["OracleConnString"].ToString());
                                var succesUpdate = p.UpdateEPayment(sessionId, casysTransactionId,
                                                                           tmisIssuedByAcquirer, string.Empty);
                                if (succesUpdate)
                                {
                                    var ob = p.GetEPaymentById(sessionId);
                                    var ePaymentSerialNumber = FillRequestInvoices(ob, lang, false);

                                    if (!string.IsNullOrEmpty(ePaymentSerialNumber))
                                    {
                                        p = new MyOracle(ConfigurationManager.ConnectionStrings["OracleConnString"].ToString());
                                        p.UpdateEPaymentSuccess(ob.SessionID, ePaymentSerialNumber);
                                    }
                                    else
                                    {
                                        //Auditing.LogAction(AuditAction.InvoiceNoSerial, false, "0", info, DateTime.Now);
                                        throw new ArgumentException("InvoiceNoSerial");
                                    }

                                    //Auditing.LogAction(AuditAction.InvoiceSuccess, true, "0", info, DateTime.Now);
                                }
                                else
                                {
                                    //Auditing.LogAction(AuditAction.InvoiceUpdateinDatabaseFailed, false, "0", info, DateTime.Now);
                                    throw new ArgumentException("InvoiceUpdateinDatabaseFailed");
                                }
                            }
                            else
                            {
                                //Auditing.LogAction(AuditAction.InvoiceWrongMD5, false, "0", info, DateTime.Now);
                                throw new ArgumentException("InvoiceWrongMD5");
                            }
                        }
                        else
                        {
                            //Auditing.LogAction(AuditAction.InvoiceNotExistInDataBase1, false, "0", info, DateTime.Now);
                            throw new ArgumentException("InvoiceNotExistInDataBase1");
                        }
                    }
                    else
                    {
                        //Auditing.LogAction(AuditAction.InvoiceAllreadyPayed, false, "0", info, DateTime.Now);
                        throw new ArgumentException("InvoiceAllreadyPayed");
                    }
                }
                else
                {
                    //Auditing.LogAction(AuditAction.InvoiceNotExistInDataBase, false, "0", info, DateTime.Now);
                    throw new ArgumentException("InvoiceNotExistInDataBase");
                }
            }
            catch (Exception ex)
            {
                // ErrorSignal.FromCurrentContext().Raise(ex);
                Log(ex);
                //Auditing.LogAction(AuditAction.InvoiceGeneralError, false, "0", info, DateTime.Now);
            }
        }

        private void Translate()
        {
            ltrSuccessMsg.Text = ("InvoiceSuccessMsg");
            Literal1.Text = ("InvoicesNumber");
            Literal2.Text = ("IznosNaFaktura");
            Literal3.Text = ("Status");

            ((Literal)RepeaterInvoices.FindControl("Literal1")).Text = ("ServiceProcessingLabel");
            invoiceсWithPay.Text = ("EBillBackTo");
        }

        private string CheckForm()
        {
            var doc = new XmlDocument();

            #region [Kreira XML kade sto go zapisuva tipot na HttpMethod (GET ili POST) - koga ce se pusti vo live okolina EnableLogging vo web.config treba da e false ]
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);
            XmlNode paramsNode = doc.CreateElement("Parameters");
            doc.AppendChild(paramsNode);
            #endregion

            #region [Kreira XML kade sto gi zapisuva return parametrite - koga ce se pusti vo live okolina EnableLogging vo web.config treba da e false ]

            XmlNode paramNode = doc.CreateElement("parameter");
            var nameAttr = doc.CreateAttribute("HttpMethod");
            nameAttr.Value = Request.HttpMethod;
            if (paramNode.Attributes != null)
            {
                paramNode.Attributes.Append(nameAttr);
            }

            var valueAttr = doc.CreateAttribute("value");
            valueAttr.Value = Request.QueryString["orderId"];
            if (paramNode.Attributes != null)
            {
                paramNode.Attributes.Append(valueAttr);
            }

            paramsNode.AppendChild(paramNode);

            var strFilename = "C:\\Temp\\returnParmsInvoices" + DateTime.Now.Ticks + ".xml";
            doc.Save(strFilename);

            return Request.HttpMethod.ToLower();
            #endregion
        }

        private Collection<ReturnedParameters> GetFormParams()
        {
            var rtpColl = new Collection<ReturnedParameters>();
            for (var i = 0; i < Request.Form.Count; i++)
            {
                {
                    var rtp = new ReturnedParameters
                    {
                        ParameterName = Request.Form.GetKey(i),
                        ParameterValue = Request.Form.Get(i)
                    };
                    rtpColl.Add(rtp);
                }
            }

            return rtpColl;
        }

        private string FillRequestInvoices(InvoiceObject currentResponse, string lang, bool onlyShow)
        {
            var info = new LoginInfo();
            info.SessionId = currentResponse.SessionID;
            if (string.IsNullOrEmpty(currentResponse.SessionID))
            {
                return string.Empty;
            }

            if (!onlyShow)
            {
                if (string.IsNullOrEmpty(currentResponse.CaSySTransactionID))
                {
                    return string.Empty;
                }
            }

            try
            {
                var objectForSend = new ChargeInvoicesRequestData
                {
                    DealerId = ConfigurationManager.AppSettings["DealerID"],
                    SessionId = currentResponse.SessionID,
                    CustomerData = new WebServices.eRecharge.Customer
                    {
                        Email = currentResponse.ConfirmationEmail,
                        MSISDN = currentResponse.ConfirmationTelephone
                    },
                    PaymentData = new Payment
                    {
                        AuthCode = currentResponse.CaSySTransactionID,
                        BankID =
                            Convert.ToInt32(
                                ConfigurationManager.AppSettings["BankID"]),
                        CardID =
                            Convert.ToInt32(
                                currentResponse.TmisIssuedByAcquirer)
                    },
                    InvoiceData = new PaidInvoice[currentResponse.InvoiceDetais.Count]
                };

                for (var i = 0; i < currentResponse.InvoiceDetais.Count; i++)
                {
                    objectForSend.InvoiceData[i] = new PaidInvoice
                    {
                        Paid_Amount = currentResponse.InvoiceDetais[i].Amount,
                        Invoice_Ref_Num =
                            currentResponse.InvoiceDetais[i].InvoiceNumber
                    };
                }

                switch (lang.ToLower())
                {
                    case "mk":
                        objectForSend.Language = WebServices.eRecharge.type_Language.MK;
                        break;
                    case "al":
                        objectForSend.Language = WebServices.eRecharge.type_Language.AL;
                        break;
                    case "en":
                        objectForSend.Language = WebServices.eRecharge.type_Language.EN;
                        break;
                }

                if (!onlyShow)
                {
                    var dorecharge = Invoices.DoChargeInvoices(objectForSend);

                    if (dorecharge != null && dorecharge.ResponseType != null)
                    {
                        if (dorecharge.ResponseType.Status == 1)
                        {
                            return "1";
                        }
                    }
                }
                else
                {
                    AzurirajLabeli(objectForSend);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Log(ex);
                //Auditing.LogAction(AuditAction.InvoiceGeneralError, false, "0", info, DateTime.Now);
            }

            return string.Empty;
        }

        private void AzurirajLabeli(ChargeInvoicesRequestData objectForSend)
        {
            decimal vkupno = 0;
            var payedInvoices = new Collection<PayedInvoices>();
            foreach (var invoice in objectForSend.InvoiceData)
            {
                var p = new PayedInvoices
                {
                    InvoiceNumber = invoice.Invoice_Ref_Num,

                    InvoicePayedAmount = Utils.FormatPriceValue(MyTranslation.GetResourceString("den"), invoice.Paid_Amount, true)
                };
                vkupno = vkupno + Convert.ToDecimal(invoice.Paid_Amount);

                payedInvoices.Add(p);
            }

            RepeaterInvoices.DataSource = payedInvoices;
            RepeaterInvoices.DataBind();
            ltrVkupno.Text = "vkupna suma";
        }

        /// <summary>
        /// Zima Naracka za Vaucer od baza koja nema trasaction ID
        /// </summary>
        /// <param name="sessionId">
        /// sessionId na narackata za eTopUp
        /// </param>
        /// <param name="casysTransactionId">
        /// Vraka Id na transakcijata vo CaSyS
        /// </param>
        /// <param name="language">
        /// Jazik na koj se vrsi celata transakcija
        /// </param>
        /// <param name="alredyPayed">
        /// The alredy Payed.
        /// </param>
        /// <returns>
        /// Vraka objekt Casys za narackata
        /// </returns>
        private Casys GetEPaymentFromDb(string sessionId, string casysTransactionId, string language, out string alredyPayed)
        {
            alredyPayed = null;
            var c = new Casys();
            var p = new MyOracle(ConfigurationManager.ConnectionStrings["OracleConnString"].ToString());
            var o = p.GetEPaymentById(sessionId);
            if (o != null)
            {
                c.Email = o.ConfirmationEmail;
                c.Telephone = o.ConfirmationTelephone;
                c.Details1 = o.ConfirmationTelephone;
                c.Details2 = o.SessionID;
                c.Details3 = language;

                decimal totalamount = 0;
                foreach (var amount in o.InvoiceDetais)
                {
                    totalamount += amount.Amount;
                }

                c.AmountToPay = InvoicesAmountForCaSyS(totalamount);
                c.CPayPaymentRef = casysTransactionId;

                // Some default values 
                c.PaymentOKURL = EnvironmentHelper.PaymentOkurlInvoices + "?orderId=" + o.SessionID + "&lang=" + language;
                c.PaymentFailURL = EnvironmentHelper.PaymentFailurlInvoices + "?orderId=" + o.SessionID + "&lang=" + language;
                c.PayToMerchant = ConfigurationManager.AppSettings["MerchantId"];
                c.AmountCurrency = "MKD";
                c.MerchantName = ConfigurationManager.AppSettings["MerchantName"];
                c.CasysPassword = EnvironmentHelper.CasysPasswordInvoices;

                alredyPayed = o.PaymentSuccess;
            }

            return c;
        }

        private static string InvoicesAmountForCaSyS(decimal amount)
        {
            try
            {
                amount = amount * 100;

                var forreturnallInvoicesAmount = Convert.ToInt32(amount).ToString();

                return forreturnallInvoicesAmount;
            }
            catch (Exception ex)
            {
                Log(ex);
                throw;
            }
        }

        public static void Log(object o)
        {
            //var l = new LoginInfo();
            //if (l != null)
            //{
            //    if (l.MSISDN != null)
            //    {
            //        if (l.MSISDN.Length > 0)
            //        {
            //            Utils.LogError(o);
            //            return;
            //        }
            //    }
            //}
            //Utils.LogError(o);
        }

        public static void Log(string s, object o)
        {
            //var l = new LoginInfo();
            //if (l != null)
            //{
            //    if (l.MSISDN != null)
            //    {
            //        if (l.MSISDN.Length > 0)
            //        {
            //            Utils.LogError(o);
            //            return;
            //        }
            //    }
            //}
            //Utils.LogError(o);
        }

        public class PayedInvoices
        {
            public string InvoiceNumber { get; set; }

            public string InvoicePayedAmount { get; set; }
        }
    }
}