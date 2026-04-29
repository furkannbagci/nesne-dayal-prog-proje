namespace TedarikLojistik.Web.Models.Entities;

/// <summary>
/// Sipariş kalemi — hangi üründen kaç adet sipariş verildiğini tutar.
/// </summary>
public class OrderItem : BaseEntity
{
    /// <summary>Bağlı olduğu sipariş (FK)</summary>
    public int OrderId { get; set; }
    public Order? Order { get; set; }

    /// <summary>Sipariş edilen ürün (FK)</summary>
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    /// <summary>Ürün adı (snapshot — ürün değişse bile sipariş geçmişi korunur)</summary>
    public string UrunAdi { get; set; } = string.Empty;

    /// <summary>Sipariş anındaki birim fiyat (snapshot)</summary>
    public decimal BirimFiyat { get; set; }

    /// <summary>Sipariş edilen miktar</summary>
    public int Miktar { get; set; }

    /// <summary>Kalem tutarı</summary>
    public decimal ToplamFiyat => BirimFiyat * Miktar;
}

