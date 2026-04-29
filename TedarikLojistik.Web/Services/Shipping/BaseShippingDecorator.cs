using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Interfaces.Patterns;

namespace TedarikLojistik.Web.Services.Shipping;

/// <summary>
/// Decorator Pattern - Temel Decorator soyut sınıfı.
/// Diğer decorator'lar bu sınıftan türeyecek.
/// </summary>
public abstract class BaseShippingDecorator : IShippingCostCalculator
{
    protected readonly IShippingCostCalculator _calculator;

    public BaseShippingDecorator(IShippingCostCalculator calculator)
    {
        _calculator = calculator;
    }

    public virtual decimal CalculateCost(Order order)
    {
        return _calculator.CalculateCost(order);
    }
}

