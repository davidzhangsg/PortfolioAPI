using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Bond> Bonds { get; set; }
    public DbSet<Fund> Funds { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Inheritance Mapping (TPH) with Discriminator
        modelBuilder.Entity<Asset>()
            .HasDiscriminator<AssetType>("AssetType")
            .HasValue<Stock>(AssetType.Stock)
            .HasValue<Bond>(AssetType.Bond)
            .HasValue<Fund>(AssetType.Fund);

        // Required fields for Stock
        modelBuilder.Entity<Stock>()
            .Property(s => s.Exchange)
            .IsRequired();

        modelBuilder.Entity<Stock>()
            .Property(s => s.Sector)
            .IsRequired();

        // Required field for Bond
        modelBuilder.Entity<Bond>()
            .Property(b => b.Issuer)
            .IsRequired();
    }
}
