using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Services.Shipping;

public class GlobalExpresAdapter : IShippingAdapter
{
    public string CreateShipment(Order order)
    {
        return $"GLB-{Guid.NewGuid().ToString()[..8]}";
    }

    public decimal CalculateBasePrice(Order order, int distanceKm)
    {
        var weight = order.Kalemler.Sum(k => k.Product?.ToplamAgirlikHesapla() * k.Miktar ?? 0);
        return Math.Max(90m, 70m + (decimal)weight * 14m + distanceKm * 0.09m);
    }

    public string CheckStatus(string trackingNumber)
    {
        return "Gümrük/uzak dağıtım kontrolünde";
    }
}
