namespace TedarikLojistik.Web.Models.Enums;

/// <summary>
/// Ödeme yöntemi seçenekleri.
/// Strategy Pattern bu enum değerlerine göre doğru stratejiyi seçer.
/// </summary>
public enum PaymentMethod
{
    KrediKarti = 0,   // Kredi Kartı ödemesi
    Havale = 1,        // Banka Havalesi
    Kripto = 2         // Kripto Para ödemesi
}

