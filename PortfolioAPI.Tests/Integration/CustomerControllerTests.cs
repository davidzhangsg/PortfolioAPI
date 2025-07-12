using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace PortfolioAPI.Tests.Integration
{
    public class CustomersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CustomersControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task FullFlow_CreateGetUpdateDeleteCustomer_Success()
        {
            // Create customer
            var createDto = new CustomerCreateDto { Name = "Integration Test Customer" };
            var createResponse = await _client.PostAsJsonAsync("/api/customers", createDto);
            createResponse.EnsureSuccessStatusCode();

            var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.NotNull(createdCustomer);
            Assert.Equal("Integration Test Customer", createdCustomer.Name);

            // Get all customers
            var allResponse = await _client.GetAsync("/api/customers");
            allResponse.EnsureSuccessStatusCode();
            var customers = await allResponse.Content.ReadFromJsonAsync<List<CustomerDto>>();
            Assert.Contains(customers, c => c.Id == createdCustomer.Id);

            // Get customer by ID
            var getResponse = await _client.GetAsync($"/api/customers/{createdCustomer.Id}");
            getResponse.EnsureSuccessStatusCode();
            var fetchedCustomer = await getResponse.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.Equal(createdCustomer.Name, fetchedCustomer!.Name);

            // Update customer
            var updateDto = new CustomerUpdateDto { Name = "Updated Test Customer" };
            var updateResponse = await _client.PutAsJsonAsync($"/api/customers/{createdCustomer.Id}", updateDto);
            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            // Get customer again to confirm update
            var getUpdatedResponse = await _client.GetAsync($"/api/customers/{createdCustomer.Id}");
            getUpdatedResponse.EnsureSuccessStatusCode();
            var updatedCustomer = await getUpdatedResponse.Content.ReadFromJsonAsync<CustomerDto>();
            Assert.Equal("Updated Test Customer", updatedCustomer!.Name);

            // Get customer with portfolios (should be empty portfolios)
            var getWithPortfoliosResponse = await _client.GetAsync($"/api/customers/{createdCustomer.Id}/portfolios");
            getWithPortfoliosResponse.EnsureSuccessStatusCode();
            var customerWithPortfolios = await getWithPortfoliosResponse.Content.ReadFromJsonAsync<CustomerPortfolioDto>();
            Assert.Equal(updatedCustomer.Id, customerWithPortfolios!.Id);
            Assert.Empty(customerWithPortfolios.Portfolios);

            // Delete customer
            var deleteResponse = await _client.DeleteAsync($"/api/customers/{createdCustomer.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            // Confirm deletion
            var confirmDeleteResponse = await _client.GetAsync($"/api/customers/{createdCustomer.Id}");
            Assert.Equal(HttpStatusCode.NotFound, confirmDeleteResponse.StatusCode);
        }
    }
}
