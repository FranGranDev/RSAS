using System;

namespace Application.DTOs
{
    public class DeliveryDto
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Notes { get; set; }
    }

    public class CreateDeliveryDto
    {
        public string Address { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Notes { get; set; }
    }

    public class UpdateDeliveryDto
    {
        public string Address { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Notes { get; set; }
    }
} 