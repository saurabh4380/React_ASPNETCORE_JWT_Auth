using React_ASPNETCORE_JWT_Auth.DataAccess.Configurations;
using React_ASPNETCORE_JWT_Auth.DataAccess.Repositories;

namespace React_ASPNETCORE_JWT_Auth.Extensions
{
    public static class ServicesConfiguration
    {
        public static void AddDBServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(); //Adds DbContext with Scoped Lifetime

            services.Configure<DBSettings>(configuration.GetSection("DBSettings"));

            services.AddTransient(typeof(IGenericRepository<>), typeof(DbRepository<>));

            services.AddTransient<UserRepository, UserRepository>();

            services.AddTransient<RefreshTokenRepository, RefreshTokenRepository>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
