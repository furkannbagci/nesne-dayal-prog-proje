using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Interfaces.Patterns;

namespace TedarikLojistik.Web.Services.OrderStates;

public class ReturnedState : IOrderState
{
    public void NextState(Order order)
    {
        throw new InvalidOperationException("İade sürecindeki sipariş ileri taşınamaz.");
    }

    public void Cancel(Order order)
    {
        throw new InvalidOperationException("İade sürecindeki sipariş iptal edilemez.");
    }

    public void Return(Order order)
    {
        throw new InvalidOperationException("Sipariş zaten iade sürecindedir.");
    }

    public string GetStatusName() => "İade";
}
