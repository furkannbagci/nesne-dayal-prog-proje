using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TedarikLojistik.Web.Authorization;
using TedarikLojistik.Web.Data;
using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Interfaces.Repositories;
using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Models.Enums;
using TedarikLojistik.Web.Models.ViewModels;
using TedarikLojistik.Web.Services.Factories;
using TedarikLojistik.Web.Services.OrderStates;
using TedarikLojistik.Web.Services.Shipping;
using TedarikLojistik.Web.Services.Stock;

namespace TedarikLojistik.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private static readonly IReadOnlyList<DestinationOption> Destinations =
    [
        new("İstanbul", 30),
        new("Ankara", 450),
        new("İzmir", 480),
        new("Antalya", 720),
        new("Trabzon", 1060)
    ];

    private readonly IGenericRepository<Product> _productRepo;
    private readonly IGenericRepository<Order> _orderRepo;
    private readonly IStockSubject _stockManager;
    private readonly IEnumerable<IStockObserver> _stockObservers;
    private readonly Microsoft.AspNetCore.Identity.UserManager<AppUser> _userManager;
    private readonly AppDbContext _context;
    private readonly IAppLogger _logger;

    public HomeController(
        IGenericRepository<Product> productRepo,
        IGenericRepository<Order> orderRepo,
        IStockSubject stockManager,
        IEnumerable<IStockObserver> stockObservers,
        Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager,
        AppDbContext context,
        IAppLogger logger)
    {
        _productRepo = productRepo;
        _orderRepo = orderRepo;
        _stockManager = stockManager;
        _stockObservers = stockObservers;
        _userManager = userManager;
        _context = context;
        _logger = logger;

        foreach (var observer in _stockObservers)
        {
            _stockManager.Attach(observer);
        }
    }

    public IActionResult Index()
    {
        if (User.IsInRole(AppRoles.Admin))
            return RedirectToAction("Admin");

        if (User.IsInRole(AppRoles.DepoGorevlisi))
            return RedirectToAction("Admin");

        if (User.IsInRole(AppRoles.Kurye))
            return RedirectToAction("Logistics");

        return RedirectToAction("Customer");
    }

    [Authorize(Roles = AppRoles.Musteri)]
    public async Task<IActionResult> Customer()
    {
        var products = await EnsureProductsAsync();
        var currentUser = await _userManager.GetUserAsync(User);
        var orders = currentUser == null
            ? Enumerable.Empty<Order>()
            : await _context.Orders
                .Include(o => o.Kalemler)
                .Where(o => o.AppUserId == currentUser.Id)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

        return View(new CustomerDashboardViewModel
        {
            Products = products,
            Orders = orders,
            Destinations = Destinations
        });
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Musteri)]
    public async Task<IActionResult> CreateOrder(int productId, PaymentMethod paymentMethod, CargoCompany cargoCompany, string destinationCity, bool isFragile, bool isInsured)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null) return Challenge();

        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null) return NotFound();

        if (product.StokMiktari < 1)
        {
            TempData["Error"] = $"{product.Ad} için stok tükendi. Sipariş verilemez.";
            return RedirectToAction("Customer");
        }

        var destination = Destinations.FirstOrDefault(d => d.City == destinationCity) ?? Destinations[0];

        var order = new Order
        {
            SiparisNo = "ORD-" + Guid.NewGuid().ToString()[..4].ToUpper(),
            AppUserId = currentUser.Id,
            TeslimatAdresi = $"{destination.City} - {destination.DistanceKm} km",
            MesafeKm = destination.DistanceKm,
            Durum = OrderStatus.Beklemede,
            KargoFirmasi = cargoCompany,
            OdemeYontemi = paymentMethod
        };

        order.Kalemler.Add(new OrderItem
        {
            Product = product,
            UrunAdi = product.Ad,
            BirimFiyat = product.Fiyat,
            Miktar = 1
        });

        if (_stockManager is StockManager manager)
        {
            manager.DecreaseStock(product, 1);
            _productRepo.Update(product);
        }

        var shippingAdapter = ShippingAdapterFactory.Create(cargoCompany);
        // Firma adapterı ağırlık ve mesafeye göre temel kargo ücretini hesaplar.
        IShippingCostCalculator calculator = new AdapterShippingCost(shippingAdapter, destination.DistanceKm);
        if (isInsured) calculator = new InsuranceDecorator(calculator);
        if (isFragile) calculator = new FragileDecorator(calculator);

        order.KargoUcreti = calculator.CalculateCost(order);
        order.ToplamTutar = order.UrunToplami + order.KargoUcreti;
        order.OdemeTamamlandi = PaymentStrategyFactory.Create(order.OdemeYontemi).Pay(order);

        TempData["TrackingNo"] = shippingAdapter.CreateShipment(order);

        await _orderRepo.AddAsync(order);
        await _orderRepo.SaveChangesAsync();

        var username = User.Identity?.Name;
        _logger.LogInfo("Siparis", $"{order.SiparisNo} numaralı sipariş oluşturuldu.", username, order.Id);
        _logger.LogInfo("StokDegisimi", $"{product.Ad} stoğu sipariş nedeniyle 1 azaltıldı. Kalan stok: {product.StokMiktari}.", username, product.Id);
        _logger.LogInfo("Odeme", $"{order.SiparisNo} için {order.OdemeYontemi} ödemesi {(order.OdemeTamamlandi ? "onaylandı" : "onaylanamadı")}.", username, order.Id);
        if (product.StokEsikAltinda)
        {
            _logger.LogWarning("StokUyari", $"{product.Ad} kritik stok seviyesine düştü. Depo görevlisi paneline bildirim bırakıldı.", username, product.Id);
        }

        TempData["Message"] = $"Sipariş oluşturuldu. Toplam: {order.ToplamTutar:C}";
        return RedirectToAction("Customer");
    }

    [Authorize(Roles = AppRoles.AdminOrKurye)]
    public async Task<IActionResult> Logistics()
    {
        var orders = await _context.Orders
            .Include(o => o.AppUser)
            .Include(o => o.Kalemler)
            .OrderByDescending(o => o.Id)
            .ToListAsync();

        return View(orders);
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.AdminOrKurye)]
    public async Task<IActionResult> NextState(int orderId)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null) return RedirectToAction("Logistics");

        try
        {
            var oldStatus = order.Durum;
            OrderStateFactory.GetState(order.Durum).NextState(order);

            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();
            _logger.LogInfo("SiparisDurum", $"{order.SiparisNo} durumu {oldStatus} -> {order.Durum} olarak güncellendi.", User.Identity?.Name, order.Id);
            TempData["Message"] = $"Siparişin yeni durumu: {order.Durum}";
        }
        catch (Exception ex)
        {
            _logger.LogWarning("SiparisDurum", $"Sipariş durum güncellemesi reddedildi: {ex.Message}", User.Identity?.Name, order.Id);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Logistics");
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.AdminOrMusteri)]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var order = await GetAuthorizedOrderWithProductsAsync(orderId);
        if (order == null) return Forbid();

        try
        {
            var oldStatus = order.Durum;
            OrderStateFactory.GetState(order.Durum).Cancel(order);
            RestoreStock(order, "sipariş iptali");

            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();
            _logger.LogInfo("SiparisIptal", $"{order.SiparisNo} siparişi {oldStatus} durumundan iptal edildi.", User.Identity?.Name, order.Id);
            TempData["Message"] = "Sipariş iptal edildi ve ürün stoğu eski haline getirildi.";
        }
        catch (Exception ex)
        {
            _logger.LogWarning("SiparisIptal", $"Sipariş iptali reddedildi: {ex.Message}", User.Identity?.Name, order.Id);
            TempData["Error"] = ex.Message;
        }

        return RedirectAfterCustomerAction();
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.AdminOrMusteri)]
    public async Task<IActionResult> ReturnOrder(int orderId)
    {
        var order = await GetAuthorizedOrderWithProductsAsync(orderId);
        if (order == null) return Forbid();

        try
        {
            var oldStatus = order.Durum;
            OrderStateFactory.GetState(order.Durum).Return(order);
            RestoreStock(order, "sipariş iadesi");

            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();
            _logger.LogInfo("SiparisIade", $"{order.SiparisNo} siparişi {oldStatus} durumundan iade sürecine alındı.", User.Identity?.Name, order.Id);
            TempData["Message"] = "Sipariş iade sürecine alındı ve ürün stoğu eski haline getirildi.";
        }
        catch (Exception ex)
        {
            _logger.LogWarning("SiparisIade", $"Sipariş iade işlemi reddedildi: {ex.Message}", User.Identity?.Name, order.Id);
            TempData["Error"] = ex.Message;
        }

        return RedirectAfterCustomerAction();
    }

    [Authorize(Roles = AppRoles.AdminOrDepo)]
    public async Task<IActionResult> Admin()
    {
        var products = await EnsureProductsAsync();
        var warnings = User.IsInRole(AppRoles.DepoGorevlisi)
            ? await _context.SystemLogs
                .Where(l => l.Kategori == "StokUyari")
                .OrderByDescending(l => l.Id)
                .Take(6)
                .ToListAsync()
            : [];

        return View(new StockDashboardViewModel
        {
            Products = products,
            StockWarnings = warnings
        });
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.AdminOrDepo)]
    public async Task<IActionResult> DecreaseStock(int productId, int amount)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product != null && _stockManager is StockManager manager)
        {
            var oldStock = product.StokMiktari;
            // Stok eşik altına düşerse Observer bildirimleri tetiklenir.
            manager.DecreaseStock(product, amount);
            _productRepo.Update(product);
            await _productRepo.SaveChangesAsync();
            _logger.LogInfo("StokDegisimi", $"{product.Ad} stoğu yönetici tarafından {oldStock} -> {product.StokMiktari} olarak azaltıldı.", User.Identity?.Name, product.Id);
            TempData["Message"] = $"{product.Ad} stoğu azaltıldı. Kalan: {product.StokMiktari}.";
            if (product.StokEsikAltinda)
            {
                _logger.LogWarning("StokUyari", $"{product.Ad} kritik stok seviyesine düştü. Depo görevlisi paneline bildirim bırakıldı.", User.Identity?.Name, product.Id);
            }
        }

        return RedirectToAction("Admin");
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.AdminOrDepo)]
    public async Task<IActionResult> IncreaseStock(int productId, int amount)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product != null && amount > 0)
        {
            var oldStock = product.StokMiktari;
            product.StokMiktari += amount;
            _productRepo.Update(product);
            await _productRepo.SaveChangesAsync();
            _logger.LogInfo("StokDegisimi", $"{product.Ad} stoğu yönetici tarafından {oldStock} -> {product.StokMiktari} olarak artırıldı.", User.Identity?.Name, product.Id);
            TempData["Message"] = $"{product.Ad} stoğu artırıldı. Yeni stok: {product.StokMiktari}.";
        }

        return RedirectToAction("Admin");
    }

    private async Task<IEnumerable<Product>> EnsureProductsAsync()
    {
        var products = await _context.Products
            .Include(p => ((AssemblyProduct)p).Bilesenler)
            .ToListAsync();

        var removedNames = new[] { "Hafif Numune Paket", "Ağır Numune Paket" };
        var changed = false;
        foreach (var removedProduct in products.Where(p => removedNames.Contains(p.Ad) && p.Aktif))
        {
            removedProduct.Aktif = false;
            _productRepo.Update(removedProduct);
            changed = true;
        }

        var names = products.Where(p => p.Aktif).Select(p => p.Ad).ToHashSet();

        if (!names.Contains("Kurşun Kalem"))
        {
            var pencil = ProductFactory.CreateProduct(ProductType.Basit, "Kurşun Kalem", 12, 0.03, 25);
            pencil.Aciklama = "Basit ürün / kırtasiye";
            pencil.StokMiktari = 120;
            await _productRepo.AddAsync(pencil);
            changed = true;
        }

        if (!names.Contains("A4 Defter"))
        {
            var notebook = ProductFactory.CreateProduct(ProductType.Basit, "A4 Defter", 85, 0.4, 15);
            notebook.Aciklama = "Basit ürün / ofis";
            notebook.StokMiktari = 40;
            await _productRepo.AddAsync(notebook);
            changed = true;
        }

        if (!names.Contains("Gaming Laptop"))
        {
            var laptop = ProductFactory.CreateProduct(ProductType.Basit, "Gaming Laptop", 25000, 2.5, 5);
            laptop.Aciklama = "Basit ürün / elektronik";
            laptop.StokMiktari = 10;
            await _productRepo.AddAsync(laptop);
            changed = true;
        }

        if (!names.Contains("Montajlı Bilgisayar Kasası"))
        {
            var pcCase = (AssemblyProduct)ProductFactory.CreateProduct(ProductType.Montaj, "Montajlı Bilgisayar Kasası", 42000, 4.2, 4);
            pcCase.Aciklama = "Montaj ürün / RAM, CPU, SSD ve güç kaynağı içerir";
            pcCase.StokMiktari = 6;
            pcCase.Bilesenler.Add(new ProductComponent { Ad = "RAM 32GB", Agirlik = 0.08, Miktar = 2 });
            pcCase.Bilesenler.Add(new ProductComponent { Ad = "CPU Ryzen 7", Agirlik = 0.05, Miktar = 1 });
            pcCase.Bilesenler.Add(new ProductComponent { Ad = "NVMe SSD 1TB", Agirlik = 0.04, Miktar = 1 });
            pcCase.Bilesenler.Add(new ProductComponent { Ad = "750W Güç Kaynağı", Agirlik = 1.6, Miktar = 1 });
            await _productRepo.AddAsync(pcCase);
            changed = true;
        }

        if (!names.Contains("Ergonomik Çalışma Masası"))
        {
            var desk = (AssemblyProduct)ProductFactory.CreateProduct(ProductType.Montaj, "Ergonomik Çalışma Masası", 6500, 12, 6);
            desk.Aciklama = "Montaj ürün / tabla, ayak seti ve bağlantı parçaları";
            desk.StokMiktari = 14;
            desk.Bilesenler.Add(new ProductComponent { Ad = "Ahşap Tabla", Agirlik = 8, Miktar = 1 });
            desk.Bilesenler.Add(new ProductComponent { Ad = "Metal Ayak Seti", Agirlik = 5, Miktar = 1 });
            desk.Bilesenler.Add(new ProductComponent { Ad = "Vida ve Bağlantı Seti", Agirlik = 0.4, Miktar = 1 });
            await _productRepo.AddAsync(desk);
            changed = true;
        }

        if (changed)
            await _productRepo.SaveChangesAsync();

        return await _context.Products
            .Include(p => ((AssemblyProduct)p).Bilesenler)
            .Where(p => p.Aktif)
            .ToListAsync();
    }

    private async Task<Order?> GetAuthorizedOrderWithProductsAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Kalemler)
            .ThenInclude(k => k.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return null;
        if (User.IsInRole(AppRoles.Admin)) return order;

        var currentUser = await _userManager.GetUserAsync(User);
        return currentUser != null && order.AppUserId == currentUser.Id ? order : null;
    }

    private void RestoreStock(Order order, string reason)
    {
        // İptal ve iade sonrası ürünler depoya geri alınır.
        foreach (var item in order.Kalemler)
        {
            if (item.Product == null) continue;

            item.Product.StokMiktari += item.Miktar;
            _productRepo.Update(item.Product);
            _logger.LogInfo("StokDegisimi", $"{item.Product.Ad} stoğu {reason} nedeniyle {item.Miktar} artırıldı. Yeni stok: {item.Product.StokMiktari}.", User.Identity?.Name, item.Product.Id);
        }
    }

    private IActionResult RedirectAfterCustomerAction()
    {
        return User.IsInRole(AppRoles.Admin)
            ? RedirectToAction("Logistics")
            : RedirectToAction("Customer");
    }
}
