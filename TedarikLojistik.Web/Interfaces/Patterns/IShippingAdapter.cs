using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Interfaces.Patterns;

/// <summary>
/// Adapter Pattern - Kargo firmalarının farklı API'lerini standartlaştırmak için arayüz.
/// Sisteme eklenecek her yeni kargo firması bu arayüze uyumlu bir Adapter sınıfına sahip olmalıdır.
/// </summary>
public interface IShippingAdapter
{
    /// <summary>
    /// İlgili kargo firması üzerinden kargo talebi oluşturur.
    /// </summary>
    /// <param name="order">Kargoya verilecek sipariş</param>
    /// <returns>Kargo firmasından dönen takip numarası</returns>
    string CreateShipment(Order order);

    /// <summary>
    /// Firmanın kendi fiyatlandırma algoritmasıyla temel kargo ücretini hesaplar.
    /// </summary>
    decimal CalculateBasePrice(Order order);
    
    /// <summary>
    /// Takip numarası ile kargonun durumunu sorgular.
    /// </summary>
    string CheckStatus(string trackingNumber);
}

