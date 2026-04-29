using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Services.Logging;

namespace TedarikLojistik.Web.Services.Payments;

/// <summary>
/// Kredi Kartı Ödeme Stratejisi
/// </summary>
public class CreditCardPayment : IPaymentStrategy
{
    public bool Pay(Order order)
    {
        // Gerçekte burada sanal pos (iyzico vb) API çağrısı olur.
        bool result = order.ToplamTutar > 0;
        
        AppLogger.Instance.LogInfo("Odeme", $"Kredi Kartı ile ödeme alındı. Tutar: {order.ToplamTutar:C}", entityId: order.Id);
        
        return result;
    }
}

