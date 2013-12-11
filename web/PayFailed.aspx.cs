using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;
using CaSys;

namespace web
{
    public partial class PayFailed : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Utilities utilities = new Utilities();
            var lang = utilities.LanguageSetAndReturn(Request.QueryString["lang"] ?? string.Empty);
            //if (!string.IsNullOrEmpty(Request.QueryString["lang"]))
            //{
            //    lang = Request.QueryString["lang"].ToUpper();
            switch (lang)
            {
                case "MK":
                    LiteralInfo.Text = MyTranslation.GetResourceString("UnsuccessfulTransaction");
                    invoiceсWithPay.InnerText = MyTranslation.GetResourceString("MENU_INVOICES");
                    break;
                case "EN":
                    LiteralInfo.Text = MyTranslation.GetResourceString("UnsuccessfulTransaction");
                    invoiceсWithPay.InnerText = MyTranslation.GetResourceString("MENU_INVOICES");
                    break;
                case "AL":
                    LiteralInfo.Text = MyTranslation.GetResourceString("UnsuccessfulTransaction");
                    invoiceсWithPay.InnerText = MyTranslation.GetResourceString("MENU_INVOICES");
                    break;
                default:
                    LiteralInfo.Text = MyTranslation.GetResourceString("UnsuccessfulTransaction");
                    invoiceсWithPay.InnerText = MyTranslation.GetResourceString("MENU_INVOICES");
                    break;
            }
            //}

            var sessionId = string.Empty;
            var casysTransactionId = string.Empty;
            var requestResponse = new RequestResponse();
            var returnedParams = GetFormParams();
            var infoLog = new StringBuilder();
            var doc = new XmlDocument();
            XmlNode paramsNode = null;
            const string strFilename = "C:\\Temp\\returnParms.xml";
            var tmisIssuedByAcquirer = string.Empty;
            var tmPmtReasonOfDecline = string.Empty;

            if (CheckForm() == "get")
            {
                if (!string.IsNullOrEmpty(Request.QueryString["orderId"]))
                {
                    MyOracle p = new MyOracle(ConfigurationManager.ConnectionStrings["OracleConnString"].ToString());
                    if (p.GetEPaymentById(Request.QueryString["orderId"]) != null)
                    {
                        var o = p.GetEPaymentById(Request.QueryString["orderId"]);
                        if (o.Count > 0)
                        {
                            LiteralReasonOfDecline.Text = MyTranslation.GetResourceString("error_casys_Invoices" + o.TmPmtReasonOfDecline);
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
                    doc.Save(strFilename);
                }

                sessionId = requestResponse.Details2;
                lang = requestResponse.Details3;

                requestResponse.CasysPassword = EnvironmentHelper.CasysPasswordInvoices;
                requestResponse.CasysUrl = EnvironmentHelper.ReturnCasysUrl(lang);
                casysTransactionId = requestResponse.CPayPaymentRef;
                tmisIssuedByAcquirer = requestResponse.TMisIssuedByAcquirer;
                tmPmtReasonOfDecline = requestResponse.TMPmtReasonOfDecline;
            }

            try
            {
                info.SessionId = sessionId;
                MyOracle p = new MyOracle(ConfigurationManager.ConnectionStrings["OracleConnString"].ToString());
                var succesUpdate = p.UpdateEPayment(sessionId, casysTransactionId,
                                           tmisIssuedByAcquirer, tmPmtReasonOfDecline);
            }
            catch (Exception ex)
            {
                Utils.LogError(ex);

            }
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
    }
}