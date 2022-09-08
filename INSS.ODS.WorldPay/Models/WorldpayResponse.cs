using System.Runtime.Serialization;

namespace INSS.ODS.WorldPay.Models
{
    [DataContract]
    public class WorldpayResponse
    {
        [DataMember]
        public string OrderCode { get; set; }
        [DataMember]
        public string RedirectUrl { get; set; }
        [DataMember]
        public string Error { get; set; }
    }
}