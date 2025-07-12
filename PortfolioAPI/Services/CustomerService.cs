using Microsoft.EntityFrameworkCore;

public class CustomerService : ICustomerService
{
    private readonly IRepository<Customer> _customerRepo;
    private readonly AppDbContext _context;

    public CustomerService(AppDbContext context, IRepository<Customer> customerRepo)
    {
        _customerRepo = customerRepo;
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync() => await _customerRepo.GetAllAsync();

    public async Task<Customer> GetByIdAsync(int id) => await _customerRepo.GetByIdAsync(id);

    public async Task<Customer> GetCustomerWithPortfoliosAsync(int customerId)
    {
        return await _context.Customers
            .Include(c => c.Portfolios)
                .ThenInclude(p => p.Assets)
            .FirstOrDefaultAsync(c => c.Id == customerId);
    }

    public async Task AddAsync(Customer customer) => await _customerRepo.AddAsync(customer);

    public async Task UpdateAsync(Customer customer) => await _customerRepo.UpdateAsync(customer);

    public async Task DeleteAsync(int id) => await _customerRepo.DeleteAsync(id);



}
