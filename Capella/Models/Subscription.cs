namespace Capella.Models
{
    public class Subscription
    {
        public int Id { get; set; } // Clé primaire
        public int SubscriberId { get; set; } // ID de l'utilisateur abonné
        public int SubscribedToId { get; set; } // ID de l'utilisateur auquel il est abonné

        // Propriétés de navigation
        public User Subscriber { get; set; }
        public User SubscribedTo { get; set; }
    }
}
