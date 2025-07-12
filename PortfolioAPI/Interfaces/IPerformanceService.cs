public interface IPerformanceService
{
    Task<PerformanceDto> CalculatePerformanceAsync(int portfolioId, DateTime startDate, DateTime endDate);
}
