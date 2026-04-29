namespace TedarikLojistik.Web.Models.Enums;

/// <summary>
/// Ürün türlerini belirtir.
/// Factory Method hangi ürün sınıfını üretecekse buna göre karar verir.
/// </summary>
public enum ProductType
{
    Basit = 0,     // Tek parça, montaj gerektirmeyen ürün
    Montaj = 1     // Alt bileşenlerden oluşan karmaşık ürün
}

