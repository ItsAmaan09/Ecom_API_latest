using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Core;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class OrderController : ControllerBase
	{
		private readonly OrderManager _orderManager;
		public OrderController(OrderManager orderManager)
		{
			_orderManager = orderManager;
		}

		[HttpGet]
		public IActionResult GetAllOrders(String Status)
		{
			try
			{
				var orders = _orderManager.GetAllOrders(Status);
				return Ok(orders);
			}
			catch (System.Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpPost]
		[Route("Create")]
		public IActionResult CreateOrder(OrderDTO orderDTO)
		{
			try
			{
				var result = _orderManager.CreateOrder(orderDTO);
				return Ok(result);
			}
			catch (System.Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}