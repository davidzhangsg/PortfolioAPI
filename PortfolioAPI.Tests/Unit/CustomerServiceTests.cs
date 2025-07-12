using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortfolioAPI.Tests.Unit
{
    public class CustomerServiceTests
    {
        private readonly Mock<IRepository<Customer>> _customerRepoMock;
        private readonly CustomerService _service;
        private readonly AppDbContext _context;
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public CustomerServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(_dbContextOptions);

            _customerRepoMock = new Mock<IRepository<Customer>>();
            _service = new CustomerService(_context, _customerRepoMock.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldCallAdd()
        {
            var customer = new Customer { Name = "Test Customer" };
            await _service.AddAsync(customer);

            _customerRepoMock.Verify(r => r.AddAsync(customer), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCustomer()
        {
            var customer = new Customer { Id = 1, Name = "Test Customer" };
            _customerRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Test Customer", result.Name);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCustomers()
        {
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Customer One" },
                new Customer { Id = 2, Name = "Customer Two" }
            };
            _customerRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);

            var result = await _service.GetAllAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "Customer One");
            Assert.Contains(result, c => c.Name == "Customer Two");
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallUpdate()
        {
            var customer = new Customer { Id = 1, Name = "Updated Customer" };
            await _service.UpdateAsync(customer);

            _customerRepoMock.Verify(r => r.UpdateAsync(customer), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallDelete()
        {
            await _service.DeleteAsync(1);

            _customerRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetCustomerWithPortfoliosAsync_ReturnsCustomerWithPortfoliosAndAssets()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                Name = "Customer With Portfolios",
                Portfolios = new List<Portfolio>
                {
                    new Portfolio
                    {
                        Id = 1,
                        Name = "Portfolio One",
                        Assets = new List<Asset>
                        {
                            new Stock { Id = 1, Name = "Stock Asset", Ticker = "STK", Exchange = "NYSE", Sector = "Tech" }
                        }
                    }
                }
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetCustomerWithPortfoliosAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Customer With Portfolios", result.Name);
            Assert.Single(result.Portfolios);
            Assert.Single(result.Portfolios.First().Assets);
            Assert.Equal("Stock Asset", result.Portfolios.First().Assets.First().Name);
        }
    }
}
