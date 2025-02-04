namespace Capella.Models
{
    public class Post
    {
        public int Id_Post { get; set; } // Clé primaire
        public string Contenu { get; set; } // Contenu du post
        public int? PostId { get; set; } // Clé étrangère pour les réponses
        public int UserId { get; set; } // Clé étrangère vers l'utilisateur
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Propriétés de navigation
        public User User { get; set; }
        public List<Post> Replies { get; set; } = new List<Post>();
    }
}