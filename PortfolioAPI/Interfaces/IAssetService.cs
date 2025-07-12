public interface IAssetService
{
    Task<IEnumerable<Asset>> GetAllAsync();
    Task<Asset> GetByIdAsync(int id);
    Task<IEnumerable<Asset>> GetByPortfolioIdAsync(int portfolioId);
    Task AddStockAsync(Stock stock);
    Task AddBondAsync(Bond bond);
    Task AddFundAsync(Fund fund);
    Task UpdateAsync(Asset asset);
    Task DeleteAsync(int id);
}