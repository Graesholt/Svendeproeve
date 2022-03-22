namespace WebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Runs")]
    public class Run
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RunId { get; set; }

        public DateTime DateTime { get; set; }

        public User User { get; set; }

        public List<Point> Points { get; set; }

        public bool Deleted { get; set; }
    }
}
