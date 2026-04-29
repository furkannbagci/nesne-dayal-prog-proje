using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Services.Logging;

namespace TedarikLojistik.Web.Services.Stock;

public class SystemNotifier : IStockObserver
{
    public void Update(Product product)
    {
        AppLogger.Instance.LogWarning("StokUyari",
            $"[DEPO SORUMLUSU SİSTEM BİLDİRİMİ] {product.Ad} kritik stok seviyesinde. Depo kontrolü gerekiyor.");
    }
}
