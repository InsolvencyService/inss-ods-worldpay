using System;

namespace INSS.ODS.WorldPay.Models.HealthChecks;

public class HealthCheckEntry
{
    public string Name { get; set; }
    public string Status { get; set; }
    public TimeSpan Duration { get; set; }
    public string Description { get; set; }
    public string Error { get; set; }
}
