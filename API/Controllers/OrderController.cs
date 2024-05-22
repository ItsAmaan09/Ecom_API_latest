using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Core
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
				var response = _orderManager.GetAllOrders(Status);
				return Ok(response);
			}
			catch (System.Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{id}")]
		public IActionResult GetOrderDetails(int id)
		{
			try
			{
				var response = _orderManager.GetOrderDetails(id);
				return Ok(response);
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
				var response = _orderManager.CreateOrder(orderDTO);
				return Ok(response);
			}
			catch (System.Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Route("{id}/Confirm")]
		public IActionResult ConfirmOrder(int id)
		{
			try
			{
				var response = _orderManager.ConfirmOrder(id);
				return Ok(response);
			}
			catch (System.Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Route("{id}/status")]
		public IActionResult UpdateOrderStatus(int id, OrderStatusDTO orderStatusDTO)
		{
			try
			{
				var response = _orderManager.UpdateOrderStatus(id, orderStatusDTO.Status);
				return Ok(response);
			}
			catch (System.Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}