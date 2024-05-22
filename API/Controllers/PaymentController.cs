using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Core
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class PaymentController : ControllerBase
	{
		private readonly PaymentManager _paymentManager;
		public PaymentController(PaymentManager paymentManager)
		{
			_paymentManager = paymentManager;
		}

		[HttpGet("{id}")]
		public IActionResult GetPaymentDetails(int id)
		{
			try
			{
				var response = _paymentManager.GetPaymentDetails(id);
				return Ok(response);
			}
			catch (System.Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPost]
		[Route("MakePayment")]
		public IActionResult MakePayment(PaymentDTO paymentDTO)
		{
			try
			{
				var response = _paymentManager.MakePayment(paymentDTO);
				return Ok(response);
			}
			catch (System.Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpPost]
		[Route("UpdatePaymentStatus/{id}")]
		public IActionResult UpdatePaymentStatus(int id,PaymentStatusDTO paymentStatusDTO)
		{
			try
			{
				var response = _paymentManager.UpdatePaymentStatus(id,paymentStatusDTO.Status);
				return Ok(response);
			}
			catch (System.Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}