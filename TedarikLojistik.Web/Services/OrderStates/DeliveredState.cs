using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Models.Enums;
using TedarikLojistik.Web.Interfaces.Patterns;

namespace TedarikLojistik.Web.Services.OrderStates;

public class DeliveredState : IOrderState
{
    public void NextState(Order order)
    {
        throw new InvalidOperationException("Sipariş zaten teslim edildi. İleri bir durum mevcut değil.");
    }

    public void Cancel(Order order)
    {
        throw new InvalidOperationException("Teslim edilmiş sipariş iptal edilemez. İade sürecini başlatınız.");
    }

    public void Return(Order order)
    {
        order.Durum = OrderStatus.Iade;
    }

    public string GetStatusName() => "Teslim Edildi";
}

