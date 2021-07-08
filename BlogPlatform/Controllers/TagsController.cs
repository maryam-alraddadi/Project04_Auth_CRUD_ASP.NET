using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlogPlatform.Data;
using BlogPlatform.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace BlogPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public TagsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                var tags = await _context.Tags.Where(t => t.ArticleTags.Count > 0).ToListAsync();
                return Ok(_mapper.Map<IEnumerable<TagDto>>(tags));
            }

            var result = await _context.Tags.Where(t => t.Name.Contains(search)).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<TagDto>>(result));
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<IPagedList<object>>> GetArticlesByTag(int id,
            [FromQuery] ArticlesRequestParams requestParams)
        {
            var articles = await (from tag in _context.Tags
                    from article in tag.ArticleTags.DefaultIfEmpty()
                    where article.TagId == id
                    select new
                    {
                        ArticleId = article.ArticleId,
                        Title = article.Article.Title,
                        Body = article.Article.Body,
                        Author = new
                        {
                            Email = article.Article.Author.Email,
                            DisplayName = article.Article.Author.DisplayName,
                            Bio = article.Article.Author.Bio,
                            ImageUrl = article.Article.Author.ImageUrl,
                            DateJoined = article.Article.Author.DateJoined
                        },
                        ImageUrl = article.Article.ImageUrl,
                        Tags = article.Article.ArticleTags.Select(a=>a.Tag.Name).ToList(),
                        Comments = article.Article.Comments,
                        CreatedAt = article.Article.CreatedAt,
                        UpdatedAt = article.Article.UpdatedAt
                    })
                .OrderByDescending(a => a.CreatedAt)
                .AsNoTracking()
                .ToPagedListAsync(requestParams.Page, 10);
            
            if (articles == null)
            {
                return NotFound();
            }
        
            return Ok(articles);
        }
    }
}