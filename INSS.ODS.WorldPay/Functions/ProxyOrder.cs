using INSS.ODS.WorldPay.Contants;
using INSS.ODS.WorldPay.Services;

namespace INSS.ODS.WorldPay.Functions;

public class ProxyOrder
{
    private readonly ILogger<ProxyOrder> _logger;
    private readonly IPaymentService _paymentService;

    public ProxyOrder(ILogger<ProxyOrder> log, IPaymentService paymentService)
    {
        _logger = log;
        _paymentService = paymentService;
    }

    [FunctionName(nameof(ProxyOrder))]
    [OpenApiOperation(operationId: "Run", tags: new[] { ApiOperation.Order, ApiOperation.Proxy, ApiOperation.WorldPay }, Summary = "Order Proxy")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: ContentTypes.XmlUtf8, bodyType: typeof(string), Description = ApiResponseDescription.Ok)]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = ApiResponseDescription.BadRequest)]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "proxy")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        var response = _paymentService.PostOrder(requestBody);

        var content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return new ContentResult() { Content = content };
        }

        return new BadRequestObjectResult(content);
    }
}