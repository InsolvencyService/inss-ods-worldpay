using INSS.ODS.WorldPay.Contants;
using INSS.ODS.WorldPay.Data;
using INSS.ODS.WorldPay.Extensions;
using INSS.ODS.WorldPay.Services;
using System.Net.Http;

namespace INSS.ODS.WorldPay.Functions
{
    public class MakePaymentFunc
    {
        private readonly ILogger<MakePaymentFunc> _logger;
        private readonly IPaymentXmlParserService _paymentXmlParserService;
        private readonly IOrderService _orderService;

        public MakePaymentFunc(ILogger<MakePaymentFunc> log, IPaymentXmlParserService paymentXmlParserService, IOrderService orderService)
        {
            _logger = log;
            _paymentXmlParserService = paymentXmlParserService;
            _orderService = orderService;

        }

        [FunctionName(nameof(MakePaymentFunc))]
        [OpenApiOperation(operationId: "Run", tags: new[] { ApiOperation.Payment }, Summary = "Make payment,Refund payment or Cancel Payment")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(HttpResponseMessage), Description = ApiResponseDescription.Ok)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = ApiResponseDescription.BadRequest)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "PaymentService")]
            HttpRequest req)
        {

            var xml = await new StreamReader(req.Body).ReadToEndAsync();

            var parsedData = _paymentXmlParserService.ParseResult(xml);

            switch (parsedData)
            {
                case null:
                    return new BadRequestObjectResult("Error parsing request");
                case OrderData data:
                    {
                        var xmlString = _orderService.PostOrder(data);
                        return xmlString.ToUtf8ContentResult();
                    }
                case RefundData refundData:
                    {
                        var xmlString = _orderService.PostRefundData(refundData);
                        return xmlString.ToUtf8ContentResult();
                    }
                case CancelData cancelData:
                    {
                        var xmlString = _orderService.PostCancelData(cancelData);
                        return xmlString.ToUtf8ContentResult();
                    }
                default:
                    return new BadRequestResult();
            }
        }
    }
}

