namespace ECommerce.Core;
public class CustomerManager
{
	private CustomerRepository _customerRepository;

	public CustomerManager(CustomerRepository customerRepository)
	{
		_customerRepository = customerRepository;
	}

	public IEnumerable<Customer> GetCustomers()
	{
		IEnumerable<Customer> customers = new List<Customer>();
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
	public IEnumerable<Customer> GetCustomer(int id)
	{
		IEnumerable<Customer> customer = new List<Customer>();
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
	public int AddCustomer(Customer customer)
	{
		try
		{
			if (!this.IsCustomerExists(customer.CustomerId))
			{
				throw new Exception("The customer is not exists");
			}
			var id = this._customerRepository.AddCustomer(customer);
			return id;
		}
		catch (Exception ex)
		{
			throw;
		}
	}
	public bool UpdateCustomer(Customer customer)
	{
		bool result = false;
		try
		{
			if (!this.IsCustomerExists(customer.CustomerId))
			{
				throw new Exception("The customer is not exists");
			}
			result = this._customerRepository.UpdateCustomer(customer);
		}
		catch (Exception ex)
		{
			throw;
		}
		return true;
	}
	public bool DeleteCustomer(int id)
	{
		bool result = false;
		try
		{
			if (!this.IsCustomerExists(id))
			{
				throw new Exception("The customer is not exists");

			}
			result = this._customerRepository.DeleteCustomer(id);
		}
		catch (Exception ex)
		{
			throw;
		}
		return result;
	}
	public bool IsCustomerExists(int id)
	{
		IEnumerable<Customer> customers = this.GetCustomer(id).Where(x => x.CustomerId == id);
		return customers.Count() == 1;
	}
}
