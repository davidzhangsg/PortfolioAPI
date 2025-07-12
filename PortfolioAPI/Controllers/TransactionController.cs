using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Serilog;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IAssetService _assetService;
    private readonly IMapper _mapper;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(
        ITransactionService transactionService,
        IAssetService assetService,
        IMapper mapper,
        ILogger<TransactionsController> logger)
    {
        _transactionService = transactionService;
        _assetService = assetService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new transaction.
    /// </summary>
    /// <param name="dto">Transaction creation data.</param>
    /// <returns>The created transaction.</returns>
    /// <response code="201">Returns the newly created transaction</response>
    /// <response code="400">If the asset ID is invalid</response>
    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create([FromBody] TransactionCreateDto dto)
    {
        _logger.LogInformation("Attempting to create transaction for Asset ID {AssetId}", dto.AssetId);

        var asset = await _assetService.GetByIdAsync(dto.AssetId);
        if (asset == null)
        {
            _logger.LogWarning("Transaction creation failed: Asset ID {AssetId} not found", dto.AssetId);
            return BadRequest("Invalid asset ID");
        }

        var transaction = _mapper.Map<Transaction>(dto);
        await _transactionService.AddAsync(transaction);
        var resultDto = _mapper.Map<TransactionDto>(transaction);

        _logger.LogInformation("Created transaction ID {TransactionId} for Asset ID {AssetId}", resultDto.Id, dto.AssetId);
        return CreatedAtAction(nameof(Get), new { id = resultDto.Id }, resultDto);
    }

    /// <summary>
    /// Updates an existing transaction.
    /// </summary>
    /// <param name="id">Transaction ID.</param>
    /// <param name="dto">Updated transaction data.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Transaction updated successfully</response>
    /// <response code="404">If the transaction is not found</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TransactionUpdateDto dto)
    {
        _logger.LogInformation("Updating transaction with ID {Id}", id);

        var transaction = await _transactionService.GetByIdAsync(id);
        if (transaction == null)
        {
            _logger.LogWarning("Transaction with ID {Id} not found for update", id);
            return NotFound();
        }

        try
        {
            _mapper.Map(dto, transaction); // Only maps allowed fields
            await _transactionService.UpdateAsync(transaction);
            _logger.LogInformation("Updated transaction with ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction with ID {Id}", id);
            return StatusCode(500, "An error occurred while updating the transaction.");
        }
    }

    /// <summary>
    /// Gets a transaction by its ID.
    /// </summary>
    /// <param name="id">Transaction ID.</param>
    /// <returns>The transaction details.</returns>
    /// <response code="200">Returns the transaction</response>
    /// <response code="404">If the transaction is not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> Get(int id)
    {
        _logger.LogInformation("Fetching transaction with ID {Id}", id);
        var txn = await _transactionService.GetByIdAsync(id);
        if (txn == null)
        {
            _logger.LogWarning("Transaction with ID {Id} not found", id);
            return NotFound();
        }

        return Ok(_mapper.Map<TransactionDto>(txn));
    }

    /// <summary>
    /// Gets all transactions for a specific asset.
    /// </summary>
    /// <param name="assetId">Asset ID.</param>
    /// <returns>List of transactions linked to the asset.</returns>
    /// <response code="200">Returns the list of transactions</response>
    /// <response code="404">If no transactions are found for the asset</response>
    [HttpGet("by-asset/{assetId}")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetByAssetId(int assetId)
    {
        _logger.LogInformation("Fetching transactions for Asset ID {AssetId}", assetId);

        var transactions = await _transactionService.GetByAssetIdAsync(assetId);
        if (transactions == null || !transactions.Any())
        {
            _logger.LogWarning("No transactions found for Asset ID {AssetId}", assetId);
            return NotFound();
        }

        return Ok(_mapper.Map<IEnumerable<TransactionDto>>(transactions));
    }

    /// <summary>
    /// Searches transactions within a date range with pagination support.
    /// </summary>
    /// <param name="startDate">Start date of the search range.</param>
    /// <param name="endDate">End date of the search range.</param>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Number of records per page (default 10).</param>
    /// <returns>Paginated list of transactions and total count.</returns>
    /// <response code="200">Returns paginated transactions</response>
    [HttpGet("search")]
    public async Task<ActionResult<object>> Search(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation("Searching transactions from {StartDate} to {EndDate}, Page: {Page}, PageSize: {PageSize}",
            startDate, endDate, page, pageSize);

        var (transactions, totalCount) = await _transactionService.GetByDateRangeAsync(startDate, endDate, page, pageSize);

        var result = new
        {
            totalCount,
            page,
            pageSize,
            transactions = _mapper.Map<IEnumerable<TransactionDto>>(transactions)
        };

        return Ok(result);
    }

    /// <summary>
    /// Deletes a transaction by its ID.
    /// </summary>
    /// <param name="id">Transaction ID.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Transaction deleted successfully</response>
    /// <response code="404">If the transaction is not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting transaction with ID {Id}", id);

        var txn = await _transactionService.GetByIdAsync(id);
        if (txn == null)
        {
            _logger.LogWarning("Transaction with ID {Id} not found", id);
            return NotFound();
        }

        await _transactionService.DeleteAsync(id);

        _logger.LogInformation("Deleted transaction with ID {Id}", id);
        return NoContent();
    }
}
