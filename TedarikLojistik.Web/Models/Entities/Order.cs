using TedarikLojistik.Web.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace TedarikLojistik.Web.Models.Entities;

/// <summary>
/// Sipariş entity'si. State Pattern'in ana context (bağlam) nesnesidir.
/// Durumu IOrderState arayüzü üzerinden polimorfik olarak yönetilir.
/// Kargo firması Adapter Pattern ile soyutlanmıştır.
/// Ödeme Strategy Pattern üzerinden gerçekleştirilir.
/// </summary>
public class Order : BaseEntity
{
    /// <summary>Sipariş numarası (benzersiz, okunabilir)</summary>
    public string SiparisNo { get; set; } = string.Empty;

    /// <summary>Siparişi veren kullanıcı</summary>
    public int AppUserId { get; set; }
    public AppUser? AppUser { get; set; }

    /// <summary>Mevcut sipariş durumu — State Pattern ile yönetilir</summary>
    public OrderStatus Durum { get; set; } = OrderStatus.Beklemede;

    /// <summary>Seçilen ödeme yöntemi — Strategy Pattern tetikler</summary>
    public PaymentMethod OdemeYontemi { get; set; }

    /// <summary>Ödeme tamamlandı mı?</summary>
    public bool OdemeTamamlandi { get; set; } = false;

    /// <summary>Seçilen kargo firması — Adapter Pattern ile soyutlanır</summary>
    public CargoCompany KargoFirmasi { get; set; }

    /// <summary>Hesaplanan kargo ücreti — Decorator Pattern ile belirlenir</summary>
    public decimal KargoUcreti { get; set; }

    /// <summary>Teslimat adresi</summary>
    public string TeslimatAdresi { get; set; } = string.Empty;

    /// <summary>Sipariş oluşturulurken kargo fiyatına etki eden mesafe. Veritabanında tutulmaz.</summary>
    [NotMapped]
    public int MesafeKm { get; set; }

    /// <summary>Tahmini teslimat tarihi</summary>
    public DateTime? TahminiTeslimatTarihi { get; set; }

    /// <summary>Sipariş toplam tutarı (ürün + kargo)</summary>
    public decimal ToplamTutar { get; set; }

    /// <summary>İptal veya iade nedeni</summary>
    public string? IptalIadeNedeni { get; set; }

    /// <summary>Sipariş kalemlerinin listesi</summary>
    public ICollection<OrderItem> Kalemler { get; set; } = new List<OrderItem>();

    /// <summary>Ürün tutarı (kargo hariç)</summary>
    public decimal UrunToplami => Kalemler.Sum(k => k.BirimFiyat * k.Miktar);
}

