namespace ECommerce.Core;

public class CustomerDTO
{
    public int CustomerId { get; set; } // Used only for updates, not for inserts
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
}
