﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Shop.Entities.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string ApplictionUserId { get; set; }
        [ForeignKey("ApplictionUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime ?OrderDate { get; set; }
        public DateTime ?ShippingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string ? OrederStatus { get; set; }
        public string ? PaymentStatus { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Carrier { get; set; }
        public DateTime? PaymentDate { get; set; }
        //Stripe Properties
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        //Data Of User
        public string Name { get; set; }
        public string Address { get; set; }
        public string ? PhoneNumber { get; set; }
        public string City { get; set; }









    }
}
