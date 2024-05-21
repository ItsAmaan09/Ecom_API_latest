using Microsoft.Data.SqlClient;

namespace ECommerce.Core
{
	public class ProductRepository
	{
		private readonly SqlConnectionFactory _sqlConnectionFactory;

		public ProductRepository(SqlConnectionFactory sqlConnectionFactory)
		{
			_sqlConnectionFactory = sqlConnectionFactory;
		}
		public IEnumerable<Product> GetAllProducts()
		{
			IList<Product> products = new List<Product>();
			using (SqlConnection sqlConnection = _sqlConnectionFactory.CreateConnection())
			{
				var query = "SELECT ProductId,Name,Price,Quantity,Description FROM Products WHERE IsDeleted = 0";
				using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
				{
					sqlConnection.Open();
					using (SqlDataReader reader = sqlCommand.ExecuteReader())
					{
						while (reader.Read())
						{
							products.Add(new Product
							{
								ProductId = (int)reader["ProductId"],
								Name = (string)reader["Name"],
								Price = (decimal)reader["Price"],
								Quantity = (int)reader["Quantity"],
								Description = (string)reader["Description"]
							});
						}
					}
				}
			}
			return products;
		}
		public IEnumerable<Product> GetProductById(int productId)
		{
			IList<Product> products = new List<Product>();
			using (SqlConnection sqlConnection = _sqlConnectionFactory.CreateConnection())
			{
				var query = "SELECT ProductId,Name,Price,Quantity,Description FROM Products WHERE ProductId = @ProductId AND IsDeleted = 0";
				using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
				{
					sqlCommand.Parameters.AddWithValue("@ProductId", productId);
					sqlConnection.Open();
					using (SqlDataReader reader = sqlCommand.ExecuteReader())
					{
						while (reader.Read())
						{
							products.Add(new Product
							{
								ProductId = (int)reader["ProductId"],
								Name = (string)reader["Name"],
								Price = (decimal)reader["Price"],
								Quantity = (int)reader["Quantity"],
								Description = (string)reader["Description"]
							});
						}
					}
				}
			}
			return products;
		}
		public int AddProduct(Product product)
		{
			try
			{
				using (SqlConnection sqlConnection = _sqlConnectionFactory.CreateConnection())
				{
					var query = @"INSERT INTO Products (Name, Price, Quantity, Description, IsDeleted) VALUES (@Name, @Price, @Quantity, @Description, 0); SELECT CAST(SCOPE_IDENTITY() as int);";
					using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
					{
						sqlCommand.Parameters.AddWithValue("@Name", product.Name);
						sqlCommand.Parameters.AddWithValue("@Price", product.Price);
						sqlCommand.Parameters.AddWithValue("@Quantity", product.Quantity);
						sqlCommand.Parameters.AddWithValue("@Description", product.Description);
						sqlConnection.Open();
						int productId = (int)sqlCommand.ExecuteScalar();
						return productId;
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			return 0;
		}
		public bool UpdateProduct(Product product)
		{
			try
			{
				using (var connection = _sqlConnectionFactory.CreateConnection())
				{
					var query = "UPDATE Products SET Name = @Name, Price = @Price, Quantity = @Quantity, Description = @Description WHERE ProductId = @ProductId";
					using (var command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@ProductId", product.ProductId);
						command.Parameters.AddWithValue("@Name", product.Name);
						command.Parameters.AddWithValue("@Price", product.Price);
						command.Parameters.AddWithValue("@Quantity", product.Quantity);
						command.Parameters.AddWithValue("@Description", product.Description);
						connection.Open();
						int affectedRows = command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			return true;
		}
		public bool DeleteProduct(int productId)
		{
			try
			{
				using (SqlConnection connection = _sqlConnectionFactory.CreateConnection())
				{
					var query = "UPDATE Products SET IsDeleted = 1 WHERE ProductId = @ProductId";
					using (SqlCommand sqlCommand = new SqlCommand(query, connection))
					{
						sqlCommand.Parameters.AddWithValue("@ProductId", productId);
						connection.Open();
						int affectedRows = sqlCommand.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			return true;
		}
	}
}