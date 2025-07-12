using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

/// <summary>
/// API controller for managing customers and their associated portfolios.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(
        ICustomerService customerService,
        IMapper mapper,
        ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all customers in the system.
    /// </summary>
    /// <returns>A list of customers.</returns>
    /// <response code="200">Returns the list of customers</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        _logger.LogInformation("Fetching all customers");
        var customers = await _customerService.GetAllAsync();
        return Ok(_mapper.Map<IEnumerable<CustomerDto>>(customers));
    }

    /// <summary>
    /// Retrieves a specific customer by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the customer.</param>
    /// <returns>The customer details.</returns>
    /// <response code="200">Returns the customer</response>
    /// <response code="404">If the customer is not found</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> Get(int id)
    {
        _logger.LogInformation("Fetching customer with ID {CustomerId}", id);
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found", id);
            return NotFound();
        }
        return Ok(_mapper.Map<CustomerDto>(customer));
    }

    /// <summary>
    /// Retrieves a customer along with their portfolios and associated assets.
    /// </summary>
    /// <param name="id">The unique identifier of the customer.</param>
    /// <returns>The customer details including portfolios and assets.</returns>
    /// <response code="200">Returns the customer with portfolios and assets</response>
    /// <response code="404">If the customer is not found</response>
    [HttpGet("{id}/portfolios")]
    public async Task<ActionResult<CustomerPortfolioDto>> GetWithPortfolios(int id)
    {
        _logger.LogInformation("Fetching customer with portfolios. ID = {Id}", id);
        var customer = await _customerService.GetCustomerWithPortfoliosAsync(id);
        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {Id} not found", id);
            return NotFound();
        }

        var dto = _mapper.Map<CustomerPortfolioDto>(customer);
        return Ok(dto);
    }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="dto">The customer creation data.</param>
    /// <returns>The newly created customer.</returns>
    /// <response code="201">Returns the created customer</response>
    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] CustomerCreateDto dto)
    {
        _logger.LogInformation("Creating a new customer with name {CustomerName}", dto.Name);
        var customer = _mapper.Map<Customer>(dto);
        await _customerService.AddAsync(customer);
        var result = _mapper.Map<CustomerDto>(customer);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="id">The unique identifier of the customer to update.</param>
    /// <param name="dto">The updated customer data.</param>
    /// <returns>No content if the update was successful.</returns>
    /// <response code="204">Customer updated successfully</response>
    /// <response code="404">If the customer is not found</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdateDto dto)
    {
        _logger.LogInformation("Updating customer with ID {CustomerId}", id);
        var existing = await _customerService.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found for update", id);
            return NotFound();
        }

        _mapper.Map(dto, existing);
        await _customerService.UpdateAsync(existing);
        _logger.LogInformation("Updated customer with ID {CustomerId}", id);
        return NoContent();
    }

    /// <summary>
    /// Deletes a customer by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the customer to delete.</param>
    /// <returns>No content if the deletion was successful.</returns>
    /// <response code="204">Customer deleted successfully</response>
    /// <response code="404">If the customer is not found</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting customer with ID {CustomerId}", id);
        var existing = await _customerService.GetByIdAsync(id);
        if (existing == null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found for deletion", id);
            return NotFound();
        }

        await _customerService.DeleteAsync(id);
        _logger.LogInformation("Deleted customer with ID {CustomerId}", id);
        return NoContent();
    }
}
