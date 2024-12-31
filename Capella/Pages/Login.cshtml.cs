using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Capella.Data;

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
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == Email && u.Mdp == Password);

            if (user != null)
            {
                // Save user information in session
                HttpContext.Session.SetString("UserId", user.Id_User.ToString());
                return RedirectToPage("/Index");
            }

            ErrorMessage = "Invalid email or password.";
            return Page();
        }
    }
}
