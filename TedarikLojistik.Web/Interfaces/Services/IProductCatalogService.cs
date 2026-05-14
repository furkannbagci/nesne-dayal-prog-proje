using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Interfaces.Services;

/// <summary>
/// Urun listesini hazirlar ve eksik ornek urunleri sisteme ekler.
/// </summary>
public interface IProductCatalogService
{
    Task<IEnumerable<Product>> EnsureProductsAsync();
}
