using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Datos; 
using ProyectoIdentity.Repositories.Interfaces;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProyectoIdentity.Repositories.Implementations
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<T> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Esto asume que la clave primaria se puede pasar como un solo objeto a FindAsync
            // Si la PK no es simple (ej. clave compuesta), deberías ajustar esto.
            // Para Abogado, Cliente, etc. que tienen PK int, funciona.
            // Alternativa más robusta si FindAsync falla por PK compleja:
            // var parameter = Expression.Parameter(typeof(T), "e");
            // var property = Expression.Property(parameter, "Id"); // Asume que la PK se llama 'Id'
            // var lambda = Expression.Lambda<Func<T, bool>>(Expression.Equal(property, Expression.Constant(id)), parameter);
            // return await query.FirstOrDefaultAsync(lambda);
            return await query.SingleOrDefaultAsync(e => EF.Property<int>(e, "AbogadoId") == id); // Usa EF.Property si la PK no es "Id"
        }

        // NUEVO: Implementación de GetAllWithIncludesAsync
        public async Task<T> GetSingleOrDefaultWithIncludesAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Aplica el predicado para encontrar el elemento (ej. e => e.DocumentoId == id)
            return await query.SingleOrDefaultAsync(predicate); // <-- ESTA ES LA LÍNEA CLAVE
        }

        public async Task<IEnumerable<T>> GetAllWithIncludesAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            // Nota: Esto asume que la PK es 'Id' o 'EntidadId'. Si usas otros nombres,
            // necesitarás un método específico o usar FindAsync.
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    
    }
}