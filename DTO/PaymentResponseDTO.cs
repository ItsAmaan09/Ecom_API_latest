using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Core
{
	public class PaymentResponseDTO
	{
		public int PaymentId { get; set; }
		public string Status { get; set; }
		public string Message { get; set; }
		public bool IsCreated { get; set; }
	}
}