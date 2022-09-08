namespace INSS.ODS.WorldPay.Extensions
{
    public static class StringExtensions
    {
        public static ContentResult ToUtf8ContentResult(this string value)
        {
            return new ContentResult { Content = value, ContentType = "text/xml; charset=utf-8" };
        }
    }
}
