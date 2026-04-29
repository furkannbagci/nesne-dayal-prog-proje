using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Services.Shipping;

public class YurticiAdapter : IShippingAdapter
{
    public string CreateShipment(Order order)
    {
        return $"YRT-{Guid.NewGuid().ToString()[..8]}";
    }

    public decimal CalculateBasePrice(Order order)
    {
        var weight = order.Kalemler.Sum(k => k.Product?.ToplamAgirlikHesapla() * k.Miktar ?? 0);
        return Math.Max(50m, 28m + (decimal)weight * 9.5m);
    }

    public string CheckStatus(string trackingNumber)
    {
        return "Yolda";
    }
}
