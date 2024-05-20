using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Core
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class CustomerController : ControllerBase
	{
		private readonly CustomerManager _customerManager;
		public CustomerController(CustomerManager customerManager)
		{
			_customerManager = customerManager;
		}

		[HttpGet]
		public IActionResult GetAllCustomers()
		{
			try
			{
				var customers = _customerManager.GetCustomers();
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
				var customer = _customerManager.GetCustomer(id);
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
		public IActionResult AddCustomer(Customer customer)
		{
			try
			{
				var customerId = _customerManager.AddCustomer(customer);
				var responseDTO = new CustomerResponseDTO { CustomerId = customerId };
				return Ok(responseDTO);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id}")]

		public IActionResult UpdateCustomer(Customer customer)
		{
			try
			{
				var result = _customerManager.UpdateCustomer(customer);
				return Ok(result);
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
				var result = _customerManager.DeleteCustomer(id);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}