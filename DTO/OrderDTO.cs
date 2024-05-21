using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Core
{
	public class OrderDTO
	{
		public int CustomerId { get; set; }
		public List<OrderItemDetailsDTO> Items { get; set; }
	}
}