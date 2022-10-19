using INSS.ODS.WorldPay.Data;

namespace INSS.ODS.WorldPay.Services
{
    public interface IOrderService
    {
        string PostOrder(OrderData orderData);
        string PostRefundData(RefundData orderData);
        string PostCancelData(CancelData orderData);
    }
}
