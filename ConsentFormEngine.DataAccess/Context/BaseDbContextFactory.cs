using ConsentFormEngine.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ConsentFormEngine.DataAccess.Context
{
    public class BaseDbContextFactory : IDesignTimeDbContextFactory<BaseDbContext>
    {
        public BaseDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>();
            optionsBuilder.UseSqlServer(
        "Server=localhost,1433;Database=ConsentDb;User Id=sa;Password=Strong!Pass123;Encrypt=False;TrustServerCertificate=True;"            );

            return new BaseDbContext(optionsBuilder.Options, new DummyCurrentUserService());
        }
    }

    public class DummyCurrentUserService : ICurrentUserService
    {
        public Guid? UserId => Guid.Empty;
        public string IpAddress => "127.0.0.1";
    }

}