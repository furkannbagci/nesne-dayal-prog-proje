using Microsoft.Extensions.DependencyInjection;
using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Interfaces.Patterns;
using TedarikLojistik.Web.Data;

namespace TedarikLojistik.Web.Services.Logging;

/// <summary>
/// Singleton Pattern - Thread-safe loglama nesnesi.
/// Veritabanına (SystemLog) asenkron log yazar.
/// DI kullanılarak IAppLogger olarak servis edilir, ancak instance tektir.
/// </summary>
public class AppLogger : IAppLogger
{
    // Lazy kullanımı sistem genelinde tek logger örneği oluşturur.
    private static readonly Lazy<AppLogger> _instance = new Lazy<AppLogger>(() => new AppLogger());
    
    // IServiceProvider, singleton içinde scope'lu DbContext kullanabilmek için gereklidir
    private IServiceProvider? _serviceProvider;

    private AppLogger() { }

    public static AppLogger Instance => _instance.Value;

    public void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private void Log(string level, string category, string message, string? username, int? entityId)
    {
        if (_serviceProvider == null) return;

        // Singleton içinde yeni bir scope açarak veritabanı işlemi yapıyoruz
        Task.Run(async () => 
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var log = new SystemLog
                {
                    Seviye = level,
                    Kategori = category,
                    Mesaj = message,
                    KullaniciAdi = username,
                    IlgiliEntityId = entityId
                };

                context.SystemLogs.Add(log);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // DB loglama hatası, console'a düşsün
                Console.WriteLine($"LOG ERROR: {ex.Message}");
            }
        });
    }

    public void LogInfo(string category, string message, string? username = null, int? entityId = null) => 
        Log("INFO", category, message, username, entityId);

    public void LogWarning(string category, string message, string? username = null, int? entityId = null) => 
        Log("WARNING", category, message, username, entityId);

    public void LogError(string category, string message, string? username = null, int? entityId = null) => 
        Log("ERROR", category, message, username, entityId);
}

