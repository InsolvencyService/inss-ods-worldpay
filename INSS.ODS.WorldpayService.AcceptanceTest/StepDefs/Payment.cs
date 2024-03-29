using System;
using TechTalk.SpecFlow;
using TestFramework.Hooks;
using INSS.ODS.WorldpayService.AcceptanceTest.Models;
using NUnit.Framework;
using Newtonsoft.Json;
using INSS.ODS.WorldpayService.AcceptanceTest;
using System.IO;
using System.Xml;
using System.Threading;
using RestSharp.Serializers;
using System.Xml.Linq;
using RestSharp;

namespace DROTestAutomation.StepDefs
{
    [Binding]
    public class WorldpayStepDefinitions
    {
        Context _context;
        worldpay _WorldpayModel;
        public WorldpayStepDefinitions(Context context, worldpay WorldpayModel)
        {
            _context = context;
            _WorldpayModel = WorldpayModel;
        }


        [Given(@"the worldpay service is running with resource (.*)")]
        public void GivenTheWorldpayServiceIsRunningWithResourceWorldpayInstallationidRefund(string resources)
        {

        }

        [When(@"a post request is made to initiate payment using '([^']*)' '([^']*)' '([^']*)' '([^']*)' '([^']*)' '([^']*)' '([^']*)' '([^']*)' '([^']*)' '([^']*)' '([^']*)'")]
        public void WhenAPostRequestIsMadeToInitiatePaymentUsingOrderCodeDescriptionOrderValueEmailFirstNameLastNameIncludeAddressAddressPostCodeCityTelephoneNumber(string OrderCode, string Description, string OrderValue, string Email, string FirstName, string LastName, string IncludeAddress, string Address1, string PostCode, string City, string TelephoneNumber)
        {
            _WorldpayModel.OrderCode = OrderCode;
            _WorldpayModel.Description = Description;
            _WorldpayModel.OrderValue = OrderValue;
            _WorldpayModel.Email = Email;
            _WorldpayModel.FirstName = FirstName;
            _WorldpayModel.LastName = LastName;
            _WorldpayModel.IncludeAddress = IncludeAddress;
            _WorldpayModel.Address1 = Address1;
            _WorldpayModel.Address2 = "test add2";
            _WorldpayModel.Address3 = "test add2";
            _WorldpayModel.PostCode = PostCode;
            _WorldpayModel.City = City;
            _WorldpayModel.State = "leeds";
            _WorldpayModel.CountryCode = "GB";
            _WorldpayModel.TelephoneNumber = TelephoneNumber;
            _WorldpayModel.CurrencyCode = "GBP";
            string jsonbody = JsonConvert.SerializeObject(_WorldpayModel);
            Console.WriteLine(jsonbody);
            _context.CallPostEndpointswithresources(DataAppSetting.InitConfiguration()["BaseUrl"], DataAppSetting.InitConfiguration()["WorldPayment"], jsonbody);

        }

        [Then(@"the response status code is (.*)")]
        public void ThenTheResponseStatusCodeIs(string expectedstatuscode)
        {
            Assert.That(_context.statusCode, Is.EqualTo(expectedstatuscode), "invalid Code");
        }

        [Then(@"the content of the url is validated")]
        public void ThenTheContentOfTheUrlIsValidated()
        {
            var result = JsonConvert.DeserializeObject<ResponseModel>(_context.content);
            Assert.That(result.RedirectUrl, Does.Contain("https://app-uksouth-dev-ods-mock-worldpay-api.azurewebsites.net/selectpaymentmethod?orderKey=INSSDRO^abck"), " test failed");
        }

        [When(@"a post request is made to refund payment using '([^']*)' '([^']*)'")]
        public void WhenAPostRequestIsMadeToRefundPaymentUsing(string orderCode, string amount)
        {
           
            _WorldpayModel.OrderCode = orderCode;
            _WorldpayModel.CurrencyCode = "GBP";
            _WorldpayModel.RefundValue = amount;
            string jsonbody = JsonConvert.SerializeObject(_WorldpayModel);
            _context.CallPostEndpointswithresources(DataAppSetting.InitConfiguration()["BaseUrl"], DataAppSetting.InitConfiguration()["RefundPayment"], jsonbody);

        }

        [When(@"a post request is made to cancel payment")]
        public void WhenAPostRequestIsMadeToCancelPayment()
        {
           
            string jsonbody = JsonConvert.SerializeObject(_WorldpayModel);
            _context.CallPostEndpointswithresources(DataAppSetting.InitConfiguration()["BaseUrl"], DataAppSetting.InitConfiguration()["WorldPay"], jsonbody);
        }

        [When(@"a get request for payment health check is sent with (.*)")]
        public void WhenAGetRequestForPaymentHealthCheckIsSentWithTest(string resources)
        {
            _context.GetMethod(DataAppSetting.InitConfiguration()["BaseUrl"], resources);

        }

        [Then(@"the (.*) response content is validated")]
        public void ThenTheRefundResponseContentIsValidated(string endpontType)
        {
            var result = JsonConvert.DeserializeObject(_context.content);
            Assert.That(result.ToString, Is.EqualTo("True"), "invalid Code");
        }

        [When(@"a post request is made to proxy payment")]
        public void WhenAPostRequestIsMadeToProxyPayment()
        {
            string path = Path.GetFullPath("Proxy.xml");
            XDocument doc = XDocument.Load(path);

            _context.CallXmlPostProxyEndpoint(DataAppSetting.InitConfiguration()["BaseUrl"], DataAppSetting.InitConfiguration()["OrderProxy"], doc);
        }

        [When(@"a post request is made to update order")]
        public void WhenAPostRequestIsMadeToUpdateOrder()
        {
            string path = Path.GetFullPath("UpdateOrder.xml");
            XDocument doc = XDocument.Load(path);

            _context.CallXmlPostProxyEndpoint(DataAppSetting.InitConfiguration()["BaseUrl"], DataAppSetting.InitConfiguration()["OrderUpdate"], doc);
        }

        [When(@"a post request is made to make payment")]
        public void WhenAPostRequestIsMadeToMakePayment()
        {
            string path = Path.GetFullPath("MakePayment.xml");
            XDocument doc = XDocument.Load(path);
            _context.CallXmlPostProxyEndpoint(DataAppSetting.InitConfiguration()["BaseUrl"], DataAppSetting.InitConfiguration()["PaymentService"], doc);
        }

    }
}




