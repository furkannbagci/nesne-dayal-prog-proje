using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Interfaces.Patterns;

namespace TedarikLojistik.Web.Services.Shipping;

/// <summary>
/// Kırılacak eşya için taşıma bedeline sabit +30 TL ekler.
/// </summary>
public class FragileDecorator : BaseShippingDecorator
{
    public FragileDecorator(IShippingCostCalculator calculator) : base(calculator) { }

    public override decimal CalculateCost(Order order)
    {
        decimal baseCost = base.CalculateCost(order);
        return baseCost + 30m; // Kırılacak eşya ek ücreti
    }
}

