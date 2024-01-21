using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(1000)]
        public string Name { get; set; }
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
        [ValidateNever]
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Required]
        [Range(0, 100000)]
        public int Qty { get; set; }
        [Required]
        [StringLength(1000)]
        public string SerialNumber { get; set; }
        [Required]
        [Range(0, 100000)]
        public int Price { get; set; }
        [Required]
        [Range(0, 100000)]
        public int BulkPrice { get; set;}
        [Required]
        [StringLength(1000)]
        public string status { get; set; }
        public string? supplliername { get; set; }
        public string? Origan { get; set; }
        [ValidateNever]
        public List<Images> Images { get; set; }
        [ValidateNever]
        public string? ImageUrl { get; set; }

    }
}
