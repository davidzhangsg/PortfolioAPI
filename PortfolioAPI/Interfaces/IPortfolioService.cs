public interface IPortfolioService
{
    Task<IEnumerable<Portfolio>> GetAllAsync();
    Task<Portfolio> GetByIdAsync(int id);
    Task AddAsync(Portfolio portfolio);
    Task UpdateAsync(Portfolio portfolio);
    Task DeleteAsync(int id);
    Task<IEnumerable<Portfolio>> GetByCustomerIdAsync(int customerId);

}

