using Microsoft.EntityFrameworkCore;
using TedarikLojistik.Web.Models.Entities;
using TedarikLojistik.Web.Interfaces.Repositories;
using TedarikLojistik.Web.Data;

namespace TedarikLojistik.Web.Data.Repositories;

/// <summary>
/// IGenericRepository implementasyonu.
/// Temel CRUD işlemlerini merkezi olarak EF Core üzerinden gerçekleştirir.
/// </summary>
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        entity.Aktif = false; // Soft-Delete
        Update(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

