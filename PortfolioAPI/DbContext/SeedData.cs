
public static class SeedData
{
    public static void Seed(AppDbContext context)
    {
        if (context.Portfolios.Any()) return; // Prevent duplicate seeding

        var customer = new Customer { Id = 1, Name = "John Doe" };
        context.Customers.Add(customer);

        var portfolio = new Portfolio { Id = 1, Name = "Default Portfolio", CustomerId = 1 };
        context.Portfolios.Add(portfolio);

        var stock = new Stock
        {
            Id = 1,
            Name = "Apple Inc",
            Ticker = "AAPL",
            Exchange = "NASDAQ",
            Sector = "Technology",
            DividendYield = 0.6m,
            PortfolioId = 1,
            Type = AssetType.Stock
        };

        var bond = new Bond
        {
            Id = 2,
            Name = "US Treasury 10Y",
            Ticker = "UST10",
            Issuer = "US Govt",
            CouponRate = 2.5m,
            MaturityDate = DateTime.UtcNow.AddYears(10),
            PortfolioId = 1,
            Type = AssetType.Bond
        };

        var fund = new Fund
        {
            Id = 3,
            Name = "S&P 500 ETF",
            Ticker = "SPY",
            FundType = FundType.Index,
            ExpenseRatio = 0.09m,
            PortfolioId = 1,
            FundManager = "Vanguard",
            Type = AssetType.Fund
        };

        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = 1,
                AssetId = 1,
                Date = DateTime.UtcNow.AddDays(-20),
                Quantity = 10,
                Price = 150,
                Type = TransactionType.Buy
            },
            new Transaction
            {
                Id = 2,
                AssetId = 2,
                Date = DateTime.UtcNow.AddDays(-15),
                Quantity = 5,
                Price = 100,
                Type = TransactionType.Buy
            },
            new Transaction
            {
                Id = 3,
                AssetId = 3,
                Date = DateTime.UtcNow.AddDays(-10),
                Quantity = 3,
                Price = 300,
                Type = TransactionType.Buy
            }
        };

        context.Assets.AddRange(stock, bond, fund);
        context.Transactions.AddRange(transactions);

        context.SaveChanges();
    }
}

