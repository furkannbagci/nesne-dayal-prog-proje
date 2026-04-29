using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Interfaces.Patterns;

/// <summary>
/// Observer Pattern - Dinleyici (Observer) arayüzü.
/// Stok azaldığında bildirim alacak sınıflar (Email, SMS, Sistem Bildirimi) bu arayüzü implemente eder.
/// </summary>
public interface IStockObserver
{
    /// <summary>
    /// İzlenen üründe stok uyarısı oluştuğunda tetiklenir.
    /// </summary>
    /// <param name="product">Stok eşiğinin altına düşen ürün</param>
    void Update(Product product);
}

