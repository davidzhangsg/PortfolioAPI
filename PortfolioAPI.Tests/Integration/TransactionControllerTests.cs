using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace PortfolioAPI.Tests.Integration
{
    public class TransactionsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TransactionsControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateUpdateGetSearchDelete_Transaction_FullFlow()
        {
            // 🌱 Create Customer
            var customerResponse = await _client.PostAsJsonAsync("/api/customers", new
            {
                name = "Test Customer"
            });
            customerResponse.EnsureSuccessStatusCode();
            var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.NotNull(customer);

            // 🌱 Create Portfolio
            var portfolioResponse = await _client.PostAsJsonAsync("/api/portfolios", new
            {
                name = "Test Portfolio",
                customerId = customer!.Id
            });
            portfolioResponse.EnsureSuccessStatusCode();
            var portfolio = await portfolioResponse.Content.ReadFromJsonAsync<PortfolioDto>();
            Assert.NotNull(portfolio);

            // 🌱 Create Asset
            var assetResponse = await _client.PostAsJsonAsync("/api/assets/stock", new
            {
                portfolioId = portfolio!.Id,
                name = "Test Stock",
                ticker = "TST",
                exchange = "NASDAQ",
                sector = "Tech",
                dividendYield = 1.5m,
                type = AssetType.Stock
            });
            assetResponse.EnsureSuccessStatusCode();
            var asset = await assetResponse.Content.ReadFromJsonAsync<StockDto>();
            Assert.NotNull(asset);

            // 🌱 Create Transaction
            var transactionResponse = await _client.PostAsJsonAsync("/api/transactions", new
            {
                assetId = asset!.Id,
                quantity = 50,
                price = 200,
                type = TransactionType.Buy,
                date = DateTime.UtcNow
            });
            transactionResponse.EnsureSuccessStatusCode();
            var transaction = await transactionResponse.Content.ReadFromJsonAsync<TransactionDto>();
            Assert.NotNull(transaction);
            Assert.Equal(50, transaction!.Quantity);

            // ✏️ Update Transaction
            var updateResponse = await _client.PutAsJsonAsync($"/api/transactions/{transaction.Id}", new
            {
                quantity = 75,
                price = 210,
                type = TransactionType.Buy,
                date = DateTime.UtcNow
            });
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            // 🔍 Get Transaction by ID
            var getResponse = await _client.GetAsync($"/api/transactions/{transaction.Id}");
            getResponse.EnsureSuccessStatusCode();
            var updatedTransaction = await getResponse.Content.ReadFromJsonAsync<TransactionDto>();
            Assert.Equal(75, updatedTransaction!.Quantity);

            // 🔍 Get Transactions by Asset ID
            var byAssetResponse = await _client.GetAsync($"/api/transactions/by-asset/{asset.Id}");
            byAssetResponse.EnsureSuccessStatusCode();
            var assetTransactions = await byAssetResponse.Content.ReadFromJsonAsync<List<TransactionDto>>();
            Assert.Single(assetTransactions);

            // 🔍 Search Transactions
            var searchResponse = await _client.GetAsync($"/api/transactions/search?startDate=2020-01-01&endDate=2030-01-01&page=1&pageSize=5");
            searchResponse.EnsureSuccessStatusCode();
            var searchResult = await searchResponse.Content.ReadFromJsonAsync<JsonElement>();
            Assert.True(searchResult.GetProperty("totalCount").GetInt32() >= 1);

            // Delete the transaction (should succeed)
            var deleteTxnResponse = await _client.DeleteAsync($"/api/transactions/{transaction.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteTxnResponse.StatusCode);


            // ✅ Verify Deletion
            var verifyDeleteResponse = await _client.GetAsync($"/api/transactions/{transaction.Id}");
            Assert.Equal(HttpStatusCode.NotFound, verifyDeleteResponse.StatusCode);

            // 🧹 Cleanup: Delete Asset, Portfolio, Customer
            var deleteAssetResponse = await _client.DeleteAsync($"/api/assets/{asset.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteAssetResponse.StatusCode);

            var deletePortfolioResponse = await _client.DeleteAsync($"/api/portfolios/{portfolio.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deletePortfolioResponse.StatusCode);

            var deleteCustomerResponse = await _client.DeleteAsync($"/api/customers/{customer.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteCustomerResponse.StatusCode);
        }
    }
}
