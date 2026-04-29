using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Services.Shipping;

public class ArasAdapter : IShippingAdapter
{
    public string CreateShipment(Order order)
    {
        return $"ARS-{Guid.NewGuid().ToString()[..8]}";
    }

    public decimal CalculateBasePrice(Order order)
    {
        var weight = order.Kalemler.Sum(k => k.Product?.ToplamAgirlikHesapla() * k.Miktar ?? 0);
        return Math.Max(45m, 35m + (decimal)weight * 8m);
    }

    public string CheckStatus(string trackingNumber)
    {
        return "Transfer merkezinde";
    }
}
