using Microsoft.EntityFrameworkCore;

public class PortfolioService : IPortfolioService
{
    private readonly AppDbContext _context;

    public PortfolioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Portfolio>> GetAllAsync() =>
        await _context.Portfolios.Include(p => p.Assets).Include(p => p.Customer).ToListAsync();

    public async Task<Portfolio?> GetByIdAsync(int id) =>
        await _context.Portfolios.Include(p => p.Assets).Include(p => p.Customer)
                                 .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Portfolio>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Portfolios.Include(p => p.Assets)
                                        .Where(p => p.CustomerId == customerId)
                                        .ToListAsync();
    }

    public async Task AddAsync(Portfolio portfolio)
    {
        _context.Portfolios.Add(portfolio);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Portfolio portfolio)
    {
        _context.Portfolios.Update(portfolio);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var portfolio = await _context.Portfolios.FindAsync(id);
        if (portfolio != null)
        {
            _context.Portfolios.Remove(portfolio);
            await _context.SaveChangesAsync();
        }
    }

}
