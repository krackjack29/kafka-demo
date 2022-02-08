namespace KafkaConsumerDemo.Models
{
    public class OrderProcessingRequest
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int ConsumerId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
    }
}
