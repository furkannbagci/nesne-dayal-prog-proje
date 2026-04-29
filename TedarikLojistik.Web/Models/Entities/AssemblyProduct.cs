using TedarikLojistik.Web.Models.Enums;

namespace TedarikLojistik.Web.Models.Entities;

/// <summary>
/// Alt bileşenlerden oluşan karmaşık/montaj ürününü temsil eder.
/// Factory Method / Builder deseni tarafından üretilir.
/// Composite yapısında alt bileşenlerin (ProductComponent) listesini tutar.
/// </summary>
public class AssemblyProduct : Product
{
    public AssemblyProduct()
    {
        UrunTuru = ProductType.Montaj;
        Bilesenler = new List<ProductComponent>();
    }

    /// <summary>Bu ürünü oluşturan alt bileşenler</summary>
    public ICollection<ProductComponent> Bilesenler { get; set; }

    /// <summary>
    /// Montaj ürünün ağırlığı: kendi gövdesi + tüm bileşenlerin ağırlığı.
    /// Polimorfik hesaplama — switch-case kullanılmaz.
    /// </summary>
    public override double ToplamAgirlikHesapla() =>
        Agirlik + Bilesenler.Sum(b => b.Agirlik * b.Miktar);

    /// <summary>Montaj ürün özeti</summary>
    public override string Ozet() =>
        $"[Montaj Ürün] {Ad} | Bileşen Sayısı: {Bilesenler.Count} | " +
        $"Fiyat: {Fiyat:C} | Toplam Ağırlık: {ToplamAgirlikHesapla()}kg";
}

