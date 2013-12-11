// --------------------------------------------------------------------------------------------------------------------- 
// <copyright file="RemotePost.cs" company="Nextsense">
//   © 2010 Nextsense
// </copyright>
// <summary>
//   Defines the RemotePost type.
// </summary>
// ---------------------------------------------------------------------------------------------------------------------

namespace CaSys
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.Security;
    using System.Xml;

    public class RemotePost
    {
        private readonly NameValueCollection _inputs = new NameValueCollection();
        private string _url = string.Empty;
        public const string Method = "post";
        public const string FormName = "frmPayment";

        public void Post(Casys casys)
        {
            _url = casys.CasysUrl;
            GenerateCheckSumHeader(PostData(casys, false), casys);
            PostData(casys, true);
            if (IsValid(casys))
            {
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
                HttpContext.Current.Response.Write(
                    "<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'>");
                HttpContext.Current.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">",
                                                                 FormName));
                HttpContext.Current.Response.Write("</head><body>");
                HttpContext.Current.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >",
                                                                 FormName, Method, _url));
                const string strFilename = "C:\\Temp\\sendParms.xml";
                var doc = new XmlDocument();
                XmlNode paramsNode = null;
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableLogging"]))
                {
                    #region [Kreira XML kade sto gi zapisuva return parametrite - koga ce se pusti vo live okolina EnableLogging vo web.config treba da e false ]

                    if (File.Exists(strFilename))
                    {
                        File.Delete(strFilename);
                    }

                    doc = new XmlDocument();
                    XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    doc.AppendChild(docNode);
                    paramsNode = doc.CreateElement("Parameters");
                    doc.AppendChild(paramsNode);

                    #endregion
                }

                for (var i = 0; i < _inputs.Keys.Count; i++)
                {
                    // TODO: na live okolina ovoj region treba da se izbrise
                    if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableLogging"]))
                    {
                        #region [Kreira XML kade sto gi zapisuva return parametrite - koga ce se pusti vo live okolina EnableLogging vo web.config treba da e false ]

                        XmlNode paramNode = doc.CreateElement("parameter");
                        var nameAttr = doc.CreateAttribute("name");
                        nameAttr.Value = _inputs.Keys[i];
                        paramNode.Attributes.Append(nameAttr);
                        var valueAttr = doc.CreateAttribute("value");
                        valueAttr.Value = _inputs[_inputs.Keys[i]];
                        paramNode.Attributes.Append(valueAttr);
                        paramsNode.AppendChild(paramNode);

                        #endregion
                    }

                    HttpContext.Current.Response.Write(
                        string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", _inputs.Keys[i],
                                      _inputs[_inputs.Keys[i]]));
                }

                // koga ce se pusti vo live okolina EnableLogging vo web.config treba da e false
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["EnableLogging"]))
                {
                    doc.Save(strFilename);
                }

                HttpContext.Current.Response.Write("</form>");
                HttpContext.Current.Response.Write("</body></html>");
                HttpContext.Current.Response.End();
            }
        }

        private void GenerateCheckSumHeader(NameValueCollection inputs, Casys casys)
        {
            var headerCounter = 0;
            var headerParams = string.Empty;
            var headerParamsLength = string.Empty;
            var headerParamsValues = string.Empty;

            for (var i = 0; i < inputs.Count; i++)
            {
                if (string.IsNullOrEmpty(inputs.Get(i)))
                {
                    continue;
                }

                headerCounter += 1;
                headerParams += inputs.GetKey(i);
                headerParamsLength += inputs.Get(i).Length.ToString("000");
                headerParamsValues += inputs.Get(i);

                if (i != inputs.Count - 1)
                {
                    headerParams += ",";
                }
            }

            casys.CheckSumHeader = headerCounter.ToString("00") + headerParams + "," + headerParamsLength;
            var checkSumHeaderPass = casys.CheckSumHeader + headerParamsValues + casys.CasysPassword;
            casys.CheckSum = FormsAuthentication.HashPasswordForStoringInConfigFile(checkSumHeaderPass, "MD5");

            _inputs.Add("CheckSumHeader", casys.CheckSumHeader);
            _inputs.Add("CheckSum", casys.CheckSum);
            GenerateTmCheckSum(casys);
        }

        private void GenerateTmCheckSum(Casys casys)
        {
            var headerParamsValues = string.Empty;
            for (var i = 0; i < casys.TMNameValues.Count; i++)
            {
                var nameValue = casys.TMNameValues;
                if (string.IsNullOrEmpty(nameValue.Get(i)))
                {
                    continue;
                }

                headerParamsValues += nameValue.Get(i);
            }

            var tmCheckSumHeader = headerParamsValues;
            casys.TMCheckSum = FormsAuthentication.HashPasswordForStoringInConfigFile(headerParamsValues + casys.CasysPassword, "MD5");
            _inputs.Add("TMCheckSumHeader", tmCheckSumHeader);
            _inputs.Add("TMChecksum", casys.TMCheckSum);
        }

        public NameValueCollection PostData(Casys casys, bool isFormParams)
        {
            if (_inputs != null)
            {
                if (!isFormParams)
                {
                    _inputs.Add("AmountToPay", casys.AmountToPay);
                    _inputs.Add("AmountCurrency", casys.AmountCurrency);
                    _inputs.Add("Details1", casys.Details1);
                    _inputs.Add("Details2", casys.Details2);
                    _inputs.Add("Details3", casys.Details3);
                    _inputs.Add("PayToMerchant", casys.PayToMerchant);
                    _inputs.Add("MerchantName", casys.MerchantName);
                    _inputs.Add("PaymentOKURL", HttpContext.Current.Server.HtmlDecode(casys.PaymentOKURL));
                    _inputs.Add("PaymentFailURL", casys.PaymentFailURL);
                    _inputs.Add("FirstName", casys.FirstName);
                    _inputs.Add("LastName", casys.LastName);
                    _inputs.Add("Address", casys.Address);
                    _inputs.Add("City", casys.City);
                    _inputs.Add("Zip", casys.Zip);
                    _inputs.Add("Country", casys.Country);
                    _inputs.Add("Telephone", casys.Telephone);
                    _inputs.Add("Email", casys.Email);
                }
                else
                {
                    for (var i = 0; i < casys.TMNameValues.Count; i++)
                    {
                        if (string.IsNullOrEmpty(casys.TMNameValues.Get(i)))
                        {
                            continue;
                        }

                        _inputs.Add(casys.TMNameValues.GetKey(i), casys.TMNameValues.Get(i));
                    }
                }
            }

            return _inputs;
        }

        /// <summary>
        /// Metod koj proveruva dali objektot koj se praka vo CaSyS e spored dokumentacija validen
        /// </summary>
        /// <param name="casys"></param>
        /// <returns></returns>
        public bool IsValid(Casys casys)
        {

            {
                if (string.IsNullOrEmpty(casys.CasysUrl)) return false;

                if (string.IsNullOrEmpty(casys.AmountToPay)) return false;
                if (string.IsNullOrEmpty(casys.AmountCurrency)) return false;
                //if (string.IsNullOrEmpty(casys.Details1)) return false;
                if (string.IsNullOrEmpty(casys.Details2)) return false;
                if (string.IsNullOrEmpty(casys.PayToMerchant)) return false;
                if (string.IsNullOrEmpty(casys.MerchantName)) return false;

                if (string.IsNullOrEmpty(casys.PaymentOKURL)) return false;
                if (string.IsNullOrEmpty(casys.PaymentFailURL)) return false;

                return true;
            }
        }
    }
}
