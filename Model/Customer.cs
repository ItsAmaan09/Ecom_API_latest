
namespace ECommerce.Core
{
	public class Customer
	{
		public int CustomerId { get; set; }
		public string? Name { get; set; }
		public string? Email { get; set; }
		public string? Address { get; set; }
		public bool IsValid()
		{
			return !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Address);
		}
	}
}