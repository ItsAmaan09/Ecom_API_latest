using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Core
{
	public class OrderStatusResponseDTO
	{
		public int OrderId { get; set; }
		public string Status { get; set; }
		public bool IsUpdated { get; set; }
		public string Message { get; set; }
	}
}