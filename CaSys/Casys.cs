// --------------------------------------------------------------------------------------------------------------------- 
// <copyright file="Casys.cs" company="Nextsense">
//   © 2010 Nextsense
// </copyright>
// <summary>
//   Defines the Casys type.
// </summary>
// ---------------------------------------------------------------------------------------------------------------------

using System.Collections.Specialized;

namespace CaSys
{
    public class Casys
    {
        private string _amountToPay = string.Empty;
        private string _amountCurrency = "MKD";
        private string _details1 = string.Empty;
        private string _details2 = string.Empty;
        private string _details3 = string.Empty;
        private string _payToMerchant = string.Empty;
        private string _merchantName = string.Empty;
        private string _paymentOkurl = string.Empty;
        private string _paymentFailUrl = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private string _address = string.Empty;
        private string _city = string.Empty;
        private string _zip = string.Empty;
        private string _country = string.Empty;
        private string _telephone = string.Empty;
        private string _email = string.Empty;
        private string _checkSumHeader = string.Empty;
        private string _checkSum = string.Empty;
        private string _casysPassword = string.Empty;
        private string _casysUrl = string.Empty;
        private string _submitControl = string.Empty;
        private string _cPayPaymentRef = string.Empty;


        public string AmountToPay
        {
            get { return _amountToPay; }
            set { _amountToPay = value; }
        }

        public string AmountCurrency
        {
            get { return _amountCurrency; }
            set { _amountCurrency = value; }
        }

        public string Details1
        {
            get { return _details1; }
            set { _details1 = value; }
        }

        public string Details2
        {
            get { return _details2; }
            set { _details2 = value; }
        }

        public string Details3
        {
            get { return _details3; }
            set { _details3 = value; }
        }

        public string PayToMerchant
        {
            get { return _payToMerchant; }
            set { _payToMerchant = value; }
        }

        public string MerchantName
        {
            get { return _merchantName; }
            set { _merchantName = value; }
        }

        public string PaymentOKURL
        {
            get { return _paymentOkurl; }
            set { _paymentOkurl = value; }
        }

        public string PaymentFailURL
        {
            get { return _paymentFailUrl; }
            set { _paymentFailUrl = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        public string Zip
        {
            get { return _zip; }
            set { _zip = value; }
        }

        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        public string Telephone
        {
            get { return _telephone; }
            set { _telephone = value; }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        public string CheckSumHeader
        {
            get { return _checkSumHeader; }
            set { _checkSumHeader = value; }
        }

        public string CheckSum
        {
            get { return _checkSum; }
            set { _checkSum = value; }
        }

        public string CasysPassword
        {
            get { return _casysPassword; }
            set { _casysPassword = value; }
        }

        public string SubmitControl
        {
            get { return _submitControl; }
            set { _submitControl = value; }
        }

        public string CasysUrl
        {
            get { return _casysUrl; }
            set { _casysUrl = value; }
        }
        public string cPayPaymentRef
        {
            get { return _cPayPaymentRef; }
            set { _cPayPaymentRef = value; }
        }
    }
}
