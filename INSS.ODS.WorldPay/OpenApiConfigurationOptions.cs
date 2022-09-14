using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using System;

namespace INSS.ODS.WorldPay
{
    public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
    {
        public override OpenApiInfo Info { get; set; } = new()
        {
            Version = "1.0.0",
            Title = "WorldPay API",
            Description = "WorldPay API",
            License = new OpenApiLicense
            {
                Name = "Insolvency Service",
                Url = new Uri("https://www.gov.uk/government/organisations/insolvency-service"),
            }
        };

        public override OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;
    }
}
