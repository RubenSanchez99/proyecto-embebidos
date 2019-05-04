using System;
using System.ComponentModel.DataAnnotations;

namespace Payment.API.Model
{
    public class PaymentAccount
    {
        [Key]
        public Guid BuyerId { get; set; }

        public decimal AmountAvailable { get; set; }
    }
}