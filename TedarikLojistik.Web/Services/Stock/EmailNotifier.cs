using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Services.Logging;

namespace TedarikLojistik.Web.Services.Stock;

public class EmailNotifier : IStockObserver
{
    public void Update(Product product)
    {
        AppLogger.Instance.LogWarning("StokUyari",
            $"[SATIN ALMA E-POSTA] {product.Ad} ürününün stoğu eşik altına düştü. Mevcut: {product.StokMiktari}, Eşik: {product.StokEsikDegeri}. Tedarik süreci başlatılmalı.");
    }
}
