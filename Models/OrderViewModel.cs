using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Part1Cloud2B.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.ComponentModel.DataAnnotations;

    public class OrderViewModel
    {
        [Required]
        public string? ProductId { get; set; } // The selected product ID

        public List<SelectListItem>? Products { get; set; } // List of products for the drop-down

        [Required]
        public string? CustomerName { get; set; }

        [Required]
        public string ?CustomerSurname { get; set; }

        [Required]
        [EmailAddress]
        public string ?CustomerEmail { get; set; }

       
        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; }
    }


}
