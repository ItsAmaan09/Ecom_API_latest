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
		public IEnumerable<Order> GetOrderDetails(int orderId)
		{
			IList<Order> orders = new List<Order>();
			var query = "SELECT OrderId,CustomerId,TotalAmount,Status,OrderDate FROM Orders WHERE OrderId = @OrderId";
			using (SqlConnection sqlConnection = _sqlConnectionFactory.CreateConnection())
			{
				sqlConnection.Open();
				using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
				{
					sqlCommand.Parameters.AddWithValue("@OrderId", orderId);
					using (SqlDataReader reader = sqlCommand.ExecuteReader())
					{
						while (reader.Read())
						{
							orders.Add(new Order
							{
								OrderId = (int)reader["OrderId"],
								CustomerId = (int)reader["CustomerId"],
								TotalAmount = (decimal)reader["TotalAmount"],
								Status = (string)reader["Status"],
								OrderDate = (DateTime)reader["OrderDate"]
							});
						}
					}
				}
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
		public ConfirmOrderResponseDTO ConfirmOrder(int orderId)
		{
			// Queries to fetch order and payment details
			var orderDetailsQuery = "SELECT TotalAmount FROM Orders WHERE OrderId = @OrderId";
			var paymentDetailsQuery = "SELECT Amount, Status FROM Payments WHERE OrderId = @OrderId";
			var updateOrderStatusQuery = "UPDATE Orders SET Status = 'Confirmed' WHERE OrderId = @OrderId";
			var getOrderItemQuery = "SELECT ProductId, Quantity FROM OrderItems WHERE OrderId = @OrderId";
			var updateProductQuery = "UPDATE Products SET Quantity = Quantity - @Quantity WHERE ProductId = @ProductId";

			ConfirmOrderResponseDTO confirmOrderResponseDTO = new ConfirmOrderResponseDTO()
			{
				OrderId = orderId
			};

			using (SqlConnection sqlConnection = _sqlConnectionFactory.CreateConnection())
			{
				sqlConnection.Open();

				using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
				{
					try
					{
						decimal orderAmount = 0;
						decimal paymentAmount = 0;
						string paymentStatus = string.Empty;

						using (SqlCommand orderCommand = new SqlCommand(orderDetailsQuery, sqlConnection, sqlTransaction))
						{
							orderCommand.Parameters.AddWithValue("@OrderId", orderId);
							using (SqlDataReader reader = orderCommand.ExecuteReader())
							{
								if (reader.Read())
								{
									orderAmount = (decimal)reader["TotalAmount"];
								}
								// reader.Close();
							}
						}
						using (SqlCommand paymentCommand = new SqlCommand(paymentDetailsQuery, sqlConnection, sqlTransaction))
						{
							paymentCommand.Parameters.AddWithValue("@OrderId", orderId);
							using (SqlDataReader reader = paymentCommand.ExecuteReader())
							{
								if (reader.Read())
								{
									paymentAmount = (decimal)reader["Amount"];
									paymentStatus = (string)reader["Status"];
								}
								// reader.Close();
							}
						}
						// Check if payment is complete and matches the order total
						if (paymentStatus == "Completed" && paymentAmount == orderAmount)
						{
							List<(int ProductId, int Quantity)> orderItems = new List<(int ProductId, int Quantity)>(); //

							using (SqlCommand itemCommand = new SqlCommand(getOrderItemQuery, sqlConnection, sqlTransaction))
							{
								itemCommand.Parameters.AddWithValue("@OrderId", orderId);

								using (SqlDataReader reader = itemCommand.ExecuteReader())
								{
									while (reader.Read())
									{
										int productId = (int)reader["ProductId"];
										int quantity = (int)reader["Quantity"];
										orderItems.Add((productId, quantity));

										// using (SqlCommand updateProductCommand = new SqlCommand(updateProductQuery, sqlConnection, sqlTransaction))
										// {
										// 	updateProductCommand.Parameters.AddWithValue("@ProductId", productId);
										// 	updateProductCommand.Parameters.AddWithValue("@Quantity", quantity);
										// 	updateProductCommand.ExecuteNonQuery();  // error for reader
										// }
									}
									// reader.Close();
								}
							}
							//
							// Updating product quantities
							foreach (var item in orderItems)
							{
								using (SqlCommand updateProductCommand = new SqlCommand(updateProductQuery, sqlConnection, sqlTransaction))
								{
									updateProductCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
									updateProductCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
									updateProductCommand.ExecuteNonQuery();
								}
							}
							//
							// Update order status to 'Confirmed'

							using (SqlCommand statusCommand = new SqlCommand(updateOrderStatusQuery, sqlConnection, sqlTransaction))
							{
								statusCommand.Parameters.AddWithValue("@OrderId", orderId);
								statusCommand.ExecuteNonQuery();
							}
							sqlTransaction.Commit();
							confirmOrderResponseDTO.IsConfirmed = true;
							confirmOrderResponseDTO.Message = "Order Confirmed Successfully";
							return confirmOrderResponseDTO;
						}
						else
						{
							sqlTransaction.Rollback();
							confirmOrderResponseDTO.IsConfirmed = false;
							confirmOrderResponseDTO.Message = "Cannot Confirm Order: Payment is either incomplete or does not match the order total.";
							return confirmOrderResponseDTO;
						}
					}
					catch (System.Exception ex)
					{
						sqlTransaction.Rollback();
						throw new Exception("error confirm order" + ex.Message);
					}
				}
			}
		}

		// Update the order status with conditions
		// An order cannot move directly from "Pending" to "Delivered".
		// An order can only be set to "Cancelled" if it is currently "Pending".
		// An order can be marked as "Processing" only if it's currently "Confirmed"
		public OrderStatusResponseDTO UpdateOrderStatus(int orderId, string newStatus)
		{
			OrderStatusResponseDTO orderStatusResponseDTO = new OrderStatusResponseDTO()
			{
				OrderId = orderId
			};

			using (SqlConnection sqlConnection = _sqlConnectionFactory.CreateConnection())
			{
				sqlConnection.Open();
				try
				{
					var currentStatusQuery = "SELECT Status FROM Orders WHERE OrderId = @OrderId";
					string currentStatus;
					using (SqlCommand statusCommand = new SqlCommand(currentStatusQuery, sqlConnection))
					{
						statusCommand.Parameters.AddWithValue("@OrderId", orderId);
						var result = statusCommand.ExecuteScalar();
						if (result == null)
						{
							orderStatusResponseDTO.Message = "Order not found";
							orderStatusResponseDTO.IsUpdated = false;
							return orderStatusResponseDTO;
						}
						currentStatus = result.ToString();
					}

					if (!IsValidStatusTransaction(currentStatus, newStatus))
					{
						orderStatusResponseDTO.Message = $"Invalid status transition from {currentStatus} to {newStatus}";
						orderStatusResponseDTO.IsUpdated = false;
						return orderStatusResponseDTO;
					}

					// Update if valid
					var updateStatusQuery = "UPDATE Orders SET Status = @NewStatus WHERE OrderId = @OrderId";
					using (SqlCommand updateCommand = new SqlCommand(updateStatusQuery, sqlConnection))
					{
						updateCommand.Parameters.AddWithValue("@OrderId", orderId);
						updateCommand.Parameters.AddWithValue("@NewStatus", newStatus);

						int rowAffected = updateCommand.ExecuteNonQuery();
						if (rowAffected > 0)
						{
							orderStatusResponseDTO.Message = $"Order Status updated to {newStatus}";
							orderStatusResponseDTO.Status = newStatus;
							orderStatusResponseDTO.IsUpdated = true;
						}
						else
						{
							orderStatusResponseDTO.Message = $"No order found with id {orderId}";
							orderStatusResponseDTO.IsUpdated = false;
						}
					}
					return orderStatusResponseDTO;
				}
				catch (System.Exception ex)
				{
					throw new Exception("Error updating order status: " + ex.Message, ex);
				}
			}
		}
		private bool IsValidStatusTransaction(string currentStatus, string newStatus)
		{
			switch (currentStatus)
			{
				case "Pending":
					return newStatus == "Processing" || newStatus == "Cancelled";
				case "Confirmed":
					return newStatus == "Processing";
				case "Processing":
					return newStatus == "Delivered";
				case "Delivered":
					return false;
				case "Cancelled":
					return false;

				default:
					return false;
			}

		}
	}
}