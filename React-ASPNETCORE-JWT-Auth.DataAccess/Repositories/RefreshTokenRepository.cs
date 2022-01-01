using Microsoft.EntityFrameworkCore;
using React_ASPNETCORE_JWT_Auth.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace React_ASPNETCORE_JWT_Auth.DataAccess.Repositories
{
    public class RefreshTokenRepository : DbRepository<RefreshToken>
    {
        public RefreshTokenRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
           
        }

        public RefreshToken GetRefreshTokenWithUser(string token)
        {
            return _dbSet.Where(x => x.Token == token).Include(x => x.User).FirstOrDefault();
        }
    }
}
