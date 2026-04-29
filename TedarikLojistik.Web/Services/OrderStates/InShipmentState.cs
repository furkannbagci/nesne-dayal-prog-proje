using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Models.Enums;
using TedarikLojistik.Web.Interfaces.Patterns;

namespace TedarikLojistik.Web.Services.OrderStates;

public class InShipmentState : IOrderState
{
    public void NextState(Order order)
    {
        order.Durum = OrderStatus.TeslimEdildi;
    }

    public void Cancel(Order order)
    {
        // Kural: Kargodaki ürün iptal edilemez!
        throw new InvalidOperationException("Kargoya verilmiş ürün iptal edilemez! Lütfen teslimattan sonra iade talebi oluşturun.");
    }

    public void Return(Order order)
    {
        order.Durum = OrderStatus.Iade;
    }

    public string GetStatusName() => "Kargoda";
}

