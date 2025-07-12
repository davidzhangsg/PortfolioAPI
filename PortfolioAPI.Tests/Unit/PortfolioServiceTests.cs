using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortfolioAPI.Tests.Unit
{
    public class PortfolioServiceTests
    {
        private readonly PortfolioService _service;
        private readonly AppDbContext _context;

        public PortfolioServiceTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(dbContextOptions);
            _service = new PortfolioService(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddPortfolio()
        {
            var portfolio = new Portfolio { Name = "Test Portfolio" };
            await _service.AddAsync(portfolio);

            var saved = await _context.Portfolios.FirstOrDefaultAsync(p => p.Name == "Test Portfolio");
            Assert.NotNull(saved);
            Assert.Equal("Test Portfolio", saved!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsPortfolio_WithAssetsAndCustomer()
        {
            // Arrange
            var customer = new Customer { Id = 1, Name = "Test Customer" };
            var portfolio = new Portfolio
            {
                Id = 1,
                Name = "Test Portfolio",
                Customer = customer,
                Assets = new List<Asset>
                {
                    new Stock
                    {
                        Id = 1,
                        Name = "Asset 1",
                        Ticker = "A1",
                        Type = AssetType.Stock,
                        Exchange = "NASDAQ",
                        Sector = "Technology"
                    }
                }
            };

            _context.Customers.Add(customer);
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Portfolio", result!.Name);
            Assert.NotNull(result.Customer);
            Assert.Equal("Test Customer", result.Customer.Name);
            Assert.Single(result.Assets);
            Assert.Equal("Asset 1", result.Assets.First().Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePortfolio()
        {
            var portfolio = new Portfolio { Name = "Old Name" };
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();

            // Act
            portfolio.Name = "Updated Name";
            await _service.UpdateAsync(portfolio);

            var updated = await _context.Portfolios.FindAsync(portfolio.Id);

            // Assert
            Assert.NotNull(updated);
            Assert.Equal("Updated Name", updated!.Name);
        }

        [Fact]
        public async Task DeleteAsync_RemovesPortfolio()
        {
            var portfolio = new Portfolio { Name = "To Delete" };
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();

            await _service.DeleteAsync(portfolio.Id);

            var deleted = await _context.Portfolios.FindAsync(portfolio.Id);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllPortfolios()
        {
            // Arrange: add a dummy customer
            var customer = new Customer { Name = "Test Customer" };
            _context.Customers.Add(customer);

            // Add portfolios with the customer relationship
            _context.Portfolios.AddRange(
                new Portfolio { Name = "Portfolio 1", Customer = customer },
                new Portfolio { Name = "Portfolio 2", Customer = customer }
            );

            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Name == "Portfolio 1");
            Assert.Contains(result, p => p.Name == "Portfolio 2");
        }


        [Fact]
        public async Task GetByCustomerIdAsync_ReturnsPortfoliosForCustomer()
        {
            var customer = new Customer { Id = 99, Name = "Customer A" };
            _context.Customers.Add(customer);
            _context.Portfolios.AddRange(
                new Portfolio { Name = "Customer A Portfolio 1", CustomerId = 99 },
                new Portfolio { Name = "Customer A Portfolio 2", CustomerId = 99 },
                new Portfolio { Name = "Unrelated Portfolio", CustomerId = 100 }
            );
            await _context.SaveChangesAsync();

            var result = await _service.GetByCustomerIdAsync(99);

            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.Equal(99, p.CustomerId));
        }
    }
}
