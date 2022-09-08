using INSS.ODS.WorldPay.Contants;
using INSS.ODS.WorldPay.Helpers;
using INSS.ODS.WorldPay.Models;
using INSS.ODS.WorldPay.Services;
using INSS.ODS.WorldPay.Settings;
using Microsoft.Extensions.Options;
using System;

namespace INSS.ODS.WorldPay.Functions
{
    public class CancelOrder
    {
        private readonly ILogger<CreateOrder> _logger;
        private readonly IPaymentService _paymentService;
        private readonly SiteSitting _settings;

        public CancelOrder(ILogger<CreateOrder> log, IPaymentService paymentService, IOptions<SiteSitting> options)
        {
            _logger = log;
            _paymentService = paymentService;

            if (options == null) throw new ArgumentNullException(nameof(options));

            _settings = options.Value;
        }

        [FunctionName(nameof(CancelOrder))]
        [OpenApiOperation(operationId: "Run", tags: new[] { ApiOperation.WorldPay, ApiOperation.CancelOrder }, Summary = "Cancel Order")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: ContentTypes.ApplicationJson, bodyType: typeof(WorldpayResponse), Description = ApiResponseDescription.Ok)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = ApiResponseDescription.BadRequest)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = ApiResponseDescription.InternalServerError)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "worldpay/{installationId}/cancel/{orderCode}")]
            HttpRequest req, string installationId, string orderCode)
        {
            _logger.LogInformation($"Creating cancellation for order code: {orderCode}");

            try
            {
                var xml = Xml.CreateCancelOrRefundXml(installationId, orderCode, _settings.MerchantCode);

                var response = _paymentService.PostOrder(xml);

                if (response.IsSuccessStatusCode)
                {
                    var responseContentXml = await response.Content.ReadAsStringAsync();
                    _logger.LogDebug(responseContentXml);

                    var worldPayResponse = Xml.ParseCancelOrRefundRequestReplyXml(responseContentXml);

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

