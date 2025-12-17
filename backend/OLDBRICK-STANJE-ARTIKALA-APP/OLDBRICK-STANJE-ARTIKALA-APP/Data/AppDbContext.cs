using Microsoft.EntityFrameworkCore;
using OLDBRICK_STANJE_ARTIKALA_APP.Entities;

namespace OLDBRICK_STANJE_ARTIKALA_APP.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Users> Users => Set<Users>();

        public DbSet<Beer> Beers => Set<Beer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(e =>
            {
                e.ToTable("users"); 
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Username).IsUnique();

                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.Username).HasColumnName("username");
                e.Property(x => x.PasswordHash).HasColumnName("password_hash");
                e.Property(x => x.Role).HasColumnName("role");
                e.Property(x => x.IsActive).HasColumnName("is_active");
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
            });

            modelBuilder.Entity<Beer>(e =>
            {
                e.ToTable("TAB1");
                e.Property(e => e.Id).HasColumnName("id");
                e.Property(e => e.CreatedAt).HasColumnName("created_at");
                e.Property(e => e.NazivPiva).HasColumnName("naziv_piva");
                e.Property(e => e.TipMerenja).HasColumnName("tip_merenja");
            });
        }
    }
}
