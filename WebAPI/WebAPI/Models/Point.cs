namespace WebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Points")]
    public class Point
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int pointId { get; set; }

        public DateTime dateTime { get; set; }

        public double longitude { get; set; }

        public double latitude { get; set; }

        public double ? altitude { get; set; }


    }
}
