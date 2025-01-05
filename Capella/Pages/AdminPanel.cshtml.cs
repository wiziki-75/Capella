using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Capella.Data;
using Capella.Models;

namespace Capella.Pages
{
    public class AdminPanelModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminPanelModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<User> Users { get; set; } = new List<User>();
        public List<Post> Posts { get; set; } = new List<Post>();

        public IActionResult OnGet()
        {
            // Vérifier si l'utilisateur est connecté
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToPage("/Login");
            }

            // Récupérer l'utilisateur connecté
            var userId = int.Parse(HttpContext.Session.GetString("UserId"));
            var user = _context.Users.FirstOrDefault(u => u.Id_User == userId);

            // Vérifier si l'utilisateur a les droits d'accès (role_id = 2 ou 3)
            if (user == null || (user.Role_Id != 2 && user.Role_Id != 3))
            {
                return RedirectToPage("/AccessDenied");
            }

            // Charger les données pour les administrateurs
            Users = _context.Users.ToList();
            Posts = _context.Posts.Include(p => p.User).ToList();

            return Page();
        }

        public IActionResult OnPostDeleteUser(int UserId)
        {
            var user = _context.Users.Find(UserId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDeletePost(int PostId)
        {
            var post = _context.Posts.Find(PostId);
            if (post != null)
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }
}
