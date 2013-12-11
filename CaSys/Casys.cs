namespace CaSys
{
    public class Casys
    {
        public Casys()
        {
            CPayPaymentRef = string.Empty;
            CasysUrl = string.Empty;
            SubmitControl = string.Empty;
            CasysPassword = string.Empty;
            CheckSum = string.Empty;
            CheckSumHeader = string.Empty;
            Email = string.Empty;
            Telephone = string.Empty;
            Country = string.Empty;
            Zip = string.Empty;
            City = string.Empty;
            Address = string.Empty;
            LastName = string.Empty;
            FirstName = string.Empty;
            PaymentFailURL = string.Empty;
            PaymentOKURL = string.Empty;
            MerchantName = string.Empty;
            PayToMerchant = string.Empty;
            Details3 = string.Empty;
            Details2 = string.Empty;
            Details1 = string.Empty;
            AmountCurrency = "MKD";
            AmountToPay = string.Empty;
        }

        public string AmountToPay { get; set; }
        public string AmountCurrency { get; set; }
        public string Details1 { get; set; }
        public string Details2 { get; set; }
        public string Details3 { get; set; }
        public string PayToMerchant { get; set; }
        public string MerchantName { get; set; }
        public string PaymentOKURL { get; set; }
        public string PaymentFailURL { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string CheckSumHeader { get; set; }
        public string CheckSum { get; set; }
        public string CasysPassword { get; set; }
        public string SubmitControl { get; set; }
        public string CasysUrl { get; set; }
        public string CPayPaymentRef { get; set; }
    }
}
