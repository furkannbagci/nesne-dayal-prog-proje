namespace TedarikLojistik.Web.Models.Enums;

/// <summary>
/// Sistemdeki kullanıcı rollerini tanımlar.
/// Rol bazlı yetkilendirme (Authorization) bu enum üzerinden yönetilir.
/// </summary>
public enum UserRole
{
    Admin = 0,          // Yönetici: Tam yetkili
    DepoGorevlisi = 1,  // Depo Görevlisi: Stok ve ürün işlemleri
    Musteri = 2         // Kurye / Müşteri: Sipariş takibi
}

