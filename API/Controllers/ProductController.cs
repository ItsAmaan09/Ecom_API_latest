using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ECommerce.Core
{
	[ApiController]
	[Route("api/v1/[controller]")]
	public class ProductController : ControllerBase
	{
		private readonly ProductManager _productManager;
		public ProductController(ProductManager productManager)
		{
			_productManager = productManager;
		}
		[HttpGet]
		public IActionResult GetAllProducts()
		{
			try
			{
				var products = _productManager.GetAllProducts();
				return Ok(products);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpGet("{id}")]
		public IActionResult GetProductById(int id)
		{
			try
			{
				var products = _productManager.GetProductById(id);
				return Ok(products);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpPost]
		[Route("Add")]
		public IActionResult AddProduct(Product product)
		{
			try
			{
				var productId = _productManager.AddProduct(product);
				var responseDTO = new ProductResponseDTO { ProductId = productId };
				return Ok(responseDTO);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpPost]
		[Route("Update")]
		public IActionResult UpdateProduct(Product product)
		{
			try
			{
				var result = _productManager.UpdateProduct(product);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		[HttpDelete("{id}")]
		public IActionResult DeleteProduct(int id)
		{
			try
			{
				var result = _productManager.DeleteProduct(id);
				return Ok(result);
			}
			catch (Exception ex)
			{

				return BadRequest(ex.Message);
			}
		}

	}
}