using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Capella.Data;
using System.Linq;

namespace Capella.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; } // User's email

        [BindProperty]
        public string Password { get; set; } // User's password

        public string ErrorMessage { get; set; } // Error message

        public IActionResult OnPost()
        {
            // Authenticate the user
            var user = _context.Users.FirstOrDefault(u => u.Email == Email && u.Mdp == Password);

            if (user != null)
            {
                // Set session values
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("UserId", user.Id_User.ToString());
                return RedirectToPage("/Index"); // Redirect to the homepage
            }

            ErrorMessage = "Invalid email or password.";
            return Page();
        }
    }
}
