using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Interfaces.Patterns;

namespace TedarikLojistik.Web.Services.Shipping;

/// <summary>
/// Decorator Pattern - Temel bileşen. Standart ağırlık bazlı hesaplama.
/// </summary>
public class StandardShippingCost : IShippingCostCalculator
{
    public decimal CalculateCost(Order order)
    {
        // Temel hesaplama: Ağırlık * 10 TL, minimum 50 TL
        double totalWeight = order.Kalemler.Sum(k => k.Product?.ToplamAgirlikHesapla() * k.Miktar ?? 0);
        decimal cost = (decimal)(totalWeight * 10);
        return cost < 50 ? 50 : cost;
    }
}

