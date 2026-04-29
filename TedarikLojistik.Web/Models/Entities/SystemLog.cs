namespace TedarikLojistik.Web.Models.Entities;

/// <summary>
/// Sistem genelindeki kritik işlemlerin log kaydı.
/// Singleton AppLogger tarafından bu tabloya yazılır.
/// </summary>
public class SystemLog : BaseEntity
{
    /// <summary>Log düzeyi: INFO, WARNING, ERROR</summary>
    public string Seviye { get; set; } = "INFO";

    /// <summary>İşlem kategorisi: StokDegisimi, Odeme, SiparisDurum vb.</summary>
    public string Kategori { get; set; } = string.Empty;

    /// <summary>Log mesajı</summary>
    public string Mesaj { get; set; } = string.Empty;

    /// <summary>Varsa ilgili entity Id'si (örn: SiparisId)</summary>
    public int? IlgiliEntityId { get; set; }

    /// <summary>İşlemi gerçekleştiren kullanıcı adı</summary>
    public string? KullaniciAdi { get; set; }
}

