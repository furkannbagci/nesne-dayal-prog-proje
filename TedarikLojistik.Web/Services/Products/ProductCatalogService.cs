using Microsoft.EntityFrameworkCore;
using TedarikLojistik.Web.Data;
using TedarikLojistik.Web.Interfaces.Repositories;
using TedarikLojistik.Web.Interfaces.Services;
using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Models.Enums;
using TedarikLojistik.Web.Services.Factories;

namespace TedarikLojistik.Web.Services.Products;

/// <summary>
/// Ornek urunleri hazirlayan basit servis.
/// Controller'in veri hazirlama isini azaltir.
/// </summary>
public class ProductCatalogService : IProductCatalogService
{
    private readonly AppDbContext _context;
    private readonly IGenericRepository<Product> _productRepo;

    public ProductCatalogService(AppDbContext context, IGenericRepository<Product> productRepo)
    {
        _context = context;
        _productRepo = productRepo;
    }

    public async Task<IEnumerable<Product>> EnsureProductsAsync()
    {
        var products = await GetProductsWithComponentsAsync();

        // Eski demo urunleri soft-delete ile pasife alinir.
        var removedNames = new[] { "Hafif Numune Paket", "Ağır Numune Paket" };
        var changed = false;
        foreach (var removedProduct in products.Where(p => removedNames.Contains(p.Ad) && p.Aktif))
        {
            removedProduct.Aktif = false;
            _productRepo.Update(removedProduct);
            changed = true;
        }

        var names = products.Where(p => p.Aktif).Select(p => p.Ad).ToHashSet();

        // Factory Method ile basit ve montaj urunleri olusturulur.
        if (!names.Contains("Kurşun Kalem"))
        {
            var pencil = ProductFactory.CreateProduct(ProductType.Basit, "Kurşun Kalem", 12, 0.03, 25);
            pencil.Aciklama = "Basit ürün / kırtasiye";
            pencil.StokMiktari = 120;
            await _productRepo.AddAsync(pencil);
            changed = true;
        }

        if (!names.Contains("A4 Defter"))
        {
            var notebook = ProductFactory.CreateProduct(ProductType.Basit, "A4 Defter", 85, 0.4, 15);
            notebook.Aciklama = "Basit ürün / ofis";
            notebook.StokMiktari = 40;
            await _productRepo.AddAsync(notebook);
            changed = true;
        }

        if (!names.Contains("Gaming Laptop"))
        {
            var laptop = ProductFactory.CreateProduct(ProductType.Basit, "Gaming Laptop", 25000, 2.5, 5);
            laptop.Aciklama = "Basit ürün / elektronik";
            laptop.StokMiktari = 10;
            await _productRepo.AddAsync(laptop);
            changed = true;
        }

        if (!names.Contains("Montajlı Bilgisayar Kasası"))
        {
            var pcCase = (AssemblyProduct)ProductFactory.CreateProduct(ProductType.Montaj, "Montajlı Bilgisayar Kasası", 42000, 4.2, 4);
            pcCase.Aciklama = "Montaj ürün / RAM, CPU, SSD ve güç kaynağı içerir";
            pcCase.StokMiktari = 6;
            pcCase.Bilesenler.Add(new ProductComponent { Ad = "RAM 32GB", Agirlik = 0.08, Miktar = 2 });
            pcCase.Bilesenler.Add(new ProductComponent { Ad = "CPU Ryzen 7", Agirlik = 0.05, Miktar = 1 });
            pcCase.Bilesenler.Add(new ProductComponent { Ad = "NVMe SSD 1TB", Agirlik = 0.04, Miktar = 1 });
            pcCase.Bilesenler.Add(new ProductComponent { Ad = "750W Güç Kaynağı", Agirlik = 1.6, Miktar = 1 });
            await _productRepo.AddAsync(pcCase);
            changed = true;
        }

        if (!names.Contains("Ergonomik Çalışma Masası"))
        {
            var desk = (AssemblyProduct)ProductFactory.CreateProduct(ProductType.Montaj, "Ergonomik Çalışma Masası", 6500, 12, 6);
            desk.Aciklama = "Montaj ürün / tabla, ayak seti ve bağlantı parçaları";
            desk.StokMiktari = 14;
            desk.Bilesenler.Add(new ProductComponent { Ad = "Ahşap Tabla", Agirlik = 8, Miktar = 1 });
            desk.Bilesenler.Add(new ProductComponent { Ad = "Metal Ayak Seti", Agirlik = 5, Miktar = 1 });
            desk.Bilesenler.Add(new ProductComponent { Ad = "Vida ve Bağlantı Seti", Agirlik = 0.4, Miktar = 1 });
            await _productRepo.AddAsync(desk);
            changed = true;
        }

        if (changed)
            await _productRepo.SaveChangesAsync();

        return await GetActiveProductsAsync();
    }

    private async Task<List<Product>> GetProductsWithComponentsAsync()
    {
        return await _context.Products
            .Include(p => ((AssemblyProduct)p).Bilesenler)
            .ToListAsync();
    }

    private async Task<List<Product>> GetActiveProductsAsync()
    {
        return await _context.Products
            .Include(p => ((AssemblyProduct)p).Bilesenler)
            .Where(p => p.Aktif)
            .ToListAsync();
    }
}
