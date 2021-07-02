using System.Linq;
using AutoMapper;
using BlogPlatform.Models;
using BlogPlatform.Models.Dto;

namespace BlogPlatform.Configurations
{
    public class MapperInitializer : Profile
    {
        public MapperInitializer()
        {
            CreateMap<Article, CreateArticleDto>().ReverseMap();
            CreateMap<Article, UpdateArticleDto>().ReverseMap();
            CreateMap<Article, ArticleRsDto>().ReverseMap();
            CreateMap<Article, ArticleRsDto>()
                .ForMember(u => u.Author, opt => opt.MapFrom(a => a.Author))
                .ForMember(a => a.Tags, 
                    opt => opt.MapFrom(a => a.ArticleTags.Select(a=>a.Tag.Name).ToList()));
            CreateMap<Person, AuthorDto>().ReverseMap();
            CreateMap<Tag, TagDto>().ReverseMap();
        }
    }
}