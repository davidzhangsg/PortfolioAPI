/// <summary>
/// Data Transfer Object for returning customer details.
/// </summary>
public class CustomerDto
{
    /// <summary>Unique identifier of the customer.</summary>
    public int Id { get; set; }

    /// <summary>Name of the customer.</summary>
    public string Name { get; set; }

}

/// <summary>
/// Data Transfer Object for creating a new customer.
/// </summary>
public class CustomerCreateDto
{
    /// <summary>Name of the customer.</summary>
    public string Name { get; set; }
}

/// <summary>
/// Data Transfer Object for updating an existing customer.
/// </summary>
public class CustomerUpdateDto
{
    /// <summary>Name of the customer.</summary>
    public string Name { get; set; }
}

/// <summary>
/// DTO representing a customer along with their investment portfolios and associated assets.
/// </summary>
public class CustomerPortfolioDto
{
    /// <summary>
    /// Unique identifier of the customer.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the customer.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// List of portfolios owned by the customer, each including associated assets.
    /// </summary>
    public List<PortfolioWithAssetsDto> Portfolios { get; set; }
}


