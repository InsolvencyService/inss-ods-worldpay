using INSS.ODS.WorldPay.Helpers;
using INSS.ODS.WorldPay.Models;

namespace INSS.ODS.WorldPay.Tests.Helpers
{
    public class XmlTests
    {
        [Fact]
        public void CreateOrderXml_DecimalStartingWith0()
        {
            #region Test data

            var currentDateTime = DateTime.Now;

            var order = new WorldpayOrder()
            {
                OrderCode = "Order Code",
                Description = "",
                OrderValue = 180.01M,
                Email = "",
                FirstName = "",
                LastName = "",
                IncludeAddress = false,
                Address1 = "",
                Address2 = "",
                Address3 = "",
                PostCode = "",
                City = "",
                State = "",
                CountryCode = "",
                TelephoneNumber = "",
                CurrencyCode = "",
            };

            #endregion

            #region Expected Result

            var expectedResult = @"﻿<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE paymentService PUBLIC "" -//WorldPay//DTD WorldPay PaymentService v1//EN"" ""http://dtd.worldpay.com/paymentService_v1.dtd"">
<paymentService version=""1.4"" merchantCode="""">
  <submit>
    <order orderCode=""Order Code"" installationId=""iniId"">
      <description />
      <amount currencyCode="""" exponent=""2"" value=""18001"" />
      <orderContent><![CDATA[<strong>: £180.01</strong>]]></orderContent>
      <paymentMethodMask>
        <include code=""ALL"" />
      </paymentMethodMask>
      <shopper>
        <shopperEmailAddress />
      </shopper>
      <riskData>
        <authenticationRiskData authenticationMethod=""localAccount"">
          <authenticationTimestamp>
            <date year=""" + currentDateTime.Year + @""" month=""" + currentDateTime.Month + @""" dayOfMonth=""" + currentDateTime.Day + @""" hour=""" + currentDateTime.Hour + @""" minute=""" + currentDateTime.Minute + @""" second=""" + currentDateTime.Second + @""" />
          </authenticationTimestamp>
        </authenticationRiskData>
        <shopperAccountRiskData previousSuspiciousActivity=""false"" shippingNameMatchesAccountName=""true"" />
        <transactionRiskData shippingMethod=""digital"" deliveryTimeframe=""electronicDelivery"" preOrderPurchase=""true"" />
      </riskData>
    </order>
  </submit>
</paymentService>";

            #endregion

            var result = Xml.CreateOrderXml("iniId", order, "", currentDateTime);

            Assert.Equal(expectedResult, result);
        }

    }
}
