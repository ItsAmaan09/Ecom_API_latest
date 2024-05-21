
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
	}
}