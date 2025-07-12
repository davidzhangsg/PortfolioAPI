using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PortfolioAPI.Tests.Integration
{
    public class AssetsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AssetsControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task FullFlow_CreateGetUpdateDeleteAsset_Success()
        {
            // Arrange: create a customer and portfolio
            var customer = new CustomerCreateDto { Name = "Integration Customer" };
            var customerResponse = await _client.PostAsJsonAsync("/api/customers", customer);
            customerResponse.EnsureSuccessStatusCode();
            var createdCustomer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();

            var portfolio = new PortfolioCreateDto { Name = "Integration Portfolio", CustomerId = createdCustomer!.Id };
            var portfolioResponse = await _client.PostAsJsonAsync("/api/portfolios", portfolio);
            portfolioResponse.EnsureSuccessStatusCode();
            var createdPortfolio = await portfolioResponse.Content.ReadFromJsonAsync<PortfolioDto>();

            // Create stock
            var stockDto = new StockCreateDto
            {
                PortfolioId = createdPortfolio!.Id,
                Name = "Apple Inc",
                Ticker = "AAPL",
                Exchange = "NASDAQ",
                Sector = "Technology",
                DividendYield = 0.5m,
                Type = AssetType.Stock
            };

            var stockResponse = await _client.PostAsJsonAsync("/api/assets/stock", stockDto);
            stockResponse.EnsureSuccessStatusCode();
            var createdStock = await stockResponse.Content.ReadFromJsonAsync<StockDto>();

            // Get asset by ID
            var getResponse = await _client.GetAsync($"/api/assets/{createdStock!.Id}");
            getResponse.EnsureSuccessStatusCode();
            var fetchedAsset = await getResponse.Content.ReadFromJsonAsync<StockDto>();
            Assert.Equal("Apple Inc", fetchedAsset!.Name);

            // Get assets by Portfolio ID
            var getByPortfolioResponse = await _client.GetAsync($"/api/assets/by-portfolio/{createdPortfolio.Id}");
            getByPortfolioResponse.EnsureSuccessStatusCode();
            var portfolioAssets = await getByPortfolioResponse.Content.ReadFromJsonAsync<List<AssetDto>>();
            Assert.NotNull(portfolioAssets);
            Assert.Single(portfolioAssets);
            Assert.Equal("Apple Inc", portfolioAssets[0].Name);

            // Update asset
            fetchedAsset.Name = "Apple Inc Updated";
            var updateResponse = await _client.PutAsJsonAsync($"/api/assets/{createdStock.Id}", fetchedAsset);
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            // Get all assets
            var allAssetsResponse = await _client.GetAsync("/api/assets");
            allAssetsResponse.EnsureSuccessStatusCode();
            var allAssets = await allAssetsResponse.Content.ReadFromJsonAsync<List<AssetDto>>();
            Assert.Contains(allAssets, a => a.Name == "Apple Inc Updated");

            // Delete asset
            var deleteResponse = await _client.DeleteAsync($"/api/assets/{createdStock.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Confirm deletion
            var confirmDeleteResponse = await _client.GetAsync($"/api/assets/{createdStock.Id}");
            Assert.Equal(HttpStatusCode.NotFound, confirmDeleteResponse.StatusCode);

            // Clean up
            await _client.DeleteAsync($"/api/portfolios/{createdPortfolio.Id}");
            await _client.DeleteAsync($"/api/customers/{createdCustomer.Id}");
        }
    }
}
