using System;

namespace INSS.ODS.WorldPay.Models.HealthChecks;

public class HealthCheckResultLive
{
    public string OverallStatus { get; set; }
    public TimeSpan TotalChecksDuration { get; set; }
}
