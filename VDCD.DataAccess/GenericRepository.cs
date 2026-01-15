using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace VDCD.DataAccess
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> Raw => _dbSet.AsQueryable();

        // ----------------------
        // Gets: read/write hoặc read-only
        // ----------------------
        public IEnumerable<T> Gets(bool isReadOnly,
                                   Expression<Func<T, bool>> spec = null,
                                   Func<IQueryable<T>, IQueryable<T>> preFilter = null,
                                   params Func<IQueryable<T>, IQueryable<T>>[] postFilter)
        {
            IQueryable<T> query = isReadOnly ? _dbSet.AsNoTracking() : _dbSet;

            if (spec != null)
                query = query.Where(spec);

            if (preFilter != null)
                query = preFilter(query);

            if (postFilter != null)
            {
                foreach (var filter in postFilter)
                    query = filter(query);
            }

            return query.ToList();
        }

        public IEnumerable<T> GetsReadOnly(Expression<Func<T, bool>> spec = null,
                                   Func<IQueryable<T>, IQueryable<T>> preFilter = null,
                                   params Func<IQueryable<T>, IQueryable<T>>[] postFilter)
        {
            return Gets(true, spec, preFilter, postFilter);
        }

        public IEnumerable<TOutput> GetsAs<TOutput>(Expression<Func<T, TOutput>> projector,
                                                    Expression<Func<T, bool>> spec = null,
                                                    Func<IQueryable<T>, IQueryable<T>> preFilter = null,
                                                    params Func<IQueryable<T>, IQueryable<T>>[] postFilter)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (spec != null)
                query = query.Where(spec);

            if (preFilter != null)
                query = preFilter(query);

            if (postFilter != null)
            {
                foreach (var filter in postFilter)
                    query = filter(query);
            }

            return query.Select(projector).ToList();
        }

        // ----------------------
        // Get single entity
        // ----------------------
        public T Get(params object[] ids) => _dbSet.Find(ids);

        public T Get(bool isReadOnly, Expression<Func<T, bool>> spec)
        {
            IQueryable<T> query = isReadOnly ? _dbSet.AsNoTracking() : _dbSet;
            return query.FirstOrDefault(spec);
        }

        public T GetReadOnly(Expression<Func<T, bool>> spec)
        {
            return Get(true, spec);
        }

        public TOutput GetAs<TOutput>(Expression<Func<T, TOutput>> projector,
                                      Expression<Func<T, bool>> specs = null)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (specs != null)
                query = query.Where(specs);

            return query.Select(projector).FirstOrDefault();
        }

        // ----------------------
        // Count / Exist
        // ----------------------
        public bool Exist(Expression<Func<T, bool>> spec = null)
        {
            if (spec == null)
                return _dbSet.Any();
            return _dbSet.Any(spec);
        }

        public int Count(Expression<Func<T, bool>> spec = null)
        {
            if (spec == null)
                return _dbSet.Count();
            return _dbSet.Count(spec);
        }

        // ----------------------
        // CRUD
        // ----------------------
        public void Create(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
        public void DeleteRange(IEnumerable<T> entity)
        {
            _dbSet.RemoveRange(entity);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
