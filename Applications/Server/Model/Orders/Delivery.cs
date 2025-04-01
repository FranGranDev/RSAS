namespace Application.Models
{
    public class Delivery
    {
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public DateTime DeliveryDate { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string Flat { get; set; }
        public string PostalCode { get; set; }
        public string Status { get; set; }
    }
}