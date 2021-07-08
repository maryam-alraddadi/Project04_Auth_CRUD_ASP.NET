using System;

namespace BlogPlatform.Models.Dto
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        
        public string Body { get; set; }

        public AuthorDto Author { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}