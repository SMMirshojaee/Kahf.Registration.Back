global using Registration.API.Entity.Models;
global using Registration.API.Entity;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Registration.API.Business;

public class GenericBusiness<T>(RegContext context) where T : BaseEntity
{
    private RegContext Context => context;

    private IQueryable<T> Command => Context.Set<T>();
    private IQueryable<T> Query => Context.Set<T>().AsNoTracking();

    internal Task<List<T>> GetAll() => Query.ToListAsync();
    internal IQueryable<T> Where(Expression<Func<T, bool>> condition, bool track = false) =>
        (track ? Command : Query).Where(condition);

    internal Task<T?> GetById(short id, bool track = false) =>
        (track ? Command : Query).FirstOrDefaultAsync(e => e.Id == id);
}