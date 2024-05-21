
namespace ECommerce.Core
{
	public class OrderManager
	{
		private OrderRepository _orderRepository;
		public OrderManager(OrderRepository orderRepository)
		{
			_orderRepository = orderRepository;
		}
		public IEnumerable<Order> GetAllOrders(string Status)
		{
			IEnumerable<Order> orders = new List<Order>();
			try
			{
				orders = _orderRepository.GetAllOrders(Status);
			}
			catch (Exception ex)
			{
				throw;
			}
			return orders;
		}
		public IEnumerable<Order> GetOrderDetails(int orderId)
		{
			IEnumerable<Order> orders = new List<Order>();
			try
			{
				orders = _orderRepository.GetOrderDetails(orderId);
			}
			catch (System.Exception)
			{

				throw;
			}
			return orders;
		}
		public CreateOrderResponseDTO CreateOrder(OrderDTO orderDTO)
		{
			CreateOrderResponseDTO createOrderResponseDTO = new CreateOrderResponseDTO();
			try
			{
				createOrderResponseDTO = _orderRepository.CreateOrder(orderDTO);
			}
			catch (Exception ex)
			{
				throw;
			}
			return createOrderResponseDTO;
		}
		public ConfirmOrderResponseDTO ConfirmOrder(int orderId)
		{
			ConfirmOrderResponseDTO confirmOrderResponseDTO = new ConfirmOrderResponseDTO();
			try
			{
				confirmOrderResponseDTO = _orderRepository.ConfirmOrder(orderId);
			}
			catch (System.Exception)
			{

				throw;
			}
			return confirmOrderResponseDTO;
		}

	}
}