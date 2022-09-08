using INSS.ODS.WorldPay.Data;
using System;
using System.Xml;

namespace INSS.ODS.WorldPay.Services
{
    public class PaymentXmlParserService : IPaymentXmlParserService
    {
        public IWorldpayRequest ParseResult(string xml)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNode root = doc.DocumentElement;

                if (root.SelectSingleNode("submit") != null)
                {
                    return ParseOrderData(xml);
                }

                var modifyNode = root.SelectSingleNode("modify");
                var orderModificationNode = modifyNode.SelectSingleNode("orderModification");

                if (orderModificationNode != null)
                {
                    if (orderModificationNode.SelectSingleNode("refund") != null)
                    {
                        return ParseRefundData(xml);
                    }
                    else if (orderModificationNode.SelectSingleNode("cancelOrRefund") != null)
                    {
                        return ParseCancelOrRefundData(xml);
                    }


                }

                throw new XmlException("Could not parse xml");

            }
            catch (Exception)
            {
                return null;
            }
        }

        private OrderData ParseOrderData(string xml)
        {
            try
            {
                var result = new OrderData();

                var doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNode root = doc.DocumentElement;

                result.MerchantCode = root.Attributes["merchantCode"].Value;

                var nSubmit = root.SelectSingleNode("submit");
                var nOrder = nSubmit.SelectSingleNode("order");
                var nDescription = nOrder.SelectSingleNode("description");
                var nAmount = nOrder.SelectSingleNode("amount");

                result.OrderCode = nOrder.Attributes["orderCode"].Value;
                result.Description = nDescription.InnerText;
                result.Value = nAmount.Attributes["value"].Value;
                result.Currency = nAmount.Attributes["currencyCode"].Value;

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private RefundData ParseRefundData(string xml)
        {
            try
            {
                var result = new RefundData();

                var doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNode root = doc.DocumentElement;

                var nModify = root.SelectSingleNode("modify");
                var nOrderModification = nModify.SelectSingleNode("orderModification");
                var nRefund = nOrderModification.SelectSingleNode("refund");
                var nAmount = nRefund.SelectSingleNode("amount");

                result.OrderCode = nOrderModification.Attributes["orderCode"].Value;
                result.RefundValue = nAmount.Attributes["value"].Value;
                result.Currency = nAmount.Attributes["currencyCode"].Value;

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private CancelData ParseCancelOrRefundData(string xml)
        {
            try
            {
                var result = new CancelData();

                var doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNode root = doc.DocumentElement;

                var nModify = root.SelectSingleNode("modify");
                var nOrderModification = nModify.SelectSingleNode("orderModification");
                var nRefund = nOrderModification.SelectSingleNode("cancelOrRefund");
                result.OrderCode = nOrderModification.Attributes["orderCode"].Value;

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
