using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Interfaces.Patterns;

/// <summary>
/// Decorator Pattern - Kargo ücretini dinamik olarak hesaplamak için temel arayüz.
/// Ekstra hizmetler (Sigorta, Kırılacak Eşya vs.) bu arayüz üzerinden temel fiyata eklenecek.
/// </summary>
public interface IShippingCostCalculator
{
    /// <summary>
    /// Siparişin kargo ücretini hesaplar.
    /// </summary>
    decimal CalculateCost(Order order);
}

