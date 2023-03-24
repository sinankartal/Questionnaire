using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence.Data;
using Persistence.IRepositories;
using Persistence.Models;

namespace Persistence;

public abstract class Repository<T> : IRepository<T> where T : BaseEntity
{
    #region property

    private readonly QuestionnaireDbContext _questionnaireDbContext;
    private DbSet<T> entities;

    #endregion

    #region Constructor

    protected Repository(QuestionnaireDbContext questionnaireDbContext)
    {
        _questionnaireDbContext = questionnaireDbContext;
        entities = _questionnaireDbContext.Set<T>();
    }

    #endregion

    public virtual ValueTask<T?> FindAsync(int id)
    {
        return entities.FindAsync(id);
    }

    public T Find(int id)
    {
        return entities.Find(id);
    }

    public virtual Task<bool> Exists(int id)
    {
        return entities.AnyAsync(e=>e.Id.Equals(id));
    }

    public virtual Task SaveAsync()
    {
         _questionnaireDbContext.SaveChangesAsync();
         return Task.CompletedTask;
    }

    public virtual Task Add(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException("entity");
        }
        
        entities.AddAsync(entity);
        _questionnaireDbContext.SaveChanges();

        return Task.CompletedTask;
    }
    
    public virtual Task AddRange(List<T> entityList)
    {
        if (entityList.IsNullOrEmpty())
        {
            throw new ArgumentNullException("entities");
        }

        entities.AddRangeAsync(entityList);
        _questionnaireDbContext.SaveChanges();

        return Task.CompletedTask;
    }
    
}