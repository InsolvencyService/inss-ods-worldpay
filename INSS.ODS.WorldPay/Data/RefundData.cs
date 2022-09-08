namespace INSS.ODS.WorldPay.Data
{
    public class RefundData : IWorldpayRequest
    {
        public string OrderCode { get; set; }
        public string RefundValue { get; set; }
        public string Currency { get; set; }
    }
}