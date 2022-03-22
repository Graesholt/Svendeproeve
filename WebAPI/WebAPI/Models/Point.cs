namespace WebAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Points")]
    public class Point
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PointId { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public DateTime DateTime { get; set; }
    }
}
