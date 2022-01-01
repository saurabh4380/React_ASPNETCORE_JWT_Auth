using React_ASPNETCORE_JWT_Auth.DataAccess.Entities;
using System.Linq.Expressions;

namespace React_ASPNETCORE_JWT_Auth.DataAccess.Repositories
{
     public interface IGenericRepository<T> where T : BaseModel
    {
        public void Add(T record);
        public T GetById(Guid id);
        public IEnumerable<T> GetAll();
        public IEnumerable<T> GetByFilter(Expression<Func<T, bool>> filter);
        public void Update(T record);
        public void Delete(T record);
        public void DeleteById(Guid id);
        //public void SaveChanges();
    }

}
