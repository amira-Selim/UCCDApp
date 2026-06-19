using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UCCD_App.Context;
using UCCD_App.Models;

namespace UCCD_App.Repo;

public class GenericRepo<T> : IGenericRepo<T> where T : BaseEntity
{
    private readonly AppDbContext context;

    public GenericRepo(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? Criteria = null, List<Expression<Func<T, object>>>? Includes = null)
    {
        var query = context.Set<T>().AsQueryable();

        if (Criteria is not null)
            query = query.Where(Criteria);

        if (Includes is not null)
            foreach (var include in Includes)
            {
                query = query.Include(include);
            }

        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id, List<Expression<Func<T, object>>>? Includes = null)
    {
        var query = context.Set<T>().AsQueryable();

        if (Includes is not null)
            foreach (var include in Includes)
            {
                query = query.Include(include);
            }
        return await query.FirstOrDefaultAsync(b => b.Id == id);

    }

    public Task<int> GetCountAsync(Expression<Func<T, bool>>? Criteria = null)
    {
        throw new NotImplementedException();
    }

    public void Update(T Entity)
    {
        context.Update(Entity);
        context.SaveChanges();
    }
    public async Task AddAsync(T Entity)
    {
        await context.Set<T>().AddAsync(Entity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var entity = await GetByIdAsync(id);
        context.Set<T>().Remove(entity!);
        await context.SaveChangesAsync();
    }
}
