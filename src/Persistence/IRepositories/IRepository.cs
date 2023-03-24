namespace Persistence.IRepositories;

public interface IRepository<T>
{
    Task Add(T entity);
    
    ValueTask<T?> FindAsync(int id);

    T Find(int id);

    Task<bool> Exists(int id);

    Task SaveAsync();

    Task AddRange(List<T> entities);
}