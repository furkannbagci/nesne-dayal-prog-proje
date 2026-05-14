using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Models.Enums;

namespace TedarikLojistik.Web.Interfaces.Services;

/// <summary>
/// Siparis icin kargo ucreti ve takip numarasi uretir.
/// </summary>
public interface IOrderPricingService
{
    OrderPricingResult CalculateShipping(Order order, CargoCompany cargoCompany, int distanceKm, bool isFragile, bool isInsured);
}

/// <summary>
/// Kargo hesaplama sonucunu tek yerde tasir.
/// </summary>
public record OrderPricingResult(decimal ShippingCost, string TrackingNumber);
