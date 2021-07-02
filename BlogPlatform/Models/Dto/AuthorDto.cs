using System;
using System.Collections.Generic;

namespace BlogPlatform.Models.Dto
{
    public class AuthorDto
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }

        public string ImageUrl { get; set; }
        public DateTime DateJoined { get; set; }
    }
}