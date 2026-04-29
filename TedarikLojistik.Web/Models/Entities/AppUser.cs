using Microsoft.AspNetCore.Identity;
using TedarikLojistik.Web.Models.Enums;

namespace TedarikLojistik.Web.Models.Entities;

/// <summary>
/// Sistemdeki kullanıcıyı temsil eder.
/// ASP.NET Core Identity ile entegre çalışır.
/// </summary>
public class AppUser : IdentityUser<int>
{
    /// <summary>Ad Soyad</summary>
    public string AdSoyad { get; set; } = string.Empty;

    /// <summary>Rol bazlı yetkilendirme için kullanıcı rolü</summary>
    public UserRole Rol { get; set; } = UserRole.Musteri;

    /// <summary>Kaydın oluşturulma zamanı</summary>
    public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;

    /// <summary>Son güncellenme zamanı</summary>
    public DateTime? GuncellemeTarihi { get; set; }

    /// <summary>Soft-delete desteği: false ise silinmiş sayılır</summary>
    public bool Aktif { get; set; } = true;

    /// <summary>Bu kullanıcının verdiği siparişler</summary>
    public ICollection<Order> Siparisler { get; set; } = new List<Order>();
}

