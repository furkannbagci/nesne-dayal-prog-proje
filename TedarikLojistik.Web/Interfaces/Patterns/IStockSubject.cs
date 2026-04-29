using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Interfaces.Patterns;

/// <summary>
/// Observer Pattern - Dinlenen (Subject/Publisher) arayüzü.
/// Kendisini dinleyen observer'ları yönetir ve olay gerçekleştiğinde onlara haber verir.
/// </summary>
public interface IStockSubject
{
    /// <summary>
    /// Yeni bir dinleyici (Observer) ekler.
    /// </summary>
    void Attach(IStockObserver observer);

    /// <summary>
    /// Mevcut bir dinleyiciyi çıkarır.
    /// </summary>
    void Detach(IStockObserver observer);

    /// <summary>
    /// Stok durumu değiştiğinde tüm dinleyicilere haber verir.
    /// </summary>
    void Notify(Product product);
}

