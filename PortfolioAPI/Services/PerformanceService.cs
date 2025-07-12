using Microsoft.EntityFrameworkCore;

public class PerformanceService : IPerformanceService
{
    private readonly AppDbContext _context;
    private readonly ILogger<PerformanceService> _logger;

    public PerformanceService(AppDbContext context, ILogger<PerformanceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PerformanceDto> CalculatePerformanceAsync(int portfolioId, DateTime startDate, DateTime endDate)
    {
        var portfolio = await _context.Portfolios
            .Include(p => p.Assets)
                .ThenInclude(a => a.Transactions)
            .FirstOrDefaultAsync(p => p.Id == portfolioId);

        if (portfolio == null) return null;

        var assetPerformances = new List<AssetPerformanceDto>();
        var valueOverTime = new List<ValueOverTimeDto>();
        decimal totalValue = 0;

        var dailyDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                   .Select(offset => startDate.AddDays(offset))
                                   .ToList();

        foreach (var date in dailyDates)
        {
            decimal dailyTotal = 0;

            foreach (var asset in portfolio.Assets)
            {
                var transactions = asset.Transactions
                    .Where(t => t.Date <= date)
                    .OrderBy(t => t.Date)
                    .ToList();

                decimal quantity = 0;
                decimal costBasis = 0;
                decimal realizedGain = 0;

                foreach (var txn in transactions)
                {
                    if (txn.Type == TransactionType.Buy)
                    {
                        quantity += txn.Quantity;
                        costBasis += txn.Quantity * txn.Price;
                    }
                    else if (txn.Type == TransactionType.Sell)
                    {
                        var avgCost = quantity > 0 ? costBasis / quantity : 0;
                        realizedGain += (txn.Price - avgCost) * txn.Quantity;
                        quantity -= txn.Quantity;
                        costBasis -= avgCost * txn.Quantity;
                    }
                }

                var currentPrice = transactions.LastOrDefault()?.Price ?? 0;
                var value = currentPrice * quantity;

                if (date == endDate)
                {
                    var unrealizedGain = value - costBasis;

                    assetPerformances.Add(new AssetPerformanceDto
                    {
                        AssetId = asset.Id,
                        Name = asset.Name,
                        Ticker = asset.Ticker,
                        Value = value,
                        RealizedGain = realizedGain,
                        UnrealizedGain = unrealizedGain
                    });
                }

                dailyTotal += value;
            }

            valueOverTime.Add(new ValueOverTimeDto
            {
                Date = date,
                Value = dailyTotal
            });

            if (date == endDate)
            {
                totalValue = dailyTotal;
            }
        }

        // Asset Allocation
        var allocation = assetPerformances
            .Select(a => new AssetAllocationDto
            {
                AssetName = a.Name,
                Value = a.Value,
                AllocationPercentage = totalValue > 0 ? a.Value / totalValue * 100 : 0
            }).ToList();

        return new PerformanceDto
        {
            PortfolioId = portfolioId,
            StartDate = startDate,
            EndDate = endDate,
            TotalValue = totalValue,
            Assets = assetPerformances,
            ValueOverTime = valueOverTime,
            Allocation = allocation
        };
    }
}
