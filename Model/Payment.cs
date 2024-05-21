namespace  ECommerce.Core
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending";
        public string PaymentType { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
    }
}