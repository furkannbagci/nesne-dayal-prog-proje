using TedarikLojistik.Web.Models.Enums;

namespace TedarikLojistik.Web.Models.Entities;

/// <summary>
/// Ürün entity'sinin soyut temel sınıfı.
/// Factory Method deseni bu sınıftan türeyen SimpleProduct ve AssemblyProduct üretir.
/// Polimorfizm sayesinde switch-case'e gerek kalmadan ürün davranışı ayrışır.
/// </summary>
public abstract class Product : BaseEntity
{
    /// <summary>Ürün adı</summary>
    public string Ad { get; set; } = string.Empty;

    /// <summary>Ürün açıklaması</summary>
    public string Aciklama { get; set; } = string.Empty;

    /// <summary>Birim fiyatı (TL)</summary>
    public decimal Fiyat { get; set; }

    /// <summary>Mevcut stok miktarı</summary>
    public int StokMiktari { get; set; }

    /// <summary>
    /// Stok uyarı eşiği. Bu değerin altına düşüldüğünde
    /// Observer deseni devreye girerek ilgili birimlere bildirim gönderir.
    /// </summary>
    public int StokEsikDegeri { get; set; } = 10;

    /// <summary>Ürün ağırlığı (kg) — kargo ücreti hesabında kullanılır</summary>
    public double Agirlik { get; set; }

    /// <summary>Ürün türü (Basit / Montaj)</summary>
    public ProductType UrunTuru { get; set; }

    /// <summary>Stok eşik değerinin altında mı? Observer tetikleme kontrolü için</summary>
    public bool StokEsikAltinda => StokMiktari < StokEsikDegeri;

    /// <summary>
    /// Ürünün toplam ağırlığını polimorfik olarak hesaplar.
    /// Basit ürün kendi ağırlığını, montaj ürün bileşenlerinin toplamını döner.
    /// </summary>
    public abstract double ToplamAgirlikHesapla();

    /// <summary>
    /// Ürünün insan okunabilir özetini döner.
    /// </summary>
    public abstract string Ozet();
}

