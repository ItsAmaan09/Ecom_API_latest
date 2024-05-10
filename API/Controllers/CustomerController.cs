using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Core
{
	[ApiController]
	[Route("api/[controller]")]
	public class CustomerController : ControllerBase
	{
		private readonly CustomerRepository _customerRepository;
		public CustomerController(CustomerRepository customerRepository)
		{
			_customerRepository = customerRepository;
		}

		[HttpGet]
		public IActionResult GetAllCustomers()
		{
			try
			{
				var customers = _customerRepository.GetAllCustomers();
				return Ok(customers);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpGet("{id}")]
		public IActionResult GetCustomerById(int id)
		{
			try
			{
				var customer = _customerRepository.GetCustomerById(id);
				if (customer == null)
				{
					return NotFound();
				}
				return Ok(customer);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		public IActionResult AddCustomer(CustomerDTO customerDTO)
		{
			try
			{
				var customerId = _customerRepository.AddCustomer(customerDTO);
				var responseDTO = new CustomerResponseDTO { CustomerId = customerId };
				return Ok(responseDTO);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id}")]

		public IActionResult UpdateCustomer(int id, CustomerDTO customerDTO)
		{
			try
			{
				if (id != customerDTO.CustomerId)
				{
					return BadRequest("Id not found");
				}
				_customerRepository.UpdateCustomer(customerDTO);
				return Ok(true);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public IActionResult DeleteCustomer(int id)
		{
			try
			{
				var customer = _customerRepository.GetCustomerById(id);
				if (customer == null)
				{
					return NotFound();
				}
				_customerRepository.DeleteCustomer(id);
				return Ok(true);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}