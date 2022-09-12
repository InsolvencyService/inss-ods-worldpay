using System;
using System.Collections.Generic;

namespace INSS.ODS.WorldPay.Models.HealthChecks;

public class HealthCheckResultReady
{
    public string ServiceName { get; set; }
    public string ServiceStatus { get; set; }
    public TimeSpan Duration { get; set; }
    public ICollection<HealthCheckEntry> Details { get; set; }
}