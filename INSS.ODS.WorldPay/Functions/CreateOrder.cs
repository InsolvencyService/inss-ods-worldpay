using INSS.ODS.WorldPay.Helpers;
using INSS.ODS.WorldPay.Models;
using INSS.ODS.WorldPay.Services;
using INSS.ODS.WorldPay.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;

namespace INSS.ODS.WorldPay.Functions
{
    public class CreateOrder
    {
        private readonly ILogger<CreateOrder> _logger;
        private readonly IPaymentService _paymentService;
        private readonly SiteSitting _settings;

        public CreateOrder(ILogger<CreateOrder> log, IPaymentService paymentService, IOptions<SiteSitting> options)
        {
            _logger = log;
            _paymentService = paymentService;

            if (options == null) throw new ArgumentNullException(nameof(options));

            _settings = options.Value;
        }

        [FunctionName(nameof(CreateOrder))]
        [OpenApiOperation(operationId: "Run", tags: new[] { ApiOperation.WorldPay, ApiOperation.CreateUpdate }, Summary = "Create Order")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: ContentTypes.ApplicationJson, bodyType: typeof(WorldpayResponse), Description = ApiResponseDescription.Ok)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = ApiResponseDescription.InternalServerError)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "worldpay/{installationId}")] HttpRequest req, string installationId)
        {

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var worldPayOrder = JsonConvert.DeserializeObject<WorldpayOrder>(requestBody);

            _logger.LogInformation($"Creating order for order code: {worldPayOrder.OrderCode}");

            try
            {
                var xml = Xml.CreateOrderXml(installationId, worldPayOrder, _settings.MerchantCode, DateTime.Now);

                var response = _paymentService.PostOrder(xml);

                if (response.IsSuccessStatusCode)
                {
                    var responseContentXml = await response.Content.ReadAsStringAsync();
                    _logger.LogDebug(responseContentXml);

                    var worldPayResponse = Xml.ParseReplyXml(responseContentXml);

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

