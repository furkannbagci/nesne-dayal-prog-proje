using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Interfaces.Patterns;

/// <summary>
/// State Pattern - Sipariş durumlarının (Beklemede, Onaylandı, vs.) davranışlarını tanımlar.
/// İç içe if-else bloklarını önlemek için her durum kendi geçiş kurallarını bilir.
/// </summary>
public interface IOrderState
{
    /// <summary>
    /// Siparişi bir sonraki adıma geçirir (Örn: Beklemede -> Onaylandı).
    /// Eğer geçiş kurallara aykırıysa Exception fırlatır.
    /// </summary>
    void NextState(Order order);
    
    /// <summary>
    /// Siparişi iptal eder. Her durum iptale izin vermeyebilir (Örn: Kargodaki ürün iptal edilemez).
    /// </summary>
    void Cancel(Order order);

    /// <summary>
    /// Siparişi iade sürecine alır. Bu işlem sadece uygun durumlarda yapılabilir.
    /// </summary>
    void Return(Order order);
    
    /// <summary>
    /// Şu anki durumun adını döner.
    /// </summary>
    string GetStatusName();
}

