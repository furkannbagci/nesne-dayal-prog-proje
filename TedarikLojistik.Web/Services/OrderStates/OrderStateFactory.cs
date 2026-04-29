using TedarikLojistik.Web.Models.Enums;
using TedarikLojistik.Web.Interfaces.Patterns;

namespace TedarikLojistik.Web.Services.OrderStates;

/// <summary>
/// Siparişin veritabanındaki durumuna karşılık gelen State sınıfını örneklendirir.
/// </summary>
public static class OrderStateFactory
{
    private static readonly IReadOnlyDictionary<OrderStatus, Func<IOrderState>> States =
        new Dictionary<OrderStatus, Func<IOrderState>>
        {
            [OrderStatus.Beklemede] = () => new PendingState(),
            [OrderStatus.Onaylandi] = () => new ApprovedState(),
            [OrderStatus.Hazirlaniyor] = () => new PreparingState(),
            [OrderStatus.Kargoda] = () => new InShipmentState(),
            [OrderStatus.TeslimEdildi] = () => new DeliveredState(),
            [OrderStatus.Iade] = () => new ReturnedState()
        };

    public static IOrderState GetState(OrderStatus status)
    {
        if (States.TryGetValue(status, out var stateFactory))
            return stateFactory();

        throw new ArgumentOutOfRangeException(nameof(status), "Durum geçişi için uygun state bulunamadı.");
    }
}
