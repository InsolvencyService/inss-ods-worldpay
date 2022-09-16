using INSS.ODS.WorldPay.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace INSS.ODS.WorldPay.Services
{
    public class PaymentReceiptService : IPaymentReceiptService
    {
        private readonly ExternalAppSettings _settings;

        public PaymentReceiptService(IOptions<ExternalAppSettings> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _settings = options.Value;
        }
        private HttpClient GetHttpClient()
        {
            var client = new HttpClient { BaseAddress = new Uri(_settings.PaymentReceiptServiceUrl) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public async Task<HttpResponseMessage> UpdatePaymentReceipt(string transactionId, string status)
        {
            using var client = GetHttpClient();

            var uri = $"paymentreceipt/{transactionId}/updatestatus/{status}";
            var result = await client.PostAsync(uri, new StringContent(""));
            return result;
        }
    }
}