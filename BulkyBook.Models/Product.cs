﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models {
    public class Product {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Range(1, 10000)]
        [DisplayName("List Price")]
        public double ListPrice { get; set; }
        [Required]
        [Range(1, 10000)]
        public double Price { get; set; }
        [Required]
        [Range(1, 10000)]
        [DisplayName("Price for 50+")]
        public double Price50 { get; set; }
        [Required]
        [Range(1, 10000)]
        [DisplayName("Price for 100+")]
        public double Price100 { get; set; }
        [DisplayName("Image URL")]
        [ValidateNever]
        public string ImageUrl { get; set; }
        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")] // Optional 
        [ValidateNever]
        public Category Category { get; set; }
        [Required]
        public int CoverTypeId { get; set; }
        [DisplayName("Cover Type")]
        [ValidateNever]
        public CoverType CoverType { get; set; }
    }
}
