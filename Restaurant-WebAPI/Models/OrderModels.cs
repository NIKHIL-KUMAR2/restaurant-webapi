using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_WebAPI.Models
{
    public class OrderItemRequest
    {
        [Required(ErrorMessage = "MenuItemId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "MenuItemId must be a positive integer.")]
        public int MenuItemId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer.")]
        public int Quantity { get; set; }
    }

    public class NewOrderRequest
    {
        [Required(ErrorMessage = "RestaurantId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "RestaurantId must be a positive integer.")]
        public int RestaurantId { get; set; }

        // Required for the list to be valid
        [Required(ErrorMessage = "Items are required.")]
        public List<OrderItemRequest> Items { get; set; }
    }

    public class OrderCreationResult
    {
        public int OrderId { get; set; }
        public decimal OrderAmount { get; set; }
    }
}
