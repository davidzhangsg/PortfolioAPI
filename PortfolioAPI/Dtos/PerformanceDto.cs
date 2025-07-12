/// <summary>
/// Data transfer object representing the performance of a portfolio over a date range.
/// </summary>
public class PerformanceDto
{
    /// <summary>
    /// Identifier of the portfolio.
    /// </summary>
    public int PortfolioId { get; set; }

    /// <summary>
    /// Start date of the performance period.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// End date of the performance period.
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Total value of the portfolio at the end of the period.
    /// </summary>
    public decimal TotalValue { get; set; }

    /// <summary>
    /// List of individual asset performance within the portfolio.
    /// </summary>
    public List<AssetPerformanceDto> Assets { get; set; } = new();

    /// <summary>
    /// Historical portfolio value over time during the performance period.
    /// </summary>
    public List<ValueOverTimeDto> ValueOverTime { get; set; } = new();

    /// <summary>
    /// Breakdown of the asset allocation within the portfolio.
    /// </summary>
    public List<AssetAllocationDto> Allocation { get; set; } = new();
}

/// <summary>
/// Represents portfolio value on a specific date.
/// </summary>
public class ValueOverTimeDto
{
    /// <summary>
    /// Date of the valuation.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Total value of the portfolio on that date.
    /// </summary>
    public decimal Value { get; set; }
}

/// <summary>
/// Represents asset allocation information within a portfolio.
/// </summary>
public class AssetAllocationDto
{
    /// <summary>
    /// Name of the asset.
    /// </summary>
    public string AssetName { get; set; }

    /// <summary>
    /// Current value of the asset.
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Percentage allocation of the asset in the total portfolio.
    /// </summary>
    public decimal AllocationPercentage { get; set; }
}
