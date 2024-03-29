﻿using INSS.ODS.WorldPay.Models;
using System;
using System.Globalization;
using System.Text;
using System.Xml;

namespace INSS.ODS.WorldPay.Helpers
{
    public static class Xml
    {
        /// <summary>
        /// Creates a Worldpay Xml Order
        /// </summary>
        /// <param name="installationId">The id of the Worldpay Installation</param>
        /// <param name="order">A Worldpay Order instance, populated with data</param>
        /// <param name="merchantCode">Merchant Code</param>
        /// <param name="currentDateTime">Current DateTime</param>
        /// <returns>Worldpay-format Order Xml</returns>
        public static string CreateOrderXml(string installationId, WorldpayOrder order, string merchantCode, DateTime currentDateTime)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            using (var memStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memStream, settings))
                {
                    //document start and doc type declaration
                    writer.WriteStartDocument();
                    writer.WriteDocType("paymentService", " -//WorldPay//DTD WorldPay PaymentService v1//EN",
                        "http://dtd.worldpay.com/paymentService_v1.dtd", null);

                    //payment service element

                    writer.WriteStartElement("paymentService");
                    writer.WriteAttributeString("version", "1.4");
                    writer.WriteAttributeString("merchantCode", merchantCode);

                    //submit / order element
                    writer.WriteStartElement("submit");
                    writer.WriteStartElement("order");
                    writer.WriteAttributeString("orderCode", order.OrderCode);
                    writer.WriteAttributeString("installationId", installationId);

                    //description
                    writer.WriteElementString("description", order.Description);

                    //amount
                    writer.WriteStartElement("amount");
                    writer.WriteAttributeString("currencyCode", order.CurrencyCode);
                    writer.WriteAttributeString("exponent", "2");
                    writer.WriteAttributeString("value", GetValueAsString(order.OrderValue));

                    writer.WriteEndElement(); //end amount

                    //order content
                    writer.WriteStartElement("orderContent");
                    writer.WriteCData($"<strong>{order.Description}: {order.OrderValue.ToString("C", CultureInfo.GetCultureInfo("en-GB"))}</strong>");
                    writer.WriteEndElement(); //end order content

                    //payment method masks
                    writer.WriteStartElement("paymentMethodMask");
                    writer.WriteStartElement("include");
                    writer.WriteAttributeString("code", "ALL");
                    writer.WriteEndElement();
                    writer.WriteEndElement(); //payment method mask

                    //shopper
                    writer.WriteStartElement("shopper");
                    writer.WriteElementString("shopperEmailAddress", order.Email);
                    writer.WriteEndElement(); //end shopper

                    //address (billing)
                    if (order.IncludeAddress)
                    {
                        writer.WriteStartElement("shippingAddress");
                        writer.WriteStartElement("address");
                        writer.WriteElementString("firstName", order.FirstName);
                        writer.WriteElementString("lastName", order.LastName);
                        writer.WriteElementString("address1", order.Address1);
                        writer.WriteElementString("address2", order.Address2);
                        writer.WriteElementString("address3", order.Address3);
                        writer.WriteElementString("postalCode", order.PostCode);
                        writer.WriteElementString("city", order.City);
                        writer.WriteElementString("countryCode", order.CountryCode);
                        writer.WriteEndElement(); //end address
                        writer.WriteEndElement(); //end shipping address

                        writer.WriteStartElement("billingAddress");
                        writer.WriteStartElement("address");
                        writer.WriteElementString("firstName", order.FirstName);
                        writer.WriteElementString("lastName", order.LastName);
                        writer.WriteElementString("address1", order.Address1);
                        writer.WriteElementString("address2", order.Address2);
                        writer.WriteElementString("address3", order.Address3);
                        writer.WriteElementString("postalCode", order.PostCode);
                        writer.WriteElementString("city", order.City);
                        writer.WriteElementString("countryCode", order.CountryCode);
                        writer.WriteElementString("telephoneNumber", order.TelephoneNumber);
                        writer.WriteEndElement(); //end address
                        writer.WriteEndElement(); //end billing address
                    }

                    //riskData for 3DS2
                    writer.WriteStartElement("riskData");

                    writer.WriteStartElement("authenticationRiskData");
                    writer.WriteAttributeString("authenticationMethod", "localAccount");
                    writer.WriteStartElement("authenticationTimestamp");
                    writer.WriteStartElement("date");
                    writer.WriteAttributeString("year", currentDateTime.Year.ToString());
                    writer.WriteAttributeString("month", currentDateTime.Month.ToString());
                    writer.WriteAttributeString("dayOfMonth", currentDateTime.Day.ToString());
                    writer.WriteAttributeString("hour", currentDateTime.Hour.ToString());
                    writer.WriteAttributeString("minute", currentDateTime.Minute.ToString());
                    writer.WriteAttributeString("second", currentDateTime.Second.ToString());
                    writer.WriteEndElement();
                    writer.WriteEndElement(); //end authenticationTimestamp
                    writer.WriteEndElement(); //end authenticationRiskData

                    writer.WriteStartElement("shopperAccountRiskData");
                    writer.WriteAttributeString("previousSuspiciousActivity", "false");
                    writer.WriteAttributeString("shippingNameMatchesAccountName", "true");
                    writer.WriteEndElement(); //end shopperAccountRiskData

                    writer.WriteStartElement("transactionRiskData");
                    writer.WriteAttributeString("shippingMethod", "digital");
                    writer.WriteAttributeString("deliveryTimeframe", "electronicDelivery");
                    writer.WriteAttributeString("preOrderPurchase", "true");
                    writer.WriteEndElement(); //end transactionRiskData

                    writer.WriteEndElement(); //end riskData

                    writer.WriteEndElement(); //end order
                    writer.WriteEndElement(); //end submit

                    writer.WriteEndElement(); //end payment service

                    writer.WriteEndDocument(); //end doc

                }

                return Encoding.UTF8.GetString(memStream.ToArray());
            }

        }

        private static string GetValueAsString(decimal value)
        {
            value = value * 100;
            return value.ToString("00");

        }

        public static WorldpayResponse ParseReplyXml(string xml)
        {
            var response = new WorldpayResponse();

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode root = doc.DocumentElement;

            var replyNode = root.SelectSingleNode("reply");
            var nodeOrderStatus = replyNode.SelectSingleNode("orderStatus");


            var errorNode = replyNode.SelectSingleNode("error");
            if (errorNode != null)
            {
                response.Error = errorNode.InnerText;
                return response;
            }

            response.OrderCode = nodeOrderStatus.Attributes["orderCode"].Value;

            var nodeReference = nodeOrderStatus.SelectSingleNode("reference");
            var redirectUrl = nodeReference.InnerText;

            response.RedirectUrl = redirectUrl;

            return response;
        }

        public static WorldpayOrderStatusUpdate ParseOrderUpdate(string xml, ILogger logger)
        {
            var response = new WorldpayOrderStatusUpdate();

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNode root = doc.DocumentElement;

                var notifyNode = root.SelectSingleNode("notify");
                var orderStatusEvent = notifyNode.SelectSingleNode("orderStatusEvent");
                var paymentNode = orderStatusEvent.SelectSingleNode("payment");
                var lastEventNode = paymentNode.SelectSingleNode("lastEvent");

                response.OrderCode = orderStatusEvent.Attributes["orderCode"].Value;
                response.Status = lastEventNode.InnerText;

                return response;
            }
            catch (Exception)
            {
                logger.LogError("Error parsing Order Update xml:");
                logger.LogError(xml);
                throw;
            }

        }


        public static string CreateRefundXml(string installationId, WorldpayRefundRequest refundRequest, string merchantCode)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            using (var memStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memStream, settings))
                {
                    //document start and doc type declaration
                    writer.WriteStartDocument();
                    writer.WriteDocType("paymentService", " -//WorldPay//DTD WorldPay PaymentService v1//EN",
                        "http://dtd.worldpay.com/paymentService_v1.dtd", null);

                    //payment service element

                    writer.WriteStartElement("paymentService");
                    writer.WriteAttributeString("version", "1.4");
                    writer.WriteAttributeString("merchantCode", merchantCode);

                    //submit / order element
                    writer.WriteStartElement("modify");
                    writer.WriteStartElement("orderModification");
                    writer.WriteAttributeString("orderCode", refundRequest.OrderCode);
                    //writer.WriteAttributeString("installationId", installationId);
                    writer.WriteStartElement("refund");

                    writer.WriteStartElement("amount");
                    writer.WriteAttributeString("value", GetValueAsString(refundRequest.RefundValue));
                    writer.WriteAttributeString("currencyCode", refundRequest.CurrencyCode);
                    writer.WriteAttributeString("exponent", "2");
                    writer.WriteEndElement(); //end amount

                    writer.WriteEndElement(); //end refund
                    writer.WriteEndElement(); //end order
                    writer.WriteEndElement(); //end orderModification

                    writer.WriteEndElement(); //end payment service

                    writer.WriteEndDocument(); //end doc

                }

                return Encoding.UTF8.GetString(memStream.ToArray());
            }

        }


        public static bool ParseRefundRequestReplyXml(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode root = doc.DocumentElement;

            var replyNode = root.SelectSingleNode("reply");
            var okNode = replyNode.SelectSingleNode("ok");

            return okNode != null;
        }



        public static string CreateCancelOrRefundXml(string installationId, string orderCode, string merchantCode)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = Encoding.UTF8
            };

            using (var memStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memStream, settings))
                {
                    //document start and doc type declaration
                    writer.WriteStartDocument();
                    writer.WriteDocType("paymentService", " -//WorldPay//DTD WorldPay PaymentService v1//EN",
                        "http://dtd.worldpay.com/paymentService_v1.dtd", null);

                    //payment service element

                    writer.WriteStartElement("paymentService");
                    writer.WriteAttributeString("version", "1.4");
                    writer.WriteAttributeString("merchantCode", merchantCode);

                    //submit / order element
                    writer.WriteStartElement("modify");
                    writer.WriteStartElement("orderModification");
                    writer.WriteAttributeString("orderCode", orderCode);
                    writer.WriteStartElement("cancelOrRefund");
                    writer.WriteEndElement(); //end cancelOrRefund
                    writer.WriteEndElement(); //end order
                    writer.WriteEndElement(); //end orderModification

                    writer.WriteEndElement(); //end payment service

                    writer.WriteEndDocument(); //end doc

                }

                return Encoding.UTF8.GetString(memStream.ToArray());
            }

        }

        public static bool ParseCancelOrRefundRequestReplyXml(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode root = doc.DocumentElement;

            var replyNode = root.SelectSingleNode("reply");
            var okNode = replyNode.SelectSingleNode("ok");

            return okNode != null;
        }


    }
}
