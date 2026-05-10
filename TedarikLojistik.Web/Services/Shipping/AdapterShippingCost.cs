using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Services.Shipping;

public class AdapterShippingCost : IShippingCostCalculator
{
    private readonly IShippingAdapter _adapter;
    private readonly int _distanceKm;

    public AdapterShippingCost(IShippingAdapter adapter, int distanceKm)
    {
        _adapter = adapter;
        _distanceKm = distanceKm;
    }

    public decimal CalculateCost(Order order)
    {
        return _adapter.CalculateBasePrice(order, _distanceKm);
    }
}
