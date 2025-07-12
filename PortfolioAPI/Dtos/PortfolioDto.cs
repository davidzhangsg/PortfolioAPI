/// <summary>
/// Represents a portfolio belonging to a customer.
/// </summary>
public class PortfolioDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the portfolio.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the portfolio name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the customer who owns the portfolio.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the customer who owns the portfolio.
    /// </summary>
    public CustomerDto Customer { get; set; }

    /// <summary>
    /// Gets or sets the list of assets associated with the portfolio.
    /// </summary>
    public List<AssetDto> Assets { get; set; } = new();
}


/// <summary>
/// DTO for creating a new portfolio.
/// </summary>
public class PortfolioCreateDto
{
    /// <summary>
    /// Gets or sets the portfolio name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the customer who will own the portfolio.
    /// </summary>
    public int CustomerId { get; set; }
}

/// <summary>
/// DTO for updating an existing portfolio.
/// </summary>
public class PortfolioUpdateDto
{
    /// <summary>
    /// Gets or sets the updated portfolio name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the updated customer identifier for the portfolio.
    /// </summary>
    public int CustomerId { get; set; }
}

/// <summary>
/// Represents a portfolio along with its associated assets.
/// </summary>
public class PortfolioWithAssetsDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the portfolio.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the portfolio.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the customer who owns the portfolio.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the list of assets associated with this portfolio.
    /// </summary>
    public List<AssetDto> Assets { get; set; } = new();
}
