/// <summary>
/// Base DTO representing a general asset.
/// </summary>
public class AssetDto
{
    /// <summary>Unique identifier of the asset.</summary>
    public int Id { get; set; }

    /// <summary>Identifier of the portfolio this asset belongs to.</summary>
    public int PortfolioId { get; set; }

    /// <summary>Ticker symbol of the asset.</summary>
    public string Ticker { get; set; }

    /// <summary>Name of the asset.</summary>
    public string Name { get; set; }

    /// <summary>Type of the asset (e.g., Stock, Bond, Fund).</summary>
    public AssetType Type { get; set; }

    /// <summary>
    /// Collection of all transactions (buys, sells) related to this asset.
    /// </summary>
    public List<TransactionDto> Transactions { get; set; } = new();
}

/// <summary>
/// DTO representing a stock asset.
/// </summary>
public class StockDto : AssetDto
{
    /// <summary>Stock exchange where the stock is listed.</summary>
    public string Exchange { get; set; }

    /// <summary>Sector the stock belongs to (e.g., Technology, Finance).</summary>
    public string Sector { get; set; }

    /// <summary>Dividend yield percentage of the stock.</summary>
    public decimal DividendYield { get; set; }
}

/// <summary>
/// DTO representing a bond asset.
/// </summary>
public class BondDto : AssetDto
{
    /// <summary>Coupon rate percentage of the bond.</summary>
    public decimal CouponRate { get; set; }

    /// <summary>Maturity date of the bond.</summary>
    public DateTime MaturityDate { get; set; }

    /// <summary>Issuer of the bond.</summary>
    public string Issuer { get; set; }

    /// <summary>Type of bond (e.g., Government, Corporate).</summary>
    public BondType BondType { get; set; }
}

/// <summary>
/// DTO representing a fund asset.
/// </summary>
public class FundDto : AssetDto
{
    /// <summary>Name of the fund manager.</summary>
    public string FundManager { get; set; }

    /// <summary>Type of fund (e.g., Index, Mutual).</summary>
    public FundType FundType { get; set; }

    /// <summary>Expense ratio percentage of the fund.</summary>
    public decimal ExpenseRatio { get; set; }
}

/// <summary>
/// Base DTO for creating an asset.
/// </summary>
public class AssetCreateDto
{
    /// <summary>Portfolio ID to associate this asset with.</summary>
    public int PortfolioId { get; set; }

    /// <summary>Ticker symbol of the asset.</summary>
    public string Ticker { get; set; }

    /// <summary>Name of the asset.</summary>
    public string Name { get; set; }

    /// <summary>Type of the asset.</summary>
    public AssetType Type { get; set; }
}

/// <summary>
/// DTO for creating a stock asset.
/// </summary>
public class StockCreateDto : AssetCreateDto
{
    /// <summary>Stock exchange where the stock is listed.</summary>
    public string Exchange { get; set; }

    /// <summary>Sector the stock belongs to.</summary>
    public string Sector { get; set; }

    /// <summary>Dividend yield percentage of the stock.</summary>
    public decimal DividendYield { get; set; }
}

/// <summary>
/// DTO for creating a bond asset.
/// </summary>
public class BondCreateDto : AssetCreateDto
{
    /// <summary>Coupon rate percentage of the bond.</summary>
    public decimal CouponRate { get; set; }

    /// <summary>Maturity date of the bond.</summary>
    public DateTime MaturityDate { get; set; }

    /// <summary>Issuer of the bond.</summary>
    public string Issuer { get; set; }

    /// <summary>Type of bond.</summary>
    public BondType BondType { get; set; }
}

/// <summary>
/// DTO for creating a fund asset.
/// </summary>
public class FundCreateDto
{
    /// <summary>Portfolio ID to associate this fund with.</summary>
    public int PortfolioId { get; set; }

    /// <summary>Name of the fund.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Ticker symbol of the fund.</summary>
    public string Ticker { get; set; } = string.Empty;

    /// <summary>Type of fund.</summary>
    public FundType FundType { get; set; }

    /// <summary>Expense ratio percentage of the fund.</summary>
    public decimal ExpenseRatio { get; set; }

    /// <summary>Name of the fund manager.</summary>
    public string? FundManager { get; set; } = string.Empty;

    /// <summary>Type of asset, default is Fund.</summary>
    public AssetType Type { get; set; } = AssetType.Fund;
}

/// <summary>
/// DTO representing the performance of an asset.
/// </summary>
public class AssetPerformanceDto
{
    /// <summary>Unique identifier of the asset.</summary>
    public int AssetId { get; set; }

    /// <summary>Name of the asset.</summary>
    public string Name { get; set; }

    /// <summary>Ticker symbol of the asset.</summary>
    public string Ticker { get; set; }

    /// <summary>Current total value of the asset.</summary>
    public decimal Value { get; set; }

    /// <summary>Total realized gain for the asset.</summary>
    public decimal RealizedGain { get; set; }

    /// <summary>Total unrealized gain for the asset.</summary>
    public decimal UnrealizedGain { get; set; }
}
