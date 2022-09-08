using INSS.ODS.WorldPay.Contants;
using INSS.ODS.WorldPay.Services;

namespace INSS.ODS.WorldPay.Functions;

public class UpdateOrder
{
    private readonly ILogger<UpdateOrder> _logger;
    private readonly IPaymentReceiptService _paymentReceiptService;

    public UpdateOrder(ILogger<UpdateOrder> log, IPaymentReceiptService paymentReceiptService)
    {
        _logger = log;
        _paymentReceiptService = paymentReceiptService;
    }

    [FunctionName(nameof(UpdateOrder))]
    [OpenApiOperation(operationId: "Run", tags: new[] { ApiOperation.WorldPay, ApiOperation.OrderUpdate }, Summary = "Update Order")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: ContentTypes.ApplicationJson, bodyType: typeof(string), Description = ApiResponseDescription.Ok)]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest, Description = ApiResponseDescription.BadRequest)]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orderupdate")] HttpRequest req)
    {

        var xml = await new StreamReader(req.Body).ReadToEndAsync();


        var update = Helpers.Xml.ParseOrderUpdate(xml, _logger);

        _logger.LogInformation($"Received Order Status Update for Order: {update.OrderCode} to status: {update.Status}");

        //parse this xml into an object and update payment receipt accordingly
        var result = await _paymentReceiptService.UpdatePaymentReceipt(update.OrderCode, update.Status);

        if (result.IsSuccessStatusCode)
        {
            _logger.LogInformation($"Success updating payment receipt for {update.OrderCode}");
            return new ContentResult { Content = "[OK]" };
        }

        var errorString = await result.Content.ReadAsStringAsync();
        _logger.LogError($"Error updating payment receipt for {update.OrderCode}: {result.StatusCode} {errorString}");
        return new BadRequestResult();

    }
}