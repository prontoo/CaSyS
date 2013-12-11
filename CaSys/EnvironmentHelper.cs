// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnvironmentHelper.cs" company="Nextsense">
//   © 2010 Nextsense
// </copyright>
// <summary>
//   Defines the EnvironmentHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Configuration;
using System.Web;

namespace CaSys
{
    public static class EnvironmentHelper
    {
        public static string PaymentOkurl
        {
            get;
            set;
        }

        public static string PaymentFailurl
        {
            get;
            set;
        }

        public static string PaymentOkurlInvoices
        {
            get;
            set;
        }

        public static string PaymentFailurlInvoices
        {
            get;
            set;
        }

        public static string CasysPassword
        {
            get;
            set;
        }

        public static string CasysPasswordInvoices
        {
            get;
            set;
        }

        public static string CasysUrlLang
        {
            get;
            set;
        }

        public static int BankId
        {
            get;
            set;
        }

        public static string DealerId
        {
            get;
            set;
        }

        public static string Language
        {
            get;
            set;
        }

        public static bool EnableLogging
        {
            get;
            set;
        }

        public static bool EnableShopping
        {
            get;
            set;
        }

        public static string ReturnCasysUrl(string lang)
        {
            if (IsTestEnvironment())
            {
                switch (lang.ToUpper())
                {
                    case "MK":
                        CasysUrlLang = ConfigurationManager.AppSettings["TestCasysURL"];
                        break;
                    case "EN":
                        CasysUrlLang = ConfigurationManager.AppSettings["TestCasysURL_EN"];
                        break;
                    case "AL":
                        CasysUrlLang = ConfigurationManager.AppSettings["TestCasysURL_AL"];
                        break;
                }
            }
            else
            {
                switch (lang.ToUpper())
                {
                    case "MK":
                        CasysUrlLang = ConfigurationManager.AppSettings["CasysURL"];
                        break;
                    case "EN":
                        CasysUrlLang = ConfigurationManager.AppSettings["CasysURL_EN"];
                        break;
                    case "AL":
                        CasysUrlLang = ConfigurationManager.AppSettings["CasysURL_AL"];
                        break;
                }
            }

            return CasysUrlLang;
        }

        static EnvironmentHelper()
        {
            try
            {
                var language = string.Empty;
                if (HttpContext.Current.Session["lang"] != null)
                {
                    language = HttpContext.Current.Session["lang"].ToString().ToUpper();
                }

                if (IsTestEnvironment())
                {
                    switch (language.ToUpper())
                    {
                        case "MK":
                            CasysUrlLang = ConfigurationManager.AppSettings["TestCasysURL"];
                            break;
                        case "EN":
                            CasysUrlLang = ConfigurationManager.AppSettings["TestCasysURL_EN"];
                            break;
                        case "AL":
                            CasysUrlLang = ConfigurationManager.AppSettings["TestCasysURL_AL"];
                            break;
                        default:
                            CasysUrlLang = ConfigurationManager.AppSettings["TestCasysURL"];
                            break;
                    }

                    PaymentOkurlInvoices = ConfigurationManager.AppSettings["TestCaSysOkUrl"];
                    PaymentFailurlInvoices = ConfigurationManager.AppSettings["TestCaSysFailUrl"];
                    CasysPasswordInvoices = ConfigurationManager.AppSettings["CasysPassword"];
                }
                else
                {
                    PaymentOkurlInvoices = ConfigurationManager.AppSettings["CaSysOkUrl"];
                    PaymentFailurlInvoices = ConfigurationManager.AppSettings["CaSysFailUrl"];
                    switch (language.ToUpper())
                    {
                        case "MK":
                            CasysUrlLang = ConfigurationManager.AppSettings["CasysURL"];
                            break;
                        case "EN":
                            CasysUrlLang = ConfigurationManager.AppSettings["CasysURL_EN"];
                            break;
                        case "AL":
                            CasysUrlLang = ConfigurationManager.AppSettings["CasysURL_AL"];
                            break;
                    }

                    CasysPasswordInvoices = ConfigurationManager.AppSettings["CasysPassword"];
                }

                BankId = Convert.ToInt32(ConfigurationManager.AppSettings["BankID"]);

                DealerId = ConfigurationManager.AppSettings["DealerID"];
                EnableShopping = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableShopping"]);
                Language = language;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

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