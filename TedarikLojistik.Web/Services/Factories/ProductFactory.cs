using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Models.Enums;

namespace TedarikLojistik.Web.Services.Factories;

/// <summary>
/// Factory Method Pattern.
/// İstemci, nesnenin nasıl üretildiğini bilmez; sadece tipi ister.
/// </summary>
public static class ProductFactory
{
    private static readonly IReadOnlyDictionary<ProductType, Func<string, decimal, double, int, Product>> Creators =
        new Dictionary<ProductType, Func<string, decimal, double, int, Product>>
        {
            [ProductType.Basit] = (name, price, weight, stockThreshold) => new SimpleProduct
            {
                Ad = name,
                Fiyat = price,
                Agirlik = weight,
                StokEsikDegeri = stockThreshold,
                StokMiktari = 0
            },
            [ProductType.Montaj] = (name, price, weight, stockThreshold) => new AssemblyProduct
            {
                Ad = name,
                Fiyat = price,
                Agirlik = weight,
                StokEsikDegeri = stockThreshold,
                StokMiktari = 0
            }
        };

    public static Product CreateProduct(ProductType type, string name, decimal price, double weight, int stockThreshold = 10)
    {
        if (Creators.TryGetValue(type, out var creator))
            return creator(name, price, weight, stockThreshold);

        throw new ArgumentException("Bilinmeyen ürün tipi");
    }
}
