namespace Capella.Models
{
    public class Like
    {
        public int Id { get; set; } // Primary key (maps to id_post)
        public int UserId { get; set; } // Maps to user_id
        public int PostId { get; set; } // Maps to post_id

        // Navigation properties
        public User User { get; set; }
        public Post Post { get; set; }
    }
}
