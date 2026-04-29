using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Services.Logging;

namespace TedarikLojistik.Web.Services.Payments;

/// <summary>
/// Banka Havalesi / EFT Ödeme Stratejisi
/// </summary>
public class BankTransferPayment : IPaymentStrategy
{
    public bool Pay(Order order)
    {
        AppLogger.Instance.LogInfo("Odeme", $"Banka Havalesi/EFT ile ödeme talebi oluşturuldu. Tutar: {order.ToplamTutar:C}", entityId: order.Id);
        return true;
    }
}

