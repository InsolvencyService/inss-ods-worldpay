using INSS.ODS.WorldPay.Data;

namespace INSS.ODS.WorldPay.Services
{
    public interface IPaymentXmlParserService
    {
        IWorldpayRequest ParseResult(string xml);
    }
}
