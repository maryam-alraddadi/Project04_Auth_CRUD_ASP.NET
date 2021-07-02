using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BlogPlatform.Data;
using BlogPlatform.Models;
using BlogPlatform.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private IMapper _mapper;
        public ArticlesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetArticles(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return await _context.Articles
                    .Include(a=>a.Author)
                    .Select(x => new
                    {
                       ArticleId = x.ArticleId, Title= x.Title, Body=x.Body,
                       Tags = x.ArticleTags.Select(t => new {name = t.Tag.Name}).ToList(),
                       Author = x.Author.DisplayName,
                       CreatedAt = x.CreatedAt
                    })
                    .ToListAsync();
            }

            return await _context.Articles.Where(a => a.Title.Contains(search)).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetArticle(int id)
        {
            var article = await _context.Articles
                .Where(a => a.ArticleId == id)
                .Include(a => a.ArticleTags)
                .ThenInclude(t => t.Tag)
                .Include(c=>c.Comments)
                .Select(x => new
                {
                    ArticleId = x.ArticleId, Title = x.Title, Body = x.Body, Author = x.Author.DisplayName, 
                    Tags = x.ArticleTags.Select(t => new {name = t.Tag.Name}).ToList(),
                    Comments = x.Comments.Select(b=> new {body = b.Body}).ToList(),
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();
            
            if (article == null)
            {
                return NotFound();
            }

            return article;
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
            foreach (var tag in article.Tags)
            {
                var newTag = await _context.Tags.AddAsync(new Tag() {Name = tag});
                await _context.ArticleTags.AddAsync(new ArticleTag() {Article = article, Tag = newTag.Entity});
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
        
        //get article by username
        [HttpGet("user/{username}")]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticleByUsername(string username)
        {
            var user = await _context.Users
                .Include(a=>a.Articles)
                .Where(u=>u.UserName==username)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            return user.Articles;
        }
        
        [HttpPost("{id}/tags")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Tag>>> AddArticleTag(int id, [FromBody] List<Tag> tags)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            await _context.Tags.AddRangeAsync(tags.Except(_context.Tags));


            // foreach (var tag in tags)
            // {
            //     if (!string.IsNullOrWhiteSpace(tag.Name))
            //     {
            //         var newTag = await _context.Tags.AddAsync(new Tag() {Name = tag.Name});
            //         await _context.ArticleTags.AddAsync(new ArticleTag() {Article = article, Tag = newTag.Entity});
            //     }
            // }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetArticle), new {id = article.ArticleId});
        }
        
        [HttpDelete("{id}/tags/{tagId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Tag>>> DeleteArticleTag(int id, int tagId)
        {
            var article = await _context.Articles
                .Include(at => at.ArticleTags)
                .FirstOrDefaultAsync(a => a.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            var tag = article.ArticleTags.FirstOrDefault(t => t.TagId == tagId);
            article.ArticleTags.Remove(tag);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArticle), new {id = article.ArticleId});
        }

        [HttpGet("{id}/comments")]
        public async Task<ActionResult<IEnumerable<object>>> GetArticleComments(int id)
        {
            var article = await _context.Articles
                .Include(c => c.Comments)
                .ThenInclude(a=>a.Author)
                .FirstOrDefaultAsync();
            if (article == null)
            {
                return NotFound();
            }

            return article.Comments.ToList().Select(x => new
            {
                Author = x.Author,
                Body = x.Body,
                CreatedAt = x.CreatedAt,
                CommentId = x.CommentId
            }).ToList();
        }
        
        [HttpPost("{id}/comments")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Comment>>> AddComment(int id, Comment comment)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(a => a.Articles)
                .Where(u => u.UserName == User.Identity.Name)
                .FirstOrDefaultAsync();
            if (!string.IsNullOrWhiteSpace(comment.Body))
            {
                var newComment = await _context.Comments.AddAsync(new Comment()
                {
                    Body = comment.Body,
                    CreatedAt = DateTime.Now,
                    ArticleId = article.ArticleId,
                    Author = user
                });
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetArticleComments), new {id = article.ArticleId});
        }
        
        [HttpDelete("{id}/comments/{commentId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Comment>>> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
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