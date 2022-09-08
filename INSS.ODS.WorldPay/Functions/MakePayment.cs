using INSS.ODS.WorldPay.Contants;
using INSS.ODS.WorldPay.Data;
using INSS.ODS.WorldPay.Services;

namespace INSS.ODS.WorldPay.Functions;

public class MakePayment
{
    private readonly IPaymentXmlParserService _paymentXmlParserService;
    private readonly IOrderService _orderService;

    public MakePayment(IPaymentXmlParserService paymentXmlParserService, IOrderService orderService)
    {
        _paymentXmlParserService = paymentXmlParserService;
        _orderService = orderService;

    }

    [FunctionName(nameof(MakePayment))]
    [OpenApiOperation(operationId: "Run", tags: new[] { ApiOperation.Payment, ApiOperation.WorldPay }, Summary = "Make Payment")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: ContentTypes.XmlUtf8, bodyType: typeof(string), Description = ApiResponseDescription.Ok)]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = ApiResponseDescription.BadRequest)]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PaymentService")]
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
                    return new ContentResult { Content = xmlString };
                }
            case RefundData refundData:
                {
                    var xmlString = _orderService.PostRefundData(refundData);
                    return new ContentResult { Content = xmlString };
                }
            case CancelData cancelData:
                {
                    var xmlString = _orderService.PostCancelData(cancelData);
                    return new ContentResult { Content = xmlString };
                }
            default:
                return new BadRequestResult();
        }
    }
}