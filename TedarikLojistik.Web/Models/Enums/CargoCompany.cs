namespace TedarikLojistik.Web.Models.Enums;

/// <summary>
/// Desteklenen kargo firmalarını tanımlar.
/// Her firma için ayrı bir Adapter sınıfı mevcuttur.
/// </summary>
public enum CargoCompany
{
    Aras = 0,          // Aras Kargo
    Yurtici = 1,       // Yurtiçi Kargo
    GlobalExpres = 2   // Global Expres (uluslararası)
}

