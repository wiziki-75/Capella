using Microsoft.AspNetCore.Mvc.RazorPages;
using Capella.Data;
using Capella.Models;
using System.Collections.Generic;
using System.Linq;

namespace Capella.Pages
{
    public class TestDbModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public TestDbModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<User> Users { get; set; }

        public void OnGet()
        {
            // Query all users from the database
            Users = _context.Users.ToList();
        }
    }
}
