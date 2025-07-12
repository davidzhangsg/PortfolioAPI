using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortfolioAPI.Tests.Unit
{
    public class TransactionServiceTests
    {
        private readonly Mock<IRepository<Transaction>> _transactionRepoMock;
        private readonly AppDbContext _context;
        private readonly TransactionService _service;

        public TransactionServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _transactionRepoMock = new Mock<IRepository<Transaction>>();

            _service = new TransactionService(_context, _transactionRepoMock.Object);
        }

        [Fact]
        public async Task AddAsync_CallsRepositoryAdd()
        {
            // Arrange
            var txn = new Transaction { Quantity = 10 };

            // Act
            await _service.AddAsync(txn);

            // Assert
            _transactionRepoMock.Verify(r => r.AddAsync(txn), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CallsRepositoryUpdate()
        {
            // Arrange
            var txn = new Transaction { Id = 1, Quantity = 20 };

            // Act
            await _service.UpdateAsync(txn);

            // Assert
            _transactionRepoMock.Verify(r => r.UpdateAsync(txn), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsTransaction_FromRepository()
        {
            // Arrange
            var txn = new Transaction { Id = 1, Quantity = 5 };
            _transactionRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(txn);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Quantity);
        }

        [Fact]
        public async Task GetByAssetIdAsync_ReturnsTransactions_FromDbContext()
        {
            // Arrange
            int assetId = 99;
            _context.Transactions.AddRange(new List<Transaction>
            {
                new Transaction { Id = 1, AssetId = assetId, Quantity = 10, Date = DateTime.UtcNow },
                new Transaction { Id = 2, AssetId = assetId, Quantity = 5, Date = DateTime.UtcNow.AddDays(-1) },
                new Transaction { Id = 3, AssetId = 100, Quantity = 7, Date = DateTime.UtcNow } // Different asset
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByAssetIdAsync(assetId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, t => Assert.Equal(assetId, t.AssetId));
        }

        [Fact]
        public async Task GetByDateRangeAsync_ReturnsPagedTransactions_FromDbContext()
        {
            // Arrange
            _context.Transactions.AddRange(new List<Transaction>
            {
                new Transaction { Id = 1, Date = new DateTime(2025, 1, 1), Quantity = 10 },
                new Transaction { Id = 2, Date = new DateTime(2025, 2, 1), Quantity = 15 },
                new Transaction { Id = 3, Date = new DateTime(2025, 3, 1), Quantity = 20 }
            });
            await _context.SaveChangesAsync();

            var startDate = new DateTime(2025, 1, 1);
            var endDate = new DateTime(2025, 12, 31);

            // Act
            var (transactions, totalCount) = await _service.GetByDateRangeAsync(startDate, endDate, page: 1, pageSize: 2);

            // Assert
            Assert.Equal(3, totalCount);
            Assert.Equal(2, transactions.Count());
        }

        [Fact]
        public async Task DeleteAsync_RemovesTransaction_FromDbContext()
        {
            // Arrange
            var txn = new Transaction { Id = 1, Quantity = 50 };
            _context.Transactions.Add(txn);
            await _context.SaveChangesAsync();

            // Act
            await _service.DeleteAsync(txn.Id);

            // Assert
            var deleted = await _context.Transactions.FindAsync(txn.Id);
            Assert.Null(deleted);
        }
    }
}
