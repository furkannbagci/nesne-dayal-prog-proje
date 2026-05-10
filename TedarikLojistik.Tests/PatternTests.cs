using Xunit;
using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Models.Enums;
using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Services.OrderStates;
using TedarikLojistik.Web.Services.Payments;
using TedarikLojistik.Web.Services.Shipping;
using TedarikLojistik.Web.Services.Logging;

namespace TedarikLojistik.Tests;

public class PatternTests
{
    #region SINGLETON PATTERN TESTLERI

    [Fact]
    public void AppLogger_Should_Return_Same_Instance()
    {
        var firstLogger = AppLogger.Instance;
        var secondLogger = AppLogger.Instance;

        Assert.Same(firstLogger, secondLogger);
    }

    #endregion

    #region STATE PATTERN TESTLERİ

    [Fact]
    public void OrderState_Should_Transition_From_Pending_To_Approved()
    {
        // Arrange - Beklemede bir sipariş oluşturulur
        var order = new Order { Durum = OrderStatus.Beklemede };
        var state = OrderStateFactory.GetState(order.Durum);

        // Act - Bir sonraki aşamaya geçmesi istenir
        state.NextState(order);

        // Assert - Yeni durum "Onaylandı" olmalıdır
        Assert.Equal(OrderStatus.Onaylandi, order.Durum);
    }

    [Fact]
    public void OrderState_InShipment_Cannot_Be_Canceled()
    {
        // Arrange - Kargoya verilmiş bir sipariş durumu
        var order = new Order { Durum = OrderStatus.Kargoda };
        var state = OrderStateFactory.GetState(order.Durum);

        // Act & Assert - Kargodaki ürün iptal edilmeye çalışıldığında InvalidOperationException fırlatmalıdır
        var exception = Assert.Throws<InvalidOperationException>(() => state.Cancel(order));
        Assert.Contains("Kargoya verilmiş ürün iptal edilemez", exception.Message);
    }

    [Fact]
    public void OrderState_InShipment_Cannot_Be_Returned()
    {
        var order = new Order { Durum = OrderStatus.Kargoda };
        var state = OrderStateFactory.GetState(order.Durum);

        var exception = Assert.Throws<InvalidOperationException>(() => state.Return(order));

        Assert.Contains("Kargodaki sipariş iade edilemez", exception.Message);
    }

    [Fact]
    public void OrderState_Delivered_Can_Be_Returned()
    {
        var order = new Order { Durum = OrderStatus.TeslimEdildi };
        var state = OrderStateFactory.GetState(order.Durum);

        state.Return(order);

        Assert.Equal(OrderStatus.Iade, order.Durum);
    }

    #endregion

    #region STRATEGY PATTERN TESTLERİ

    [Fact]
    public void PaymentStrategy_Should_Return_True_For_Valid_Order()
    {
        // Arrange - 1000 TL'lik bir sipariş ve Kredi Kartı stratejisi
        var order = new Order { ToplamTutar = 1000m };
        IPaymentStrategy creditCardPayment = new CreditCardPayment();

        // Act - Ödeme alınır
        bool isSuccess = creditCardPayment.Pay(order);

        // Assert
        Assert.True(isSuccess);
    }

    #endregion

    #region DECORATOR PATTERN TESTLERİ

    [Fact]
    public void Decorator_Should_Calculate_Correct_Shipping_Cost_With_Fragile_And_Insurance()
    {
        // Arrange
        var order = new Order();
        var simpleProduct = new SimpleProduct { Fiyat = 2000, Agirlik = 5 }; // Fiyat: 2000 TL, Ağırlık: 5 Kg
        order.Kalemler.Add(new OrderItem { Product = simpleProduct, BirimFiyat = simpleProduct.Fiyat, Miktar = 1 });
        
        // Act - İç içe geçirme (Decoration) işlemi
        // 1. Temel Hesap (5 Kg * 10 TL = 50 TL)
        IShippingCostCalculator baseCalculator = new StandardShippingCost(); 
        
        // 2. Sigorta Ekleniyor (Ürün toplamının %5'i -> 2000 * 0.05 = +100 TL)
        IShippingCostCalculator insuredCalculator = new InsuranceDecorator(baseCalculator); 
        
        // 3. Kırılacak Eşya bedeli ekleniyor (+30 TL)
        IShippingCostCalculator fullyDecoratedCalculator = new FragileDecorator(insuredCalculator); 

        decimal finalCost = fullyDecoratedCalculator.CalculateCost(order);

        // Assert - Beklenen toplam kargo bedeli: 50 + 100 + 30 = 180 TL
        Assert.Equal(180m, finalCost);
    }

    #endregion
}

