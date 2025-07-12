using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a transaction record for an asset.
/// </summary>
public class TransactionDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the transaction.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the related asset.
    /// </summary>
    public int AssetId { get; set; }

    /// <summary>
    /// Gets or sets the date when the transaction occurred.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the asset transacted.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Gets or sets the price per unit for the transaction.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the type of transaction (e.g., Buy or Sell).
    /// </summary>
    public TransactionType Type { get; set; }
}

/// <summary>
/// DTO for creating a new transaction for an asset.
/// </summary>
public class TransactionCreateDto
{
    /// <summary>
    /// Gets or sets the identifier of the related asset.
    /// </summary>
    public int AssetId { get; set; }

    /// <summary>
    /// Gets or sets the date when the transaction occurred.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the asset transacted.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Gets or sets the price per unit for the transaction.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the type of transaction (e.g., Buy or Sell).
    /// </summary>
    public TransactionType Type { get; set; }
}

/// <summary>
/// DTO for updating transaction for an asset.
/// </summary>
public class TransactionUpdateDto
{
    /// <summary>
    /// Gets or sets the quantity of the asset transacted.
    /// </summary>
    [Required]
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the price per unit for the transaction.
    /// </summary>
    [Required]
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the type of transaction (e.g., Buy or Sell).
    /// </summary>
    [Required]
    public TransactionType Type { get; set; }

    /// <summary>
    /// Gets or sets the date when the transaction occurred.
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

}
