global using Registration.API.Entity.Models;
global using Registration.API.Entity;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Registration.API.Common;
using System.Net;

namespace Registration.API.Business;

public class GenericBusiness<T>(RegContext context, IMapper mapper) where T : BaseEntity
{
    private RegContext Context => context;
    public IMapper Mapper => mapper;
    private IQueryable<T> Command => Context.Set<T>();
    private IQueryable<T> Query => Context.Set<T>().AsNoTracking();
    private DbSet<T> Entity => Context.Set<T>();

    internal Task<List<T>> GetAll() => Query.ToListAsync();
    internal IQueryable<T> Where(Expression<Func<T, bool>> condition, bool track = false) =>
        (track ? Command : Query).Where(condition);
    internal Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> condition, bool track = false) =>
        (track ? Command : Query).FirstOrDefaultAsync(condition);

    internal Task<T?> GetById(int id, bool track = false) =>
        (track ? Command : Query).FirstOrDefaultAsync(e => e.Id == id);

    internal async Task<ActionReport> Add(T entity)
    {
        try
        {
            await context.AddAsync(entity);
            await context.SaveChangesAsync();
            return ActionReport.Success();
        }
        catch (Exception e)
        {
            return new ActionReport
            {
                Code = HttpStatusCode.InternalServerError,
                Exception = e,
                Message = e.Message
            };
        }
    }
    internal async Task<ActionReport> Add(List<T> entities)
    {
        try
        {
            await context.AddRangeAsync(entities);
            await context.SaveChangesAsync();
            return ActionReport.Success();
        }
        catch (Exception e)
        {
            return new ActionReport
            {
                Code = HttpStatusCode.InternalServerError,
                Exception = e,
                Message = e.Message
            };
        }
    }

    internal async Task<ActionReport> Delete(int id)
    {
        try
        {
            T? entity = await FirstOrDefaultAsync(e => e.Id == id);
            if (entity is null)
                return ActionReport.Error(HttpStatusCode.NotFound);
            Entity.Remove(entity);
            return await SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    internal async Task<ActionReport> SaveChanges()
    {
        try
        {
            await Context.SaveChangesAsync();
            return ActionReport.Success();
        }
        catch (Exception e)
        {
            return ActionReport.Error(HttpStatusCode.InternalServerError, "خطا در ثبت تغییرات", e);
        }
    }
}