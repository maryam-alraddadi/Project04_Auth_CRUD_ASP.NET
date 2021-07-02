using System;
using System.ComponentModel.DataAnnotations;

namespace BlogPlatform.Models.Dto
{
    public class CreateCommentDto
    {
        [Required]
        [StringLength(maximumLength: 250, ErrorMessage = "Comment length must be under 250 characters")]
        public string Body { get; set; }
    }
}