namespace TedarikLojistik.Web.Models.Enums;

/// <summary>
/// Sipariş durum makinesi için durum listesi.
/// State Pattern bu enum değerleri üzerinden çalışır.
/// Geçiş kuralları: Beklemede → Onaylandi → Hazirlaniyor → Kargoda → TeslimEdildi
///                  Kargoda → Iade (hatalı ürün)
///                  Kargoda durumunda İptal YAPILAMAZ.
/// </summary>
public enum OrderStatus
{
    Beklemede = 0,
    Onaylandi = 1,
    Hazirlaniyor = 2,
    Kargoda = 3,
    TeslimEdildi = 4,
    Iade = 5,
    Iptal = 6
}

