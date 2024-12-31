using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capella.Models
{
    [Table("users")] // Optional: explicitly map to the existing "users" table
    public class User
    {
        [Key]
        [Column("id_user")] // Map to the "id_user" column in the database
        public int Id_User { get; set; }

        [Column("nom")]
        public string Nom { get; set; }

        [Column("prenom")]
        public string Prenom { get; set; }

        [Column("etablissement")]
        public string Etablissement { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("mdp")]
        public string Mdp { get; set; }

        [Column("role_id")]
        public int Role_Id { get; set; }
    }
}
