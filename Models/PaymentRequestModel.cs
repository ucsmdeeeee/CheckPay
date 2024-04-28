namespace WebApplication10.Models
{
    public class PaymentRequestModel
    {
        public string qrType { get; set; }
        public int amount { get; set; }
        public string order { get; set; }    
        public string sbpMerchantId { get; set; }
        
    }
}
