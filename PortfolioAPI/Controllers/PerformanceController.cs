using Microsoft.AspNetCore.Mvc;
using Serilog;

/// <summary>
/// API controller for retrieving portfolio performance data.
/// </summary>
[ApiController]
[Route("api/portfolios/{portfolioId}/performance")]
public class PerformanceController : ControllerBase
{
    private readonly IPerformanceService _performanceService;
    private readonly ILogger<PerformanceController> _logger;

    public PerformanceController(IPerformanceService performanceService, ILogger<PerformanceController> logger)
    {
        _performanceService = performanceService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves the performance metrics for a specific portfolio within a date range.
    /// </summary>
    /// <param name="portfolioId">The unique identifier of the portfolio.</param>
    /// <param name="startDate">The start date of the performance period (inclusive).</param>
    /// <param name="endDate">The end date of the performance period (inclusive).</param>
    /// <returns>The performance data for the specified portfolio and date range.</returns>
    /// <response code="200">Returns the portfolio performance data</response>
    /// <response code="404">If no performance data is found for the portfolio</response>
    [HttpGet]
    public async Task<ActionResult<PerformanceDto>> GetPerformance(int portfolioId, DateTime startDate, DateTime endDate)
    {
        _logger.LogInformation(
            "Retrieving performance for portfolio ID {PortfolioId} from {StartDate} to {EndDate}",
            portfolioId, startDate, endDate);

        var result = await _performanceService.CalculatePerformanceAsync(portfolioId, startDate, endDate);

        if (result == null)
        {
            _logger.LogWarning("No performance data found for portfolio ID {PortfolioId}", portfolioId);
            return NotFound();
        }

        _logger.LogInformation("Successfully retrieved performance for portfolio ID {PortfolioId}", portfolioId);
        return Ok(result);
    }
}
