using Microsoft.EntityFrameworkCore;
using Server.Services.TenantAccessor;

namespace Server.Services.DataService
{
    public class AppDBContext<T>(DbContextOptions<T> dbContextOptions) : DbContext(dbContextOptions)
        where T:DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSeeding((context, _) =>
            {
                context.Set<Product>().AddRange(
                    new Product() { Id = 1, Tenant = "localhost:1551", Name = "Product1551-1" },
                    new Product() {Id=2, Tenant = "localhost:1551", Name = "Product1551-2" },
                    new Product() {Id=3, Tenant = "localhost:1551", Name = "Product1551-3" },
                    new Product() {Id=4, Tenant = "localhost:1551", Name = "Product1551-4" },
                    new Product() {Id=5, Tenant = "localhost:1552", Name = "Product1552-1" },
                    new Product() {Id=6, Tenant = "localhost:1552", Name = "Product1552-2" },
                    new Product() {Id=7, Tenant = "localhost:1552", Name = "Product1552-3" },
                    new Product() {Id=8, Tenant = "localhost:1552", Name = "Product1552-4" }
                    );
                context.SaveChanges();
            });
            base.OnConfiguring(optionsBuilder);
        }
    }

    public class AdminAppDbContext : AppDBContext<AdminAppDbContext>
    {
        public AdminAppDbContext(DbContextOptions<AdminAppDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
    }

    public class TenantBaseAppDbContext : AppDBContext<TenantBaseAppDbContext>
    {
        private readonly ITenantAccessor _tenantAccessor;

        public TenantBaseAppDbContext(DbContextOptions<TenantBaseAppDbContext> dbContextOptions,ITenantAccessor tenantAccessor) : base(dbContextOptions)
        {
            _tenantAccessor = tenantAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasQueryFilter(x=>x.Tenant == _tenantAccessor.Tenant);
        }
    }
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tenant { get; set; }
    }


    public interface ISeedData
    {
        void SeedData();
    }

    internal class SeedDataService(AdminAppDbContext adminAppDbContext) : ISeedData
    {
        private readonly AdminAppDbContext _adminAppDbContext = adminAppDbContext;

        public void SeedData()
        {
            if (_adminAppDbContext.Database.GetPendingMigrations().Any())
            {
                _adminAppDbContext.Database.Migrate();
            }
            if (!_adminAppDbContext.Products.Any())
            {
                _adminAppDbContext.Database.EnsureCreated();
            }
        }
    }

    public static class Setup
    {
        public static IServiceCollection AddDbService(this IServiceCollection services, IConfiguration configuration)
            => services.AddDbContext<AdminAppDbContext>(opts => opts.UseSqlite(configuration.GetConnectionString("Main")), ServiceLifetime.Scoped)
            .AddDbContext<TenantBaseAppDbContext>(opts => opts.UseSqlite(configuration.GetConnectionString("Main")), ServiceLifetime.Scoped)
            .AddScoped<ISeedData,SeedDataService>();
    }
}
