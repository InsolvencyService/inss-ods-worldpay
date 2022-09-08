namespace INSS.ODS.WorldPay.Data
{
    public class CancelData : IWorldpayRequest
    {
        public string OrderCode { get; set; }
    }
}