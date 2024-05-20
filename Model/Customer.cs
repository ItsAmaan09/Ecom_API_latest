
namespace ECommerce.Core
{
    public class Customer
	{
		public int CustomerId { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
	}
}