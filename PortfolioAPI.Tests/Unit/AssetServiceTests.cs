using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortfolioAPI.Tests.Unit
{
    public class AssetServiceTests
    {
        private readonly Mock<IRepository<Asset>> _assetRepoMock;
        private readonly Mock<IRepository<Stock>> _stockRepoMock;
        private readonly Mock<IRepository<Bond>> _bondRepoMock;
        private readonly Mock<IRepository<Fund>> _fundRepoMock;
        private readonly AssetService _service;
        private readonly AppDbContext _context;

        public AssetServiceTests()
        {
            _assetRepoMock = new Mock<IRepository<Asset>>();
            _stockRepoMock = new Mock<IRepository<Stock>>();
            _bondRepoMock = new Mock<IRepository<Bond>>();
            _fundRepoMock = new Mock<IRepository<Fund>>();

            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(dbContextOptions);

            _service = new AssetService(
                _context,
                _assetRepoMock.Object,
                _stockRepoMock.Object,
                _bondRepoMock.Object,
                _fundRepoMock.Object
            );
        }

        [Fact]
        public async Task AddStockAsync_CallsAdd()
        {
            var stock = new Stock { Name = "Test Stock", Ticker = "STK", Exchange = "NYSE", Sector = "Tech" };
            await _service.AddStockAsync(stock);
            _stockRepoMock.Verify(r => r.AddAsync(stock), Times.Once);
        }

        [Fact]
        public async Task AddBondAsync_CallsAdd()
        {
            var bond = new Bond { Name = "Gov Bond", Ticker = "GOV10", Issuer = "Gov", CouponRate = 2.5m, MaturityDate = DateTime.UtcNow.AddYears(10) };
            await _service.AddBondAsync(bond);
            _bondRepoMock.Verify(r => r.AddAsync(bond), Times.Once);
        }

        [Fact]
        public async Task AddFundAsync_CallsAdd()
        {
            var fund = new Fund { Name = "Index Fund", Ticker = "IDX", FundManager = "XYZ", FundType = FundType.Index, ExpenseRatio = 0.1m };
            await _service.AddFundAsync(fund);
            _fundRepoMock.Verify(r => r.AddAsync(fund), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsAsset_FromDb()
        {
            var stock = new Stock { Id = 1, Name = "Test Stock", Ticker = "TST", Exchange = "NYSE", Sector = "Tech" };
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Test Stock", result.Name);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllAssets()
        {
            var stock1 = new Stock { Name = "Asset A", Ticker = "A", Exchange = "NYSE", Sector = "Finance" };
            var stock2 = new Stock { Name = "Asset B", Ticker = "B", Exchange = "NASDAQ", Sector = "Tech" };
            _context.Stocks.AddRange(stock1, stock2);
            await _context.SaveChangesAsync();

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.Name == "Asset A");
            Assert.Contains(result, a => a.Name == "Asset B");
        }

        [Fact]
        public async Task GetByPortfolioIdAsync_ReturnsAssetsForPortfolio()
        {
            // Arrange
            int portfolioId = 42;
            _context.Stocks.AddRange(new List<Stock>
            {
                new Stock { Name = "Stock A", PortfolioId = portfolioId, Ticker = "A", Exchange = "NYSE", Sector = "Tech" },
                new Stock { Name = "Stock B", PortfolioId = portfolioId, Ticker = "B", Exchange = "NASDAQ", Sector = "Finance" },
                new Stock { Name = "Stock C", PortfolioId = 99, Ticker = "C", Exchange = "LSE", Sector = "Health" } // Different portfolio
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByPortfolioIdAsync(portfolioId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, a => Assert.Equal(portfolioId, a.PortfolioId));
        }

        [Fact]
        public async Task UpdateAsync_CallsUpdate()
        {
            var asset = new Stock { Id = 1, Name = "Update Me" };
            await _service.UpdateAsync(asset);
            _assetRepoMock.Verify(r => r.UpdateAsync(asset), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsDelete()
        {
            await _service.DeleteAsync(1);
            _assetRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }
    }
}
