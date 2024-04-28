using System.ComponentModel.DataAnnotations;

namespace WebApplication10.Models
{
    public class Order_
    {
        [Key]
        public int OrderId { get; set; }
        public string OrderNum { get; set; }
        public string QrId { get; set; }
    }
}
