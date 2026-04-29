using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Data;

/// <summary>
/// Projenin ana veritabanı bağlamı.
/// IdentityDbContext'den miras alınarak kullanıcı tabloları otomatik sağlanır.
/// </summary>
public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<SimpleProduct> SimpleProducts { get; set; }
    public DbSet<AssemblyProduct> AssemblyProducts { get; set; }
    public DbSet<ProductComponent> ProductComponents { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<SystemLog> SystemLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Identity tabloları için zorunlu

        // Table-Per-Hierarchy (TPH) stratejisi: Product sınıfları (Simple/Assembly) tek tabloda.
        builder.Entity<Product>()
            .HasDiscriminator<string>("ProductTypeString")
            .HasValue<SimpleProduct>("Simple")
            .HasValue<AssemblyProduct>("Assembly");

        // OrderItem -> Order ilişkisi
        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Kalemler)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProductComponent -> AssemblyProduct ilişkisi
        builder.Entity<ProductComponent>()
            .HasOne(pc => pc.AssemblyProduct)
            .WithMany(ap => ap.Bilesenler)
            .HasForeignKey(pc => pc.AssemblyProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

