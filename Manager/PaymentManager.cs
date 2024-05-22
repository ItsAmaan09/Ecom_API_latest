


namespace ECommerce.Core
{
	public class PaymentManager
	{
		private PaymentRepository _paymentRepository;
		public PaymentManager(PaymentRepository paymentRepository)
		{
			_paymentRepository = paymentRepository;
		}
		public IEnumerable<Payment> GetPaymentDetails(int paymentId)
		{
			IEnumerable<Payment> payments = new List<Payment>();
			try
			{
				payments = _paymentRepository.GetPaymentDetails(paymentId);
			}
			catch (System.Exception)
			{

				throw;
			}
			return payments;
		}
		public PaymentResponseDTO MakePayment(PaymentDTO paymentDTO)
		{
			PaymentResponseDTO paymentResponseDTO = new PaymentResponseDTO();
			try
			{
				paymentResponseDTO = _paymentRepository.MakePayment(paymentDTO);
			}
			catch (System.Exception)
			{

				throw;
			}
			return paymentResponseDTO;
		}
		public UpdatePaymentResponseDTO UpdatePaymentStatus(int paymentId, string paymentStatus)
		{
			UpdatePaymentResponseDTO paymentResponseDTO = new UpdatePaymentResponseDTO();
			try
			{
				paymentResponseDTO = _paymentRepository.UpdatePaymentStatus(paymentId, paymentStatus);
			}
			catch (System.Exception)
			{

				throw;
			}
			return paymentResponseDTO;

		}
	}
}