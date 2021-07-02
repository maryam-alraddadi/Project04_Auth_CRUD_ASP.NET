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
        }
    }
}