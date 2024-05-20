using Microsoft.Data.SqlClient;


namespace ECommerce.Core
{
	public class CustomerRepository
	{
		private readonly SqlConnectionFactory _sqlConnectionFactory;

		public CustomerRepository(SqlConnectionFactory sqlConnectionFactory)
		{
			_sqlConnectionFactory = sqlConnectionFactory;
		}
		public IEnumerable<Customer> GetAllCustomers()
		{
			IList<Customer> customers = new List<Customer>();
			var query = "SELECT CustomerId,Name,Email,Address FROM Customers WHERE IsDeleted = 0";
			using (var connection = _sqlConnectionFactory.CreateConnection())
			{
				connection.Open();
				using (var command = new SqlCommand(query, connection))
				{
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							customers.Add(new Customer
							{
								CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
								Name = reader.GetString(reader.GetOrdinal("Name")),
								Email = reader.GetString(reader.GetOrdinal("Email")),
								Address = reader.GetString(reader.GetOrdinal("Address")),
							});
						}
					}
				}
			}
			return customers;
		}
		public IEnumerable<Customer> GetCustomerById(int customerId)
		{
			IList<Customer> customer = new List<Customer>();
			using (var connection = _sqlConnectionFactory.CreateConnection())
			{
				var query = "SELECT CustomerId, Name, Email, Address FROM Customers WHERE CustomerId = @CustomerId AND IsDeleted = 0";
				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@CustomerId", customerId);
					connection.Open();
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							Customer customer1 = new Customer
							{
								CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
								Name = reader.GetString(reader.GetOrdinal("Name")),
								Email = reader.GetString(reader.GetOrdinal("Email")),
								Address = reader.GetString(reader.GetOrdinal("Address")),
							};
							customer.Add(customer1);
						}
					}
				}
			}
			return customer;
		}
		public IEnumerable<Customer> GetCustomerByEmail(string Email)
		{
			IList<Customer> customers = new List<Customer>();
			using (var connection = _sqlConnectionFactory.CreateConnection())
			{
				var query = "SELECT CustomerId, Name, Email, Address, IsDeleted FROM Customers WHERE Email = @Email AND IsDeleted = 0";
				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@Email", Email);
					connection.Open();
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							Customer customer = new Customer
							{
								CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
								Name = reader.GetString(reader.GetOrdinal("Name")),
								Email = reader.GetString(reader.GetOrdinal("Email")),
								Address = reader.GetString(reader.GetOrdinal("Address")),
							};
							customers.Add(customer);
						}
					}
				}
			}
			return customers;
		}

		public int AddCustomer(Customer customer)
		{
			try
			{
				using (var connection = _sqlConnectionFactory.CreateConnection())
				{
					var query = @"INSERT INTO Customers (Name, Email, Address, IsDeleted) VALUES (@Name, @Email, @Address, 0); SELECT CAST(SCOPE_IDENTITY() as int);";
					using (var command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@Name", customer.Name);
						command.Parameters.AddWithValue("@Email", customer.Email);
						command.Parameters.AddWithValue("@Address", customer.Address);
						// ExecuteScalar is used here to return the first column of the first row in the result set
						connection.Open();
						int customerId = (int)command.ExecuteScalar();
						return customerId;
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
		public bool UpdateCustomer(Customer customer)
		{
			try
			{
				using (var connection = _sqlConnectionFactory.CreateConnection())
				{
					var query = "UPDATE Customers SET Name = @Name, Email = @Email, Address = @Address WHERE CustomerId = @CustomerId";
					using (var command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
						command.Parameters.AddWithValue("@Name", customer.Name);
						command.Parameters.AddWithValue("@Email", customer.Email);
						command.Parameters.AddWithValue("@Address", customer.Address);
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
		public bool DeleteCustomer(int customerId)
		{
			try
			{
				using (SqlConnection connection = _sqlConnectionFactory.CreateConnection())
				{
					var query = "UPDATE Customers SET IsDeleted = 1 WHERE CustomerId = @CustomerId";
					using (var command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@CustomerId", customerId);
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
	}
}