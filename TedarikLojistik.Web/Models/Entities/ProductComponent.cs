namespace TedarikLojistik.Web.Models.Entities;

/// <summary>
/// Montaj ürününe ait alt bileşeni temsil eder.
/// AssemblyProduct içinde liste olarak tutulur.
/// </summary>
public class ProductComponent : BaseEntity
{
    /// <summary>Bileşenin adı</summary>
    public string Ad { get; set; } = string.Empty;

    /// <summary>Bileşen ağırlığı (kg)</summary>
    public double Agirlik { get; set; }

    /// <summary>Bu bileşenden kaç adet kullanılır</summary>
    public int Miktar { get; set; } = 1;

    /// <summary>Bağlı olduğu montaj ürünün Id'si (FK)</summary>
    public int AssemblyProductId { get; set; }

    /// <summary>Navigation property</summary>
    public AssemblyProduct? AssemblyProduct { get; set; }
}

