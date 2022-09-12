using INSS.ODS.WorldPay.Contants;

namespace INSS.ODS.WorldPay.Functions.HealthCheck;

public class HealthCheckLive
{
    [FunctionName(nameof(HealthCheckLive))]
    [OpenApiOperation(operationId: "Run", tags: new[] { ApiOperation.WorldPay, ApiOperation.HealthCheck }, Summary = "Get service health status", Description = "Get service health status")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: ContentTypes.ApplicationJson, bodyType: typeof(OkResult), Description = ApiResponseDescription.Ok)]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health/ping")] HttpRequest req)
    {
        return await Task.FromResult(new OkResult());
    }
}