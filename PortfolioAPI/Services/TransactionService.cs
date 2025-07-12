using Microsoft.EntityFrameworkCore;

public class TransactionService : ITransactionService
{
    private readonly IRepository<Transaction> _transactionRepo;
    private readonly AppDbContext _context;

    public TransactionService(AppDbContext context, IRepository<Transaction> transactionRepo)
    {
        _transactionRepo = transactionRepo;
        _context = context;
    }

    public async Task<(IEnumerable<Transaction> Transactions, int TotalCount)> GetByDateRangeAsync(DateTime start, DateTime end, int page, int pageSize)
    {
        var query = _context.Transactions
            .Where(t => t.Date >= start && t.Date <= end)
            .OrderByDescending(t => t.Date);

        var totalCount = await query.CountAsync();

        var transactions = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (transactions, totalCount);
    }

    public async Task<Transaction> GetByIdAsync(int id) =>
        await _transactionRepo.GetByIdAsync(id);

    public async Task<IEnumerable<Transaction>> GetByAssetIdAsync(int assetId)
    {
        return await _context.Transactions
            .Where(t => t.AssetId == assetId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task AddAsync(Transaction transaction) =>
        await _transactionRepo.AddAsync(transaction);

    public async Task UpdateAsync(Transaction transaction) =>
        await _transactionRepo.UpdateAsync(transaction);

    public async Task DeleteAsync(int id)
    {
        var txn = await _context.Transactions.FindAsync(id);
        if (txn == null) return;

        _context.Transactions.Remove(txn);
        await _context.SaveChangesAsync();
    }

}
