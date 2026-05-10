using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Interfaces.Patterns;

public interface IShippingAdapter
{
    string CreateShipment(Order order);
    decimal CalculateBasePrice(Order order, int distanceKm);
    string CheckStatus(string trackingNumber);
}
