using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Services.Shipping;

public class AdapterShippingCost : IShippingCostCalculator
{
    private readonly IShippingAdapter _adapter;

    public AdapterShippingCost(IShippingAdapter adapter)
    {
        _adapter = adapter;
    }

    public decimal CalculateCost(Order order)
    {
        return _adapter.CalculateBasePrice(order);
    }
}
