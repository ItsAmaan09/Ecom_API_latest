using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Core
{
	public class PaymentStatusDTO
	{
		public int PaymentId { get; set; }
		public string Status { get; set; }
	}
}