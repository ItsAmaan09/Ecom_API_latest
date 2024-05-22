using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTO;
using ECommerce.Core;
using Microsoft.Data.SqlClient;

namespace ECommerce.Core
{
	public class PaymentRepository
	{
		private readonly SqlConnectionFactory _sqlConnectionFactory;
		public PaymentRepository(SqlConnectionFactory sqlConnectionFactory)
		{
			_sqlConnectionFactory = sqlConnectionFactory;
		}
		public PaymentResponseDTO MakePayment(PaymentDTO paymentDTO)
		{
			var orderValidationQuery = "SELECT TotalAmount FROM Orders WHERE OrderId = @OrderId AND Status = 'Pending'";
			var insertPaymentQuery = "INSERT INTO Payments (OrderId, Amount, Status, PaymentType, PaymentDate) OUTPUT INSERTED.PaymentId VALUES (@OrderId, @Amount, 'Pending', @PaymentType, @PaymentDate)";
			var updatePaymentStatusQuery = "UPDATE Payments SET Status = @Status WHERE PaymentId = @PaymentId";

			PaymentResponseDTO paymentResponseDTO = new PaymentResponseDTO();
			using (SqlConnection sqlConnection = _sqlConnectionFactory.CreateConnection())
			{
				sqlConnection.Open();
				using (SqlTransaction sqlTransaction = sqlConnection.BeginTransaction())
				{
					try
					{
						decimal orderAmount = 0;
						using (SqlCommand validationCommand = new SqlCommand(orderValidationQuery, sqlConnection, sqlTransaction))
						{
							validationCommand.Parameters.AddWithValue("@OrderId", paymentDTO.OrderId);
							var result = validationCommand.ExecuteScalar();
							if (result == null)
							{
								paymentResponseDTO.Message = "Order either does not exist or is not in a pending state.";
								return paymentResponseDTO;
							}
							orderAmount = (decimal)result;
						}
						if (orderAmount != paymentDTO.Amount)
						{
							paymentResponseDTO.Message = "Payment amount does not match with order total";
							return paymentResponseDTO;
						}
						// Insert initial payment record with 'Pending' status
						int paymentId;
						using (SqlCommand insertCommand = new SqlCommand(insertPaymentQuery, sqlConnection, sqlTransaction))
						{
							insertCommand.Parameters.AddWithValue("@OrderId", paymentDTO.OrderId);
							insertCommand.Parameters.AddWithValue("@Amount", paymentDTO.Amount);
							insertCommand.Parameters.AddWithValue("@PaymentType", paymentDTO.PaymentType);
							insertCommand.Parameters.AddWithValue("@PaymentDate", DateTime.Now);
							paymentId = (int)insertCommand.ExecuteScalar();
						}
						string paymentStatus = SimulatePaymentGatewayInteraction(paymentDTO);
						using (SqlCommand updateCommand = new SqlCommand(updatePaymentStatusQuery, sqlConnection, sqlTransaction))
						{
							updateCommand.Parameters.AddWithValue("@Status", paymentStatus);
							updateCommand.Parameters.AddWithValue("@PaymentId", paymentId);
							updateCommand.ExecuteNonQuery();
							paymentResponseDTO.IsCreated = true;
							paymentResponseDTO.Status = paymentStatus;
							paymentResponseDTO.PaymentId = paymentId;
							paymentResponseDTO.Message = $"Payment Processed with status {paymentStatus}";
						}
						sqlTransaction.Commit();
						return paymentResponseDTO;
					}
					catch (System.Exception)
					{
						sqlTransaction.Rollback();
						throw;
					}
				}
			}
		}
		private string SimulatePaymentGatewayInteraction(PaymentDTO paymentDTO)
		{
			switch (paymentDTO.PaymentType)
			{
				case "COD":
					return "Completed";
				case "CC":
					return "Completed";
				case "DC":
					return "Failed";
				default:
					return "Completed";
			}
		}
		public UpdatePaymentResponseDTO UpdatePaymentStatus(int paymentId, string newStatus)
		{
			var paymentDetailsQuery = "SELECT p.OrderId, p.Amount, p.Status, o.Status AS OrderStatus FROM Payments p INNER JOIN Orders o ON p.OrderId = o.OrderId WHERE p.paymentId = @PaymentId";
			var updatePaymentStatusQuery = "UPDATE Payments SET Status = @Status WHERE PaymentId = @PaymentId";

			UpdatePaymentResponseDTO updatePaymentResponseDTO = new UpdatePaymentResponseDTO { PaymentId = paymentId };

			using (SqlConnection sqlConnection = _sqlConnectionFactory.CreateConnection())
			{
				sqlConnection.Open();
				int orderId;
				decimal paymentAmount;
				string currentPaymentStatus = string.Empty, orderStatus = string.Empty;

				using (SqlCommand command = new SqlCommand(paymentDetailsQuery, sqlConnection))
				{
					command.Parameters.AddWithValue("@PaymentId", paymentId);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							orderId = (int)reader["OrderId"];
							paymentAmount = (decimal)reader["Amount"];
							currentPaymentStatus = (string)reader["Status"];
							orderStatus = (string)reader["OrderStatus"];
							updatePaymentResponseDTO.CurrentStatus = currentPaymentStatus;
						}
					}
					if (!IsValidStatusTransition(currentPaymentStatus, newStatus, orderStatus))
					{
						updatePaymentResponseDTO.IsUpdated = false;
						updatePaymentResponseDTO.Message = $"Invalid status transition from {currentPaymentStatus} to {newStatus} for order status {orderStatus}";
						return updatePaymentResponseDTO;
					}
					using (SqlCommand updateCommmand = new SqlCommand(updatePaymentStatusQuery, sqlConnection))
					{
						updateCommmand.Parameters.AddWithValue("@PaymentId", paymentId);
						updateCommmand.Parameters.AddWithValue("@Status", newStatus);
						updateCommmand.ExecuteNonQuery();
						updatePaymentResponseDTO.IsUpdated = true;
						updatePaymentResponseDTO.UpdateStatus = newStatus;
						updatePaymentResponseDTO.Message = $"Payment status updated from {currentPaymentStatus} to {newStatus}";
						return updatePaymentResponseDTO;
					}
				}
			}
		}
		public IEnumerable<Payment> GetPaymentDetails(int paymentId)
		{
			IList<Payment> payments = new List<Payment>();

			using (SqlConnection sqlConnection = _sqlConnectionFactory.CreateConnection())
			{
				sqlConnection.Open();
				var query = "SELECT PaymentId, OrderId, Amount, Status, PaymentType, PaymentDate FROM Payments WHERE PaymentId = @PaymentId";
				using (SqlCommand command = new SqlCommand(query, sqlConnection))
				{
					command.Parameters.AddWithValue("@PaymentId", paymentId);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							payments.Add(new Payment
							{
								PaymentId = (int)reader["PaymentId"],
								OrderId = (int)reader["OrderId"],
								Amount = (decimal)reader["Amount"],
								Status = (string)reader["Status"],
								PaymentType = (string)reader["PaymentType"],
								PaymentDate = (DateTime)reader["PaymentDate"]
							});
						}
					}
				}
				return payments;
			}
		}

		private bool IsValidStatusTransition(string currentStatus, string newStatus, string orderStatus)
		{
			if (currentStatus == "Completed" && newStatus != "Refund")
			{
				return false;
			}
			if (currentStatus == "Pending" && newStatus == "Cancelled")
			{
				return true;
			}
			if (currentStatus == "Completed" && newStatus == "Refund" && orderStatus != "Returned")
			{
				return false;
			}
			if (newStatus == "Failed" && (currentStatus == "Completed" || currentStatus == "Cancelled"))
			{
				return false;
			}
			if (currentStatus == "Pending" && newStatus == "Completed" && (orderStatus == "Shipped" || orderStatus
			== "Confirmed"))
			{
				return true;
			}
			return true;
		}
	}
}