using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Core
{
	public class PaymentDTO
	{
		public int OrderId { get; set; }
		public decimal Amount { get; set; }
		public string PaymentType { get; set; }
	}
}