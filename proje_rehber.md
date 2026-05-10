# Proje: Akıllı Tedarik ve Lojistik Yönetim Sistemi - Web Uygulaması Geliştirme Talimatları

Sen uzman bir C# Yazılım Mimarı ve Kıdemli Geliştiricisin. Amacımız "Akıllı Tedarik ve Lojistik Yönetim Sistemi" projesini, Nesneye Dayalı Analiz ve Tasarım (OOAD) prensiplerine uygun, SOLID ilkelerini temel alan ve çalışan bir **ASP.NET Core MVC Web Uygulaması** olarak sıfırdan geliştirmektir.

Gereksiz onay soruları sorma veya her adımda benden izin bekleme. Sadece veritabanı seçimi (örn: In-Memory vs Entity Framework SQL) veya temel UI kütüphanesi gibi çok kritik teknik karar aşamalarında bana danış. Bunun dışında projeyi aşağıda belirtilen fazlara göre adım adım, modül modül kodlayarak bana sun. Tüm kodlarda açıklayıcı Türkçe yorum satırları kullan.

## 1. Mimari ve Teknik Gereksinimler
* **Mimari:** Uygulama ASP.NET Core MVC mimarisine uygun olarak katmanlandırılmalıdır (Core, Data, Services/Business, Web).
* **Temel Kural:** Kod yazarken `switch-case` veya iç içe geçmiş uzun `if-else` bloklarından kesinlikle kaçınılmalıdır. İş kurallarındaki her farklılaşma, uygun bir Tasarım Deseni (Design Pattern) ile polimorfik olarak çözülmelidir.
* **Prensipler:** Kod tabanı SOLID prensiplerine tam uyumlu olmalıdır (Özellikle Dependency Inversion).

## 2. Sistem Senaryosu ve İş Kuralları
Sistem, bir ana depo ve buna bağlı çalışan tedarikçi ile kargo birimlerini yönetir.
* **Roller:** Yönetici (Admin), Depo Görevlisi, Kurye/Müşteri. Rol bazlı yetkilendirme (Authorization) yapılmalıdır.
* **Ürün ve Stok:** Basit ürünler ve karmaşık/montaj gerektiren ürünler (alt bileşenli) olmalıdır. Stok eşik değerin altına düştüğünde Observer deseni ile ilgili birimlere (Email ve Sistem içi bildirim) haber verilmelidir.
* **Sipariş ve Ödeme:**
  * Sipariş Durumları: `Beklemede -> Onaylandı -> Hazırlanıyor -> Kargoda -> Teslim Edildi`. (Ürün hatalıysa Kargoda aşamasından İade'ye geçiş yapılabilir. Kargodaki ürün iptal edilemez).
  * Ödeme Yöntemleri: Kredi Kartı, Havale, Kripto Ödeme (Strategy deseni ile).
* **Kargo Stratejileri:**
  * Aras, Yurtiçi, GlobalExpres firmalarının sahte (mock) API'leri Adapter deseni ile tek tipe dönüştürülmelidir.
  * Kargo ücreti; ağırlık, mesafe ve ek hizmetlere (Sigorta, Kırılacak eşya) göre Decorator deseni ile dinamik hesaplanmalıdır.
* **Loglama:** Her kritik işlem (stok değişimi, ödeme) Singleton deseni kullanılarak bir dosyaya veya veritabanına loglanmalıdır.

## 3. Kullanılacak Tasarım Desenleri (Kesin Uygulanacaklar)
* **Creational (Yaratımsal):** 1. `Singleton` (Loglama mekanizması için).
  2. `Factory Method` veya `Builder` (Karmaşık ve basit ürünlerin üretimi için).
* **Structural (Yapısal):** 1. `Adapter` (Farklı kargo API'lerini standartlaştırmak için).
  2. `Decorator` (Kargo ücretine ek hizmet bedellerini eklemek için).
* **Behavioral (Davranışsal):** 1. `Observer` (Stok azaldığında bildirim göndermek için).
  2. `State` (Sipariş durum geçişleri ve kısıtlamaları için - if/else yığınlarını önlemek adına).
  3. `Strategy` (Farklı ödeme yöntemlerini yönetmek için).

## 4. Geliştirme Akışı (Senin İzleyeceğin Adımlar)

Lütfen aşağıdaki adımları sırayla, kodları eksiksiz vererek uygula. Bir adımı bitirdiğinde hemen diğerine geçebileceğini belirt veya doğrudan kodu vermeye devam et.

* **Adım 1: Proje Kurulumu ve Katmanların Tanımlanması**
  * ASP.NET Core MVC proje yapısını, klasör ağacını (Models/Domain, Interfaces, Services, Controllers) oluştur.
* **Adım 2: Domain Sınıfları ve Interface'ler**
  * Ürün, Sipariş, Kullanıcı vb. temel varlıkları (Entity) ve yukarıdaki tasarım desenleri için gereken Interface'leri tanımla.
* **Adım 3: Tasarım Desenlerinin Gerçeklenmesi (İş Mantığı)**
  * Pattern'leri (Singleton, Factory, Adapter, Decorator, Observer, State, Strategy) kurallara ve senaryoya uygun şekilde implemente et.
* **Adım 4: Controller ve View (Web Katmanı)**
  * Yazılan iş mantığını dışarıya açacak ASP.NET Core Controller sınıflarını oluştur.
  * Temel işlemleri test edebileceğimiz basit, temiz View (Razor .cshtml) veya API Endpoint (Eğer API tercih edersen) yapılarını oluştur.
* **Adım 5: Birim Testleri (Unit Tests)**
  * xUnit kullanarak yazdığın State, Strategy ve Decorator desenlerinin düzgün çalıştığını kanıtlayan kritik birim testlerini yaz.

Eğer talimatları anladıysan ve benden kritik bir teknik (örn: veritabanı tercihi) kararı beklemiyorsan, **doğrudan Adım 1'in klasör yapısı ve kodları ile** geliştirmeye başla.