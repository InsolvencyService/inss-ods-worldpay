using NUnit.Framework;


namespace INSS.ODS.WorldpayService.AcceptanceTest
{
    public static class EnvironmentData
    {
        public static string BaseUrl { get; } = TestContext.Parameters["BaseUrl"];
        public static string APIbaseUrl { get; } = TestContext.Parameters["APIbaseUrl"];
    }
}
