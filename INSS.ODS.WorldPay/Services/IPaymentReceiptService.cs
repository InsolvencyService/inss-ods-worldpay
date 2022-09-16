using System.Net.Http;

namespace INSS.ODS.WorldPay.Services
{
    public interface IPaymentReceiptService
    {
        Task<HttpResponseMessage> UpdatePaymentReceipt(string transactionId, string status);
    }
}