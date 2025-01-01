using System;
using System.Collections.Generic;

namespace Capella.Models
{
    public class Post
    {
        public int Id_Post { get; set; } // Primary key
        public string Contenu { get; set; } // Content of the post
        public int? PostId { get; set; } // Nullable, used for replies (maps to post_id)
        public int UserId { get; set; } // Foreign key to User (maps to user_id)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } // Reference to the User
        public ICollection<Post> Replies { get; set; } // Replies to this post
    }
}
