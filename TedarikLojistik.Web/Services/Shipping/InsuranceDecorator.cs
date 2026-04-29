using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Interfaces.Patterns;

namespace TedarikLojistik.Web.Services.Shipping;

/// <summary>
/// Sipariş tutarının %5'i kadar sigorta bedeli ekler.
/// </summary>
public class InsuranceDecorator : BaseShippingDecorator
{
    public InsuranceDecorator(IShippingCostCalculator calculator) : base(calculator) { }

    public override decimal CalculateCost(Order order)
    {
        decimal baseCost = base.CalculateCost(order);
        decimal insuranceCost = order.UrunToplami * 0.05m; // %5 sigorta bedeli
        return baseCost + insuranceCost;
    }
}

