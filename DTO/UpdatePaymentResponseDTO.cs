using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Core
{
	public class UpdatePaymentResponseDTO
	{
		public int PaymentId { get; set; }
		public string CurrentStatus { get; set; }
		public string UpdateStatus { get; set; }
		public string Message { get; set; }
		public bool IsUpdated { get; set; }

	}
}