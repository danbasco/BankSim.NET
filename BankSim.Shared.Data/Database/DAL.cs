using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace BankSim.Database
{
    public class DAL<T> where T : class
    {

        private readonly BankSimContext context;

        public DAL(BankSimContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<T> GetAll() => context.Set<T>().AsNoTracking().ToList();

        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
            context.SaveChanges();
        }

        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
            context.SaveChanges();
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
            context.SaveChanges();
        }

        public T? GetBy(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>().FirstOrDefault(predicate);
        }

        public IQueryable<T> Query()
        {
            return context.Set<T>().AsNoTracking();
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

    }
}
