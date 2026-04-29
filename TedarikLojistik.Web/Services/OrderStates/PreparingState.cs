using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Models.Enums;
using TedarikLojistik.Web.Interfaces.Patterns;

namespace TedarikLojistik.Web.Services.OrderStates;

public class PreparingState : IOrderState
{
    public void NextState(Order order)
    {
        order.Durum = OrderStatus.Kargoda;
    }

    public void Cancel(Order order)
    {
        // Hazırlanma aşamasında da iptal edilebilir
        order.Durum = OrderStatus.Iptal;
    }

    public void Return(Order order)
    {
        throw new InvalidOperationException("Hazırlanan sipariş iade edilemez. Önce kargoya verilmelidir.");
    }

    public string GetStatusName() => "Hazırlanıyor";
}

