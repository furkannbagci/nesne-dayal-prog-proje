using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Interfaces.Services;
using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Models.Enums;
using TedarikLojistik.Web.Services.Factories;
using TedarikLojistik.Web.Services.Shipping;

namespace TedarikLojistik.Web.Services.Orders;

/// <summary>
/// Adapter ve Decorator desenlerini kullanarak kargo islemlerini toplar.
/// </summary>
public class OrderPricingService : IOrderPricingService
{
    public OrderPricingResult CalculateShipping(Order order, CargoCompany cargoCompany, int distanceKm, bool isFragile, bool isInsured)
    {
        // Secilen firmaya gore uygun Adapter nesnesi uretilir.
        var shippingAdapter = ShippingAdapterFactory.Create(cargoCompany);

        // Temel hesaplayici firma adapterindan gelen fiyat bilgisini kullanir.
        IShippingCostCalculator calculator = new AdapterShippingCost(shippingAdapter, distanceKm);

        // Ek hizmetler Decorator olarak sirayla hesaba eklenir.
        if (isInsured) calculator = new InsuranceDecorator(calculator);
        if (isFragile) calculator = new FragileDecorator(calculator);

        var shippingCost = calculator.CalculateCost(order);
        var trackingNumber = shippingAdapter.CreateShipment(order);

        return new OrderPricingResult(shippingCost, trackingNumber);
    }
}
