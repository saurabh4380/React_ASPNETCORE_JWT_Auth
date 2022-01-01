using React_ASPNETCORE_JWT_Auth.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace React_ASPNETCORE_JWT_Auth.DataAccess.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        public UserRepository UserRepository { get; set; }

        public RefreshTokenRepository RefreshTokenRepository { get; set; }  
        public void SaveChanges();
    }
}
