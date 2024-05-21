using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ECommerce.Core
{
	public class OrderRepository
	{
		private readonly SqlConnectionFactory _sqlConnectionFactory;
		public OrderRepository(SqlConnectionFactory sqlConnectionFactory)
		{
			_sqlConnectionFactory = sqlConnectionFactory;
		}
		public IEnumerable<Order> GetAllOrders(string Status)
		{
			IList<Order> orders = new List<Order>();
			try
			{
				using (SqlConnection sqlConnection = _sqlConnectionFactory.CreateConnection())
				{
					var query = "SELECT OrderId, CustomerId, TotalAmount, Status, OrderDate FROM Orders WHERE Status = @Status";
					using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
					{
						sqlConnection.Open();
						sqlCommand.Parameters.AddWithValue("@Status", Status);
						using (SqlDataReader reader = sqlCommand.ExecuteReader())
						{
							while (reader.Read())
							{
								Order order = new Order
								{
									OrderId = (int)reader["OrderId"],
									CustomerId = (int)reader["CustomerId"],
									TotalAmount = (decimal)reader["TotalAmount"],
									Status = (string)reader["Status"],
									OrderDate = (DateTime)reader["OrderDate"]
								};
								orders.Add(order);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			return orders;
		}
		public CreateOrderResponseDTO CreateOrder(OrderDTO orderDTO)
		{
			var productQuery = "SELECT ProductId, Price, Quantity FROM Products WHERE ProductId = @ProductId AND IsDeleted = 0";
			var orderQuery = "INSERT INTO Orders (CustomerId, TotalAmount, Status, OrderDate) OUTPUT INSERTED.OrderId VALUES (@CustomerId, @TotalAmount, @Status, @OrderDate)";
			var itemQuery = "INSERT INTO OrderItems (OrderId, ProductId, Quantity, PriceAtOrder) VALUES (@OrderId, @ProductId, @Quantity, @PriceAtOrder)";

			decimal TotalAmount = 0;
			List<OrderItem> validatedItems = new List<OrderItem>();
			CreateOrderResponseDTO createOrderResponseDTO = new CreateOrderResponseDTO();

			using (SqlConnection sqlConnection = _sqlConnectionFactory.CreateConnection())
			{
				sqlConnection.Open();
				using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
				{
					try
					{
						foreach (OrderItemDetailsDTO item in orderDTO.Items)
						{
							using (SqlCommand productCommand = new SqlCommand(productQuery, sqlConnection, sqlTransaction))
							{
								productCommand.Parameters.AddWithValue("@ProductId", item.ProductId);

								using (SqlDataReader reader = productCommand.ExecuteReader())
								{
									if (reader.Read())
									{
										int StockQuantity = (int)reader["Quantity"];
										decimal Price = (decimal)reader["Price"];
										if (StockQuantity >= item.Quantity)
										{
											TotalAmount += Price * item.Quantity;
											validatedItems.Add(new OrderItem
											{
												ProductId = item.ProductId,
												Quantity = item.Quantity,
												PriceAtOrder = Price //price from db
											});
										}
										else
										{
											createOrderResponseDTO.Message = $"Insufficient stock for Product Id {item.ProductId}";
											createOrderResponseDTO.IsCreated = false;
											return createOrderResponseDTO;
										}
									}
									else
									{
										createOrderResponseDTO.Message = $"Product not found for Product Id {item.ProductId}";
										createOrderResponseDTO.IsCreated = false;
										return createOrderResponseDTO;
									}
									reader.Close();
								}
							}
						}
						// Proceed with creating order if all items validated
						using (SqlCommand orderCommand = new SqlCommand(orderQuery, sqlConnection, sqlTransaction))
						{
							orderCommand.Parameters.AddWithValue("@CustomerId", orderDTO.CustomerId);
							orderCommand.Parameters.AddWithValue("@TotalAmount", TotalAmount);
							orderCommand.Parameters.AddWithValue("@Status", "Pending");
							orderCommand.Parameters.AddWithValue("@OrderDate", DateTime.Now);
							var orderId = (int)orderCommand.ExecuteScalar();

							foreach (var validatedItem in validatedItems)
							{
								using (SqlCommand itemCommand = new SqlCommand(itemQuery, sqlConnection, sqlTransaction))
								{
									itemCommand.Parameters.AddWithValue("@OrderId", orderId);
									itemCommand.Parameters.AddWithValue("@ProductId", validatedItem.ProductId);
									itemCommand.Parameters.AddWithValue("@Quantity", validatedItem.Quantity);
									itemCommand.Parameters.AddWithValue("@PriceAtOrder", validatedItem.PriceAtOrder);
									itemCommand.ExecuteNonQuery();
								}
							}
							sqlTransaction.Commit();
							createOrderResponseDTO.Status = "Pending";
							createOrderResponseDTO.IsCreated = true;
							createOrderResponseDTO.OrderId = orderId;
							createOrderResponseDTO.Message = "Order Created Successfully";
							return createOrderResponseDTO;

						}
					}
					catch (Exception ex)
					{
						sqlTransaction.Rollback();
						throw;
					}
				}
			}
		}
	}
}