using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace BlogPlatform.Models
{
    public class Person : IdentityUser
    {
        public override string Email { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        
        public string ImageUrl { get; set; }
        public DateTime DateJoined { get; set; }
        public List<Article> Articles { get; set; }
        public List<Comment> Comments { get; set; }
    }
}