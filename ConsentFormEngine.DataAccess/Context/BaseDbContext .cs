using Microsoft.EntityFrameworkCore;
using ConsentFormEngine.Core.Base;
using ConsentFormEngine.Core.Services;
using ConsentFormEngine.Entities.Entities;
using System.Linq.Expressions;

namespace ConsentFormEngine.DataAccess.Context
{
    public class BaseDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUser;

        public BaseDbContext(DbContextOptions<BaseDbContext> options, ICurrentUserService currentUser)
        : base(options)
        {
            _currentUser = currentUser;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<FormEntry> FormEntries { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tüm BaseEntity alt sınıfları için global soft-delete filtresi
            var baseEntityType = typeof(BaseEntity);
            var dateTimeNullable = typeof(DateTime?);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(t => baseEntityType.IsAssignableFrom(t.ClrType)))
            {
                // Parametre: e
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                // e.DeletedDate
                var deletedDateProperty = Expression.Property(parameter, nameof(BaseEntity.DeletedDate));
                // constant null (DateTime?)
                var nullConstant = Expression.Constant(null, dateTimeNullable);
                // e.DeletedDate == null
                var compareExpression = Expression.Equal(deletedDateProperty, nullConstant);
                // lambda: e => e.DeletedDate == null
                var lambda = Expression.Lambda(compareExpression, parameter);

                modelBuilder.Entity(entityType.ClrType)
                            .HasQueryFilter(lambda);
            }

            modelBuilder.Entity<Category>().HasData(
                   new Category { Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"), Name = "Seyahat Acentesi" },
                   new Category { Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"), Name = "Protokol/Yerel Yönetim" },
                   new Category { Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"), Name = "Basın" },
                   new Category { Id = Guid.Parse("a4444444-4444-4444-4444-444444444444"), Name = "Influencer" },
                   new Category { Id = Guid.Parse("a5555555-5555-5555-5555-555555555555"), Name = "Kurumsal" },
                   new Category { Id = Guid.Parse("a6666666-6666-6666-6666-666666666666"), Name = "Diğer" }
               );

            modelBuilder.Entity<User>()
                .HasOne(u => u.FormEntry)
                .WithOne(f => f.User)
                .HasForeignKey<FormEntry>(f => f.UserId);
        }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedDate = DateTime.Now;
                        break;

                    case EntityState.Deleted:
                        // soft-delete: silme yerine işaretle
                        entry.State = EntityState.Modified;
                        entry.Entity.DeletedDate = DateTime.Now;
                        entry.Entity.DeletedBy = _currentUser.UserId;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
