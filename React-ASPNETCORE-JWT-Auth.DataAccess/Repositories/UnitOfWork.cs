namespace React_ASPNETCORE_JWT_Auth.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private ApplicationDbContext _applicationDbContext;
        private bool _disposed;


        public UnitOfWork(ApplicationDbContext applicationDbContext, UserRepository userRepository, RefreshTokenRepository genericRepository)
        {
            _applicationDbContext = applicationDbContext;
            UserRepository = userRepository;
            RefreshTokenRepository = genericRepository;
        }
        public UserRepository UserRepository { get; set; }
        public RefreshTokenRepository RefreshTokenRepository { get; set; }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _applicationDbContext.Dispose();
                }
            }
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SaveChanges()
        {
            _applicationDbContext.SaveChanges();
        }
    }
}
