using System.Collections.Generic;

namespace BlogPlatform.Models.Dto
{
    public class UpdateArticleDto
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public string ImageUrl { get; set; }

        public List<string> Tags { get; set; }
    }
}