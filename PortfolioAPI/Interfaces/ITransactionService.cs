public interface ITransactionService
{
    Task<Transaction> GetByIdAsync(int id);
    Task AddAsync(Transaction transaction);
    Task DeleteAsync(int id);
    Task<IEnumerable<Transaction>> GetByAssetIdAsync(int assetId);
    Task UpdateAsync(Transaction transaction);
    Task<(IEnumerable<Transaction> Transactions, int TotalCount)> GetByDateRangeAsync(DateTime start, DateTime end, int page, int pageSize);


}