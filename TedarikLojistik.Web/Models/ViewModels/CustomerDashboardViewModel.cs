using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Models.ViewModels;

public class CustomerDashboardViewModel
{
    public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
    public IEnumerable<Order> Orders { get; set; } = Enumerable.Empty<Order>();
}
