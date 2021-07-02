using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogPlatform.Models.Dto
{
    public class CreateArticleDto
    {
        [Required]
        [StringLength(maximumLength: 150, ErrorMessage = "Title length must be under 150 characters")]
        public string Title { get; set; }
        
        [Required]
        public string Body { get; set; }
        
        public string ImageUrl { get; set; }
        
        public List<string> Tags { get; set; }
    }
}