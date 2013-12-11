using System.Collections.Specialized;
using System.ComponentModel;

namespace CaSys
{
    public class RequestResponse : Casys
    {
        private string _tmPmtReasonOfDecline = string.Empty;
        private string _tmisIssuedByAcquirer = string.Empty;
        private string _tmReturnCheckSum = string.Empty;
        private string _returnCheckSumHeader = string.Empty;
        private string _returnCheckSum = string.Empty;

        public string TMPmtReasonOfDecline
        {
            get { return _tmPmtReasonOfDecline; }
            set { _tmPmtReasonOfDecline = value; }
        }

        public string TMisIssuedByAcquirer
        {
            get { return _tmisIssuedByAcquirer; }
            set { _tmisIssuedByAcquirer = value; }
        }

        public string TMReturnCheckSum
        {
            get { return _tmReturnCheckSum; }
            set { _tmReturnCheckSum = value; }
        }

        public string ReturnCheckSumHeader
        {
            get { return _returnCheckSumHeader; }
            set { _returnCheckSumHeader = value; }
        }

        public string ReturnCheckSum
        {
            get { return _returnCheckSum; }
            set { _returnCheckSum = value; }
        }

        public NameValueCollection RequestResponseProperties()
        {
            var nameValueCollection = new NameValueCollection();
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(this))
            {
                nameValueCollection.Add(prop.Name, prop.GetValue(this).ToString());
            }
            return nameValueCollection;
        }
    }
}
