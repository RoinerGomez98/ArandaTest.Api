using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using ArandaTest.Domain.Interfaces;
using ArandaTest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ArandaTest.Infrastructure.Repositories
{
    public class AppRepository<T>(AppDbContext context) : IAppRepository<T> where T : class
    {
        private readonly AppDbContext _context = context;

        public async Task<T?> GetByIdAsync(Guid id, bool includeNavigations = false)
        {
            var query = _context.Set<T>().AsQueryable();

            if (includeNavigations)
            {
                var navigationProperties = typeof(T)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => typeof(IEnumerable).IsAssignableFrom(p.PropertyType) == false &&
                                p.PropertyType.IsClass &&
                                p.PropertyType != typeof(string));

                foreach (var navProp in navigationProperties)
                {
                    query = query.Include(navProp.Name);
                }
            }

            var keyProperty = (_context.Model.FindEntityType(typeof(T))?
                .FindPrimaryKey()?
                .Properties
                .FirstOrDefault()) ?? throw new InvalidOperationException($"No se encontró clave primaria para {typeof(T).Name}");
            var keyName = keyProperty.Name;

            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, keyName);
            var constant = Expression.Constant(id);
            var equality = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

            return await query.FirstOrDefaultAsync(lambda);
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Func<IQueryable<T>, IQueryable<T>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? sort = null,
            int page = 1,
            int pageSize = 10, bool includeNavigations = false)
        {
            var query = _context.Set<T>().AsQueryable();

            if (includeNavigations)
            {
                var navigationProperties = typeof(T)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => typeof(IEnumerable).IsAssignableFrom(p.PropertyType) == false &&
                                p.PropertyType.IsClass &&
                                p.PropertyType != typeof(string));

                foreach (var navProp in navigationProperties)
                {
                    query = query.Include(navProp.Name);
                }
            }

            if (filter != null)
                query = filter(query);

            if (sort != null)
                query = sort(query);

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(Func<IQueryable<T>, IQueryable<T>>? filter = null)
        {
            var query = _context.Set<T>().AsQueryable();

            if (filter != null)
                query = filter(query);

            return await query.CountAsync();
        }

        public async Task<T> CreateAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }
    }
}
