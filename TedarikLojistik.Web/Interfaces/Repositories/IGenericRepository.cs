using TedarikLojistik.Web.Models.Entities;

namespace TedarikLojistik.Web.Interfaces.Repositories;

/// <summary>
/// Repository Pattern - Temel CRUD işlemlerini standartlaştıran Generic arayüz.
/// </summary>
/// <typeparam name="T">BaseEntity'den türeyen herhangi bir sınıf</typeparam>
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    
    // Veritabanı işlemlerini commit etmek için
    Task<int> SaveChangesAsync();
}

