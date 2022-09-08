using INSS.ODS.WorldPay.Contants;
using INSS.ODS.WorldPay.Helpers;
using INSS.ODS.WorldPay.Models;
using INSS.ODS.WorldPay.Services;
using INSS.ODS.WorldPay.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;

namespace INSS.ODS.WorldPay.Functions
{
    public class RefundOrder
    {
        private readonly ILogger<CreateOrder> _logger;
        private readonly IPaymentService _paymentService;
        private readonly SiteSitting _settings;

        public RefundOrder(ILogger<CreateOrder> log, IPaymentService paymentService, IOptions<SiteSitting> options)
        {
            _logger = log;
            _paymentService = paymentService;

            if (options == null) throw new ArgumentNullException(nameof(options));

            _settings = options.Value;
        }

        [FunctionName(nameof(RefundOrder))]
        [OpenApiOperation(operationId: "Run", tags: new[] { ApiOperation.WorldPay, ApiOperation.RefundOrder }, Summary = "Refund Order")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: ContentTypes.ApplicationJson, bodyType: typeof(WorldpayResponse), Description = ApiResponseDescription.Ok)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = ApiResponseDescription.BadRequest)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = ApiResponseDescription.InternalServerError)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "worldpay/{installationId}/refund")] HttpRequest req, string installationId)
        {

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var refundRequest = JsonConvert.DeserializeObject<WorldpayRefundRequest>(requestBody);

            _logger.LogInformation($"Creating refund for order code: {refundRequest.OrderCode}");

            try
            {
                var xml = Xml.CreateRefundXml(installationId, refundRequest, _settings.MerchantCode);

                var response = _paymentService.PostOrder(xml);

                if (response.IsSuccessStatusCode)
                {
                    var responseContentXml = await response.Content.ReadAsStringAsync();
                    _logger.LogDebug(responseContentXml);

                    var worldPayResponse = Xml.ParseRefundRequestReplyXml(responseContentXml);

                    if (!worldPayResponse)
                    {
                        _logger.LogError("Worldpay response error");

                        return new BadRequestResult();
                    }

                    return new OkObjectResult(worldPayResponse);
                }

                var errorDetail = response.StatusCode + " - " + response.Content.ReadAsStringAsync();
                var errorResponse = new WorldpayResponse { Error = errorDetail };

                return new OkObjectResult(errorResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

                return new ObjectResult(ex) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}

