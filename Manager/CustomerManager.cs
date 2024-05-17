namespace ECommerce.Core;
public class CustomerManager
{
	private CustomerRepository _customerRepository;

	public CustomerManager(CustomerRepository customerRepository)
	{
		_customerRepository = customerRepository;
	}

	public List<Customer> GetCustomers()
	{
		List<Customer> customers = new List<Customer>();
		try
		{
			customers = this._customerRepository.GetAllCustomers();
		}
		catch (Exception ex)
		{
			throw;
		}
		return customers;
	}
	public Customer? GetCustomer(int id)
	{
		Customer? customer = new Customer();
		try
		{
			customer = this._customerRepository.GetCustomerById(id);
		}
		catch (Exception ex)
		{
			throw;
		}
		return customer;
	}
	public int AddCustomer(CustomerDTO customerDTO)
	{
		var id = this._customerRepository.AddCustomer(customerDTO);
		return id;
	}
	public void UpdateCustomer(CustomerDTO customerDTO)
	{
		this._customerRepository.UpdateCustomer(customerDTO);
	}
	public void DeleteCustomer(int id)
	{
		this._customerRepository.DeleteCustomer(id);
	}
}
