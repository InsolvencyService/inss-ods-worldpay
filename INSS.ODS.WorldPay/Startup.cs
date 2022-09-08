using INSS.ODS.WorldPay.Services;
using INSS.ODS.WorldPay.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(INSS.ODS.WorldPay.Startup))]

namespace INSS.ODS.WorldPay
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IOrderService, OrderService>();
            builder.Services.AddSingleton<IPaymentXmlParserService, PaymentXmlParserService>();

            builder.Services.AddOptions<ExternalAppSettings>()
                .Configure<IConfiguration>((settings, config) =>
                {
                    config.GetSection("ExternalAppSettings").Bind(settings);
                });
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "secrets.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }
    }
}
