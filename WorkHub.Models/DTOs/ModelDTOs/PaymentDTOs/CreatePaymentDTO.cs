using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkHub.Models.DTOs.ModelDTOs.PaymentDTOs
{
    public class CreatePaymentDTO
    {
        [Required]
        public long TotalAmount { get; set; }

        public string? Description { get; set; }

        public string? ReturnUrl { get; set; }

        public string? CancelUrl { get; set; }

        public DateTimeOffset? ExpiredAt { get; set; }

        [Required]
        public List<OrderItemCreateRequest> Items { get; set; } = [];
    }
}
public class OrderItemCreateRequest
{
    [Required]
    public string? Name { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    [Required]
    public long Price { get; set; }


}