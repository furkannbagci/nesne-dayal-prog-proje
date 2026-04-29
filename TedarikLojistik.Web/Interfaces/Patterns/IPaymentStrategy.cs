using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Interfaces.Patterns;

/// <summary>
/// Strategy Pattern - Ödeme işlemleri için arayüz.
/// Farklı ödeme yöntemleri (Kredi Kartı, Havale, Kripto) bu arayüzü implemente edecektir.
/// </summary>
public interface IPaymentStrategy
{
    /// <summary>
    /// Verilen siparişin ödemesini gerçekleştirir.
    /// </summary>
    /// <param name="order">Ödemesi alınacak sipariş</param>
    /// <returns>Ödeme başarılıysa true, aksi halde false</returns>
    bool Pay(Order order);
}

