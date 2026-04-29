using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Interfaces.Patterns;

namespace TedarikLojistik.Web.Services.Stock;

/// <summary>
/// Observer Pattern - Subject Implementasyonu.
/// Stok hareketlerini yönetir ve stok kritik seviyeye inerse abonelere haber verir.
/// </summary>
public class StockManager : IStockSubject
{
    private readonly List<IStockObserver> _observers = new();

    public void Attach(IStockObserver observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void Detach(IStockObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(Product product)
    {
        foreach (var observer in _observers)
        {
            observer.Update(product);
        }
    }

    public void DecreaseStock(Product product, int quantity)
    {
        product.StokMiktari -= quantity;
        
        // Stok eşiğin altına düştüyse dinleyicileri (Email, Sistem) tetikle
        if (product.StokEsikAltinda)
        {
            Notify(product);
        }
    }
}

