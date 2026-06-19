using System.Linq.Expressions;
using UCCD_App.Models;
namespace UCCD_App.Repo;

public interface IGenericRepo<T> where T : BaseEntity
{
    Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? Criteria = null,List<Expression<Func<T,object>>>? Includes = null);
    Task<T?> GetByIdAsync(int id,List<Expression<Func<T, object>>>? Includes = null);
    Task<int> GetCountAsync(Expression<Func<T, bool>>? Criteria = null);

    Task AddAsync(T Entity);
    void Update(T Entity);
    Task Delete(int id);
}
