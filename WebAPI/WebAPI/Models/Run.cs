namespace WebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Runs")]
    public class Run
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int runId { get; set; }

        public DateTime dateTime { get; set; }

        public User user { get; set; }

        public List<Point> points { get; set; }

        public bool deleted { get; set; }
    }
}
