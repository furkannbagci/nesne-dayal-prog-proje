using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Models.Enums;
using TedarikLojistik.Web.Services.Shipping;

namespace TedarikLojistik.Web.Services.Factories;

public static class ShippingAdapterFactory
{
    private static readonly IReadOnlyDictionary<CargoCompany, Func<IShippingAdapter>> Adapters =
        new Dictionary<CargoCompany, Func<IShippingAdapter>>
        {
            [CargoCompany.Aras] = () => new ArasAdapter(),
            [CargoCompany.Yurtici] = () => new YurticiAdapter(),
            [CargoCompany.GlobalExpres] = () => new GlobalExpresAdapter()
        };

    public static IShippingAdapter Create(CargoCompany company)
    {
        if (Adapters.TryGetValue(company, out var adapterFactory))
            return adapterFactory();

        throw new ArgumentException("Bilinmeyen kargo firması");
    }
}
