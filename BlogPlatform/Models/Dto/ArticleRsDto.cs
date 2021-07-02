using System;
using System.Collections.Generic;

namespace BlogPlatform.Models.Dto
{
    public class ArticleRsDto
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public AuthorDto Author { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Tags { get; set; }
        public List<CommentDto> Comments {get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}