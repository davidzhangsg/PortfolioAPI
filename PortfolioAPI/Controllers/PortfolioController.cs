using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Serilog;

/// <summary>
/// API controller for managing investment portfolios.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PortfoliosController : ControllerBase
{
    private readonly IPortfolioService _portfolioService;
    private readonly IMapper _mapper;
    private readonly ILogger<PortfoliosController> _logger;

    public PortfoliosController(IPortfolioService portfolioService, IMapper mapper, ILogger<PortfoliosController> logger)
    {
        _portfolioService = portfolioService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all portfolios.
    /// </summary>
    /// <returns>A list of all portfolios.</returns>
    /// <response code="200">Returns the list of portfolios</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PortfolioDto>>> GetAll()
    {
        _logger.LogInformation("Fetching all portfolios");
        var portfolios = await _portfolioService.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<PortfolioDto>>(portfolios));
    }

    /// <summary>
    /// Retrieves a specific portfolio by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the portfolio.</param>
    /// <returns>The portfolio details.</returns>
    /// <response code="200">Returns the portfolio details</response>
    /// <response code="404">If the portfolio is not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<PortfolioDto>> Get(int id)
    {
        _logger.LogInformation("Fetching portfolio with ID {Id}", id);
        var portfolio = await _portfolioService.GetByIdAsync(id);
        if (portfolio == null)
        {
            _logger.LogWarning("Portfolio with ID {Id} not found", id);
            return NotFound();
        }

        return Ok(_mapper.Map<PortfolioDto>(portfolio));
    }

    /// <summary>
    /// Retrieves all portfolios for a specific customer, including their assets.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <returns>A list of portfolios with associated assets.</returns>
    /// <response code="200">Returns the list of portfolios with assets</response>
    /// <response code="404">If no portfolios are found for the customer</response>
    [HttpGet("by-customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<PortfolioWithAssetsDto>>> GetByCustomer(int customerId)
    {
        _logger.LogInformation("Fetching portfolios for customer ID {CustomerId}", customerId);
        var portfolios = await _portfolioService.GetByCustomerIdAsync(customerId);

        if (portfolios == null || !portfolios.Any())
        {
            _logger.LogWarning("No portfolios found for customer ID {CustomerId}", customerId);
            return NotFound();
        }

        return Ok(_mapper.Map<IEnumerable<PortfolioWithAssetsDto>>(portfolios));
    }

    /// <summary>
    /// Creates a new portfolio.
    /// </summary>
    /// <param name="dto">The portfolio creation data.</param>
    /// <returns>The newly created portfolio.</returns>
    /// <response code="201">Returns the created portfolio</response>
    /// <response code="400">If the input data is invalid</response>
    [HttpPost]
    public async Task<ActionResult<PortfolioDto>> Create([FromBody] PortfolioCreateDto dto)
    {
        _logger.LogInformation("Creating new portfolio with name {Name}", dto.Name);
        var portfolio = _mapper.Map<Portfolio>(dto);
        await _portfolioService.AddAsync(portfolio);
        var resultDto = _mapper.Map<PortfolioDto>(portfolio);
        _logger.LogInformation("Created portfolio with ID {Id}", resultDto.Id);
        return CreatedAtAction(nameof(Get), new { id = resultDto.Id }, resultDto);
    }

    /// <summary>
    /// Updates an existing portfolio.
    /// </summary>
    /// <param name="id">The unique identifier of the portfolio.</param>
    /// <param name="dto">The updated portfolio data.</param>
    /// <response code="204">Portfolio updated successfully</response>
    /// <response code="404">If the portfolio is not found</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PortfolioUpdateDto dto)
    {
        _logger.LogInformation("Updating portfolio with ID {Id}", id);
        var existing = await _portfolioService.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Portfolio with ID {Id} not found for update", id);
            return NotFound();
        }

        _mapper.Map(dto, existing);
        await _portfolioService.UpdateAsync(existing);
        _logger.LogInformation("Updated portfolio with ID {Id}", id);
        return NoContent();
    }

    /// <summary>
    /// Deletes a portfolio by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the portfolio.</param>
    /// <response code="204">Portfolio deleted successfully</response>
    /// <response code="400">If the portfolio has linked assets</response>
    /// <response code="404">If the portfolio is not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting portfolio with ID {Id}", id);

        var portfolio = await _portfolioService.GetByIdAsync(id);
        if (portfolio == null)
        {
            _logger.LogWarning("Portfolio with ID {Id} not found", id);
            return NotFound();
        }

        try
        {
            await _portfolioService.DeleteAsync(id);
            _logger.LogInformation("Deleted portfolio with ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting portfolio with ID {Id}", id);
            return StatusCode(500, "An error occurred while deleting the portfolio.");
        }
    }
}
