using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PortfolioAPI.Tests.Integration
{
    public class PerformanceControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PerformanceControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetPerformance_ReturnsPortfolioPerformance()
        {
            // Create customer
            var customerResponse = await _client.PostAsJsonAsync("/api/customers", new
            {
                name = "Performance Test Customer"
            });
            customerResponse.EnsureSuccessStatusCode();
            var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();

            // Create portfolio
            var portfolioResponse = await _client.PostAsJsonAsync("/api/portfolios", new
            {
                name = "Performance Portfolio",
                customerId = customer!.Id
            });
            portfolioResponse.EnsureSuccessStatusCode();
            var portfolio = await portfolioResponse.Content.ReadFromJsonAsync<PortfolioDto>();

            // Add stock asset
            var stockResponse = await _client.PostAsJsonAsync("/api/assets/stock", new
            {
                portfolioId = portfolio!.Id,
                name = "Test Stock",
                ticker = "TSTK",
                type = 0, // AssetType.Stock
                exchange = "NYSE",
                sector = "Technology",
                dividendYield = 1.5m
            });
            stockResponse.EnsureSuccessStatusCode();
            var stock = await stockResponse.Content.ReadFromJsonAsync<StockDto>();

            // Add transactions
            var txn1Response = await _client.PostAsJsonAsync("/api/transactions", new
            {
                assetId = stock!.Id,
                type = 0, // TransactionType.Buy
                quantity = 10,
                price = 100,
                date = DateTime.UtcNow.AddDays(-10)
            });
            txn1Response.EnsureSuccessStatusCode();

            var txn2Response = await _client.PostAsJsonAsync("/api/transactions", new
            {
                assetId = stock.Id,
                type = 1, // TransactionType.Sell
                quantity = 5,
                price = 120,
                date = DateTime.UtcNow.AddDays(-5)
            });
            txn2Response.EnsureSuccessStatusCode();

            // Call performance API
            var startDate = DateTime.UtcNow.AddDays(-15).ToString("yyyy-MM-dd");
            var endDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var performanceResponse = await _client.GetAsync($"/api/portfolios/{portfolio.Id}/performance?startDate={startDate}&endDate={endDate}");

            performanceResponse.EnsureSuccessStatusCode();
            var performance = await performanceResponse.Content.ReadFromJsonAsync<PerformanceDto>();

            // Assertions
            Assert.NotNull(performance);
            Assert.Equal(portfolio.Id, performance!.PortfolioId);
            Assert.NotEmpty(performance.Assets);
            Assert.True(performance.TotalValue >= 0);

            // Clean up (delete customer will cascade)
            var deleteResponse = await _client.DeleteAsync($"/api/customers/{customer.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }
    }
}
