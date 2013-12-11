using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Xml;

namespace CaSys
{
    public class Utilities
    {
        private static ResourceManager _rm;
        private static ResourceManager _rmError;

        public string LanguageSetAndReturn(string lang)
        {
            try
            {
                switch (lang.ToUpper().Trim())
                {
                    case "MK":
                        HttpContext.Current.Session["language"] = lang;
                        break;
                    case "EN":
                        HttpContext.Current.Session["language"] = lang;
                        break;
                    case "AL":
                        HttpContext.Current.Session["language"] = lang;
                        break;
                    default:
                        lang = (HttpContext.Current.Session["language"] ?? string.Empty).ToString();
                        if (string.IsNullOrEmpty(lang))
                        {
                            HttpContext.Current.Session["language"] = "MK";
                            lang = "MK";
                        }

                        break;
                }

                return lang;
            }
            catch (Exception ex)
            {
                return "MK";
            }
        }

        public static string GetResourceString(string key)
        {
            if (_rm == null)
            {
                _rm = new ResourceManager("Resources.Default", Assembly.Load("App_GlobalResources"));
            }

            try
            {
                return _rm.GetString(key);
            }
            catch (Exception ex)
            {
                // Utils.LogError(ex);
                return key;
            }
        }

        public static string GetErrorResourceString(string key)
        {
            if (_rmError == null)
            {
                _rmError = new ResourceManager("Resources.ErrorsCode", Assembly.Load("App_GlobalResources"));
            }

            try
            {
                return _rmError.GetString(key);
            }
            catch (Exception ex)
            {
                return key;
            }
        }

        #region MD5 Validation

        /// <summary>
        /// Za dva objekti CaSys go spoerduva nivniot MD5 checkSum
        /// </summary>
        /// <param name="md5Send">Prviot Casys objekt</param>
        /// <param name="receive">return CheckSumHeader</param>
        /// <returns>Vraka true dokolku se isti checkSum-te</returns>
        public static bool IsValidMd5(Casys md5Send, string receive)
        {
            var send = GenerateCheckSumHeader(md5Send);
            //return true;
            return send == receive;
        }

        /// <summary>
        /// Za dva objekti CaSys go spoerduva nivniot MD5 checkSum
        /// </summary>
        /// <param name="md5Send">Prviot Casys objekt</param>
        /// <param name="receive">return CheckSumHeader</param>
        /// <returns>Vraka true dokolku se isti checkSum-te</returns>
        public static bool IsValidMd5Invoices(Casys md5Send, string receive)
        {
            var send = GenerateCheckSumHeader(md5Send);
            //return true;
            return send == receive;
        }

        /// <summary>
        /// Generira checkSum za objekt
        /// </summary>
        /// <param name="casys">Objek za koj se generira checkSum</param>
        /// <returns>checkSum string</returns>
        private static string GenerateCheckSumHeader(Casys casys)
        {
            var inputs = PostData(casys);
            var headerCounter = 0;
            var headerParams = string.Empty;
            var headerParamsLength = string.Empty;
            var headerParamsValues = string.Empty;

            // TODO: na live okolina ovoj region treba da se izbrise
            #region [Kreira XML kade sto gi zapisuva return parametrite - koga ce se pusti vo live okolina ovoj xml treba da se izbrise ]
            const string strFilename = "C:\\Temp\\parmsForCheck.xml";
            if (File.Exists(strFilename))
            {
                File.Delete(strFilename);
            }

            var xdoc = new XmlDocument();
            XmlNode docNode = xdoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xdoc.AppendChild(docNode);
            XmlNode paramsNode = xdoc.CreateElement("Parameters");
            xdoc.AppendChild(paramsNode);
            #endregion

            for (var i = 0; i < inputs.Count; i++)
            {
                if (!string.IsNullOrEmpty(inputs.Get(i)))
                {
                    // TODO: na live okolina ovoj region treba da se izbrise
                    #region [Kreira XML kade sto gi zapisuva return parametrite - koga ce se pusti vo live okolina ovoj xml treba da se izbrise ]
                    XmlNode paramNode = xdoc.CreateElement("parameter");
                    var nameAttr = xdoc.CreateAttribute("name");
                    nameAttr.Value = inputs.GetKey(i);
                    if (paramNode.Attributes != null)
                    {
                        paramNode.Attributes.Append(nameAttr);
                    }

                    var valueAttr = xdoc.CreateAttribute("value");
                    valueAttr.Value = inputs.Get(i);
                    if (paramNode.Attributes != null)
                    {
                        paramNode.Attributes.Append(valueAttr);
                    }

                    paramsNode.AppendChild(paramNode);
                    #endregion

                    headerCounter += 1;
                    headerParams += inputs.GetKey(i);
                    headerParamsLength += inputs.Get(i).Length.ToString("000");
                    headerParamsValues += inputs.Get(i);

                    if (i != inputs.Count - 1)
                    {
                        headerParams += ",";
                    }
                }
            }

            casys.CheckSumHeader = headerCounter.ToString("00") + headerParams + "," + headerParamsLength;
            var checkSumHeaderPass = casys.CheckSumHeader + headerParamsValues + casys.CasysPassword;

            // TODO: na live okolina ovoj region treba da se izbrise
            #region [Kreira XML kade sto gi zapisuva return parametrite - koga ce se pusti vo live okolina ovoj xml treba da se izbrise ]
            XmlNode paramNodeE = xdoc.CreateElement("parameter");
            var nameAttrN = xdoc.CreateAttribute("name");
            nameAttrN.Value = "CheckSumHeader";
            if (paramNodeE.Attributes != null)
            {
                paramNodeE.Attributes.Append(nameAttrN);
            }

            var valueAttrV = xdoc.CreateAttribute("value");
            valueAttrV.Value = casys.CheckSumHeader;
            if (paramNodeE.Attributes != null)
            {
                paramNodeE.Attributes.Append(valueAttrV);
            }

            paramsNode.AppendChild(paramNodeE);
            #endregion

            // TODO: na live okolina ovoj region treba da se izbrise
            #region [Kreira XML kade sto gi zapisuva return parametrite - koga ce se pusti vo live okolina ovoj xml treba da se izbrise ]
            XmlNode paramNodeE1 = xdoc.CreateElement("parameter");
            var nameAttrN1 = xdoc.CreateAttribute("name");
            nameAttrN1.Value = "CheckSumHeaderPass";
            if (paramNodeE1.Attributes != null)
            {
                paramNodeE1.Attributes.Append(nameAttrN1);
            }

            var valueAttrV1 = xdoc.CreateAttribute("value");
            valueAttrV1.Value = checkSumHeaderPass;
            if (paramNodeE1.Attributes != null)
            {
                paramNodeE1.Attributes.Append(valueAttrV1);
            }

            paramsNode.AppendChild(paramNodeE1);
            #endregion

            var checkSumHeader = FormsAuthentication.HashPasswordForStoringInConfigFile(checkSumHeaderPass, "MD5");

            // TODO: na live okolina ovoj region treba da se izbrise
            #region [Kreira XML kade sto gi zapisuva checkSumHeader - koga ce se pusti vo live okolina ovoj xml treba da se izbrise ]
            XmlNode paramNodeE2 = xdoc.CreateElement("parameter");
            var nameAttrN2 = xdoc.CreateAttribute("name");
            nameAttrN2.Value = "checkSumHeader";
            if (paramNodeE2.Attributes != null)
            {
                paramNodeE2.Attributes.Append(nameAttrN2);
            }

            var valueAttrV2 = xdoc.CreateAttribute("value");
            valueAttrV2.Value = checkSumHeader;
            if (paramNodeE2.Attributes != null)
            {
                paramNodeE2.Attributes.Append(valueAttrV2);
            }

            paramsNode.AppendChild(paramNodeE2);
            #endregion

            // TODO: na live okolina ovoj red treba da se izbrise
            xdoc.Save(strFilename);
            return checkSumHeader;
        }

        /// <summary>
        /// Pomosna klasa za MD5 redenje na polinja
        /// </summary>
        /// <param name="casys">Cassys objektot koj se redi</param>
        /// <returns>Sreden Casys objekt za MD5 redenje</returns>
        private static NameValueCollection PostData(Casys casys)
        {
            var valueWithDdv = casys.AmountToPay;
            if (casys.AmountToPay.EndsWith(".0") || casys.AmountToPay.EndsWith(",0"))
            {
                valueWithDdv = valueWithDdv.Substring(0, valueWithDdv.Length - 2);
            }

            var inputs = new NameValueCollection
             {
                 {"AmountCurrency", casys.AmountCurrency},
                 {"AmountToPay", valueWithDdv},
                 {"Details1", casys.Details1},
                 {"Details2", casys.Details2},
                 {"Details3", casys.Details3},
                 {"PayToMerchant", casys.PayToMerchant},
                 {"MerchantName", casys.MerchantName},
                 {"PaymentOKURL", casys.PaymentOKURL},
                 {"PaymentFailURL", casys.PaymentFailURL},
                 {"FirstName", casys.FirstName},
                 {"LastName", casys.LastName},
                 {"Address", casys.Address},
                 {"City", casys.City},
                 {"Zip", casys.Zip},
                 {"Country", casys.Country},
                 {"Telephone", casys.Telephone},
                 {"Email", casys.Email},
                 {"cPayPaymentRef", casys.CPayPaymentRef}
             };
            return inputs;
        }

        #endregion

        /// <summary>
        /// Dokolku e true celiot proekt e vo test mode vklucuvajki go i CaSyS
        /// </summary>
        /// <returns>vraka true ako e Test okolina</returns>
        public static bool IsTestEnvironment()
        {
            var isTest = Convert.ToBoolean(ConfigurationManager.AppSettings["TestEnvironment"]);
            return isTest;
        }
    }
}