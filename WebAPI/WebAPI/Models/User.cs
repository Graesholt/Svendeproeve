namespace WebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int userId { get; set; }

        public string username { get; set; }

        public string password { get; set; }
    }
}
