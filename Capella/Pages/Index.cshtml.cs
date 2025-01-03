using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Capella.Data;
using System.Collections.Generic;
using System.Linq;
using Capella.Models;

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

            // Fetch all posts
            var allPosts = _context.Posts
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            // Map posts to the view model
            var postViewModels = allPosts
                .Where(p => p.PostId == null) // Only top-level posts
                .Select(p => new PostViewModel
                {
                    Id = p.Id_Post,
                    Content = p.Contenu,
                    UserName = p.User.Nom + " " + p.User.Prenom,
                    CreatedAt = p.CreatedAt,
                    LikeCount = _context.Likes.Count(l => l.PostId == p.Id_Post),
                    Replies = allPosts
                        .Where(r => r.PostId == p.Id_Post) // Fetch replies for this post
                        .Select(r => new PostViewModel
                        {
                            Id = r.Id_Post,
                            Content = r.Contenu,
                            UserName = r.User.Nom + " " + r.User.Prenom,
                            CreatedAt = r.CreatedAt,
                            LikeCount = _context.Likes.Count(l => l.PostId == r.Id_Post)
                        })
                        .OrderBy(r => r.CreatedAt)
                        .ToList()
                })
                .ToList();

            Posts = postViewModels;
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
            if (string.IsNullOrEmpty(ReplyContent))
            {
                return RedirectToPage(); // Avoid empty replies
            }

            var userId = int.Parse(HttpContext.Session.GetString("UserId"));

            var replyPost = new Post
            {
                Contenu = ReplyContent,
                UserId = userId,
                PostId = postId // Reference to the parent post
            };

            // Add the reply directly to the database
            _context.Posts.Add(replyPost);
            _context.SaveChanges(); // Save changes

            return RedirectToPage(); // Reload the page to reflect the new reply
        }

        public IActionResult OnPostLogout()
        {
            // Clear the session
            HttpContext.Session.Clear();

            // Redirect to login page
            return RedirectToPage("/Login");
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
