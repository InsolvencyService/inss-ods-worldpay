using INSS.ODS.WorldPay.Data;
using INSS.ODS.WorldPay.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Xml;

namespace INSS.ODS.WorldPay.Services
{
    public class OrderService : IOrderService
    {
        private readonly ExternalAppSettings _settings;

        public OrderService(IOptions<ExternalAppSettings> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            _settings = options.Value;
        }

        public string PostOrder(OrderData orderData)
        {
            var orderKey = $"{orderData.MerchantCode}^{orderData.OrderCode}";


            var redirectUrl = $"{_settings.WorldPayWebAppBaseUrl}/selectpaymentmethod?orderKey={orderKey}";

            OrderDataStore.Add(orderKey, orderData);

            using (var stringWriter = new StringWriter())
            {
                using (var writer = new XmlTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;

                    //document start and doc type declaration
                    var dtdPath = $"{_settings.WorldPayWebAppBaseUrl}/content/paymentService_v1.dtd";

                    writer.WriteStartDocument();
                    writer.WriteDocType("paymentService", " -//WorldPay//DTD WorldPay PaymentService v1//EN", dtdPath,
                        null);

                    //payment service element

                    writer.WriteStartElement("paymentService");
                    writer.WriteAttributeString("version", "1.4");

                    writer.WriteStartElement("reply");

                    writer.WriteStartElement("orderStatus");

                    writer.WriteAttributeString("orderCode", orderData.OrderCode);

                    writer.WriteStartElement("reference");

                    /*
                    The id attribute of the reference element can be used as a payment reference if the shopper is
                    expected to make a payment with an off-line payment method like a bank transfer or Accept Giro. In the
                    case of Accept Giro, the reference id number should be printed on the Accept Giros as the payment
                    reference.
                    */

                    writer.WriteAttributeString("id", "");

                    writer.WriteString(redirectUrl);

                    writer.WriteEndElement(); //reference

                    writer.WriteEndElement(); //order status

                    writer.WriteEndElement(); //reply
                    writer.WriteEndElement(); //payment service
                    writer.WriteEndDocument(); //end doc
                }

                return stringWriter.ToString();

            }
        }

        public string PostRefundData(RefundData orderData)
        {

            using (var stringWriter = new StringWriter())
            {
                using (var writer = new XmlTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;

                    //document start and doc type declaration
                    var dtdPath = $"{_settings.WorldPayWebAppBaseUrl}/content/paymentService_v1.dtd";
                    writer.WriteStartDocument();
                    writer.WriteDocType("paymentService", " -//WorldPay//DTD WorldPay PaymentService v1//EN", dtdPath,
                        null);

                    //payment service element

                    writer.WriteStartElement("paymentService");
                    writer.WriteAttributeString("version", "1.4");

                    writer.WriteStartElement("reply");

                    writer.WriteStartElement("ok");

                    writer.WriteStartElement("refundReceived");

                    writer.WriteAttributeString("orderCode", orderData.OrderCode);

                    writer.WriteStartElement("amount");

                    writer.WriteAttributeString("value", orderData.RefundValue);
                    writer.WriteAttributeString("currencyCode", orderData.Currency);
                    writer.WriteAttributeString("exponent", "2");
                    writer.WriteAttributeString("debitCreditIndicator", "credit");

                    writer.WriteEndElement(); //amount

                    writer.WriteEndElement(); //refundReceived
                    writer.WriteEndElement(); //ok
                    writer.WriteEndElement(); //reply
                    writer.WriteEndElement(); //payment service
                    writer.WriteEndDocument(); //end doc
                }

                return stringWriter.ToString();

            }

        }

        public string PostCancelData(CancelData orderData)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new XmlTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;

                    //document start and doc type declaration
                    var dtdPath = $"{_settings.WorldPayWebAppBaseUrl}/content/paymentService_v1.dtd";

                    writer.WriteStartDocument();
                    writer.WriteDocType("paymentService", " -//WorldPay//DTD WorldPay PaymentService v1//EN", dtdPath,
                        null);

                    //payment service element

                    writer.WriteStartElement("paymentService");
                    writer.WriteAttributeString("version", "1.4");

                    writer.WriteStartElement("reply");

                    writer.WriteStartElement("ok");

                    writer.WriteStartElement("voidReceived");

                    writer.WriteAttributeString("orderCode", orderData.OrderCode);

                    writer.WriteEndElement(); //voidReceived
                    writer.WriteEndElement(); //ok
                    writer.WriteEndElement(); //reply
                    writer.WriteEndElement(); //payment service
                    writer.WriteEndDocument(); //end doc
                }

                return stringWriter.ToString();

            }

        }
    }
}
