using TedarikLojistik.Web.Models.Enums;

namespace TedarikLojistik.Web.Models.Entities;

/// <summary>
/// Tek parça, montaj gerektirmeyen basit ürünü temsil eder.
/// Factory Method deseni tarafından üretilir.
/// </summary>
public class SimpleProduct : Product
{
    public SimpleProduct()
    {
        UrunTuru = ProductType.Basit;
    }

    /// <summary>
    /// Basit ürünün toplam ağırlığı sadece kendi ağırlığıdır.
    /// </summary>
    public override double ToplamAgirlikHesapla() => Agirlik;

    /// <summary>Basit ürün özeti</summary>
    public override string Ozet() =>
        $"[Basit Ürün] {Ad} | Fiyat: {Fiyat:C} | Stok: {StokMiktari} | Ağırlık: {Agirlik}kg";
}

