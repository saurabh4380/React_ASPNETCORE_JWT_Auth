using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using React_ASPNETCORE_JWT_Auth.DataAccess.Configurations;
using React_ASPNETCORE_JWT_Auth.DataAccess.Entities;

namespace React_ASPNETCORE_JWT_Auth.DataAccess.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        private DBSettings _dBSettings;
        public ApplicationDbContext(IOptions<DBSettings> dbSettings)
        {
            _dBSettings = dbSettings.Value;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseNpgsql(_dBSettings.ConnectionString, opt => opt.MigrationsAssembly("React-ASPNETCORE-JWT-Auth.DataAccess"));

            if (_dBSettings.IsLoggingEnabled)
            {
                optionsBuilder.EnableSensitiveDataLogging().LogTo(Console.WriteLine);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            new UserEntityConfiguration().Configure(modelBuilder.Entity<User>());

            new RefreshTokenEntityConfiguration().Configure(modelBuilder.Entity<RefreshToken>()); 

        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries().Where(x => x.Entity is BaseModel && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach(var entityEntry in entries)
            {
                ((BaseModel)entityEntry.Entity).ModificationDate = DateTime.UtcNow;

                var creationDate = ((BaseModel)entityEntry.Entity).CreationDate;

                if (entityEntry.State == EntityState.Added && creationDate == DateTime.MinValue)
                {
                    ((BaseModel)entityEntry.Entity).CreationDate = DateTime.UtcNow;
                }
            }

            return base.SaveChanges();
        }
    }

    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FirstName).IsRequired();
            builder.Property(x => x.LastName).IsRequired();
            builder.Property(x => x.UserName).IsRequired();
            builder.Property(x => x.Password).IsRequired();
            builder.Property(x => x.EmailId).IsRequired();
            builder.Property(x => x.MobileNumber).IsRequired();
            builder.Property(x => x.Role)
                   .HasConversion<string>()
                   .IsRequired();
            builder.HasMany(x => x.RefreshTokens)
                   .WithOne(x => x.User)
                   .HasForeignKey(x => x.UserId);

        }
    }

    public class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id);
            builder.Property(x => x.Token).IsRequired();
            builder.Property(x => x.Expires).IsRequired();
            builder.Property(x => x.CreationDate).IsRequired();

        }
    }
}
