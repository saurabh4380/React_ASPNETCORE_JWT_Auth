using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using React_ASPNETCORE_JWT_Auth.DataAccess.Entities;
using React_ASPNETCORE_JWT_Auth.DataAccess.Configurations;
using Microsoft.EntityFrameworkCore;

namespace React_ASPNETCORE_JWT_Auth.DataAccess.Repositories
{
    public class UserRepository : DbRepository<User>
    {
        public UserRepository(IOptions<DBSettings> mongoDbSettings, ApplicationDbContext dbContext):base(dbContext)
        {

        }

        public User GetUserWithRefreshTokens(Guid id)
        {
            return _dbSet.Where(x => x.Id == id).Include(x => x.RefreshTokens).SingleOrDefault();

          
        }

        public User GetUserWithRefreshTokens(string emailId)
        {
            return _dbSet.Where(x => x.EmailId == emailId).Include(x => x.RefreshTokens).FirstOrDefault();

        }

        public long GetUserCount()
        {
            return _dbSet.LongCount();
        }

    }
}
