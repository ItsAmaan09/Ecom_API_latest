using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Core
{
	[ApiController]
	[Route("api/[controller]")]
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
		public IActionResult AddCustomer(CustomerDTO customerDTO)
		{
			try
			{
				var customerId = _customerManager.AddCustomer(customerDTO);
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
				_customerManager.UpdateCustomer(customerDTO);
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
				var customer = _customerManager.GetCustomer(id);
				if (customer == null)
				{
					return NotFound();
				}
				_customerManager.DeleteCustomer(id);
				return Ok(true);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}