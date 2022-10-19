using System.Net.Http;

namespace INSS.ODS.WorldPay.Services
{
    public interface IPaymentService
    {
        HttpResponseMessage PostOrder(string payload);
    }
}
