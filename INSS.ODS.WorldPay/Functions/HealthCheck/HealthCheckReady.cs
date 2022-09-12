using INSS.ODS.WorldPay.ApiResult;
using INSS.ODS.WorldPay.Models.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Linq;

namespace INSS.ODS.WorldPay.Functions.HealthCheck;

public class HealthCheckReady
{
    private readonly HealthCheckService _healthCheckService;

    public HealthCheckReady(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [FunctionName(nameof(HealthCheckReady))]
    [OpenApiOperation(operationId: "Run", tags: new[] { ApiOperation.HealthCheck, ApiOperation.WorldPay }, Summary = "Get service and dependencies health status", Description = "Get service and dependencies health status")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: ContentTypes.ApplicationJson, bodyType: typeof(HealthCheckResultReady), Description = ApiResponseDescription.Ok)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: ContentTypes.ApplicationJson, bodyType: typeof(HealthCheckResultReady), Description = ApiResponseDescription.InternalServerError)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.ServiceUnavailable, contentType: ContentTypes.ApplicationJson, bodyType: typeof(HealthCheckResultReady), Description = ApiResponseDescription.ServiceNotAvailable)]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest req)
    {
        var report = await _healthCheckService.CheckHealthAsync();

        var result = new HealthCheckResultReady
        {
            ServiceName = "WorldPay Service API",
            ServiceStatus = report.Status.ToString(),
            Duration = report.TotalDuration,
            Details = report.Entries.Select(e => new HealthCheckEntry
            {
                Name = e.Key,
                Description = e.Value.Description,
                Duration = e.Value.Duration,
                Status = Enum.GetName(typeof(HealthStatus), e.Value.Status),
                Error = e.Value.Exception?.Message
            }).ToList()
        };

        return report.Status switch
        {
            HealthStatus.Unhealthy => new ServiceUnavailableObjectResult(result),
            HealthStatus.Degraded => new InternalServerErrorObjectResult(result),
            _ => new OkObjectResult(result),
        };
    }
}