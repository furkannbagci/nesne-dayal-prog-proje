using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Models.Enums;
using TedarikLojistik.Web.Services.Payments;

namespace TedarikLojistik.Web.Services.Factories;

public static class PaymentStrategyFactory
{
    private static readonly IReadOnlyDictionary<PaymentMethod, Func<IPaymentStrategy>> Strategies =
        new Dictionary<PaymentMethod, Func<IPaymentStrategy>>
        {
            [PaymentMethod.KrediKarti] = () => new CreditCardPayment(),
            [PaymentMethod.Havale] = () => new BankTransferPayment(),
            [PaymentMethod.Kripto] = () => new CryptoPayment()
        };

    public static IPaymentStrategy Create(PaymentMethod method)
    {
        if (Strategies.TryGetValue(method, out var strategyFactory))
            return strategyFactory();

        throw new ArgumentException("Bilinmeyen ödeme yöntemi");
    }
}
