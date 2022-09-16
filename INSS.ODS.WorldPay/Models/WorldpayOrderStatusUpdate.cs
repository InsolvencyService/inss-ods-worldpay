using System.Runtime.Serialization;

namespace INSS.ODS.WorldPay.Models
{
    public class WorldpayOrderStatusUpdate
    {
        [DataMember]
        public string OrderCode { get; set; }

        [DataMember]
        public string Status { get; set; }
    }
}