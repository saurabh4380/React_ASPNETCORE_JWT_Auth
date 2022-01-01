using Microsoft.EntityFrameworkCore;
using React_ASPNETCORE_JWT_Auth.DataAccess.Entities;
using System.Linq.Expressions;

namespace React_ASPNETCORE_JWT_Auth.DataAccess.Repositories
{
    public class DbRepository<TDocument> : IGenericRepository<TDocument> where TDocument : BaseModel
    {
        protected readonly ApplicationDbContext _context;
        protected DbSet<TDocument> _dbSet { get; set; }

        public DbRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
            _dbSet = _context.Set<TDocument>();
        }
        public void Add(TDocument record)
        {
            _context.Add(record);
        }

        public IEnumerable<TDocument> GetAll()
        {
           return _dbSet.AsEnumerable();
        }

        public IEnumerable<TDocument> GetByFilter(Expression<Func<TDocument, bool>> filter)
        {
            return _dbSet.Where(filter).AsEnumerable();
        }

        public TDocument GetById(Guid id)
        {
            return _dbSet.Find(id);
        }

        //public void SaveChanges()
        //{
        //    _context.SaveChanges();
        //}

        public void Update(TDocument record)
        {
            _dbSet.Update(record);
        }

        public void Delete(TDocument record)
        {
            _dbSet.Remove(record);
        }

        public void DeleteById(Guid id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
    }
}
