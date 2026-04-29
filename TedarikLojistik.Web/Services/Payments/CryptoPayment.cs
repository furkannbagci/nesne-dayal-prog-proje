using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Services.Logging;

namespace TedarikLojistik.Web.Services.Payments;

/// <summary>
/// Kripto Para Ödeme Stratejisi
/// </summary>
public class CryptoPayment : IPaymentStrategy
{
    public bool Pay(Order order)
    {
        // Blockchain onayı beklenir (Mock)
        AppLogger.Instance.LogInfo("Odeme", $"Kripto cüzdan üzerinden ödeme alındı. Tutar: {order.ToplamTutar:C}", entityId: order.Id);
        return true; // Başarılı varsayıyoruz
    }
}

