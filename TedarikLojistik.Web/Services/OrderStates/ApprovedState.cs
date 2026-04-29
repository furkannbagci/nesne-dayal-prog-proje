using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Models.Enums;
using TedarikLojistik.Web.Interfaces.Patterns;

namespace TedarikLojistik.Web.Services.OrderStates;

public class ApprovedState : IOrderState
{
    public void NextState(Order order)
    {
        order.Durum = OrderStatus.Hazirlaniyor;
    }

    public void Cancel(Order order)
    {
        order.Durum = OrderStatus.Iptal;
    }

    public void Return(Order order)
    {
        throw new InvalidOperationException("Onaylanan sipariş iade edilemez. Önce kargo sürecine geçmelidir.");
    }

    public string GetStatusName() => "Onaylandı";
}

