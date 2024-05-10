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
		public List<Customer> GetAllCustomers()
		{
			var customers = new List<Customer>();
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
								IsDeleted = false
							});
						}
					}
				}
			}
			return customers;
		}
		public Customer? GetCustomerById(int customerId)
		{
			var query = "SELECT CustomerId, Name, Email, Address FROM Customers WHERE CustomerId = @CustomerId AND IsDeleted = 0";
			Customer? customer = null;
			using (var connection = _sqlConnectionFactory.CreateConnection())
			{
				connection.Open();
				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@CustomerId", customerId);
					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							customer = new Customer
							{
								CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
								Name = reader.GetString(reader.GetOrdinal("Name")),
								Email = reader.GetString(reader.GetOrdinal("Email")),
								Address = reader.GetString(reader.GetOrdinal("Address")),
								IsDeleted = false
							};
						}
					}
				}
			}
			return customer;
		}
		public int AddCustomer(CustomerDTO customer)
		{
			var query = @"INSERT INTO Customers (Name, Email, Address, IsDeleted)
						VALUES (@Name, @Email, @Address, 0);
						SELECT CAST(SCOPE_IDENTITY() as int);";
			using (var connection = _sqlConnectionFactory.CreateConnection())
			{
				connection.Open();
				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@Name", customer.Name);
					command.Parameters.AddWithValue("@Email", customer.Email);
					command.Parameters.AddWithValue("@Address", customer.Address);
					// ExecuteScalar is used here to return the first column of the first row in the result set
					int customerId = (int)command.ExecuteScalar();
					return customerId;
				}
			}
		}
		//Method to Update an Existing Customer
		public void UpdateCustomer(CustomerDTO customer)
		{
			var query = "UPDATE Customers SET Name = @Name, Email = @Email, Address = @Address WHERE CustomerId = @CustomerId";
			using (var connection = _sqlConnectionFactory.CreateConnection())
			{
				connection.Open();
				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
					command.Parameters.AddWithValue("@Name", customer.Name);
					command.Parameters.AddWithValue("@Email", customer.Email);
					command.Parameters.AddWithValue("@Address", customer.Address);
					command.ExecuteNonQuery();
				}
			}
		}

		//Method to Delete an Existing Customer
		public void DeleteCustomer(int customerId)
		{
			var query = "UPDATE Customers SET IsDeleted = 1 WHERE CustomerId = @CustomerId";
			using (var connection = _sqlConnectionFactory.CreateConnection())
			{
				connection.Open();
				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@CustomerId", customerId);
					command.ExecuteNonQuery();
				}
			}
		}
	}
}