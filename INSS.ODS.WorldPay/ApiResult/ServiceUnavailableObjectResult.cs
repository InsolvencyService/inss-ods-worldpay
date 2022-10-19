namespace INSS.ODS.WorldPay.ApiResult;

public class ServiceUnavailableObjectResult : ObjectResult
{
    public ServiceUnavailableObjectResult(object value) : base(value)
    {
        StatusCode = StatusCodes.Status503ServiceUnavailable;
    }
}

