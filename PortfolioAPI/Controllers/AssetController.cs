using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Serilog;

/// <summary>
/// API controller for managing assets, including stocks, bonds, and funds.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assetService;
    private readonly ITransactionService _transactionService;
    private readonly IMapper _mapper;
    private readonly ILogger<AssetsController> _logger;

    public AssetsController(IAssetService assetService, ITransactionService transactionService, IMapper mapper, ILogger<AssetsController> logger)
    {
        _assetService = assetService;
        _transactionService = transactionService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all assets across all portfolios.
    /// </summary>
    /// <returns>A list of all assets.</returns>
    /// <response code="200">Returns all assets</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssetDto>>> GetAllAssets()
    {
        _logger.LogInformation("Fetching all assets");
        var assets = await _assetService.GetAllAsync();

        var assetDtos = assets.Select(asset => asset.Type switch
        {
            AssetType.Stock => _mapper.Map<StockDto>((Stock)asset),
            AssetType.Bond => _mapper.Map<BondDto>((Bond)asset),
            AssetType.Fund => _mapper.Map<FundDto>((Fund)asset),
            _ => _mapper.Map<AssetDto>(asset)
        }).ToList();

        return Ok(assetDtos);
    }

    /// <summary>
    /// Retrieves a single asset by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the asset.</param>
    /// <returns>The asset details.</returns>
    /// <response code="200">Returns the asset</response>
    /// <response code="404">If the asset is not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<AssetDto>> GetAsset(int id)
    {
        var asset = await _assetService.GetByIdAsync(id);
        if (asset == null)
        {
            _logger.LogWarning("Asset with ID {Id} not found", id);
            return NotFound();
        }

        return asset.Type switch
        {
            AssetType.Stock => Ok(_mapper.Map<StockDto>((Stock)asset)),
            AssetType.Bond => Ok(_mapper.Map<BondDto>((Bond)asset)),
            AssetType.Fund => Ok(_mapper.Map<FundDto>((Fund)asset)),
            _ => Ok(_mapper.Map<AssetDto>(asset))
        };
    }

    /// <summary>
    /// Retrieves all assets belonging to a specific portfolio.
    /// </summary>
    /// <param name="portfolioId">The unique identifier of the portfolio.</param>
    /// <returns>A list of assets for the given portfolio.</returns>
    /// <response code="200">Returns the list of assets</response>
    /// <response code="404">If no assets are found for the portfolio</response>
    [HttpGet("by-portfolio/{portfolioId}")]
    public async Task<ActionResult<IEnumerable<AssetDto>>> GetAssetsByPortfolioId(int portfolioId)
    {
        _logger.LogInformation("Fetching assets for portfolio ID {PortfolioId}", portfolioId);

        var assets = await _assetService.GetByPortfolioIdAsync(portfolioId);
        if (assets == null || !assets.Any())
        {
            _logger.LogWarning("No assets found for portfolio ID {PortfolioId}", portfolioId);
            return NotFound();
        }

        var assetDtos = assets.Select(asset => asset.Type switch
        {
            AssetType.Stock => _mapper.Map<StockDto>((Stock)asset),
            AssetType.Bond => _mapper.Map<BondDto>((Bond)asset),
            AssetType.Fund => _mapper.Map<FundDto>((Fund)asset),
            _ => _mapper.Map<AssetDto>(asset)
        }).ToList();

        return Ok(assetDtos);
    }

    /// <summary>
    /// Creates a new stock asset.
    /// </summary>
    /// <param name="dto">The stock creation data.</param>
    /// <returns>The created stock asset.</returns>
    /// <response code="201">Returns the created stock</response>
    [HttpPost("stock")]
    public async Task<ActionResult<StockDto>> CreateStock([FromBody] StockCreateDto dto)
    {
        _logger.LogInformation("Creating stock: {@Dto}", dto);
        var stock = _mapper.Map<Stock>(dto);
        await _assetService.AddStockAsync(stock);
        var resultDto = _mapper.Map<StockDto>(stock);
        _logger.LogInformation("Created stock with ID {Id}", resultDto.Id);
        return CreatedAtAction(nameof(GetAsset), new { id = resultDto.Id }, resultDto);
    }

    /// <summary>
    /// Creates a new bond asset.
    /// </summary>
    /// <param name="dto">The bond creation data.</param>
    /// <returns>The created bond asset.</returns>
    /// <response code="201">Returns the created bond</response>
    [HttpPost("bond")]
    public async Task<ActionResult<BondDto>> CreateBond([FromBody] BondCreateDto dto)
    {
        _logger.LogInformation("Creating bond: {@Dto}", dto);
        var bond = _mapper.Map<Bond>(dto);
        await _assetService.AddBondAsync(bond);
        var resultDto = _mapper.Map<BondDto>(bond);
        _logger.LogInformation("Created bond with ID {Id}", resultDto.Id);
        return CreatedAtAction(nameof(GetAsset), new { id = resultDto.Id }, resultDto);
    }

    /// <summary>
    /// Creates a new fund asset.
    /// </summary>
    /// <param name="dto">The fund creation data.</param>
    /// <returns>The created fund asset.</returns>
    /// <response code="201">Returns the created fund</response>
    [HttpPost("fund")]
    public async Task<ActionResult<FundDto>> CreateFund([FromBody] FundCreateDto dto)
    {
        _logger.LogInformation("Creating fund: {@Dto}", dto);
        var fund = _mapper.Map<Fund>(dto);
        await _assetService.AddFundAsync(fund);
        var resultDto = _mapper.Map<FundDto>(fund);
        _logger.LogInformation("Created fund with ID {Id}", resultDto.Id);
        return CreatedAtAction(nameof(GetAsset), new { id = resultDto.Id }, resultDto);
    }

    /// <summary>
    /// Updates an existing asset.
    /// </summary>
    /// <param name="id">The unique identifier of the asset to update.</param>
    /// <param name="dto">The updated asset data.</param>
    /// <response code="204">Asset updated successfully</response>
    /// <response code="404">If the asset is not found</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsset(int id, [FromBody] AssetDto dto)
    {
        _logger.LogInformation("Updating asset with ID {Id}", id);
        var existing = await _assetService.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Asset with ID {Id} not found for update", id);
            return NotFound();
        }

        existing.Name = dto.Name;
        existing.Ticker = dto.Ticker;

        await _assetService.UpdateAsync(existing);
        _logger.LogInformation("Updated asset with ID {Id}", id);
        return NoContent();
    }

    /// <summary>
    /// Deletes an asset by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the asset to delete.</param>
    /// <returns>No content on success.</returns>
    /// <response code="204">Asset deleted successfully</response>
    /// <response code="400">If the asset has existing transactions</response>
    /// <response code="404">If the asset is not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsset(int id)
    {
        var asset = await _assetService.GetByIdAsync(id);
        if (asset == null)
        {
            _logger.LogWarning("Asset with ID {Id} not found for deletion", id);
            return NotFound();
        }

        await _assetService.DeleteAsync(id);
        _logger.LogInformation("Deleted asset with ID {Id}", id);
        return NoContent();
    }
}
