namespace TedarikLojistik.Web.Models.Entities;

/// <summary>
/// Tüm entity sınıflarının türediği temel sınıf.
/// Id ve audit (denetim) alanlarını merkezi olarak tanımlar — DRY prensibi.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Veritabanı birincil anahtarı</summary>
    public int Id { get; set; }

    /// <summary>Kaydın oluşturulma zamanı</summary>
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;

    /// <summary>Son güncellenme zamanı</summary>
    public DateTime? GuncellemeTarihi { get; set; }

    /// <summary>Soft-delete desteği: false ise silinmiş sayılır</summary>
    public bool Aktif { get; set; } = true;
}

