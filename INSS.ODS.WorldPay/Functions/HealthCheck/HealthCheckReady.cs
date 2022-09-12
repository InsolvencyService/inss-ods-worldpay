using INSS.ODS.WorldPay.Contants;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace INSS.ODS.WorldPay.Functions.HealthCheck;

public class HealthCheckReady
{
    private readonly ILogger<HealthCheckLive> _logger;
    private readonly HealthCheckService _healthCheckService;

    public HealthCheckReady(ILogger<HealthCheckLive> log, HealthCheckService healthCheckService)
    {
        _logger = log;
        _healthCheckService = healthCheckService;
    }

    [FunctionName(nameof(HealthCheckReady))]
    [OpenApiOperation(operationId: "Run", tags: new[] { ApiOperation.HealthCheck, ApiOperation.WorldPay }, Summary = "Get service and dependencies health status", Description = "Get service and dependencies health status")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: ContentTypes.ApplicationJson, bodyType: typeof(ObjectResult), Description = ApiResponseDescription.Ok)]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest req)
    {
        var report = await _healthCheckService.CheckHealthAsync();

        return report.Status switch
        {
            HealthStatus.Unhealthy => new ObjectResult(report) { StatusCode = StatusCodes.Status503ServiceUnavailable },
            HealthStatus.Degraded => new ObjectResult(report) { StatusCode = StatusCodes.Status500InternalServerError },
            _ => new OkObjectResult(report),
        };
    }
}