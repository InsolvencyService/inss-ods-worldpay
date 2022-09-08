using System.Runtime.Serialization;

namespace INSS.ODS.WorldPay.Models
{
    [DataContract]
    public class WorldpayRefundRequest
    {
        [DataMember]
        public string OrderCode { get; set; }

        [DataMember]
        public decimal RefundValue { get; set; }

        [DataMember]
        public string CurrencyCode { get; set; }
    }
}