using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Core
{
	public class ProductManager
	{
		private ProductRepository _productRepository;
		public ProductManager(ProductRepository productRepository)
		{
			_productRepository = productRepository;
		}
		public IEnumerable<Product> GetAllProducts()
		{
			IEnumerable<Product> products = new List<Product>();
			try
			{
				products = this._productRepository.GetAllProducts();
			}
			catch (Exception ex)
			{
				throw;
			}
			return products;
		}
		public IEnumerable<Product> GetProductById(int id)
		{
			IEnumerable<Product> products = new List<Product>();
			try
			{
				products = this._productRepository.GetProductById(id);
			}
			catch (Exception ex)
			{
				throw;
			}
			return products;
		}
		public int AddProduct(Product product)
		{
			try
			{
				var id = this._productRepository.AddProduct(product);
				return id;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public bool UpdateProduct(Product product)
		{
			bool result = false;
			try
			{
				result = this._productRepository.UpdateProduct(product);
			}
			catch (Exception ex)
			{
				throw;
			}
			return true;
		}
		public bool DeleteProduct(int id)
		{
			bool result = false;
			try
			{
				result = this._productRepository.DeleteProduct(id);
			}
			catch (Exception ex)
			{
				throw;
			}
			return result;
		}
	}
}