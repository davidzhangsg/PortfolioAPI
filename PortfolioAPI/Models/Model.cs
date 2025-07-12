public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
}

public class Portfolio
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
}

public abstract class Asset
{
    public int Id { get; set; }
    public string Ticker { get; set; }
    public string Name { get; set; }
    public AssetType Type { get; set; }

    public int PortfolioId { get; set; }
    public Portfolio Portfolio { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}

public class Stock : Asset
{
    public string Exchange { get; set; }
    public string Sector { get; set; }
    public decimal DividendYield { get; set; }
}

public class Bond : Asset
{
    public decimal CouponRate { get; set; }
    public DateTime MaturityDate { get; set; }
    public string Issuer { get; set; }
    public BondType BondType { get; set; }
}

public class Fund : Asset
{
    public string FundManager { get; set; }
    public FundType FundType { get; set; }
    public decimal ExpenseRatio { get; set; }
}

public class Transaction
{
    public int Id { get; set; }

    public int AssetId { get; set; }
    public Asset Asset { get; set; }

    public DateTime Date { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public TransactionType Type { get; set; }
}
