namespace TedarikLojistik.Web.Interfaces.Patterns;

/// <summary>
/// Singleton Pattern - Sistem genelinde kullanılacak loglama arayüzü.
/// Dependency Injection ile servislerde kullanılabilmesi için interface olarak tanımlanır,
/// ancak somut (concrete) sınıfı Singleton yapısında olacaktır.
/// </summary>
public interface IAppLogger
{
    void LogInfo(string category, string message, string? username = null, int? entityId = null);
    void LogWarning(string category, string message, string? username = null, int? entityId = null);
    void LogError(string category, string message, string? username = null, int? entityId = null);
}

