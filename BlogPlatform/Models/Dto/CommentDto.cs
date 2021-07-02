using System;

namespace BlogPlatform.Models.Dto
{
    public class CommentDto
    {
        public string Body { get; set; }

        public AuthorDto Author { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}