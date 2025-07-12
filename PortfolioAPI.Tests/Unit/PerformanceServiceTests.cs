using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace PortfolioAPI.Tests.Unit
{
    public class PerformanceServiceTests
    {
        private readonly AppDbContext _context;
        private readonly PerformanceService _service;

        public PerformanceServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            var logger = Mock.Of<ILogger<PerformanceService>>();
            _service = new PerformanceService(_context, logger);
        }

        [Fact]
        public async Task CalculatePerformanceAsync_ShouldReturnCorrectData_ForStock()
        {
            // Arrange
            var portfolio = new Portfolio
            {
                Id = 1,
                Name = "Test Portfolio",
                Assets = new List<Asset>
            {
                new Stock
                {
                    Id = 100,
                    PortfolioId = 1,
                    Name = "Apple",
                    Ticker = "AAPL",
                    Type = AssetType.Stock,
                    Exchange = "NASDAQ",
                    Sector = "Technology",
                    DividendYield = 1.3m,
                    Transactions = new List<Transaction>
                    {
                        new Transaction { Type = TransactionType.Buy, Quantity = 10, Price = 150, Date = DateTime.Today.AddDays(-10) },
                        new Transaction { Type = TransactionType.Sell, Quantity = 5, Price = 170, Date = DateTime.Today.AddDays(-5) }
                    }
                }
            }
            };

            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.CalculatePerformanceAsync(1, DateTime.Today.AddDays(-15), DateTime.Today);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.PortfolioId);
            Assert.Single(result.Assets);
            Assert.True(result.TotalValue > 0);
        }

        [Fact]
        public async Task CalculatePerformanceAsync_ShouldWork_ForBondAndFund()
        {
            // Arrange
            var bond = new Bond
            {
                Id = 101,
                PortfolioId = 1,
                Name = "US Gov Bond",
                Ticker = "USGB10Y",
                Type = AssetType.Bond,
                BondType = BondType.Government,
                CouponRate = 2.0m,
                Issuer = "US Treasury",
                MaturityDate = DateTime.Today.AddYears(10),
                Transactions = new List<Transaction>
            {
                new Transaction { Type = TransactionType.Buy, Quantity = 20, Price = 100, Date = DateTime.Today.AddDays(-20) },
                new Transaction { Type = TransactionType.Sell, Quantity = 10, Price = 110, Date = DateTime.Today.AddDays(-10) }
            }
            };

            var fund = new Fund
            {
                Id = 102,
                PortfolioId = 1,
                Name = "Vanguard ETF",
                Ticker = "VTI",
                Type = AssetType.Fund,
                FundType = FundType.ETF,
                FundManager = "Vanguard",
                ExpenseRatio = 0.05m,
                Transactions = new List<Transaction>
            {
                new Transaction { Type = TransactionType.Buy, Quantity = 5, Price = 200, Date = DateTime.Today.AddDays(-30) }
            }
            };

            var portfolio = new Portfolio
            {
                Id = 2,
                Name = "Bond & Fund Portfolio",
                Assets = new List<Asset> { bond, fund }
            };

            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.CalculatePerformanceAsync(2, DateTime.Today.AddDays(-60), DateTime.Today);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.PortfolioId);
            Assert.Equal(2, result.Assets.Count);
            Assert.True(result.TotalValue > 0);
        }

        [Fact]
        public async Task CalculatePerformanceAsync_ReturnsNull_WhenPortfolioNotFound()
        {
            var result = await _service.CalculatePerformanceAsync(999, DateTime.Today.AddDays(-10), DateTime.Today);

            Assert.Null(result);
        }

        [Fact]
        public async Task CalculatePerformanceAsync_ShouldHandlePortfolioWithNoAssets()
        {
            var portfolio = new Portfolio { Id = 3, Name = "Empty Portfolio", Assets = new List<Asset>() };
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();

            var result = await _service.CalculatePerformanceAsync(3, DateTime.Today.AddDays(-10), DateTime.Today);

            Assert.NotNull(result);
            Assert.Equal(0, result.TotalValue);
            Assert.Empty(result.Assets);
            Assert.Empty(result.ValueOverTime.Where(v => v.Value != 0));
        }

        [Fact]
        public async Task CalculatePerformanceAsync_ShouldIgnoreTransactionsOutsideRange()
        {
            var portfolio = new Portfolio
            {
                Id = 4,
                Name = "Old Transactions",
                Assets = new List<Asset>
        {
            new Stock
            {
                Id = 200,
                PortfolioId = 4,
                Name = "Legacy Stock",
                Ticker = "LEG",
                Type = AssetType.Stock,
                Exchange = "NYSE",
                Sector = "Finance",
                Transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        Type = TransactionType.Buy,
                        Quantity = 50,
                        Price = 100,
                        Date = DateTime.Today.AddYears(-2)
                    }
                }
            }
        }
            };

            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();

            var result = await _service.CalculatePerformanceAsync(4, DateTime.Today.AddDays(-30), DateTime.Today);

            Assert.NotNull(result);

            // The 50 shares bought 2 years ago still exist and are valued at the last price (100)
            Assert.Equal(5000, result.TotalValue); // 50 * 100 = 5000

            // Also verify allocation and assets
            Assert.Single(result.Assets);
            Assert.Equal("Legacy Stock", result.Assets[0].Name);
            Assert.Equal(5000, result.Assets[0].Value);
            Assert.Equal(0, result.Assets[0].UnrealizedGain);

        }


    }
}