using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlogPlatform.Data;
using BlogPlatform.Models;
using BlogPlatform.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace BlogPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ArticlesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IPagedList<ArticleRsDto>>> GetArticles([FromQuery] ArticlesRequestParams requestParams)
        {
            if (string.IsNullOrEmpty(requestParams.Search))
            {
                var articles =  await _context.Articles
                    .Include(u => u.Author)
                    .Include(at => at.ArticleTags)
                    .ThenInclude(t => t.Tag)
                    .OrderByDescending(a=>a.CreatedAt)
                    .AsNoTracking()
                    .ToPagedListAsync(requestParams.Page, 10);
                return Ok(_mapper.Map<IEnumerable<ArticleRsDto>>(articles));
            }

            var resultArticles = await _context.Articles
                .Include(u => u.Author)
                .Include(at => at.ArticleTags)
                .ThenInclude(t => t.Tag)
                .Where(a => a.Title.Contains(requestParams.Search))
                .AsNoTracking()
                .ToPagedListAsync(requestParams.Page, 10);
            
            return Ok(_mapper.Map<IEnumerable<ArticleRsDto>>(resultArticles));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleRsDto>> GetArticle(int id)
        {
            var article = await _context.Articles
                .Where(a => a.ArticleId == id)
                .Include(u => u.Author)
                .Include(at => at.ArticleTags)
                .ThenInclude(t => t.Tag)
                .Include(c => c.Comments)
                .ThenInclude(ca => ca.Author)
                .FirstOrDefaultAsync();

            if (article == null)
            {
                return NotFound();
            }
  
            return Ok(_mapper.Map<ArticleRsDto>(article));
        }
        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Article>> PostArticle(CreateArticleDto newArticle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            var article = _mapper.Map<Article>(newArticle);
            var user = await _context.Users.Where(u => u.UserName == User.Identity.Name).FirstAsync();
            
            article.AuthorId = user.Id;
            article.CreatedAt = DateTime.Now;
            _context.Articles.Add(article);
            
            // only add if tag not exists
            foreach (var tag in article.Tags)
            {
                if (_context.Tags.Any(x => x.Name == tag))
                {
                    var existingTag = await _context.Tags.FirstAsync(t => t.Name == tag);
                    await _context.ArticleTags.AddAsync(new ArticleTag() {Article = article, Tag = existingTag});
                }
                else
                {
                    var newTag = await _context.Tags.AddAsync(new Tag() {Name = tag});
                    await _context.ArticleTags.AddAsync(new ArticleTag() {Article = article, Tag = newTag.Entity});
                }
            }
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetArticle), new {id = article.ArticleId});
        }
        
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> PutArticle(int id, UpdateArticleDto article)
        {
            var oldArticle = await _context.Articles
                .Include(at => at.ArticleTags)
                .FirstOrDefaultAsync(a => a.ArticleId == id);
            
            if (oldArticle == null)
            {
                return NotFound();
            }

            var updatedArticle = _mapper.Map<Article>(article);
            oldArticle.Title = string.IsNullOrEmpty(updatedArticle.Title) ? oldArticle.Title : updatedArticle.Title;
            oldArticle.Body = string.IsNullOrEmpty(updatedArticle.Body) ? oldArticle.Body : updatedArticle.Body;
            oldArticle.ImageUrl = string.IsNullOrEmpty(updatedArticle.ImageUrl) ? oldArticle.ImageUrl : updatedArticle.ImageUrl;
            oldArticle.UpdatedAt = DateTime.Now;
            
            if (updatedArticle.Tags != null)
            {
                oldArticle.ArticleTags = new List<ArticleTag>();
                foreach (var tag in updatedArticle.Tags)
                {
                    var newTag = await _context.Tags.AddAsync(new Tag() {Name = tag});
                    oldArticle.ArticleTags.Add(new ArticleTag() {Article = oldArticle, Tag = newTag.Entity});
                }
            }
            
            _context.Articles.Update(oldArticle);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles
                .Include(at => at.ArticleTags)
                .Include(c => c.Comments)
                .FirstOrDefaultAsync(a => a.ArticleId == id);
            
            if (article == null)
            {
                return NotFound();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        
        [HttpGet("user/{username}")]
        public async Task<ActionResult<IEnumerable<ArticleRsDto>>> GetArticleByUsername(string username)
        {

            var userArticles = await _context.Articles
                .Include(u => u.Author)
                .Include(at => at.ArticleTags)
                .ThenInclude(t => t.Tag)
                .Where(a => a.Author.UserName == username)
                .ToListAsync();
            
            var articles = _mapper.Map<IEnumerable<Article> , IEnumerable<ArticleRsDto>>(userArticles);
            
            return Ok(articles);
        }
        
        
        [HttpGet("{id}/comments")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetArticleComments(int id)
        {
            var comments = await _context.Comments
                .Where(c => c.ArticleId == id)
                .Include(a => a.Author)
                .ToListAsync();
            
            if (comments == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<CommentDto>>(comments));
        }
        
        [HttpPost("{id}/comments")]
        [Authorize]
        public async Task<ActionResult<ArticleRsDto>> AddComment(int id, CreateCommentDto newComment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var comment = _mapper.Map<Comment>(newComment);
            var user = await _context.Users.Where(u => u.UserName == User.Identity.Name).FirstAsync();

            comment.AuthorId = user.Id;
            comment.ArticleId = article.ArticleId;
            comment.CreatedAt = DateTime.Now;
            
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetArticleComments), new {id = article.ArticleId});
        }
        
        [HttpDelete("{articleId}/comments/{commentId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Comment>>> DeleteComment(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }
            
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}