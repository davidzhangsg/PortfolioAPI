using Microsoft.EntityFrameworkCore;

public class AssetService : IAssetService
{
    private readonly IRepository<Asset> _assetRepo;
    private readonly IRepository<Stock> _stockRepo;
    private readonly IRepository<Bond> _bondRepo;
    private readonly IRepository<Fund> _fundRepo;

    private readonly AppDbContext _context;

    public AssetService(
        AppDbContext context,
        IRepository<Asset> assetRepo,
        IRepository<Stock> stockRepo,
        IRepository<Bond> bondRepo,
        IRepository<Fund> fundRepo)
    {
        _assetRepo = assetRepo;
        _stockRepo = stockRepo;
        _bondRepo = bondRepo;
        _fundRepo = fundRepo;
        _context = context;
    }

    public async Task<IEnumerable<Asset>> GetAllAsync() =>
    await _context.Assets
        .Include(a => a.Transactions) 
        .OrderBy(a => a.Name)         
        .ToListAsync();

    public async Task<Asset> GetByIdAsync(int id) =>
        await _context.Assets
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<Asset>> GetByPortfolioIdAsync(int portfolioId)
    {
        return await _context.Assets
            .Where(a => a.PortfolioId == portfolioId)
            .Include(a => a.Transactions)
            .ToListAsync();
    }

    public async Task AddStockAsync(Stock stock) => await _stockRepo.AddAsync(stock);

    public async Task AddBondAsync(Bond bond) => await _bondRepo.AddAsync(bond);

    public async Task AddFundAsync(Fund fund) => await _fundRepo.AddAsync(fund);

    public async Task UpdateAsync(Asset asset) => await _assetRepo.UpdateAsync(asset);

    public async Task DeleteAsync(int id) => await _assetRepo.DeleteAsync(id);
}
