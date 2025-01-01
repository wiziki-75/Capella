using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Capella.Data;
using System.Collections.Generic;
using System.Linq;

namespace Capella.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<PostViewModel> Posts { get; set; } = new List<PostViewModel>();

        [BindProperty]
        public string Content { get; set; } // For creating new posts

        [BindProperty]
        public string ReplyContent { get; set; } // For replying to posts

        public void OnGet()
        {
            // Redirect to login if not logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                Response.Redirect("/Login");
                return;
            }

            // Fetch all posts with replies and like counts
            Posts = _context.Posts
                .Include(p => p.User)
                .Include(p => p.Replies)
                .ThenInclude(r => r.User)
                .Select(p => new PostViewModel
                {
                    Id = p.Id_Post,
                    Content = p.Contenu,
                    UserName = p.User.Nom + " " + p.User.Prenom,
                    CreatedAt = p.CreatedAt,
                    LikeCount = _context.Likes.Count(l => l.PostId == p.Id_Post),
                    Replies = p.Replies.Select(r => new PostViewModel
                    {
                        Id = r.Id_Post,
                        Content = r.Contenu,
                        UserName = r.User.Nom + " " + r.User.Prenom,
                        CreatedAt = r.CreatedAt
                    }).ToList()
                })
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
        }

        public IActionResult OnPostCreatePost()
        {
            if (!string.IsNullOrEmpty(Content))
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));

                var newPost = new Capella.Models.Post
                {
                    Contenu = Content,
                    UserId = userId,
                    PostId = null
                };

                _context.Posts.Add(newPost);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostLikePost(int postId)
        {
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            // Check if the user already liked the post
            var existingLike = _context.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);
            if (existingLike == null)
            {
                var like = new Capella.Models.Like
                {
                    PostId = postId, // Correctly mapped to post_id in the database
                    UserId = userId  // Correctly mapped to user_id in the database
                };

                _context.Likes.Add(like);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostReplyPost(int postId)
        {
            if (!string.IsNullOrEmpty(ReplyContent))
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));

                var replyPost = new Capella.Models.Post
                {
                    Contenu = ReplyContent,
                    UserId = userId,
                    PostId = postId
                };

                _context.Posts.Add(replyPost);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }

    public class PostViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikeCount { get; set; }
        public List<PostViewModel> Replies { get; set; } = new List<PostViewModel>();
    }
}
