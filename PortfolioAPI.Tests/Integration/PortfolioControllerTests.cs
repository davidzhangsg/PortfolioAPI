using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace PortfolioAPI.Tests.Integration
{
    public class PortfoliosControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public PortfoliosControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateUpdateGetDelete_Portfolio_FullFlow()
        {
            // 1️⃣ Create customer
            var customerResponse = await _client.PostAsJsonAsync("/api/customers", new
            {
                name = "Integration Test Customer"
            });
            customerResponse.EnsureSuccessStatusCode();
            var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.NotNull(customer);

            // 2️⃣ Create portfolio
            var portfolioResponse = await _client.PostAsJsonAsync("/api/portfolios", new
            {
                name = "Integration Test Portfolio",
                customerId = customer!.Id
            });
            portfolioResponse.EnsureSuccessStatusCode();
            var portfolio = await portfolioResponse.Content.ReadFromJsonAsync<PortfolioDto>();
            Assert.NotNull(portfolio);
            Assert.Equal("Integration Test Portfolio", portfolio!.Name);

            // 3️⃣ Get portfolio by ID
            var getResponse = await _client.GetAsync($"/api/portfolios/{portfolio.Id}");
            getResponse.EnsureSuccessStatusCode();
            var retrieved = await getResponse.Content.ReadFromJsonAsync<PortfolioDto>();
            Assert.Equal(portfolio.Id, retrieved!.Id);

            // 4️⃣ Get all portfolios
            var getAllResponse = await _client.GetAsync("/api/portfolios");
            getAllResponse.EnsureSuccessStatusCode();
            var allPortfolios = await getAllResponse.Content.ReadFromJsonAsync<List<PortfolioDto>>();
            Assert.Contains(allPortfolios, p => p.Id == portfolio.Id);

            // 5️⃣ Get portfolios by customer
            var getByCustomerResponse = await _client.GetAsync($"/api/portfolios/by-customer/{customer.Id}");
            getByCustomerResponse.EnsureSuccessStatusCode();
            var customerPortfolios = await getByCustomerResponse.Content.ReadFromJsonAsync<List<PortfolioWithAssetsDto>>();
            Assert.Single(customerPortfolios);
            Assert.Equal(portfolio.Id, customerPortfolios[0].Id);

            // 6️⃣ Update portfolio
            var updateResponse = await _client.PutAsJsonAsync($"/api/portfolios/{portfolio.Id}", new
            {
                name = "Updated Portfolio Name",
                customerId = customer!.Id // Include this to satisfy EF Core
            });
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);


            // Verify update
            var updatedResponse = await _client.GetAsync($"/api/portfolios/{portfolio.Id}");
            updatedResponse.EnsureSuccessStatusCode();
            var updatedPortfolio = await updatedResponse.Content.ReadFromJsonAsync<PortfolioDto>();
            Assert.Equal("Updated Portfolio Name", updatedPortfolio!.Name);

            // 7️⃣ Add asset to portfolio
            var stockResponse = await _client.PostAsJsonAsync("/api/assets/stock", new
            {
                portfolioId = portfolio.Id,
                name = "Test Stock",
                ticker = "TST",
                exchange = "NASDAQ",
                sector = "Tech",
                dividendYield = 1.5m,
                type = AssetType.Stock
            });
            stockResponse.EnsureSuccessStatusCode();
            var stock = await stockResponse.Content.ReadFromJsonAsync<StockDto>();
            Assert.NotNull(stock);

            // 9️⃣ Delete asset
            var deleteAssetResponse = await _client.DeleteAsync($"/api/assets/{stock!.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteAssetResponse.StatusCode);

            // 1️⃣0️⃣ Delete portfolio
            var deleteResponse = await _client.DeleteAsync($"/api/portfolios/{portfolio.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // 1️⃣1️⃣ Verify portfolio deletion
            var deletedGetResponse = await _client.GetAsync($"/api/portfolios/{portfolio.Id}");
            Assert.Equal(HttpStatusCode.NotFound, deletedGetResponse.StatusCode);

            // 1️⃣2️⃣ Clean up customer
            var deleteCustomerResponse = await _client.DeleteAsync($"/api/customers/{customer.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteCustomerResponse.StatusCode);
        }
    }
}
