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
	public IEnumerable<Customer> GetCustomerById(int id)
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
	public IEnumerable<Customer> GetCustomerByEmail(string Email)
	{
		IEnumerable<Customer> customer = new List<Customer>();
		try
		{
			customer = this._customerRepository.GetCustomerByEmail(Email);
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
			if(!customer.IsValid())
			{
				throw new Exception("Customer is not valid.");
			}

			if (this.IsDuplicateCustomer(customer))
			{
				throw new Exception("The customer with same email is already exists.");
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
			if(!customer.IsValid())
			{
				throw new Exception("Customer is not valid.");
			}

			if (!this.IsCustomerExists(customer.CustomerId))
			{
				throw new Exception("The customer is not exists");
			}

			if (this.IsDuplicateCustomer(customer))
			{
				throw new Exception("The customer with same email is already exists.");
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
		IEnumerable<Customer> customers = this.GetCustomerById(id).Where(x => x.CustomerId == id);
		return customers.Count() == 1;
	}
	public bool IsDuplicateCustomer(Customer customer)
	{
		IEnumerable<Customer> customers = this.GetCustomerByEmail(customer.Email);

		int count = customers.Where(x => x.CustomerId != customer.CustomerId).Count();
		return count > 0;
	}
}
