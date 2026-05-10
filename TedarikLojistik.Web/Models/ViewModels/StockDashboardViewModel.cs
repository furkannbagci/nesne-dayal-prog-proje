using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Models.ViewModels;

public class StockDashboardViewModel
{
    public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
    public IEnumerable<SystemLog> StockWarnings { get; set; } = Enumerable.Empty<SystemLog>();
}
